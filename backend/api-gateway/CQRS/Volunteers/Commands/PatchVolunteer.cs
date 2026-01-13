using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Volunteers.Commands
{
    public record PatchVolunteerCommand(int Id, VolunteerPatchDTO Volunteer) : IRequest<int>;

    public class PatchVolunteerHandler : IRequestHandler<PatchVolunteerCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public PatchVolunteerHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(PatchVolunteerCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("PatchVolunteer", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
