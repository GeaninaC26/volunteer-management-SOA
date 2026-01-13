using MediatR;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.RecruitmentFormTemplates.Commands
{
    public record DeleteRecruitmentFormTemplateCommand(int Id) : IRequest<int>;

    public class DeleteRecruitmentFormTemplateHandler : IRequestHandler<DeleteRecruitmentFormTemplateCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public DeleteRecruitmentFormTemplateHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(DeleteRecruitmentFormTemplateCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("DeleteRecruitmentFormTemplate", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
