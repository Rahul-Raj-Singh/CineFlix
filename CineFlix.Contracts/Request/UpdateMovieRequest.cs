namespace CineFlix.Contracts.Request;

public class UpdateMovieRequest
{
    public Guid MovieId { get; init; }
    public string MovieName { get; init; }
    public int YearOfRelease { get; init; }
    public IEnumerable<string> Genres { get; init; } = [];
}