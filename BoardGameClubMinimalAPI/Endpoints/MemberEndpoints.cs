using Microsoft.AspNetCore.Http;
using BoardGameClubMinimalAPI.Models;
using BoardGameClubMinimalAPI.Data;

namespace BoardGameClubMinimalAPI.Endpoints;

public static class MemberEndpoints
{
    public static void MapMemberEndpoints(this WebApplication app)
    {
        app.MapGet("/members", () => Results.Ok(DataStore.Members))
            .WithName("GetAllMembers")
            .WithOpenApi()
            .WithSummary("Отримати всіх членів клубу")
            .Produces<List<Member>>(StatusCodes.Status200OK);

        app.MapGet("/members/{id:int}", (int id) =>
        {
            var member = DataStore.Members.FirstOrDefault(m => m.Id == id);
            return member != null ? Results.Ok(member) : Results.NotFound();
        })
            .WithName("GetMemberById")
            .WithOpenApi()
            .WithSummary("Отримати члена клубу за ID")
            .Produces<Member>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/members", (Member newMember) =>
        {
            if (!Validator.ValidateString(newMember.Name, 3, "Name", out var errorName)) return Results.BadRequest(errorName);
            if (!Validator.ValidateEmail(newMember.Email, out var errorEmail)) return Results.BadRequest(errorEmail);
            newMember.Id = DataStore.Members.Any() ? DataStore.Members.Max(m => m.Id) + 1 : 1;
            DataStore.Members.Add(newMember);
            return Results.Created($"/members/{newMember.Id}", newMember);
        })
            .WithName("CreateMember")
            .WithOpenApi()
            .WithSummary("Створити нового члена клубу")
            .Produces<Member>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/members/{id:int}", (int id, Member updatedMember) =>
        {
            var member = DataStore.Members.FirstOrDefault(m => m.Id == id);
            if (member == null) return Results.NotFound();
            if (!Validator.ValidateString(updatedMember.Name, 3, "Name", out var errorName)) return Results.BadRequest(errorName);
            if (!Validator.ValidateEmail(updatedMember.Email, out var errorEmail)) return Results.BadRequest(errorEmail);
            member.Name = updatedMember.Name;
            member.Email = updatedMember.Email;
            return Results.Ok(member);
        })
            .WithName("UpdateMember")
            .WithOpenApi()
            .WithSummary("Оновити члена клубу повністю")
            .Produces<Member>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPatch("/members/{id:int}", (int id, Member patchMember) =>
        {
            var member = DataStore.Members.FirstOrDefault(m => m.Id == id);
            if (member == null) return Results.NotFound();
            if (!string.IsNullOrEmpty(patchMember.Name))
            {
                if (!Validator.ValidateString(patchMember.Name, 3, "Name", out var error)) return Results.BadRequest(error);
                member.Name = patchMember.Name;
            }
            if (!string.IsNullOrEmpty(patchMember.Email))
            {
                if (!Validator.ValidateEmail(patchMember.Email, out var error)) return Results.BadRequest(error);
                member.Email = patchMember.Email;
            }
            return Results.Ok(member);
        })
            .WithName("PatchMember")
            .WithOpenApi()
            .WithSummary("Оновити члена клубу частково")
            .Produces<Member>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapDelete("/members/{id:int}", (int id) =>
        {
            var member = DataStore.Members.FirstOrDefault(m => m.Id == id);
            if (member == null) return Results.NotFound();
            DataStore.Members.Remove(member);
            return Results.NoContent();
        })
            .WithName("DeleteMember")
            .WithOpenApi()
            .WithSummary("Видалити члена клубу")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}