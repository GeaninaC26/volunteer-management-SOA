using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.InterviewTemplates.Commands
{
    public record CreateInterviewTemplateCommand(InterviewTemplateDTO Template) : IRequest<int>;

    public class CreateInterviewTemplateHandler : IRequestHandler<CreateInterviewTemplateCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public CreateInterviewTemplateHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(CreateInterviewTemplateCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("CreateInterviewTemplate", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
