using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.RecruitmentCampaigns.Queries
{
    public record GetRecruitmentCampaignsQuery(string? Name, bool? Ongoing) : IRequest<List<RecruitmentCampaignDTO>>;

    public class GetRecruitmentCampaignsHandler : IRequestHandler<GetRecruitmentCampaignsQuery, List<RecruitmentCampaignDTO>>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetRecruitmentCampaignsHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<List<RecruitmentCampaignDTO>> Handle(GetRecruitmentCampaignsQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetRecruitmentCampaigns", request);
            return await _messageProducer.CallAsync<Message, List<RecruitmentCampaignDTO>>(message, "recruitment_queue", cancellationToken);
        }
    }
}
