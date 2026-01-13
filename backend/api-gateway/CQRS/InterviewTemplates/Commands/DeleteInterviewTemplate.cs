using MediatR;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.InterviewTemplates.Commands
{
    public record DeleteInterviewTemplateCommand(int Id) : IRequest<int>;

    public class DeleteInterviewTemplateHandler : IRequestHandler<DeleteInterviewTemplateCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public DeleteInterviewTemplateHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(DeleteInterviewTemplateCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("DeleteInterviewTemplate", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
