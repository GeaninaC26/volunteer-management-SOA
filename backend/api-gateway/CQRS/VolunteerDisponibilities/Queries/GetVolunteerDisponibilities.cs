using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.VolunteerDisponibilities.Queries
{
    public record GetVolunteerDisponibilitiesQuery() : IRequest<List<VolunteerDisponibilityDTO>>;

    public class GetVolunteerDisponibilitiesHandler : IRequestHandler<GetVolunteerDisponibilitiesQuery, List<VolunteerDisponibilityDTO>>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetVolunteerDisponibilitiesHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<List<VolunteerDisponibilityDTO>> Handle(GetVolunteerDisponibilitiesQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetVolunteerDisponibilities", request);
            return await _messageProducer.CallAsync<Message, List<VolunteerDisponibilityDTO>>(message, "recruitment_queue", cancellationToken);
        }
    }
}
