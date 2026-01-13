using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.Messaging;

namespace VolunteerManagement.CQRS.RecruitmentFormTemplates.Queries
{
    public record GetRecruitmentFormTemplateByIdQuery(int Id) : IRequest<RecruitmentFormTemplateDTO?>;

    public class GetRecruitmentFormTemplateByIdHandler : IRequestHandler<GetRecruitmentFormTemplateByIdQuery, RecruitmentFormTemplateDTO?>
    {
        private readonly RabbitMQProducer _messageProducer;

        public GetRecruitmentFormTemplateByIdHandler(RabbitMQProducer messageProducer)
        {
            _messageProducer = messageProducer;
        }

        public async Task<RecruitmentFormTemplateDTO?> Handle(GetRecruitmentFormTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            var message = new Message("GetRecruitmentFormTemplateById", request);
            return await _messageProducer.CallAsync<Message, RecruitmentFormTemplateDTO?>(message, "recruitment_queue", cancellationToken);
        }
    }
}
