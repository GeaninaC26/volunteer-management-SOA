using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.InterviewTemplates.Queries
{
    public record GetInterviewTemplateByIdQuery(int Id) : IRequest<InterviewTemplateDTO?>;

    public class GetInterviewTemplateByIdHandler : IRequestHandler<GetInterviewTemplateByIdQuery, InterviewTemplateDTO?>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetInterviewTemplateByIdHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<InterviewTemplateDTO?> Handle(GetInterviewTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetInterviewTemplateById", request);
            return await _messageProducer.CallAsync<Message, InterviewTemplateDTO?>(message, "recruitment_queue", cancellationToken);
        }
    }
}
