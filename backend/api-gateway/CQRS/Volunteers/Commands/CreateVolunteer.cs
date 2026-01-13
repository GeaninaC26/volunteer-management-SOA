using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Volunteers.Commands
{
    public record CreateVolunteerCommand(VolunteerDTO Volunteer) : IRequest<int>;

    public class CreateVolunteerHandler : IRequestHandler<CreateVolunteerCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public CreateVolunteerHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(CreateVolunteerCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("CreateVolunteer", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
