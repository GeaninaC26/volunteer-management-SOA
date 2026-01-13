using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.RecruitmentCampaigns.Commands
{
    public record CreateRecruitmentCampaignCommand(RecruitmentCampaignDTO Campaign) : IRequest<int>;

    public class CreateRecruitmentCampaignHandler : IRequestHandler<CreateRecruitmentCampaignCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public CreateRecruitmentCampaignHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(CreateRecruitmentCampaignCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("CreateRecruitmentCampaign", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
