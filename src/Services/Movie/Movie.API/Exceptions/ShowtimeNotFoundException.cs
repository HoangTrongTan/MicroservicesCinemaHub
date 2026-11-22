namespace Movies.API.Exceptions;

public class ShowtimeNotFoundException(Guid Id) : NotFoundException("Showtime", Id)
{
}