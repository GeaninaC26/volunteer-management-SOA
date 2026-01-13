using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.VolunteerDisponibilities.Queries
{
    public record GetVolunteerDisponibilityByIdQuery(int Id) : IRequest<VolunteerDisponibilityDTO?>;

    public class GetVolunteerDisponibilityByIdHandler : IRequestHandler<GetVolunteerDisponibilityByIdQuery, VolunteerDisponibilityDTO?>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetVolunteerDisponibilityByIdHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<VolunteerDisponibilityDTO?> Handle(GetVolunteerDisponibilityByIdQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetVolunteerDisponibilityById", request);
            return await _messageProducer.CallAsync<Message, VolunteerDisponibilityDTO?>(message, "recruitment_queue", cancellationToken);
        }
    }
}
