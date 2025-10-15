using Microsoft.AspNetCore.Http;
using BoardGameClubMinimalAPI.Models;
using BoardGameClubMinimalAPI.Data;

namespace BoardGameClubMinimalAPI.Endpoints;

public static class SessionEndpoints
{
    public static void MapSessionEndpoints(this WebApplication app)
    {
        app.MapGet("/sessions", () => Results.Ok(DataStore.Sessions))
            .WithName("GetAllSessions")
            .WithOpenApi()
            .WithSummary("Отримати всі сесії")
            .Produces<List<Session>>(StatusCodes.Status200OK);

        app.MapGet("/sessions/{id:int}", (int id) =>
        {
            var session = DataStore.Sessions.FirstOrDefault(s => s.Id == id);
            return session != null ? Results.Ok(session) : Results.NotFound();
        })
            .WithName("GetSessionById")
            .WithOpenApi()
            .WithSummary("Отримати сесію за ID")
            .Produces<Session>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/sessions", (Session newSession) =>
        {
            if (!Validator.CheckSession(newSession.GameId, newSession.MemberId, out var error)) return Results.BadRequest(error);
            if (!Validator.ValidateDate(newSession.DateTime, out var errorDate)) return Results.BadRequest(errorDate);
            newSession.Id = DataStore.Sessions.Any() ? DataStore.Sessions.Max(s => s.Id) + 1 : 1;
            DataStore.Sessions.Add(newSession);
            return Results.Created($"/sessions/{newSession.Id}", newSession);
        })
            .WithName("CreateSession")
            .WithOpenApi()
            .WithSummary("Створити нову сесію")
            .Produces<Session>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/sessions/{id:int}", (int id, Session updatedSession) =>
        {
            var session = DataStore.Sessions.FirstOrDefault(s => s.Id == id);
            if (session == null) return Results.NotFound();
            if (!Validator.CheckSession(updatedSession.GameId, updatedSession.MemberId, out var error)) return Results.BadRequest(error);
            if (!Validator.ValidateDate(updatedSession.DateTime, out var errorDate)) return Results.BadRequest(errorDate);
            session.GameId = updatedSession.GameId;
            session.MemberId = updatedSession.MemberId;
            session.DateTime = updatedSession.DateTime;
            return Results.Ok(session);
        })
            .WithName("UpdateSession")
            .WithOpenApi()
            .WithSummary("Оновити сесію повністю")
            .Produces<Session>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPatch("/sessions/{id:int}", (int id, Session patchSession) =>
        {
            var session = DataStore.Sessions.FirstOrDefault(s => s.Id == id);
            if (session == null) return Results.NotFound();
            if (patchSession.GameId != 0)
            {
                if (!Validator.CheckSession(patchSession.GameId, session.MemberId, out var error)) return Results.BadRequest(error);
                session.GameId = patchSession.GameId;
            }
            if (patchSession.MemberId != 0)
            {
                if (!Validator.CheckSession(session.GameId, patchSession.MemberId, out var error)) return Results.BadRequest(error);
                session.MemberId = patchSession.MemberId;
            }
            if (patchSession.DateTime != default)
            {
                if (!Validator.ValidateDate(patchSession.DateTime, out var error)) return Results.BadRequest(error);
                session.DateTime = patchSession.DateTime;
            }
            return Results.Ok(session);
        })
            .WithName("PatchSession")
            .WithOpenApi()
            .WithSummary("Оновити сесію частково")
            .Produces<Session>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapDelete("/sessions/{id:int}", (int id) =>
        {
            var session = DataStore.Sessions.FirstOrDefault(s => s.Id == id);
            if (session == null) return Results.NotFound();
            DataStore.Sessions.Remove(session);
            return Results.NoContent();
        })
            .WithName("DeleteSession")
            .WithOpenApi()
            .WithSummary("Видалити сесію")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}