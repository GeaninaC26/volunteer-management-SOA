using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.VolunteerDisponibilities.Commands
{
    public record CreateVolunteerDisponibilityCommand(VolunteerDisponibilityDTO Disponibility) : IRequest<int>;

    public class CreateVolunteerDisponibilityHandler : IRequestHandler<CreateVolunteerDisponibilityCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public CreateVolunteerDisponibilityHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(CreateVolunteerDisponibilityCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("CreateVolunteerDisponibility", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
