using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.CQRS.RecruitmentFormTemplates.Commands;
using VolunteerManagement.CQRS.RecruitmentFormTemplates.Queries;

namespace VolunteerManagement.API.Endpoints;

public static class RecruitmentFormTemplate
{
  public static WebApplication MapRecruitmentFormTemplatesEndpoints(this WebApplication app)
  {
    var recruitmentFormTemplates = app.MapGroup("/api/recruitment_form_templates");

    recruitmentFormTemplates.MapGet("/", async ([FromQuery] string? name, IMediator mediator) =>
    {
      var recruitmentFormTemplates = await mediator.Send(new GetRecruitmentFormTemplatesQuery(name));
      return Results.Ok(recruitmentFormTemplates);
    })
    .Produces<List<RecruitmentFormTemplateDTO>>(200);


    recruitmentFormTemplates.MapPost("/", async ([FromBody] RecruitmentFormTemplateDTO template, IMediator mediator) =>
    {
      var id = await mediator.Send(new CreateRecruitmentFormTemplateCommand(template));
      return Results.Ok(id);
    })
    .Accepts<RecruitmentFormTemplateDTO>("application/json")
    .ProducesValidationProblem()
    .Produces<int>(200);

    recruitmentFormTemplates.MapGet("/{id}", async ([FromRoute] int id, IMediator mediator) =>
    {
      var recruitmentFormTemplate = await mediator.Send(new GetRecruitmentFormTemplateByIdQuery(id));
      if (recruitmentFormTemplate == null)
        return Results.NotFound();
      return Results.Ok(recruitmentFormTemplate);
    })
    .ProducesValidationProblem()
    .ProducesProblem(404)
    .Produces<RecruitmentFormTemplateDTO>(200)
    .AllowAnonymous();


    recruitmentFormTemplates.MapDelete("/{id}", async ([FromRoute] int id, IMediator mediator) =>
    {
      var resId = await mediator.Send(new DeleteRecruitmentFormTemplateCommand(id));
      if (resId == 0)
        return Results.NotFound();
      return Results.Ok();
    })
    .ProducesValidationProblem()
    .ProducesProblem(404)
    .Produces(200);

    recruitmentFormTemplates.MapPost("/{id}/questions", async ([FromRoute] int id, [FromBody] string question, IMediator mediator) =>
    {
      var recruitmentFormTemplate = await mediator.Send(new AddQuestionToRecruitmentFormTemplateCommand(id, question));
      if (recruitmentFormTemplate == null) return Results.NotFound();
      return Results.Ok(recruitmentFormTemplate);
    })
    .Accepts<string>("application/json")
    .ProducesValidationProblem()
    .ProducesProblem(404)
    .Produces<RecruitmentFormTemplateDTO>(200);

    recruitmentFormTemplates.WithTags("Recruitment form templates");

    return app;
  }
}
