using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.RecruitmentCampaigns.Queries
{
    public record GetCampaignLocationsQuery(int CampaignId) : IRequest<List<LocationDTO>>;

    public class GetCampaignLocationsHandler : IRequestHandler<GetCampaignLocationsQuery, List<LocationDTO>>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetCampaignLocationsHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<List<LocationDTO>> Handle(GetCampaignLocationsQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetCampaignLocations", request);
            return await _messageProducer.CallAsync<Message, List<LocationDTO>>(message, "recruitment_queue", cancellationToken);
        }
    }
}
