using Microsoft.AspNetCore.Mvc;
using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.CQRS.VolunteerDisponibilities.Commands;
using VolunteerManagement.CQRS.VolunteerDisponibilities.Queries;

namespace VolunteerManagement.API.Endpoints;

public static class VolunteerDisponibilitiesEndpoints
{
    public static WebApplication MapVolunteerDisponibilitiesEndpoints(this WebApplication app)
    {
        var disponibilities = app.MapGroup("/api/disponibilities");

        disponibilities.MapGet("/", async ([FromQuery] string? name, IMediator mediator) =>
        {
            var disponibilities = await mediator.Send(new GetVolunteerDisponibilitiesQuery());

            return Results.Ok(disponibilities);
        })
        .Produces<List<VolunteerDisponibility>>(200);

        disponibilities.MapPost("/", async ([FromBody] VolunteerDisponibilityDTO disponibility, IMediator mediator) =>
        {
            var id = await mediator.Send(new CreateVolunteerDisponibilityCommand(disponibility));
            return Results.Ok(id);
        })
        .Accepts<VolunteerDisponibilityDTO>("application/json")
        .ProducesValidationProblem()
        .Produces<VolunteerDisponibility>(200);

        disponibilities.MapGet("/{id}", async ([FromRoute] int id, IMediator mediator) =>
        {
            var disponibility = await mediator.Send(new GetVolunteerDisponibilityByIdQuery(id));
            if (disponibility is null)
                return Results.NotFound();
            else
                return Results.Ok(disponibility);
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces<VolunteerDisponibility>(200);


        disponibilities.MapPatch("/{id}", async ([FromRoute] int id, [FromBody] VolunteerDisponibilityPatchDTO disponibility, IMediator mediator) =>
        {
            var resId = await mediator.Send(new PatchVolunteerDisponibilityCommand(id, disponibility));
            if (resId == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok();
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        disponibilities.MapDelete("/{id}", async ([FromRoute] int id, IMediator mediator) =>
        {
            var resId = await mediator.Send(new DeleteVolunteerDisponibilityCommand(id));
            if (resId == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok();
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        disponibilities.WithTags("VolunteerDisponibilities");

        return app;
    }
}