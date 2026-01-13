using Confluent.Kafka;
using System.Numerics;
using System.Text.Json;

namespace notification_service.Services;

public class KafkaNotificationConsumer : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<KafkaNotificationConsumer> _logger;
    private readonly IConfiguration _configuration;

    public KafkaNotificationConsumer(IServiceProvider serviceProvider, ILogger<KafkaNotificationConsumer> logger, IConfiguration configuration){
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;

        _consumer = new ConsumerBuilder<string, string>(new ConsumerConfig{
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = "notification-service",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            SessionTimeoutMs = 6000,
            HeartbeatIntervalMs = 2000
        }).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken){
        await Task.Yield(); // Make the method async

        _consumer.Subscribe(new[] { 
            "candidate_updates",
            "campaign_updates", 
            "location_updates", 
            "campaign_volunteer_updates",
            "interview_template_updates",
            "recruitment_form_template_updates",
            "schedule_updates"
        });

        try{
            while (!stoppingToken.IsCancellationRequested){
                try{
                    var consumeResult = _consumer.Consume(stoppingToken); 
                    if (consumeResult?.Message?.Value != null){
                        await ProcessNotificationMessage(consumeResult.Topic, consumeResult.Message.Value, stoppingToken);
                        _consumer.Commit(consumeResult);
                    }
                }catch (ConsumeException ex){
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }catch (OperationCanceledException){
                    break;
                }catch (Exception ex){
                    _logger.LogError(ex, "Unexpected error in Kafka consumer");
                }
            }
        }finally{
            _consumer.Close();
            _consumer.Dispose();
        }
    }

    private async Task ProcessNotificationMessage(string topic, string messageValue, CancellationToken cancellationToken){
        try{
            var notificationService = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<INotificationService>();
            
            _logger.LogInformation("Processing notification from topic {Topic}", topic);

            // Handle different topics with different routing strategies
            switch (topic.ToLowerInvariant()){
                case "candidate_updates":
                    await notificationService.SendToPageAsync($"/campaigns/{messageValue}", "candidate_update", new object{});
                    await notificationService.SendToPageAsync($"/campaigns", "candidate_update", new object{});
                    await notificationService.SendToPageAsync($"/schedule_interviews/{messageValue}", "candidate_update", new object{});
                    break;
                case "campaign_volunteer_updates":
                    await notificationService.SendToPageAsync($"/campaigns/{messageValue}", "campaign_volunteer_update", new object{});
                    await notificationService.SendToPageAsync($"/schedule_interviews/{messageValue}", "campaign_volunteer_update", new object{});
                    break;
                case "campaign_updates":
                    if (string.IsNullOrWhiteSpace(messageValue)){
                        await notificationService.SendToPageAsync("/campaigns", "campaign_update", new object{});
                    }else{
                        await notificationService.SendToPageAsync($"/campaigns/{messageValue}", "campaign_update", new object{});
                        await notificationService.SendToPageAsync($"/campaigns", "campaign_update", new object{});
                        await notificationService.SendToPageAsync($"/schedule_interviews/{messageValue}", "campaign_update", new object{});
                    }
                    break;
                case "location_updates":
                    await notificationService.SendToPageAsync($"/campaigns", "location_update", new object{});
                    break;
                case "interview_template_updates":
                    await notificationService.SendToPageAsync($"/campaigns", "interview_template_update", new object{});
                    break;
                case "recruitment_form_template_updates":
                    await notificationService.SendToPageAsync($"/campaigns", "recruitment_form_template_update", new object{});
                    break;
                case "schedule_updates":
                    await notificationService.SendToPageAsync($"/schedule_interviews", "schedule_update", new object{});
                    break;
                default:
                    _logger.LogWarning("Unknown topic {Topic} received, processing as general notification", topic);
                    break;
            }
        }catch (JsonException ex){
            _logger.LogError(ex, "Error deserializing notification message from topic {Topic}: {Message}", topic, messageValue);
        }catch (Exception ex){
            _logger.LogError(ex, "Error processing notification message from topic {Topic}: {Message}", topic, messageValue);
        }
    }

    public override void Dispose(){
        _consumer?.Dispose();
        base.Dispose();
    }
}

public class NotificationMessage{
    public string EventName { get; set; } = string.Empty;
    public object? Data { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Priority { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}