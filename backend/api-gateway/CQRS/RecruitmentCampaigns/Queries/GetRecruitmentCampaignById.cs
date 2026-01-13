using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.RecruitmentCampaigns.Queries
{
    public record GetRecruitmentCampaignByIdQuery(int Id) : IRequest<RecruitmentCampaignDTO?>;

    public class GetRecruitmentCampaignByIdHandler : IRequestHandler<GetRecruitmentCampaignByIdQuery, RecruitmentCampaignDTO?>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetRecruitmentCampaignByIdHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<RecruitmentCampaignDTO?> Handle(GetRecruitmentCampaignByIdQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetRecruitmentCampaignById", request);
            return await _messageProducer.CallAsync<Message, RecruitmentCampaignDTO?>(message, "recruitment_queue", cancellationToken);
        }
    }
}
