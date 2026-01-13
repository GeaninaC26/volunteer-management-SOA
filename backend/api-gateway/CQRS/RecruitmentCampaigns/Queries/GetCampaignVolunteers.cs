using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.RecruitmentCampaigns.Queries
{
    public record GetCampaignVolunteersQuery(int CampaignId, string? Name, bool? Outside) : IRequest<List<VolunteerDTO>>;

    public class GetCampaignVolunteersHandler : IRequestHandler<GetCampaignVolunteersQuery, List<VolunteerDTO>>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetCampaignVolunteersHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<List<VolunteerDTO>> Handle(GetCampaignVolunteersQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetCampaignVolunteers", request);
            return await _messageProducer.CallAsync<Message, List<VolunteerDTO>>(message, "recruitment_queue", cancellationToken);
        }
    }
}
