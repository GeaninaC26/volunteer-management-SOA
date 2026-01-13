using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VolunteerManagement.Model;
using VolunteerManagement.CQRS.InterviewTemplates.Commands;
using VolunteerManagement.CQRS.InterviewTemplates.Queries;

namespace VolunteerManagement.API.Endpoints
{
    public static class InterviewTemplates
    {
        public static WebApplication MapInterviewTemplatesEndpoints(this WebApplication app)
        {
            var interviewTemplates = app.MapGroup("/api/interview_templates");

            interviewTemplates.MapGet("/", async ([FromQuery] string? name, IMediator mediator) =>
            {
                var interviewTemplates = await mediator.Send(new GetInterviewTemplatesQuery(name));
                return Results.Ok(interviewTemplates);
            })
            .Produces<List<InterviewTemplateDTO>>(200);


            interviewTemplates.MapPost("/", async ([FromBody] InterviewTemplateDTO template, IMediator mediator) =>
            {
                var id = await mediator.Send(new CreateInterviewTemplateCommand(template));
                return Results.Ok(id);
            })
            .Accepts<InterviewTemplateDTO>("application/json")
            .ProducesValidationProblem()
            .Produces<int>(200);

            interviewTemplates.MapGet("/{id}", async ([FromRoute] int id, IMediator mediator) =>
            {
                var interviewTemplate = await mediator.Send(new GetInterviewTemplateByIdQuery(id));
                if (interviewTemplate == null)
                    return Results.NotFound();
                return Results.Ok(interviewTemplate);
            })
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .Produces<InterviewTemplateDTO>(200);


            interviewTemplates.MapDelete("/{id}", async ([FromRoute] int id, IMediator mediator) =>
            {
                var resId = await mediator.Send(new DeleteInterviewTemplateCommand(id));
                if (resId == 0)
                    return Results.NotFound();
                return Results.Ok();
            })
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .Produces(200);

            interviewTemplates.MapPost("/{id}/questions", async ([FromRoute] int id, [FromBody] string question, IMediator mediator) =>
            {
                var interviewTemplate = await mediator.Send(new AddQuestionToInterviewTemplateCommand(id, question));
                if (interviewTemplate == null) return Results.NotFound();
                return Results.Ok(interviewTemplate);
            })
            .Accepts<string>("application/json")
            .ProducesValidationProblem()
            .ProducesProblem(404)
            .Produces<InterviewTemplateDTO>(200)
            .RequireAuthorization("AdminOnly");

            interviewTemplates.WithTags("Interview templates");

            return app;
        }
    }
}