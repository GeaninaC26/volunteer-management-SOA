using Microsoft.AspNetCore.Mvc;
using MediatR;
using VolunteerManagement.Model;
using VolunteerManagement.CQRS.RecruitmentCampaigns.Commands;
using VolunteerManagement.CQRS.RecruitmentCampaigns.Queries;
using VolunteerManagement.CQRS.Candidates.Commands;
using VolunteerManagement.CQRS.Candidates.Queries;
using VolunteerManagement.CQRS.BlockedPeriods.Commands;
using VolunteerManagement.CQRS.BlockedPeriods.Queries;

namespace VolunteerManagement.API.Endpoints;

public static class RecruitmentCampaignsEndpoints
{
    public static WebApplication MapRecruitmentCampaignsEndpoints(this WebApplication app)
    {
        var campaigns = app.MapGroup("/api/campaigns");

        campaigns.MapGet("/", async ([FromQuery] string? name, [FromQuery] bool? ongoing, IMediator mediator) =>
        {
            var campaigns = await mediator.Send(new GetRecruitmentCampaignsQuery(name, ongoing));
            return Results.Ok(campaigns);
        })
        .ProducesValidationProblem()
        .Produces<List<RecruitmentCampaignDTO>>(200)
        .AllowAnonymous();

        campaigns.MapPost("/", async ([FromBody] RecruitmentCampaignDTO campaign, IMediator mediator) =>
        {
            var id = await mediator.Send(new CreateRecruitmentCampaignCommand(campaign));
            return Results.Ok(id);
        })
        .Accepts<RecruitmentCampaignDTO>("application/json")
        .ProducesValidationProblem()
        .Produces<int>(200)
        .RequireAuthorization("AdminOnly");

        campaigns.MapGet("/{id}", async ([FromRoute] int id, IMediator mediator) =>
        {
            var campaign = await mediator.Send(new GetRecruitmentCampaignByIdQuery(id));
            if (campaign == null)
                return Results.NotFound();
            else
                return Results.Ok(campaign);
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces<RecruitmentCampaignDTO>(200);

        campaigns.MapPatch("/{id}", async ([FromRoute] int id, [FromBody] RecruitmentCampaignPatchDTO campaign, IMediator mediator) =>
        {
            var resId = await mediator.Send(new PatchRecruitmentCampaignCommand(id, campaign));
            if (resId == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok(200);
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        campaigns.MapDelete("/{id}", async ([FromRoute] int id, IMediator mediator) =>
        {
            var resId = await mediator.Send(new DeleteRecruitmentCampaignCommand(id));
            if (resId == 0)
            {
                return Results.NotFound();
            }

            return Results.Ok();
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        campaigns.WithTags("Campaigns");

        // Candidates endpoints
        var candidates = campaigns.MapGroup("/{cId}/candidates/");
        candidates.MapGet("/", async ([FromRoute]int cId, [FromQuery] string? status, IMediator mediator) =>
        {
            Console.WriteLine(cId);
            var candidates = await mediator.Send(new GetCandidatesQuery(cId, status));
            return Results.Ok(candidates);
        })
        .Produces<List<CandidateDTO>>(200);

        candidates.MapPost("/", async ([FromRoute] int cId, [FromBody] CandidateDTO candidate, IMediator mediator) =>
        {
            var id = await mediator.Send(new CreateCandidateCommand(cId, candidate));
            if (id == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok(id);
        })
        .Accepts<CandidateDTO>("application/json")
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces<int>(200)
        .AllowAnonymous();

        candidates.MapGet("/{id}", async ([FromRoute] int cId, [FromRoute] int id, IMediator mediator) =>
        {
            var candidate = await mediator.Send(new GetCandidateByIdQuery(cId, id));
            if (candidate is null)
                return Results.NotFound();
            else
                return Results.Ok(candidate);
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces<CandidateDTO>(200);

        candidates.MapPatch("/{id}", async ([FromRoute] int cId, [FromRoute] int id, [FromBody] CandidatePatchDTO candidate, IMediator mediator) =>
        {
            var resId = await mediator.Send(new PatchCandidateCommand(cId, id, candidate));
            if (resId == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok(200);
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);


        candidates.MapDelete("/{id}", async ([FromRoute] int cId, [FromRoute] int id, IMediator mediator) =>
        {
            var resId = await mediator.Send(new DeleteCandidateCommand(cId, id));
            if (resId == 0)
            {
                return Results.NotFound();
            }

            return Results.Ok();
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);


        candidates.MapGet("/{id}/info", async ([FromRoute]int cId, [FromRoute]int id, IMediator mediator) =>
        {
            var info = await mediator.Send(new GetCandidateInfoQuery(cId, id));
            if (info == null)
                return Results.NotFound();
            return Results.Ok(info);
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        candidates.MapPut("/{id}/info", async ([FromRoute] int cId, [FromRoute] int id, [FromBody] PersonalInfoDTO personalInfo, IMediator mediator) =>
        {
            var resId = await mediator.Send(new UpdateCandidateInfoCommand(cId, id, personalInfo));
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

        candidates.MapPatch("/{id}/info", async ([FromRoute] int cId, [FromRoute] int id, [FromBody] PersonalInfoPatchDTO personalInfo, IMediator mediator) =>
        {
            var resId = await mediator.Send(new PatchCandidateInfoCommand(cId, id, personalInfo));
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

        candidates.WithTags("Campaigns - Candidates");

        // Locations endpoints
        
        var locations = campaigns.MapGroup("/{cId}/locations/");

        locations.MapGet("/", async ([FromRoute] int cId, IMediator mediator) =>
        {
            var locations = await mediator.Send(new GetCampaignLocationsQuery(cId));
            return Results.Ok(locations);
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces<List<LocationDTO>>(200);
        
        locations.MapPost("/", async ([FromRoute] int cId, [FromBody] int id, IMediator mediator) =>
        {
            var resId = await mediator.Send(new AddCampaignLocationCommand(cId, id));
            if (resId == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok();
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        locations.MapDelete("/", async ([FromRoute] int cId, [FromQuery] int id, IMediator mediator) =>
        {
            var resId = await mediator.Send(new RemoveCampaignLocationCommand(cId, id));
            if (resId == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok();
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        locations.WithTags("Campaigns - Locations");

        // Blocked periods endpoints
        var blockedPeriods = campaigns.MapGroup("/{cId}/blocked_periods/");
        blockedPeriods.MapGet("/", async ([FromRoute] int cId, IMediator mediator) =>
        {
            var periods = await mediator.Send(new GetBlockedPeriodsQuery(cId));
            return Results.Ok(periods);
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces<List<BlockedPeriod>>(200);

        blockedPeriods.MapPost("/", async ([FromRoute] int cId, [FromBody] BlockedPeriodDTO blockedPeriod, IMediator mediator) =>
        {
            var id = await mediator.Send(new CreateBlockedPeriodCommand(cId, blockedPeriod));
            if (id == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok(id);
        })
        .Accepts<BlockedPeriodDTO>("application/json")
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces<int>(200);

        blockedPeriods.MapGet("/{id}", async ([FromRoute] int cId, [FromRoute] int id, IMediator mediator) =>
        {
            var period = await mediator.Send(new GetBlockedPeriodByIdQuery(cId, id));
            if (period == null)
                return Results.NotFound();
            else
                return Results.Ok(period);
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces<BlockedPeriod>(200);

        blockedPeriods.MapPatch("/{id}", async ([FromRoute] int cId, [FromRoute] int id, [FromBody] BlockedPeriodPatchDTO blockedPeriod, IMediator mediator) =>
        {
            var resId = await mediator.Send(new PatchBlockedPeriodCommand(cId, id, blockedPeriod));
            if (resId == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok(200);
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        blockedPeriods.MapDelete("/{id}", async ([FromRoute] int cId, [FromRoute] int id, IMediator mediator) =>
        {
            var resId = await mediator.Send(new DeleteBlockedPeriodCommand(cId, id));
            if (resId == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok();
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        blockedPeriods.WithTags("Campaigns - Blocked Periods");

        // Volunteers endpoints
        var volunteers = campaigns.MapGroup("/{cId}/volunteers/");
        volunteers.MapGet("/", async ([FromRoute] int cId, [FromQuery] string? name, [FromQuery] bool? outside, IMediator mediator) =>
        {
            var volunteers = await mediator.Send(new GetCampaignVolunteersQuery(cId, name, outside));
            return Results.Ok(volunteers);
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces<List<VolunteerDTO>>(200);

        volunteers.MapPost("/", async ([FromRoute] int cId, [FromQuery] int id, IMediator mediator) =>
        {
            var resId = await mediator.Send(new AddCampaignVolunteerCommand(cId, id));
            if (resId == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok();
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        volunteers.MapDelete("/", async ([FromRoute] int cId, [FromQuery] int id, IMediator mediator) =>
        {
            var resId = await mediator.Send(new RemoveCampaignVolunteerCommand(cId, id));
            if (resId == 0)
            {
                return Results.NotFound();
            }
            return Results.Ok();
        })
        .ProducesValidationProblem()
        .ProducesProblem(404)
        .Produces(200);

        volunteers.WithTags("Campaigns - Volunteers");

        //

        return app;
    }
}