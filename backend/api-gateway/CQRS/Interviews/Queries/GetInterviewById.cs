using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Interviews.Queries
{
    public record GetInterviewByIdQuery(int Id) : IRequest<InterviewDTO?>;

    public class GetInterviewByIdHandler : IRequestHandler<GetInterviewByIdQuery, InterviewDTO?>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetInterviewByIdHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<InterviewDTO?> Handle(GetInterviewByIdQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetInterviewById", request);
            return await _messageProducer.CallAsync<Message, InterviewDTO?>(message, "recruitment_queue", cancellationToken);
          
        }
    }
}
