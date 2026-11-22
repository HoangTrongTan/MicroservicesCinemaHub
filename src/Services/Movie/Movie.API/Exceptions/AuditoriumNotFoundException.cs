namespace Movies.API.Exceptions;

public class AuditoriumNotFoundException(Guid Id) : NotFoundException("Auditorium", Id)
{

}