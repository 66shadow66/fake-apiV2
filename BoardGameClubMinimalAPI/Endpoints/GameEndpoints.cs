using Microsoft.AspNetCore.Http;
using BoardGameClubMinimalAPI.Models;
using BoardGameClubMinimalAPI.Data;

namespace BoardGameClubMinimalAPI.Endpoints;

public static class GameEndpoints
{
    public static void MapGameEndpoints(this WebApplication app)
    {
        app.MapGet("/games", () => Results.Ok(DataStore.Games))
            .WithName("GetAllGames")
            .WithOpenApi()
            .WithSummary("Отримати всі ігри")
            .Produces<List<Game>>(StatusCodes.Status200OK);

        app.MapGet("/games/{id:int}", (int id) =>
        {
            var game = DataStore.Games.FirstOrDefault(g => g.Id == id);
            return game != null ? Results.Ok(game) : Results.NotFound();
        })
            .WithName("GetGameById")
            .WithOpenApi()
            .WithSummary("Отримати гру за ID")
            .Produces<Game>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/games", (Game newGame) =>
        {
            if (!Validator.ValidateString(newGame.Title, 3, "Title", out var errorTitle)) return Results.BadRequest(errorTitle);
            if (!Validator.ValidatePlayers(newGame.MinPlayers, newGame.MaxPlayers, out var errorPlayers)) return Results.BadRequest(errorPlayers);
            newGame.Id = DataStore.Games.Any() ? DataStore.Games.Max(g => g.Id) + 1 : 1;
            DataStore.Games.Add(newGame);
            return Results.Created($"/games/{newGame.Id}", newGame);
        })
            .WithName("CreateGame")
            .WithOpenApi()
            .WithSummary("Створити нову гру")
            .Produces<Game>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPut("/games/{id:int}", (int id, Game updatedGame) =>
        {
            var game = DataStore.Games.FirstOrDefault(g => g.Id == id);
            if (game == null) return Results.NotFound();
            if (!Validator.ValidateString(updatedGame.Title, 3, "Title", out var errorTitle)) return Results.BadRequest(errorTitle);
            if (!Validator.ValidatePlayers(updatedGame.MinPlayers, updatedGame.MaxPlayers, out var errorPlayers)) return Results.BadRequest(errorPlayers);
            game.Title = updatedGame.Title;
            game.MinPlayers = updatedGame.MinPlayers;
            game.MaxPlayers = updatedGame.MaxPlayers;
            return Results.Ok(game);
        })
            .WithName("UpdateGame")
            .WithOpenApi()
            .WithSummary("Оновити гру повністю")
            .Produces<Game>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapPatch("/games/{id:int}", (int id, Game patchGame) =>
        {
            var game = DataStore.Games.FirstOrDefault(g => g.Id == id);
            if (game == null) return Results.NotFound();
            if (!string.IsNullOrEmpty(patchGame.Title))
            {
                if (!Validator.ValidateString(patchGame.Title, 3, "Title", out var error)) return Results.BadRequest(error);
                game.Title = patchGame.Title;
            }
            if (patchGame.MinPlayers != 0 || patchGame.MaxPlayers != 0)
            {
                if (!Validator.ValidatePlayers(patchGame.MinPlayers, patchGame.MaxPlayers, out var error)) return Results.BadRequest(error);
                game.MinPlayers = patchGame.MinPlayers;
                game.MaxPlayers = patchGame.MaxPlayers;
            }
            return Results.Ok(game);
        })
            .WithName("PatchGame")
            .WithOpenApi()
            .WithSummary("Оновити гру частково")
            .Produces<Game>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        app.MapDelete("/games/{id:int}", (int id) =>
        {
            var game = DataStore.Games.FirstOrDefault(g => g.Id == id);
            if (game == null) return Results.NotFound();
            DataStore.Games.Remove(game);
            return Results.NoContent();
        })
            .WithName("DeleteGame")
            .WithOpenApi()
            .WithSummary("Видалити гру")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}