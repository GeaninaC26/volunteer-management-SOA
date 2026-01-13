using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.BlockedPeriods.Queries
{
    public record GetBlockedPeriodsQuery(int CampaignId) : IRequest<List<BlockedPeriodDTO>>;

    public class GetBlockedPeriodsHandler : IRequestHandler<GetBlockedPeriodsQuery, List<BlockedPeriodDTO>>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetBlockedPeriodsHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<List<BlockedPeriodDTO>> Handle(GetBlockedPeriodsQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetBlockedPeriods", request);
            return await _messageProducer.CallAsync<Message, List<BlockedPeriodDTO>>(message, "recruitment_queue", cancellationToken);
        }
    }
}
