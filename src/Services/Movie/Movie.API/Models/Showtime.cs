namespace Movies.API.Models;

public class Showtime
{
    public Guid Id { get; set; }
    public Guid MovieId { get; set; }
    public Guid CinemaId { get; set; }
    public Guid AuditoriumId { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public decimal BasePrice { get; set; }
    public ShowtimeStatus Status { get; set; } = ShowtimeStatus.Scheduled;
}

public enum ShowtimeStatus
{
    Scheduled,
    Cancelled
}