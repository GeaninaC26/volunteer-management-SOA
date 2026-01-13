using Microsoft.AspNetCore.Mvc;
using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.CQRS.Locations.Commands;
using VolunteerManagement.CQRS.Locations.Queries;

namespace VolunteerManagement.API.Endpoints;

public static class LocationsEndpoints
{
    public static WebApplication MapLocationsEndpoints(this WebApplication app)
    {
        var locations = app.MapGroup("/api/locations");

        locations.MapGet("/", async ([FromQuery] string? name, IMediator mediator, ILogger<Program> logger) =>
        {
            logger.LogInformation("Getting locations with filter: {Name}", name ?? "none");
            var locations = await mediator.Send(new GetLocationsQuery(name));
            logger.LogInformation("Retrieved {Count} locations", locations?.Count ?? 0);

            return Results.Ok(locations);
        })
        .Produces<List<Location>>(200);

        locations.MapPost("/", async ([FromBody] LocationDTO location, IMediator mediator, ILogger<Program> logger) =>
        {
            logger.LogInformation("Creating new location: {LocationName}", location.Name);
            var id = await mediator.Send(new CreateLocationCommand(location));
            logger.LogInformation("Created location with ID: {LocationId}", id);
            return Results.Ok(id);
        })
        .Accepts<LocationDTO>("application/json")
        .ProducesValidationProblem()
        .Produces<Location>(200);

        locations.MapGet("/{id}", async ([FromRoute] int id, IMediator mediator, ILogger<Program> logger) =>
        {
            logger.LogInformation("Getting location by ID: {LocationId}", id);
            var location = await mediator.Send(new GetLocationByIdQuery(id));
            if (location is null)
            {
                logger.LogWarning("Location not found with ID: {LocationId}", id);
                return Results.NotFound();
            }
            else
            {
                logger.LogInformation("Successfully retrieved location: {LocationName} (ID: {LocationId})", location.Name, id);
                return Results.Ok(location);
            }
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces<Location>(200);


        locations.MapPatch("/{id}", async ([FromRoute] int id, [FromBody] LocationPatchDTO location, IMediator mediator, ILogger<Program> logger) =>
        {
            logger.LogInformation("Updating location with ID: {LocationId}", id);
            var resId = await mediator.Send(new PatchLocationCommand(id, location));
            if (resId == 0)
            {
                logger.LogWarning("Location not found for update with ID: {LocationId}", id);
                return Results.NotFound();
            }
            logger.LogInformation("Successfully updated location with ID: {LocationId}", id);
            return Results.Ok();
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        locations.MapDelete("/{id}", async ([FromRoute] int id, IMediator mediator, ILogger<Program> logger) =>
        {
            logger.LogInformation("Deleting location with ID: {LocationId}", id);
            var resId = await mediator.Send(new DeleteLocationCommand(id));
            if (resId == 0)
            {
                logger.LogWarning("Location not found for deletion with ID: {LocationId}", id);
                return Results.NotFound();
            }
            logger.LogInformation("Successfully deleted location with ID: {LocationId}", id);
            return Results.Ok();
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        locations.WithTags("Locations");

        return app;
    }
}