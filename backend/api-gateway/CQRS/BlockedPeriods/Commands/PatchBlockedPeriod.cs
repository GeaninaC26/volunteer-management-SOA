using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.BlockedPeriods.Commands
{
    public record PatchBlockedPeriodCommand(int CampaignId, int Id, BlockedPeriodPatchDTO BlockedPeriod) : IRequest<int>;

    public class PatchBlockedPeriodHandler : IRequestHandler<PatchBlockedPeriodCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public PatchBlockedPeriodHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(PatchBlockedPeriodCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("PatchBlockedPeriod", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
