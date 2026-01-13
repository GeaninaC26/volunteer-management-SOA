using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.RecruitmentFormTemplates.Commands
{
    public record CreateRecruitmentFormTemplateCommand(RecruitmentFormTemplateDTO Template) : IRequest<int>;

    public class CreateRecruitmentFormTemplateHandler : IRequestHandler<CreateRecruitmentFormTemplateCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public CreateRecruitmentFormTemplateHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(CreateRecruitmentFormTemplateCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("CreateRecruitmentFormTemplate", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
