using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Candidates.Queries
{
    public record GetCandidatesQuery(int CampaignId, string? RecruitingStatus) : IRequest<List<CandidateDTO>>;

    public class GetCandidatesHandler : IRequestHandler<GetCandidatesQuery, List<CandidateDTO>>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetCandidatesHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<List<CandidateDTO>> Handle(GetCandidatesQuery request, CancellationToken cancellationToken)
        {
            var payload = new { CampaignId = request.CampaignId, RecruitingStatus = request.RecruitingStatus };
            var message = new Message("GetCandidates", payload);

            var result = await _messageProducer.CallAsync<Message, List<CandidateDTO>>(message, "recruitment_queue", cancellationToken);
            return result ?? new List<CandidateDTO>();
        }
    }
}
