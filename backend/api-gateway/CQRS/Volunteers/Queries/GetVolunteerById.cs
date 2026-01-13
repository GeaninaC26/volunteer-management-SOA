using MediatR;
using VolunteerManagement.Model;

using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Volunteers.Queries
{
    public record GetVolunteerByIdQuery(int Id) : IRequest<VolunteerDTO?>;

    public class GetVolunteerByIdHandler : IRequestHandler<GetVolunteerByIdQuery, VolunteerDTO?>
    {
        private readonly RabbitMQProducer _messageProducer;


        public GetVolunteerByIdHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<VolunteerDTO?> Handle(GetVolunteerByIdQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetVolunteerById", request);
            return await _messageProducer.CallAsync<Message, VolunteerDTO?>(message, "recruitment_queue", cancellationToken);
        }
    }
}
