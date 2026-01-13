using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RecruitmentService.Services;
using VolunteerManagement.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RecruitmentService.Messaging
{
    public class RabbitMQListener : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMQListener(ILogger<RabbitMQListener> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string _hostName = _configuration["RabbitMQ:HostName"] ?? "rabbitmq";
            _logger.LogInformation($"Connecting to RabbitMQ at {_hostName}");
            var factory = new ConnectionFactory { HostName = _hostName };
            
            try {
                _connection = await factory.CreateConnectionAsync(stoppingToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);
                _logger.LogInformation("Connected to RabbitMQ and created channel.");

                await _channel.QueueDeclareAsync(queue: "recruitment_queue", durable: false, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    _logger.LogInformation($"Received message with CorrelationId: {ea.BasicProperties.CorrelationId}");
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    
                    var replyProps = new BasicProperties{
                        CorrelationId = ea.BasicProperties.CorrelationId
                    };

                    string? response = null;

                    try
                    {
                        var msg = JsonNode.Parse(messageJson);
                        if (msg == null) throw new Exception("Empty message");

                        string command = msg["Command"]?.GetValue<string>() ?? "";
                        _logger.LogInformation($"Processing command: {command}");
                        string payloadJson = msg["Payload"]?.ToJsonString() ?? "{}";

                        using var scope = _serviceProvider.CreateScope();
                        var candidateService = scope.ServiceProvider.GetRequiredService<CandidateService>();
                        var recruitmentService = scope.ServiceProvider.GetRequiredService<Services.RecruitmentService>();
                        var interviewTemplateService = scope.ServiceProvider.GetRequiredService<InterviewTemplateService>();
                        var locationService = scope.ServiceProvider.GetRequiredService<LocationService>();
                        var recruitmentFormTemplateService = scope.ServiceProvider.GetRequiredService<RecruitmentFormTemplateService>();
                        var volunteerDisponibilityService = scope.ServiceProvider.GetRequiredService<VolunteerDisponibilityService>();
                        var volunteerService = scope.ServiceProvider.GetRequiredService<VolunteerService>();
                        var interviewService = scope.ServiceProvider.GetRequiredService<InterviewService>();

                        response = await ProcessCommandAsync(command, payloadJson, candidateService, recruitmentService, interviewTemplateService, locationService, recruitmentFormTemplateService, volunteerDisponibilityService, volunteerService, interviewService);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message");
                        response = JsonSerializer.Serialize(new { Error = ex.Message });
                    }
                    finally 
                    {
                        if (_channel != null)
                        {
                            var responseBytes = Encoding.UTF8.GetBytes(response ?? "");
                            await _channel.BasicPublishAsync(exchange: "", routingKey: ea.BasicProperties.ReplyTo, mandatory: false, basicProperties: replyProps, body: responseBytes, cancellationToken: stoppingToken);
                        }
                    }
                };

                await _channel.BasicConsumeAsync(queue: "recruitment_queue", autoAck: true, consumer: consumer, cancellationToken: stoppingToken);
            
                // Keep the service running
                await Task.Delay(Timeout.Infinite, stoppingToken);
            } 
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Critical error in RabbitMQListener");
            }
        }

        private async Task<string> ProcessCommandAsync(string command, string payloadJson, CandidateService candidateService, RecruitmentService.Services.RecruitmentService recruitmentService, InterviewTemplateService interviewTemplateService, LocationService locationService, RecruitmentFormTemplateService recruitmentFormTemplateService, VolunteerDisponibilityService volunteerDisponibilityService, VolunteerService volunteerService, InterviewService interviewService)
        {
            var payload = JsonNode.Parse(payloadJson);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            if (payload == null) return JsonSerializer.Serialize(new { Error = "Payload is null" });

            var kafkaProducer = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<KafkaProducer>();
            
            var commandHandlers = new Dictionary<string, Func<JsonNode, Task<string>>>
            {
                // Candidate commands
                ["CreateCandidate"] = async (payload) => await HandleCreateCandidate(payload, candidateService, recruitmentService, kafkaProducer, options),
                ["GetCandidates"] = async (payload) => await HandleGetCandidates(payload, candidateService, options),
                ["GetCandidateById"] = async (payload) => await HandleGetCandidateById(payload, candidateService, options),
                ["PatchCandidate"] = async (payload) => await HandlePatchCandidate(payload, candidateService, recruitmentService, kafkaProducer, options),
                ["PatchCandidateInfo"] = async (payload) => await HandlePatchCandidateInfo(payload, candidateService, recruitmentService, kafkaProducer, options),
                ["UpdateCandidateInfo"] = async (payload) => await HandleUpdateCandidateInfo(payload, candidateService, recruitmentService, kafkaProducer, options),
                ["GetCandidateInfo"] = async (payload) => await HandleGetCandidateInfo(payload, candidateService, options),
                ["DeleteCandidate"] = async (payload) => await HandleDeleteCandidate(payload, candidateService, recruitmentService, kafkaProducer, options),
                
                // Recruitment Campaign commands
                ["CreateRecruitmentCampaign"] = async (payload) => await HandleCreateRecruitmentCampaign(payload, recruitmentService, kafkaProducer, options),
                ["GetRecruitmentCampaigns"] = async (payload) => await HandleGetRecruitmentCampaigns(payload, recruitmentService, options),
                ["GetRecruitmentCampaignById"] = async (payload) => await HandleGetRecruitmentCampaignById(payload, recruitmentService, options),
                ["PatchRecruitmentCampaign"] = async (payload) => await HandlePatchRecruitmentCampaign(payload, recruitmentService, kafkaProducer, options),
                ["DeleteRecruitmentCampaign"] = async (payload) => await HandleDeleteRecruitmentCampaign(payload, recruitmentService, kafkaProducer, options),
                ["AddCampaignLocation"] = async (payload) => await HandleAddCampaignLocation(payload, recruitmentService, kafkaProducer, options),
                ["RemoveCampaignLocation"] = async (payload) => await HandleRemoveCampaignLocation(payload, recruitmentService, kafkaProducer, options),
                ["AddCampaignVolunteer"] = async (payload) => await HandleAddCampaignVolunteer(payload, recruitmentService, kafkaProducer, options),
                ["RemoveCampaignVolunteer"] = async (payload) => await HandleRemoveCampaignVolunteer(payload, recruitmentService, kafkaProducer, options),
                ["GetCampaignVolunteers"] = async (payload) => await HandleGetCampaignVolunteers(payload, recruitmentService, options),
                ["GetCampaignLocations"] = async (payload) => await HandleGetCampaignLocations(payload, recruitmentService, options),
                
                // Blocked Period commands
                ["CreateBlockedPeriod"] = async (payload) => await HandleCreateBlockedPeriod(payload, recruitmentService, kafkaProducer, options),
                ["GetBlockedPeriods"] = async (payload) => await HandleGetBlockedPeriods(payload, recruitmentService, options),
                ["GetBlockedPeriodById"] = async (payload) => await HandleGetBlockedPeriodById(payload, recruitmentService, options),
                ["PatchBlockedPeriod"] = async (payload) => await HandlePatchBlockedPeriod(payload, recruitmentService, kafkaProducer, options),
                ["DeleteBlockedPeriod"] = async (payload) => await HandleDeleteBlockedPeriod(payload, recruitmentService, kafkaProducer, options),
                
                // Interview Template commands
                ["CreateInterviewTemplate"] = async (payload) => await HandleCreateInterviewTemplate(payload, interviewTemplateService, kafkaProducer, options),
                ["GetInterviewTemplates"] = async (payload) => await HandleGetInterviewTemplates(payload, interviewTemplateService, options),
                ["GetInterviewTemplateById"] = async (payload) => await HandleGetInterviewTemplateById(payload, interviewTemplateService, options),
                ["DeleteInterviewTemplate"] = async (payload) => await HandleDeleteInterviewTemplate(payload, interviewTemplateService, kafkaProducer, options),
                ["AddQuestionToInterviewTemplate"] = async (payload) => await HandleAddQuestionToInterviewTemplate(payload, interviewTemplateService, kafkaProducer, options),
                
                // Location commands
                ["CreateLocation"] = async (payload) => await HandleCreateLocation(payload, locationService, kafkaProducer, options),
                ["GetLocations"] = async (payload) => await HandleGetLocations(payload, locationService, options),
                ["GetLocationById"] = async (payload) => await HandleGetLocationById(payload, locationService, options),
                ["PatchLocation"] = async (payload) => await HandlePatchLocation(payload, locationService, kafkaProducer, options),
                ["DeleteLocation"] = async (payload) => await HandleDeleteLocation(payload, locationService, kafkaProducer, options),
                
                // Recruitment Form Template commands
                ["CreateRecruitmentFormTemplate"] = async (payload) => await HandleCreateRecruitmentFormTemplate(payload, recruitmentFormTemplateService, kafkaProducer, options),
                ["GetRecruitmentFormTemplates"] = async (payload) => await HandleGetRecruitmentFormTemplates(payload, recruitmentFormTemplateService, options),
                ["GetRecruitmentFormTemplateById"] = async (payload) => await HandleGetRecruitmentFormTemplateById(payload, recruitmentFormTemplateService, options),
                ["DeleteRecruitmentFormTemplate"] = async (payload) => await HandleDeleteRecruitmentFormTemplate(payload, recruitmentFormTemplateService, kafkaProducer, options),
                ["AddQuestionToRecruitmentFormTemplate"] = async (payload) => await HandleAddQuestionToRecruitmentFormTemplate(payload, recruitmentFormTemplateService, kafkaProducer, options),
                
                // Volunteer Disponibility commands
                ["CreateVolunteerDisponibility"] = async (payload) => await HandleCreateVolunteerDisponibility(payload, volunteerDisponibilityService, kafkaProducer, options),
                ["GetVolunteerDisponibilities"] = async (payload) => await HandleGetVolunteerDisponibilities(payload, volunteerDisponibilityService, options),
                ["GetVolunteerDisponibilityById"] = async (payload) => await HandleGetVolunteerDisponibilityById(payload, volunteerDisponibilityService, options),
                ["PatchVolunteerDisponibility"] = async (payload) => await HandlePatchVolunteerDisponibility(payload, volunteerDisponibilityService, kafkaProducer, options),
                ["DeleteVolunteerDisponibility"] = async (payload) => await HandleDeleteVolunteerDisponibility(payload, volunteerDisponibilityService, kafkaProducer, options),
                
                // Volunteer commands
                ["CreateVolunteer"] = async (payload) => await HandleCreateVolunteer(payload, volunteerService, kafkaProducer, options),
                ["GetVolunteers"] = async (payload) => await HandleGetVolunteers(payload, volunteerService, options),
                ["GetVolunteerById"] = async (payload) => await HandleGetVolunteerById(payload, volunteerService, options),
                ["PatchVolunteer"] = async (payload) => await HandlePatchVolunteer(payload, volunteerService, kafkaProducer, options),
                ["DeleteVolunteer"] = async (payload) => await HandleDeleteVolunteer(payload, volunteerService, kafkaProducer, options),
                ["GetVolunteerInfo"] = async (payload) => await HandleGetVolunteerInfo(payload, volunteerService, options),
                ["PatchVolunteerInfo"] = async (payload) => await HandlePatchVolunteerInfo(payload, volunteerService, kafkaProducer, options),
                ["UpdateVolunteerInfo"] = async (payload) => await HandleUpdateVolunteerInfo(payload, volunteerService, kafkaProducer, options),
                
                // Interview commands
                ["CreateInterview"] = async (payload) => await HandleCreateInterview(payload, interviewService, kafkaProducer, options),
                ["GetInterviews"] = async (payload) => await HandleGetInterviews(payload, interviewService, options),
                ["GetInterviewById"] = async (payload) => await HandleGetInterviewById(payload, interviewService, options),
                ["PatchInterview"] = async (payload) => await HandlePatchInterview(payload, interviewService, kafkaProducer, options),
                ["DeleteInterview"] = async (payload) => await HandleDeleteInterview(payload, interviewService, kafkaProducer, options)
            };
            
            if (commandHandlers.TryGetValue(command, out var handler))
            {
                return await handler(payload);
            }
            
            return JsonSerializer.Serialize(new { Error = $"Unknown Command: {command}" });
        }

        // Candidate command handlers
        private async Task<string> HandleCreateCandidate(JsonNode payload, CandidateService candidateService, RecruitmentService.Services.RecruitmentService recruitmentService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var campaignName = (await recruitmentService.RetrieveAsync(campaignId))?.Name ?? "";
            var candidateDto = payload["Candidate"].Deserialize<CandidateDTO>(options);
            var candidateId = await candidateService.CreateCandidateAsync(campaignId, candidateDto);
            await kafkaProducer.PublishEventAsync("candidate_updates", campaignName);
            return JsonSerializer.Serialize(candidateId);
        }

        private async Task<string> HandleGetCandidates(JsonNode payload, CandidateService candidateService, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var recruitingStatus = payload["RecruitingStatus"]?.GetValue<string>();
            var candidates = await candidateService.GetCandidatesAsync(campaignId, recruitingStatus);
            return JsonSerializer.Serialize(candidates);
        }

        private async Task<string> HandleGetCandidateById(JsonNode payload, CandidateService candidateService, JsonSerializerOptions options)
        {
            var candidateId = payload["Id"]?.GetValue<int>() ?? 0;
            var candidate = await candidateService.GetCandidateByIdAsync(candidateId);
            return JsonSerializer.Serialize(candidate);
        }

        private async Task<string> HandlePatchCandidate(JsonNode payload, CandidateService candidateService, RecruitmentService.Services.RecruitmentService recruitmentService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var campaignName = (await recruitmentService.RetrieveAsync(campaignId))?.Name ?? "";
            var candidateId = payload["CandidateId"]?.GetValue<int>() ?? 0;
            var candidatePatchDto = payload["Candidate"].Deserialize<CandidatePatchDTO>(options);
            var patchedCandidate = await candidateService.PatchCandidateAsync(campaignId, candidateId, candidatePatchDto);
            await kafkaProducer.PublishEventAsync("candidate_updates", campaignName);
            return JsonSerializer.Serialize(patchedCandidate);
        }

        private async Task<string> HandlePatchCandidateInfo(JsonNode payload, CandidateService candidateService, RecruitmentService.Services.RecruitmentService recruitmentService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var campaignName = (await recruitmentService.RetrieveAsync(campaignId))?.Name ?? "";
            var candidateId = payload["CandidateId"]?.GetValue<int>() ?? 0;
            var personalInfoPatchDto = payload["PersonalInfo"].Deserialize<PersonalInfoPatchDTO>(options);
            var patchedInfo = await candidateService.PatchCandidateInfoAsync(campaignId, candidateId, personalInfoPatchDto);
            await kafkaProducer.PublishEventAsync("candidate_updates", campaignName);
            return JsonSerializer.Serialize(patchedInfo);
        }

        private async Task<string> HandleUpdateCandidateInfo(JsonNode payload, CandidateService candidateService, RecruitmentService.Services.RecruitmentService recruitmentService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var campaignName = (await recruitmentService.RetrieveAsync(campaignId))?.Name ?? "";
            var candidateId = payload["CandidateId"]?.GetValue<int>() ?? 0;
            var personalInfoDto = payload["PersonalInfo"].Deserialize<PersonalInfoDTO>(options);
            var updatedInfo = await candidateService.UpdateCandidateInfoAsync(campaignId, candidateId, personalInfoDto);
            await kafkaProducer.PublishEventAsync("candidate_updates", campaignName);
            return JsonSerializer.Serialize(updatedInfo);
        }

        private async Task<string> HandleGetCandidateInfo(JsonNode payload, CandidateService candidateService, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var candidateId = payload["CandidateId"]?.GetValue<int>() ?? 0;
            var candidateInfo = await candidateService.GetCandidateInfoAsync(campaignId, candidateId);
            return JsonSerializer.Serialize(candidateInfo);
        }

        private async Task<string> HandleDeleteCandidate(JsonNode payload, CandidateService candidateService, RecruitmentService.Services.RecruitmentService recruitmentService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var candidateId = payload["Id"]?.GetValue<int>() ?? 0;
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var campaignName = (await recruitmentService.RetrieveAsync(campaignId))?.Name ?? "";
            var deletedCandidate = await candidateService.DeleteCandidateAsync(candidateId);
            await kafkaProducer.PublishEventAsync("candidate_updates", campaignName);
            return JsonSerializer.Serialize(deletedCandidate);
        }

        // Recruitment Campaign command handlers
        private async Task<string> HandleCreateRecruitmentCampaign(JsonNode payload, RecruitmentService.Services.RecruitmentService recruitmentService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var campaignDto = payload["Campaign"].Deserialize<RecruitmentCampaignDTO>(options);
            var campaignId = await recruitmentService.CreateAsync(campaignDto);
            await kafkaProducer.PublishEventAsync("campaign_updates", "");
            return JsonSerializer.Serialize(campaignId);
        }

        private async Task<string> HandleGetRecruitmentCampaigns(JsonNode payload, RecruitmentService.Services.RecruitmentService recruitmentService, JsonSerializerOptions options)
        {
            var name = payload["Name"]?.GetValue<string>();
            var ongoing = payload["Ongoing"]?.GetValue<bool>();
            var campaigns = await recruitmentService.GetAllAsync(name, ongoing);
            return JsonSerializer.Serialize(campaigns);
        }

        private async Task<string> HandleGetRecruitmentCampaignById(JsonNode payload, RecruitmentService.Services.RecruitmentService recruitmentService, JsonSerializerOptions options)
        {
            var campaignId = payload["Id"]?.GetValue<int>() ?? 0;
            var campaign = await recruitmentService.RetrieveAsync(campaignId);
            return JsonSerializer.Serialize(campaign);
        }

        private async Task<string> HandlePatchRecruitmentCampaign(JsonNode payload, RecruitmentService.Services.RecruitmentService recruitmentService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var campaignId = payload["Id"]?.GetValue<int>() ?? 0;
            var campaignName = (await recruitmentService.RetrieveAsync(campaignId)).Name;
            var campaignPatchDto = payload["Campaign"].Deserialize<RecruitmentCampaignPatchDTO>(options);
            var patchedCampaign = await recruitmentService.PatchAsync(campaignId, campaignPatchDto);
            await kafkaProducer.PublishEventAsync("campaign_updates", campaignName);
            return JsonSerializer.Serialize(patchedCampaign);
        }

        private async Task<string> HandleDeleteRecruitmentCampaign(JsonNode payload, RecruitmentService.Services.RecruitmentService recruitmentService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var campaignId = payload["Id"]?.GetValue<int>() ?? 0;
            var deletedCampaign = await recruitmentService.DeleteAsync(campaignId);
            await kafkaProducer.PublishEventAsync("campaign_updates", "");
            return JsonSerializer.Serialize(deletedCampaign);
        }

        private async Task<string> HandleAddCampaignLocation(JsonNode payload, RecruitmentService.Services.RecruitmentService recruitmentService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var campaignName = (await recruitmentService.RetrieveAsync(campaignId)).Name;
            var locationId = payload["LocationId"]?.GetValue<int>() ?? 0;
            var addedLocation = await recruitmentService.AddLocationAsync(campaignId, locationId);
            await kafkaProducer.PublishEventAsync("campaign_updates", campaignName);
            return JsonSerializer.Serialize(addedLocation);
        }

        private async Task<string> HandleRemoveCampaignLocation(JsonNode payload, RecruitmentService.Services.RecruitmentService recruitmentService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var campaignName = (await recruitmentService.RetrieveAsync(campaignId)).Name;
            var locationId = payload["LocationId"]?.GetValue<int>() ?? 0;
            var removedLocation = await recruitmentService.RemoveLocationAsync(campaignId, locationId);
            await kafkaProducer.PublishEventAsync("campaign_updates", campaignName);
            return JsonSerializer.Serialize(removedLocation);
        }

        private async Task<string> HandleAddCampaignVolunteer(JsonNode payload, RecruitmentService.Services.RecruitmentService recruitmentService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var campaignName = (await recruitmentService.RetrieveAsync(campaignId)).Name;
            var volunteerId = payload["VolunteerId"]?.GetValue<int>() ?? 0;
            var addedVolunteer = await recruitmentService.AddVolunteerAsync(campaignId, volunteerId);
            await kafkaProducer.PublishEventAsync("campaign_volunteer_updates", campaignName);
            return JsonSerializer.Serialize(addedVolunteer);
        }

        private async Task<string> HandleRemoveCampaignVolunteer(JsonNode payload, RecruitmentService.Services.RecruitmentService recruitmentService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var campaignName = (await recruitmentService.RetrieveAsync(campaignId)).Name;
            var volunteerId = payload["VolunteerId"]?.GetValue<int>() ?? 0;
            var removedVolunteer = await recruitmentService.RemoveVolunteerAsync(campaignId, volunteerId);
            await kafkaProducer.PublishEventAsync("campaign_volunteer_updates", campaignName);
            return JsonSerializer.Serialize(removedVolunteer);
        }

        private async Task<string> HandleGetCampaignVolunteers(JsonNode payload, RecruitmentService.Services.RecruitmentService recruitmentService, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var name = payload["Name"]?.GetValue<string>();
            var outside = payload["Outside"]?.GetValue<bool>();
            var campaignVolunteers = await recruitmentService.GetAllVolunteersAsync(campaignId, name, outside);
            return JsonSerializer.Serialize(campaignVolunteers);
        }

        private async Task<string> HandleGetCampaignLocations(JsonNode payload, RecruitmentService.Services.RecruitmentService recruitmentService, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var campaignLocations = await recruitmentService.GetAllLocationsAsync(campaignId);
            return JsonSerializer.Serialize(campaignLocations);
        }


        // Blocked Period command handlers
        private async Task<string> HandleCreateBlockedPeriod(JsonNode payload, RecruitmentService.Services.RecruitmentService recruitmentService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var campaignName = (await recruitmentService.RetrieveAsync(campaignId)).Name;
            var blockedPeriodDto = payload["BlockedPeriod"].Deserialize<BlockedPeriodDTO>(options);
            var blockedPeriodId = await recruitmentService.CreateBlockedPeriodAsync(campaignId, blockedPeriodDto);
            await kafkaProducer.PublishEventAsync("schedule_updates", campaignName);
            return JsonSerializer.Serialize(blockedPeriodId);
        }

        private async Task<string> HandleGetBlockedPeriods(JsonNode payload, RecruitmentService.Services.RecruitmentService recruitmentService, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var blockedPeriods = await recruitmentService.GetAllBlockedPeriodsAsync(campaignId);
            return JsonSerializer.Serialize(blockedPeriods);
        }

        private async Task<string> HandleGetBlockedPeriodById(JsonNode payload, RecruitmentService.Services.RecruitmentService recruitmentService, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var blockedPeriodId = payload["Id"]?.GetValue<int>() ?? 0;
            var blockedPeriod = await recruitmentService.RetrieveBlockedPeriodAsync(campaignId, blockedPeriodId);
            return JsonSerializer.Serialize(blockedPeriod);
        }

        private async Task<string> HandlePatchBlockedPeriod(JsonNode payload, RecruitmentService.Services.RecruitmentService recruitmentService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var campaignName = (await recruitmentService.RetrieveAsync(campaignId)).Name;
            var blockedPeriodId = payload["Id"]?.GetValue<int>() ?? 0;
            var blockedPeriodPatchDto = payload["BlockedPeriod"].Deserialize<BlockedPeriodPatchDTO>(options);
            var patchedBlockedPeriod = await recruitmentService.PatchBlockedPeriodAsync(campaignId, blockedPeriodId, blockedPeriodPatchDto);
            await kafkaProducer.PublishEventAsync("schedule_updates", campaignName);
            return JsonSerializer.Serialize(patchedBlockedPeriod);
        }

        private async Task<string> HandleDeleteBlockedPeriod(JsonNode payload, RecruitmentService.Services.RecruitmentService recruitmentService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var campaignId = payload["CampaignId"]?.GetValue<int>() ?? 0;
            var campaignName = (await recruitmentService.RetrieveAsync(campaignId)).Name;
            var blockedPeriodId = payload["Id"]?.GetValue<int>() ?? 0;
            var deletedBlockedPeriod = await recruitmentService.DeleteBlockedPeriodAsync(campaignId, blockedPeriodId);
            await kafkaProducer.PublishEventAsync("schedule_updates", campaignName);
            return JsonSerializer.Serialize(deletedBlockedPeriod);
        }

        // Interview Template command handlers
        private async Task<string> HandleCreateInterviewTemplate(JsonNode payload, InterviewTemplateService interviewTemplateService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var templateDto = payload["Template"].Deserialize<InterviewTemplateDTO>(options);
            var templateId = await interviewTemplateService.CreateAsync(templateDto);
            await kafkaProducer.PublishEventAsync("interview_template_updates", "");
            return JsonSerializer.Serialize(templateId);
        }

        private async Task<string> HandleGetInterviewTemplates(JsonNode payload, InterviewTemplateService interviewTemplateService, JsonSerializerOptions options)
        {
            var name = payload["Name"]?.GetValue<string>();
            var templates = await interviewTemplateService.GetAllAsync(name);
            return JsonSerializer.Serialize(templates);
        }

        private async Task<string> HandleGetInterviewTemplateById(JsonNode payload, InterviewTemplateService interviewTemplateService, JsonSerializerOptions options)
        {
            var templateId = payload["Id"]?.GetValue<int>() ?? 0;
            var template = await interviewTemplateService.RetrieveAsync(templateId);
            return JsonSerializer.Serialize(template);
        }

        private async Task<string> HandleDeleteInterviewTemplate(JsonNode payload, InterviewTemplateService interviewTemplateService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var templateId = payload["Id"]?.GetValue<int>() ?? 0;
            var deletedTemplate = await interviewTemplateService.DeleteAsync(templateId);
            await kafkaProducer.PublishEventAsync("interview_template_updates", "");
            return JsonSerializer.Serialize(deletedTemplate);
        }

        private async Task<string> HandleAddQuestionToInterviewTemplate(JsonNode payload, InterviewTemplateService interviewTemplateService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var templateId = payload["Id"]?.GetValue<int>() ?? 0;
            var question = payload["Question"]?.GetValue<string>() ?? "";
            var updatedTemplate = await interviewTemplateService.AddQuestionAsync(templateId, question);
            await kafkaProducer.PublishEventAsync("interview_template_updates", "");
            return JsonSerializer.Serialize(updatedTemplate);
        }

        // Location command handlers
        private async Task<string> HandleCreateLocation(JsonNode payload, LocationService locationService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var locationDto = payload["Location"].Deserialize<LocationDTO>(options);
            var locationId = await locationService.CreateAsync(locationDto);
            await kafkaProducer.PublishEventAsync("location_updates", "");
            return JsonSerializer.Serialize(locationId);
        }

        private async Task<string> HandleGetLocations(JsonNode payload, LocationService locationService, JsonSerializerOptions options)
        {
            var name = payload["Name"]?.GetValue<string>();
            var locations = await locationService.GetAllAsync(name);
            return JsonSerializer.Serialize(locations);
        }

        private async Task<string> HandleGetLocationById(JsonNode payload, LocationService locationService, JsonSerializerOptions options)
        {
            var locationId = payload["Id"]?.GetValue<int>() ?? 0;
            var location = await locationService.RetrieveAsync(locationId);
            return JsonSerializer.Serialize(location);
        }

        private async Task<string> HandlePatchLocation(JsonNode payload, LocationService locationService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var locationId = payload["Id"]?.GetValue<int>() ?? 0;
            var locationPatchDto = payload["Location"].Deserialize<LocationPatchDTO>(options);
            var patchedLocation = await locationService.PatchAsync(locationId, locationPatchDto);
            await kafkaProducer.PublishEventAsync("location_updates", "");
            return JsonSerializer.Serialize(patchedLocation);
        }

        private async Task<string> HandleDeleteLocation(JsonNode payload, LocationService locationService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var locationId = payload["Id"]?.GetValue<int>() ?? 0;
            var deletedLocation = await locationService.DeleteAsync(locationId);
            await kafkaProducer.PublishEventAsync("location_updates", "");
            return JsonSerializer.Serialize(deletedLocation);
        }

        // Recruitment Form Template command handlers
        private async Task<string> HandleCreateRecruitmentFormTemplate(JsonNode payload, RecruitmentFormTemplateService recruitmentFormTemplateService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var templateDto = payload["Template"].Deserialize<RecruitmentFormTemplateDTO>(options);
            var templateId = await recruitmentFormTemplateService.CreateAsync(templateDto);
            await kafkaProducer.PublishEventAsync("recruitment_form_template_updates", "");
            return JsonSerializer.Serialize(templateId);
        }

        private async Task<string> HandleGetRecruitmentFormTemplates(JsonNode payload, RecruitmentFormTemplateService recruitmentFormTemplateService, JsonSerializerOptions options)
        {
            var name = payload["Name"]?.GetValue<string>();
            var templates = await recruitmentFormTemplateService.GetAllAsync(name);
            return JsonSerializer.Serialize(templates);
        }

        private async Task<string> HandleGetRecruitmentFormTemplateById(JsonNode payload, RecruitmentFormTemplateService recruitmentFormTemplateService, JsonSerializerOptions options)
        {
            var templateId = payload["Id"]?.GetValue<int>() ?? 0;
            var template = await recruitmentFormTemplateService.RetrieveAsync(templateId);
            return JsonSerializer.Serialize(template);
        }

        private async Task<string> HandleDeleteRecruitmentFormTemplate(JsonNode payload, RecruitmentFormTemplateService recruitmentFormTemplateService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var templateId = payload["Id"]?.GetValue<int>() ?? 0;
            var deletedTemplate = await recruitmentFormTemplateService.DeleteAsync(templateId);
            await kafkaProducer.PublishEventAsync("recruitment_form_template_updates", "");
            return JsonSerializer.Serialize(deletedTemplate);
        }

        private async Task<string> HandleAddQuestionToRecruitmentFormTemplate(JsonNode payload, RecruitmentFormTemplateService recruitmentFormTemplateService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var templateId = payload["Id"]?.GetValue<int>() ?? 0;
            var question = payload["Question"]?.GetValue<string>() ?? "";
            var updatedTemplate = await recruitmentFormTemplateService.AddQuestionAsync(templateId, question);
            await kafkaProducer.PublishEventAsync("recruitment_form_template_updates", "");
            return JsonSerializer.Serialize(updatedTemplate);
        }

        // Volunteer Disponibility command handlers
        private async Task<string> HandleCreateVolunteerDisponibility(JsonNode payload, VolunteerDisponibilityService volunteerDisponibilityService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var disponibilityDto = payload["Disponibility"].Deserialize<VolunteerDisponibilityDTO>(options);
            var disponibilityId = await volunteerDisponibilityService.CreateAsync(disponibilityDto);
            await kafkaProducer.PublishEventAsync("schedule_updates", "");
            return JsonSerializer.Serialize(disponibilityId);
        }

        private async Task<string> HandleGetVolunteerDisponibilities(JsonNode payload, VolunteerDisponibilityService volunteerDisponibilityService, JsonSerializerOptions options)
        {
            var availabilities = await volunteerDisponibilityService.GetAllAsync();
            return JsonSerializer.Serialize(availabilities);
        }

        private async Task<string> HandleGetVolunteerDisponibilityById(JsonNode payload, VolunteerDisponibilityService volunteerDisponibilityService, JsonSerializerOptions options)
        {
            var disponibilityId = payload["Id"]?.GetValue<int>() ?? 0;
            var availability = await volunteerDisponibilityService.RetrieveAsync(disponibilityId);
            return JsonSerializer.Serialize(availability);
        }

        private async Task<string> HandlePatchVolunteerDisponibility(JsonNode payload, VolunteerDisponibilityService volunteerDisponibilityService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var disponibilityId = payload["Id"]?.GetValue<int>() ?? 0;
            var disponibilityPatchDto = payload["Disponibility"].Deserialize<VolunteerDisponibilityPatchDTO>(options);
            var patchedDisponibility = await volunteerDisponibilityService.PatchAsync(disponibilityId, disponibilityPatchDto);
            await kafkaProducer.PublishEventAsync("schedule_updates", "");
            return JsonSerializer.Serialize(patchedDisponibility);
        }

        private async Task<string> HandleDeleteVolunteerDisponibility(JsonNode payload, VolunteerDisponibilityService volunteerDisponibilityService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var disponibilityId = payload["Id"]?.GetValue<int>() ?? 0;
            var deletedDisponibility = await volunteerDisponibilityService.DeleteAsync(disponibilityId);
            await kafkaProducer.PublishEventAsync("schedule_updates", "");
            return JsonSerializer.Serialize(deletedDisponibility);
        }

        // Volunteer command handlers
        private async Task<string> HandleCreateVolunteer(JsonNode payload, VolunteerService volunteerService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var volunteerDto = payload["Volunteer"].Deserialize<VolunteerDTO>(options);
            var volunteerId = await volunteerService.CreateAsync(volunteerDto);
            await kafkaProducer.PublishEventAsync("volunteer_updates", "");
            return JsonSerializer.Serialize(volunteerId);
        }

        private async Task<string> HandleGetVolunteers(JsonNode payload, VolunteerService volunteerService, JsonSerializerOptions options)
        {
            var department = payload["Department"]?.GetValue<string>();
            var volunteerStatus = payload["VolunteerStatus"]?.GetValue<string>();
            var volunteers = await volunteerService.GetAllAsync(department, volunteerStatus);
            return JsonSerializer.Serialize(volunteers);
        }

        private async Task<string> HandleGetVolunteerById(JsonNode payload, VolunteerService volunteerService, JsonSerializerOptions options)
        {
            var volunteerId = payload["Id"]?.GetValue<int>() ?? 0;
            var volunteer = await volunteerService.RetrieveAsync(volunteerId);
            return JsonSerializer.Serialize(volunteer);
        }

        private async Task<string> HandlePatchVolunteer(JsonNode payload, VolunteerService volunteerService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var volunteerId = payload["Id"]?.GetValue<int>() ?? 0;
            var volunteerPatchDto = payload["Volunteer"].Deserialize<VolunteerPatchDTO>(options);
            var patchedVolunteer = await volunteerService.PatchAsync(volunteerId, volunteerPatchDto);
            await kafkaProducer.PublishEventAsync("volunteer_updates", "");
            return JsonSerializer.Serialize(patchedVolunteer);
        }

        private async Task<string> HandleDeleteVolunteer(JsonNode payload, VolunteerService volunteerService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var volunteerId = payload["Id"]?.GetValue<int>() ?? 0;
            var deletedVolunteer = await volunteerService.DeleteAsync(volunteerId);
            await kafkaProducer.PublishEventAsync("volunteer_updates", "");
            return JsonSerializer.Serialize(deletedVolunteer);
        }

        private async Task<string> HandleGetVolunteerInfo(JsonNode payload, VolunteerService volunteerService, JsonSerializerOptions options)
        {
            var volunteerId = payload["Id"]?.GetValue<int>() ?? 0;
            var volunteerInfo = await volunteerService.RetrieveInfoAsync(volunteerId);
            return JsonSerializer.Serialize(volunteerInfo);
        }

        private async Task<string> HandlePatchVolunteerInfo(JsonNode payload, VolunteerService volunteerService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var volunteerId = payload["Id"]?.GetValue<int>() ?? 0;
            var personalInfoPatchDto = payload["PersonalInfo"].Deserialize<PersonalInfoPatchDTO>(options);
            var patchedInfo = await volunteerService.PatchInfoAsync(volunteerId, personalInfoPatchDto);
            await kafkaProducer.PublishEventAsync("schedule_updates", "");
            return JsonSerializer.Serialize(patchedInfo);
        }

        private async Task<string> HandleUpdateVolunteerInfo(JsonNode payload, VolunteerService volunteerService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var volunteerId = payload["Id"]?.GetValue<int>() ?? 0;
            var personalInfoDto = payload["PersonalInfo"].Deserialize<PersonalInfoDTO>(options);
            var updatedInfo = await volunteerService.UpdateInfoAsync(volunteerId, personalInfoDto);
            await kafkaProducer.PublishEventAsync("schedule_updates", "");
            return JsonSerializer.Serialize(updatedInfo);
        }

        // Interview command handlers
        private async Task<string> HandleCreateInterview(JsonNode payload, InterviewService interviewService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var interviewDto = payload["Interview"].Deserialize<InterviewDTO>(options);
            var interviewId = await interviewService.CreateAsync(interviewDto);
            await kafkaProducer.PublishEventAsync("schedule_updates", "");
            return JsonSerializer.Serialize(interviewId);
        }

        private async Task<string> HandleGetInterviews(JsonNode payload, InterviewService interviewService, JsonSerializerOptions options)
        {
            var candidateId = payload["CandidateId"]?.GetValue<int>() ?? 0;
            var interviews = await interviewService.GetAllAsync();
            return JsonSerializer.Serialize(interviews);
        }

        private async Task<string> HandleGetInterviewById(JsonNode payload, InterviewService interviewService, JsonSerializerOptions options)
        {
            var interviewId = payload["Id"]?.GetValue<int>() ?? 0;
            var interview = await interviewService.RetrieveAsync(interviewId);
            return JsonSerializer.Serialize(interview);
        }

        private async Task<string> HandlePatchInterview(JsonNode payload, InterviewService interviewService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var interviewId = payload["Id"]?.GetValue<int>() ?? 0;
            var interviewPatchDto = payload["Interview"].Deserialize<InterviewPatchDTO>(options);
            var patchedInterview = await interviewService.PatchAsync(interviewId, interviewPatchDto);
            await kafkaProducer.PublishEventAsync("schedule_updates", "");
            return JsonSerializer.Serialize(patchedInterview);
        }

        private async Task<string> HandleDeleteInterview(JsonNode payload, InterviewService interviewService, KafkaProducer kafkaProducer, JsonSerializerOptions options)
        {
            var interviewId = payload["Id"]?.GetValue<int>() ?? 0;
            var deletedInterview = await interviewService.DeleteAsync(interviewId);
            await kafkaProducer.PublishEventAsync("schedule_updates", "");
            return JsonSerializer.Serialize(deletedInterview);
        }
        
    }
}
