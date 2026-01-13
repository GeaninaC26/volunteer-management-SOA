using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.Candidates.Queries
{
    public record GetCandidateByIdQuery(int CampaignId, int CandidateId) : IRequest<CandidateDTO?>;

    public class GetCandidateByIdHandler : IRequestHandler<GetCandidateByIdQuery, CandidateDTO?>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetCandidateByIdHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<CandidateDTO?> Handle(GetCandidateByIdQuery request, CancellationToken cancellationToken)
        {
             var payload = new { CampaignId = request.CampaignId, Id = request.CandidateId };
             var message = new Message("GetCandidateById", payload);
             return await _messageProducer.CallAsync<Message, CandidateDTO?>(message, "recruitment_queue", cancellationToken);
        }
    }
}
