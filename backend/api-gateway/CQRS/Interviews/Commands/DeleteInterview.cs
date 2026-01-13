using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Interviews.Commands
{
    public record DeleteInterviewCommand(int Id) : IRequest<int>;

    public class DeleteInterviewHandler : IRequestHandler<DeleteInterviewCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public DeleteInterviewHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(DeleteInterviewCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("DeleteInterview", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
