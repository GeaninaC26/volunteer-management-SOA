using MediatR;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Candidates.Commands
{
    public record DeleteCandidateCommand(int CampaignId, int CandidateId) : IRequest<int>;

    public class DeleteCandidateHandler : IRequestHandler<DeleteCandidateCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public DeleteCandidateHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(DeleteCandidateCommand request, CancellationToken cancellationToken)
        {
             var payload = new { CampaignId = request.CampaignId, Id = request.CandidateId };
             var message = new Message("DeleteCandidate", payload);
             var result = await _messageProducer.CallAsync<Message, bool>(message, "recruitment_queue", cancellationToken);
             return result ? 1 : 0;
        }
    }
}
