using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.VolunteerDisponibilities.Commands
{
    public record PatchVolunteerDisponibilityCommand(int Id, VolunteerDisponibilityPatchDTO Disponibility) : IRequest<int>;

    public class PatchVolunteerDisponibilityHandler : IRequestHandler<PatchVolunteerDisponibilityCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public PatchVolunteerDisponibilityHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(PatchVolunteerDisponibilityCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("PatchVolunteerDisponibility", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
