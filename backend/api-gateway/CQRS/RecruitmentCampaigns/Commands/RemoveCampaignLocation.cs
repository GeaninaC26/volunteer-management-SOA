using MediatR;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.RecruitmentCampaigns.Commands
{
    public record RemoveCampaignLocationCommand(int CampaignId, int LocationId) : IRequest<int>;

    public class RemoveCampaignLocationHandler : IRequestHandler<RemoveCampaignLocationCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public RemoveCampaignLocationHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(RemoveCampaignLocationCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("RemoveCampaignLocation", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
