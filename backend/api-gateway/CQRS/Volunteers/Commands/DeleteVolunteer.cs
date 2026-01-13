using MediatR;
using VolunteerManagement.Model;

using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Volunteers.Commands
{
    public record DeleteVolunteerCommand(int Id) : IRequest<int>;

    public class DeleteVolunteerHandler : IRequestHandler<DeleteVolunteerCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public DeleteVolunteerHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(DeleteVolunteerCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("DeleteVolunteer", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
