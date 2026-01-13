using Microsoft.AspNetCore.Mvc;
using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.CQRS.Interviews.Commands;
using VolunteerManagement.CQRS.Interviews.Queries;

namespace VolunteerManagement.API.Endpoints;
public static class InterviewsEndpoints
{
    public static WebApplication MapInterviewsEndpoints(this WebApplication app)
    {
        var interviews = app.MapGroup("/api/interviews");

        interviews.MapGet("/", async (IMediator mediator) =>
        {
            var interviews = await mediator.Send(new GetInterviewsQuery());
            return Results.Ok(interviews);
        })
        .Produces<List<InterviewDTO>>(200);

        interviews.MapPost("/", async ([FromBody] InterviewDTO interview, IMediator mediator) =>
        {
            var id = await mediator.Send(new CreateInterviewCommand(interview));
            if (id == -1)
                return Results.Conflict();
            return Results.Ok(id);
        })
        .Accepts<InterviewDTO>("application/json")
        .ProducesValidationProblem()
        .ProducesProblem(409)
        .Produces<int>(200);

        interviews.MapGet("/{id}", async ([FromRoute] int id, IMediator mediator) =>
        {
            var interview = await mediator.Send(new GetInterviewByIdQuery(id));
            if (interview == null)
                return Results.NotFound();
            else
                return Results.Ok(interview);
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces<InterviewDTO>(200);

        interviews.MapPatch("/{id}", async ([FromRoute] int id, [FromBody] InterviewPatchDTO interview, IMediator mediator) =>
        {
            var resId = await mediator.Send(new PatchInterviewCommand(id, interview));
            if (resId == 0)
                return Results.NotFound();
            return Results.Ok();
        })
        .Accepts<InterviewPatchDTO>("application/json")
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        interviews.MapDelete("/{id}", async ([FromRoute] int id, IMediator mediator) =>
        {
            var resId = await mediator.Send(new DeleteInterviewCommand(id));
            if (resId == 0)
                return Results.NotFound();
            return Results.Ok();
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);


        interviews.WithTags("Interviews");

        return app;
    }
}