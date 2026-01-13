using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Candidates.Commands
{
    public record CreateCandidateCommand(int CampaignId, CandidateDTO Candidate) : IRequest<int>;

    public class CreateCandidateHandler : IRequestHandler<CreateCandidateCommand, int>
    {
        private readonly RabbitMQProducer _messageProducer;

        public CreateCandidateHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<int> Handle(CreateCandidateCommand request, CancellationToken cancellationToken)
        {
            var message = new Message("CreateCandidate", request);
            return await _messageProducer.CallAsync<Message, int>(message, "recruitment_queue", cancellationToken);
        }
    }
}
