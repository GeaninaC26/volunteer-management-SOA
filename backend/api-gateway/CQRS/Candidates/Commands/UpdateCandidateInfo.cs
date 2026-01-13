using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Candidates.Commands
{
    public record UpdateCandidateInfoCommand(int CampaignId, int CandidateId, PersonalInfoDTO PersonalInfo) : IRequest<int>;

    public class UpdateCandidateInfoHandler : IRequestHandler<UpdateCandidateInfoCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public UpdateCandidateInfoHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(UpdateCandidateInfoCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("UpdateCandidateInfo", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
