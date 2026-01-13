using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.InterviewTemplates.Queries
{
    public record GetInterviewTemplatesQuery(string? Name) : IRequest<List<InterviewTemplateDTO>>;

    public class GetInterviewTemplatesHandler : IRequestHandler<GetInterviewTemplatesQuery, List<InterviewTemplateDTO>>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetInterviewTemplatesHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<List<InterviewTemplateDTO>> Handle(GetInterviewTemplatesQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetInterviewTemplates", request);
            return await _messageProducer.CallAsync<Message, List<InterviewTemplateDTO>>(message, "recruitment_queue", cancellationToken);
        }
    }
}
