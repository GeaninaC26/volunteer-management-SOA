using MediatR;
using Microsoft.AspNetCore.Mvc;
using VolunteerManagement.CQRS.Volunteers.Commands;
using VolunteerManagement.CQRS.Volunteers.Queries;
using VolunteerManagement.Model;

namespace VolunteerManagement.API.Endpoints;
public static class VolunteersEndpoints
{
    public static WebApplication MapVolunteersEndpoints(this WebApplication app)
    {
        var volunteers = app.MapGroup("/api/volunteers");

        volunteers.MapGet("/", async ([FromQuery] string? department, [FromQuery] string? volunteerStatus, IMediator mediator) =>
        {
            var volunteers = await mediator.Send(new GetVolunteersQuery(department, volunteerStatus));
            return Results.Ok(volunteers);
        })
        .Produces<List<VolunteerDTO>>(200);

        volunteers.MapPost("/", async ([FromBody] VolunteerDTO volunteer, IMediator mediator) =>
        {
            var id = await mediator.Send(new CreateVolunteerCommand(volunteer));
            return Results.Ok(id);
        })
        .Accepts<VolunteerDTO>("application/json")
        .ProducesValidationProblem()
        .Produces<int>(200);

        volunteers.MapGet("/{id}", async ([FromRoute] int id, IMediator mediator) =>
        {
            var volunteer = await mediator.Send(new GetVolunteerByIdQuery(id));
            if (volunteer == null)
                return Results.NotFound();
            else
                return Results.Ok(volunteer);
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces<VolunteerDTO>(200);

        volunteers.MapPatch("/{id}", async ([FromRoute] int id, [FromBody] VolunteerPatchDTO volunteer, IMediator mediator) =>
        {
            var resId = await mediator.Send(new PatchVolunteerCommand(id, volunteer));
            if (resId == 0)
                return Results.NotFound();
            return Results.Ok();
        })
        .Accepts<VolunteerPatchDTO>("application/json")
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        volunteers.MapDelete("/{id}", async ([FromRoute] int id, IMediator mediator) =>
        {
            var resId = await mediator.Send(new DeleteVolunteerCommand(id));
            if (resId == 0)
                return Results.NotFound();
            return Results.Ok();
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        volunteers.MapGet("/{id}/info", async (int id, IMediator mediator) =>
        {
            var info = await mediator.Send(new GetVolunteerInfoQuery(id));
            if (info == null)
                return Results.NotFound();
            return Results.Ok(info);
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        volunteers.MapPut("/{id}/info", async ([FromRoute] int id, [FromBody] PersonalInfoDTO personalInfo, IMediator mediator) =>
        {
            var resId = await mediator.Send(new UpdateVolunteerInfoCommand(id, personalInfo));
            if (resId == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok();
        })
        .Accepts<PersonalInfoDTO>("application/json")
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        volunteers.MapPatch("/{id}/info", async ([FromRoute] int id, [FromBody] PersonalInfoPatchDTO personalInfo, IMediator mediator) =>
        {
            var resId = await mediator.Send(new PatchVolunteerInfoCommand(id, personalInfo));
            if (resId == 0)
            {
                return Results.NotFound();
            }

            return Results.Ok();
        })
        .Accepts<PersonalInfoPatchDTO>("application/json")
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        volunteers.WithTags("Volunteers");

        return app;
    }
}