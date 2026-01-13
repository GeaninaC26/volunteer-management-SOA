using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Locations.Commands
{
    public record CreateLocationCommand(LocationDTO Location) : IRequest<int>;

    public class CreateLocationHandler : IRequestHandler<CreateLocationCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public CreateLocationHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("CreateLocation", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
