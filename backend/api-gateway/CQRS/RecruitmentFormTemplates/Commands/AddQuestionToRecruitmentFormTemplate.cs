using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.RecruitmentFormTemplates.Commands
{
    public record AddQuestionToRecruitmentFormTemplateCommand(int Id, string Question) : IRequest<RecruitmentFormTemplateDTO?>;

    public class AddQuestionToRecruitmentFormTemplateHandler : IRequestHandler<AddQuestionToRecruitmentFormTemplateCommand, RecruitmentFormTemplateDTO?>
    {
        private readonly RabbitMQProducer _messageProducer;

        public AddQuestionToRecruitmentFormTemplateHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<RecruitmentFormTemplateDTO?> Handle(AddQuestionToRecruitmentFormTemplateCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("AddQuestionToRecruitmentFormTemplate", request);
            return await _messageProducer.CallAsync<Message, RecruitmentFormTemplateDTO?>(message, "recruitment_queue", cancellationToken);
        }
    }
}
