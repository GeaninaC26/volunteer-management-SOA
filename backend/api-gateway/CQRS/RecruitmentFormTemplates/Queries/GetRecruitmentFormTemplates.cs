using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.RecruitmentFormTemplates.Queries
{
    public record GetRecruitmentFormTemplatesQuery(string? Name) : IRequest<List<RecruitmentFormTemplateDTO>>;

    public class GetRecruitmentFormTemplatesHandler : IRequestHandler<GetRecruitmentFormTemplatesQuery, List<RecruitmentFormTemplateDTO>>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetRecruitmentFormTemplatesHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<List<RecruitmentFormTemplateDTO>> Handle(GetRecruitmentFormTemplatesQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetRecruitmentFormTemplates", request);
            return await _messageProducer.CallAsync<Message, List<RecruitmentFormTemplateDTO>>(message, "recruitment_queue", cancellationToken);
        }
    }
}
