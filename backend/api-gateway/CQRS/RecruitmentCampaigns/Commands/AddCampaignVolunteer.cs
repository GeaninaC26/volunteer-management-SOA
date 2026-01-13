using MediatR;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.RecruitmentCampaigns.Commands
{
    public record AddCampaignVolunteerCommand(int CampaignId, int VolunteerId) : IRequest<int>;

    public class AddCampaignVolunteerHandler : IRequestHandler<AddCampaignVolunteerCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public AddCampaignVolunteerHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(AddCampaignVolunteerCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("AddCampaignVolunteer", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
