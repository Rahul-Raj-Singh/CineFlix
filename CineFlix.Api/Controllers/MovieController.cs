using CineFlix.Application;
using CineFlix.Application.Services;
using CineFlix.Contracts.Request;
using CineFlix.Contracts.Response;
using Microsoft.AspNetCore.Mvc;

namespace CineFlix.Controllers;

[ApiController]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MovieController(IMovieService movieService)
    {
        _movieService = movieService;
    }
    
    [HttpGet(ApiEndpoints.Movie.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var movies = await _movieService.GetAll();

        var movieResponses = movies.Select(x => new MovieResponse
        {
            MovieId = x.MovieId,
            MovieName = x.MovieName,
            Slug = x.Slug,
            YearOfRelease = x.YearOfRelease,
            Genres = x.Genres
        });
        
        return Ok(movieResponses);
    }
    
    [HttpGet(ApiEndpoints.Movie.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug)
    {
        var movie = Guid.TryParse(idOrSlug, out var movieId)
            ? await _movieService.GetById(movieId)
            : await _movieService.GetBySlug(idOrSlug);
        
        if (movie is null) return NotFound();

        var movieResponse = new MovieResponse
        {
            MovieId = movie.MovieId,
            Slug = movie.Slug,
            MovieName = movie.MovieName,
            YearOfRelease = movie.YearOfRelease,
            Genres = movie.Genres
        };
        
        return Ok(movieResponse);
    }
    
    [HttpPost(ApiEndpoints.Movie.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        var newMovie = await _movieService.Create(request);
        
        return Created(nameof(Get), newMovie);
    }
    
    [HttpPut(ApiEndpoints.Movie.Update)]
    public async Task<IActionResult> Update([FromBody] UpdateMovieRequest request)
    {
        var isSuccess = await _movieService.Update(request);
        
        if (!isSuccess) return NotFound("Cannot find movie!");

        return Ok();
    }
}
