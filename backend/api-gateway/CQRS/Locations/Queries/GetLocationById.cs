using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Locations.Queries
{
    public record GetLocationByIdQuery(int Id) : IRequest<LocationDTO?>;

    public class GetLocationByIdHandler : IRequestHandler<GetLocationByIdQuery, LocationDTO?>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetLocationByIdHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<LocationDTO?> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetLocationById", request);
            return await _messageProducer.CallAsync<Message, LocationDTO?>(message, "recruitment_queue", cancellationToken);
        }
    }
}
