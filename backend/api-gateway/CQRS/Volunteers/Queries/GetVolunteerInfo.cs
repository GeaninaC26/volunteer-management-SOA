using MediatR;
using VolunteerManagement.Model;

using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Volunteers.Queries
{
    public record GetVolunteerInfoQuery(int Id) : IRequest<PersonalInfoDTO?>;

    public class GetVolunteerInfoHandler : IRequestHandler<GetVolunteerInfoQuery, PersonalInfoDTO?>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetVolunteerInfoHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<PersonalInfoDTO?> Handle(GetVolunteerInfoQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetVolunteerInfo", request);
            return await _messageProducer.CallAsync<Message, PersonalInfoDTO?>(message, "recruitment_queue", cancellationToken);
        }
    }
}
