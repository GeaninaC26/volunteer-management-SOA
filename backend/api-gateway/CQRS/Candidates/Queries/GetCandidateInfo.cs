using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Candidates.Queries
{
    public record GetCandidateInfoQuery(int CampaignId, int CandidateId) : IRequest<PersonalInfoDTO?>;

    public class GetCandidateInfoHandler : IRequestHandler<GetCandidateInfoQuery, PersonalInfoDTO?>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetCandidateInfoHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<PersonalInfoDTO?> Handle(GetCandidateInfoQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetCandidateInfo", request);
            return await _messageProducer.CallAsync<Message, PersonalInfoDTO?>(message, "recruitment_queue", cancellationToken);
        }
    }
}
