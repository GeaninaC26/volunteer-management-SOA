using MediatR;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.BlockedPeriods.Commands
{
    public record DeleteBlockedPeriodCommand(int CampaignId, int Id) : IRequest<int>;

    public class DeleteBlockedPeriodHandler : IRequestHandler<DeleteBlockedPeriodCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public DeleteBlockedPeriodHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(DeleteBlockedPeriodCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("DeleteBlockedPeriod", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
