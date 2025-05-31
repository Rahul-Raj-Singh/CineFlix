using System.Text.Json;
using System.Text.Json.Nodes;
using Dapper;

namespace CineFlix.Application.Database;

public class DbInitializer(IDbConnectionFactory connectionFactory)
{
    public async Task InitializeAsync()
    {
        using var connection = await connectionFactory.GetConnection();

        const string sqlDDL = """
                           create table if not exists dbo.movies
                           (
                               movie_id uuid primary key,
                               movie_name text not null,
                               slug text not null,
                               year_of_release int not null
                           );

                           create table if not exists dbo.genres
                           (
                               genre_id serial primary key,
                               movie_id uuid,
                               genre text not null,
                               foreign key(movie_id) references dbo.movies(movie_id)
                           );

                           do $$
                           begin
                               if not exists (SELECT 1
                                       FROM pg_constraint
                                       WHERE conname = 'unique_slug')
                               then
                                   alter table dbo.movies
                                   add constraint unique_slug unique(slug);
                               end if;
                           end
                           $$;
                           """;

        await connection.ExecuteAsync(sqlDDL);

    }

    public async Task SeedAsync()
    {
        var json = await File.ReadAllTextAsync("/Users/rahulrajsingh/Developer/Learning-C#/CineFlix/movies.json");
        
        var movies = JsonSerializer.Deserialize<JsonArray>(json);
        var distinctMovies = movies.DistinctBy(x => ((JsonObject)x)["Slug"].GetValue<string>());
        
        using var connection = await connectionFactory.GetConnection();
        using var transaction = connection.BeginTransaction();
        
        const string movieInsertSql = """
                                      insert into dbo.movies
                                      (movie_id, movie_name, slug, year_of_release)
                                      values
                                      (@MovieId, @MovieName, @Slug, @YearOfRelease)
                                      """;
        
        const string genresInsertSql = """
                                      insert into dbo.genres
                                      (movie_id, genre)
                                      values
                                      (@MovieId, @Genre)
                                      """; 

        foreach (JsonObject movie in distinctMovies)
        {
            await connection.ExecuteAsync(movieInsertSql, new 
                {MovieId = movie["Id"].GetValue<Guid>(), MovieName = movie["Title"].GetValue<string>(), Slug = movie["Slug"].GetValue<string>(), YearOfRelease = movie["YearOfRelease"].GetValue<int>()});
            
            foreach (var genre in movie["Genres"].AsArray())
                await connection.ExecuteAsync(genresInsertSql, new {MovieId =movie["Id"].GetValue<Guid>(), Genre = genre.ToString()});
        }
        transaction.Commit();
    }
    
    
}