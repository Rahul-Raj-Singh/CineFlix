namespace CineFlix.Contracts.Response;

public class MovieResponse
{
    public Guid MovieId { get; init; }
    public string Slug { get; init; }
    public string MovieName { get; init; }
    public int YearOfRelease { get; init; }
    public IEnumerable<string> Genres { get; init; } = [];
}