using MediatR;
using VolunteerManagement.Model;


using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Interviews.Commands
{
    public record CreateInterviewCommand(InterviewDTO Interview) : IRequest<int>;

    public class CreateInterviewHandler : IRequestHandler<CreateInterviewCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public CreateInterviewHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(CreateInterviewCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("CreateInterview", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
