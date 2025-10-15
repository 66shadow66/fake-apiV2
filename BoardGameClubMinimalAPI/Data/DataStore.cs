using BoardGameClubMinimalAPI.Models;

namespace BoardGameClubMinimalAPI.Data;

public static class DataStore
{
    public static List<Member> Members { get; } = new()
    {
        new Member { Id = 1, Name = "Oleh", Email = "oleh@example.com" },
        new Member { Id = 2, Name = "Ira", Email = "ira@example.com" }
    };

    public static List<Game> Games { get; } = new()
    {
        new Game { Id = 1, Title = "Carcassonne", MinPlayers = 2, MaxPlayers = 5 }
    };

    public static List<Session> Sessions { get; } = new()
    {
        new Session { Id = 1, GameId = 1, MemberId = 2, DateTime = DateTime.Now.AddDays(5) }
    };
}