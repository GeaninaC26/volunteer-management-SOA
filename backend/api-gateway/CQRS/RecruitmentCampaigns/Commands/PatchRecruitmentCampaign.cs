using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.RecruitmentCampaigns.Commands
{
    public record PatchRecruitmentCampaignCommand(int Id, RecruitmentCampaignPatchDTO Campaign) : IRequest<int>;

    public class PatchRecruitmentCampaignHandler : IRequestHandler<PatchRecruitmentCampaignCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public PatchRecruitmentCampaignHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(PatchRecruitmentCampaignCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("PatchRecruitmentCampaign", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
