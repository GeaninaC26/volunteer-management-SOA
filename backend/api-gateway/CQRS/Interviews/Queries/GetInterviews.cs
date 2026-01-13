using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Interviews.Queries
{
    public record GetInterviewsQuery() : IRequest<List<InterviewDTO>>;

    public class GetInterviewsHandler : IRequestHandler<GetInterviewsQuery, List<InterviewDTO>>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetInterviewsHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<List<InterviewDTO>> Handle(GetInterviewsQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetInterviews", request);
            return await _messageProducer.CallAsync<Message, List<InterviewDTO>>(message, "recruitment_queue", cancellationToken);
        }
    }
}
