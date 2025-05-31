using CineFlix.Application.Database;
using CineFlix.Application.Models;
using CineFlix.Contracts.Request;
using Dapper;

namespace CineFlix.Application.Repositories;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> GetAll();
    Task<Movie> GetById(Guid movieId);
    Task<Movie> GetBySlug(string slug);
    Task<Movie> Create(CreateMovieRequest request);
    Task<bool> Update(UpdateMovieRequest request);
    Task<bool> Exists(Guid movieId);
}
public class MovieRepository(IDbConnectionFactory dbConnectionFactory) : IMovieRepository
{
    public async Task<IEnumerable<Movie>> GetAll()
    {
        using var connection = await dbConnectionFactory.GetConnection();
        
        const string sql = """
                           select m.movie_id, m.movie_name, m.slug, m.year_of_release, string_agg(g.genre, ',') genres 
                           from dbo.movies m
                           join dbo.genres g on m.movie_id = g.movie_id
                           group by m.movie_id, m.movie_name, m.slug, m.year_of_release
                           """;
        
        var rows =  await connection.QueryAsync(sql);
        
        return rows.Select(row => new Movie
        {
            MovieId = row.movie_id, MovieName = row.movie_name, YearOfRelease = row.year_of_release, Genres = row.genres.ToString().Split(",")
        });
    }

    public async Task<Movie> GetById(Guid movieId)
    {
        using var connection = await dbConnectionFactory.GetConnection();

        const string sql = """
                           select m.movie_id, m.movie_name, m.slug, m.year_of_release, string_agg(g.genre, ',') genres 
                           from dbo.movies m
                           join dbo.genres g on m.movie_id = g.movie_id
                           where m.movie_id = @movieId
                           group by m.movie_id, m.movie_name, m.slug, m.year_of_release
                           """;
        
        var row =  await connection.QueryFirstOrDefaultAsync(sql, new { movieId });
        
        if (row is null) return null;

        return new Movie
        {
            MovieId = row.movie_id, MovieName = row.movie_name, YearOfRelease = row.year_of_release, Genres = row.genres.ToString().Split(",")
        };
    }

    public async Task<Movie> GetBySlug(string slug)
    {
        using var connection = await dbConnectionFactory.GetConnection();

        const string sql = """
                           select m.movie_id, m.movie_name, m.slug, m.year_of_release, string_agg(g.genre, ',') genres 
                           from dbo.movies m
                           join dbo.genres g on m.movie_id = g.movie_id
                           where m.slug = @slug
                           group by m.movie_id, m.movie_name, m.slug, m.year_of_release
                           """;
        
        var row =  await connection.QueryFirstOrDefaultAsync(sql, new { slug });
        
        if (row is null) return null;
        
        return new Movie
        {
            MovieId = row.movie_id, MovieName = row.movie_name, YearOfRelease = row.year_of_release, Genres = row.genres.ToString().Split(",")
        };
    }

    public async Task<Movie> Create(CreateMovieRequest request)
    {
        var newMovie = new Movie
        {
            MovieId = Guid.NewGuid(),
            MovieName = request.MovieName,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres
        };
        
        using var connection = await dbConnectionFactory.GetConnection();
        
        using var transaction = connection.BeginTransaction();

        const string movieInsertSql = """
                           insert into dbo.movies
                           (movie_id, movie_name, slug, year_of_release)
                           values
                           (@MovieId, @MovieName, @Slug, @YearOfRelease)
                           """;

        await connection.ExecuteAsync(movieInsertSql, newMovie);
        
        const string genresInsertSql = """
                                      insert into dbo.genres
                                      (movie_id, genre)
                                      values
                                      (@MovieId, @Genre)
                                      """; 
        
        foreach (var genre in request.Genres)
            await connection.ExecuteAsync(genresInsertSql, new {MovieId = newMovie.MovieId, Genre = genre});
        
        transaction.Commit();
        
        return newMovie;
    }

    public async Task<bool> Update(UpdateMovieRequest request)
    {
        using var connection = await dbConnectionFactory.GetConnection();
        
        using var transaction = connection.BeginTransaction();

        var movieUpdateSql = """
                             update dbo.movies
                             set movie_name = @MovieName, year_of_release = @YearOfRelease
                             where movie_id = @MovieId
                             """;

        var rowsAffected = 
            await connection.ExecuteAsync(movieUpdateSql, new {request.MovieId, request.MovieName, request.YearOfRelease});
        
        const string genresDeleteSql = "delete from dbo.genres where movie_id = @MovieId";
        
        await connection.ExecuteAsync(genresDeleteSql, new {request.MovieId});
        
        const string genresInsertSql = """
                                       insert into dbo.genres
                                       (movie_id, genre)
                                       values
                                       (@MovieId, @Genre)
                                       """; 
        
        foreach (var genre in request.Genres)
            await connection.ExecuteAsync(genresInsertSql, new {MovieId = request.MovieId, Genre = genre});
        
        transaction.Commit();

        return rowsAffected > 0;
    }

    public async Task<bool> Exists(Guid movieId)
    {
        using var connection = await dbConnectionFactory.GetConnection();

        const string sql = "select count(1) from dbo.movies where movie_id = @movieId";
        
        var count =  await connection.ExecuteScalarAsync<int>(sql, new { movieId });
        
        return count > 0;
    }
}