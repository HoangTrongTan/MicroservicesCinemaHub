namespace Movie.API.Models;
public class Auditorium
{
    public Guid Id { get; set; }
    public Guid CinemaId { get; set; }
    public string Name { get; set; } = default!; // VD: "Phòng 1 - IMAX"
    public List<SeatDefinition> SeatLayout { get; set; } = [];
}

public record SeatDefinition(
  string Row,
  int Column,
  SeatType Type,
  string? PairSeatCode = null // Nếu là Couple: "J1" trỏ sang "J2" cho Seating.API dễ khóa cặp!
);

public enum SeatType
{
    Standard,
    VIP,
    Couple,
    Aisle // Lối đi - nhét thẳng vào đây vẽ grid 10x15 cực nhàn!
}