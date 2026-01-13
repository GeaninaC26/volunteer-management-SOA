using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.InterviewTemplates.Commands
{
    public record AddQuestionToInterviewTemplateCommand(int Id, string Question) : IRequest<InterviewTemplateDTO?>;

    public class AddQuestionToInterviewTemplateHandler : IRequestHandler<AddQuestionToInterviewTemplateCommand, InterviewTemplateDTO?>
    {
        private readonly RabbitMQProducer _messageProducer;

        public AddQuestionToInterviewTemplateHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<InterviewTemplateDTO?> Handle(AddQuestionToInterviewTemplateCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("AddQuestionToInterviewTemplate", request);
            return await _messageProducer.CallAsync<Message, InterviewTemplateDTO?>(message, "recruitment_queue", cancellationToken);
        }
    }
}
