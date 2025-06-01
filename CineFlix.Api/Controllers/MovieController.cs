using CineFlix.Application.Services;
using CineFlix.Contracts.Request;
using CineFlix.Contracts.Response;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CineFlix.Api.Controllers;

[ApiController]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;
    
    // Request validators
    private readonly IValidator<CreateMovieRequest> _createMovieRequestValidator;
    private readonly IValidator<UpdateMovieRequest> _updateMovieRequestValidator;

    public MovieController(IMovieService movieService, 
        IValidator<CreateMovieRequest> createMovieRequestValidator, 
        IValidator<UpdateMovieRequest> updateMovieRequestValidator)
    {
        _movieService = movieService;
        _createMovieRequestValidator = createMovieRequestValidator;
        _updateMovieRequestValidator = updateMovieRequestValidator;
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
        await _createMovieRequestValidator.ValidateAndThrowAsync(request);
        
        var newMovie = await _movieService.Create(request);
        
        return Created(nameof(Get), newMovie);
    }
    
    [HttpPut(ApiEndpoints.Movie.Update)]
    public async Task<IActionResult> Update([FromBody] UpdateMovieRequest request)
    {
        await _updateMovieRequestValidator.ValidateAndThrowAsync(request);
        
        var isSuccess = await _movieService.Update(request);
        
        if (!isSuccess) return NotFound("Cannot find movie!");

        return Ok();
    }
}
