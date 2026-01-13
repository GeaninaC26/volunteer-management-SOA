using MediatR;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.RecruitmentCampaigns.Commands
{
    public record AddCampaignLocationCommand(int CampaignId, int LocationId) : IRequest<int>;

    public class AddCampaignLocationHandler : IRequestHandler<AddCampaignLocationCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public AddCampaignLocationHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(AddCampaignLocationCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("AddCampaignLocation", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
