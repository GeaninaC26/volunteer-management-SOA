using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Interviews.Commands
{
    public record PatchInterviewCommand(int Id, InterviewPatchDTO Interview) : IRequest<int>;

    public class PatchInterviewHandler : IRequestHandler<PatchInterviewCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public PatchInterviewHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(PatchInterviewCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("PatchInterview", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
