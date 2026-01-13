using MediatR;
using VolunteerManagement.Model;

using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Volunteers.Commands
{
    public record PatchVolunteerInfoCommand(int VolunteerId, PersonalInfoPatchDTO PersonalInfo) : IRequest<int>;

    public class PatchVolunteerInfoHandler : IRequestHandler<PatchVolunteerInfoCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;


        public PatchVolunteerInfoHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(PatchVolunteerInfoCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("PatchVolunteerInfo", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
