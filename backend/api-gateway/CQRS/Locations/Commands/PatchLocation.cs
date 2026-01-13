using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Locations.Commands
{
    public record PatchLocationCommand(int Id, LocationPatchDTO Location) : IRequest<int>;

    public class PatchLocationHandler : IRequestHandler<PatchLocationCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public PatchLocationHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(PatchLocationCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("PatchLocation", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
