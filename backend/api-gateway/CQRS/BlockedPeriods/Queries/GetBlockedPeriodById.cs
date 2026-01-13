using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.BlockedPeriods.Queries
{
    public record GetBlockedPeriodByIdQuery(int CampaignId, int Id) : IRequest<BlockedPeriodDTO?>;

    public class GetBlockedPeriodByIdHandler : IRequestHandler<GetBlockedPeriodByIdQuery, BlockedPeriodDTO?>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetBlockedPeriodByIdHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<BlockedPeriodDTO?> Handle(GetBlockedPeriodByIdQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetBlockedPeriodById", request);
            return await _messageProducer.CallAsync<Message, BlockedPeriodDTO?>(message, "recruitment_queue", cancellationToken);
        }
    }
}
