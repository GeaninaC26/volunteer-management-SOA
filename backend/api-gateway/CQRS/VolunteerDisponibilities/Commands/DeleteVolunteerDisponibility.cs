using MediatR;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.VolunteerDisponibilities.Commands
{
    public record DeleteVolunteerDisponibilityCommand(int Id) : IRequest<int>;

    public class DeleteVolunteerDisponibilityHandler : IRequestHandler<DeleteVolunteerDisponibilityCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public DeleteVolunteerDisponibilityHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(DeleteVolunteerDisponibilityCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("DeleteVolunteerDisponibility", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
