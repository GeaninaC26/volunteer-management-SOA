using MediatR;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.RecruitmentCampaigns.Commands
{
    public record RemoveCampaignVolunteerCommand(int CampaignId, int VolunteerId) : IRequest<int>;

    public class RemoveCampaignVolunteerHandler : IRequestHandler<RemoveCampaignVolunteerCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public RemoveCampaignVolunteerHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(RemoveCampaignVolunteerCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("RemoveCampaignVolunteer", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
