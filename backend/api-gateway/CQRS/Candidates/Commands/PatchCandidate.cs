using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Candidates.Commands
{
    public record PatchCandidateCommand(int CampaignId, int CandidateId, CandidatePatchDTO Candidate) : IRequest<int>;

    public class PatchCandidateHandler : IRequestHandler<PatchCandidateCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public PatchCandidateHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(PatchCandidateCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("PatchCandidate", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
