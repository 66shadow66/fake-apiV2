namespace BoardGameClubMinimalAPI.Models;

public class Session
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int MemberId { get; set; }
    public DateTime DateTime { get; set; }
}