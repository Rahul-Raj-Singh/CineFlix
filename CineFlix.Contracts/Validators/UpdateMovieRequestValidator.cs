using CineFlix.Contracts.Request;
using FluentValidation;

namespace CineFlix.Contracts.Validators;

public class UpdateMovieRequestValidator : AbstractValidator<UpdateMovieRequest>
{
    public UpdateMovieRequestValidator()
    {
        RuleFor(x => x.MovieId).NotEmpty();
        RuleFor(x => x.MovieName).NotEmpty();
        RuleFor(x => x.YearOfRelease).LessThanOrEqualTo(DateTime.Now.Year);
        RuleFor(x => x.Genres).NotEmpty();
    }
}