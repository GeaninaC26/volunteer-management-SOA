using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Candidates.Commands
{
    public record PatchCandidateInfoCommand(int CampaignId, int CandidateId, PersonalInfoPatchDTO PersonalInfo) : IRequest<int>;

    public class PatchCandidateInfoHandler : IRequestHandler<PatchCandidateInfoCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public PatchCandidateInfoHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(PatchCandidateInfoCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("PatchCandidateInfo", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
