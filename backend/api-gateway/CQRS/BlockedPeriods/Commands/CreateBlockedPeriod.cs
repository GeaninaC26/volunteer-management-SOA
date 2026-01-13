using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.BlockedPeriods.Commands
{
    public record CreateBlockedPeriodCommand(int CampaignId, BlockedPeriodDTO BlockedPeriod) : IRequest<int>;

    public class CreateBlockedPeriodHandler : IRequestHandler<CreateBlockedPeriodCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public CreateBlockedPeriodHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(CreateBlockedPeriodCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("CreateBlockedPeriod", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
