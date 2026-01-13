using MediatR;
using VolunteerManagement.Model;

using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Volunteers.Commands
{
    public record UpdateVolunteerInfoCommand(int VolunteerId, PersonalInfoDTO PersonalInfo) : IRequest<int>;

    public class UpdateVolunteerInfoHandler : IRequestHandler<UpdateVolunteerInfoCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public UpdateVolunteerInfoHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(UpdateVolunteerInfoCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("UpdateVolunteerInfo", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
