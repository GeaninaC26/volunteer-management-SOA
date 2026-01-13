using MediatR;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Locations.Commands
{
    public record DeleteLocationCommand(int Id) : IRequest<int>;

    public class DeleteLocationHandler : IRequestHandler<DeleteLocationCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public DeleteLocationHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("DeleteLocation", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
