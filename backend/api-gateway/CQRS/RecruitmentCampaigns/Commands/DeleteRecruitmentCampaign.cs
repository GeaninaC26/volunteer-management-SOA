using MediatR;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.RecruitmentCampaigns.Commands
{
    public record DeleteRecruitmentCampaignCommand(int Id) : IRequest<int>;

    public class DeleteRecruitmentCampaignHandler : IRequestHandler<DeleteRecruitmentCampaignCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public DeleteRecruitmentCampaignHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(DeleteRecruitmentCampaignCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("DeleteRecruitmentCampaign", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
