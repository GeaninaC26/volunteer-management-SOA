using MediatR;
using VolunteerManagement.Model;

using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Volunteers.Queries
{
    public record GetVolunteersQuery(string? Department, string? VolunteerStatus) : IRequest<List<VolunteerDTO>>;

    public class GetVolunteersHandler : IRequestHandler<GetVolunteersQuery, List<VolunteerDTO>>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetVolunteersHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<List<VolunteerDTO>> Handle(GetVolunteersQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetVolunteers", request);
            return await _messageProducer.CallAsync<Message, List<VolunteerDTO>>(message, "recruitment_queue", cancellationToken);
        }
    }
}
