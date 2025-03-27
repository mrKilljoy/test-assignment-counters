using Assignment.Counters.Application.Models.Requests;
using FluentValidation;

namespace Assignment.Counters.Api.Infrastructure.Validation;

public class NewTeamValidator : AbstractValidator<NewTeamRequest>
{
    public NewTeamValidator()
    {
        RuleFor(x => x.TeamName)
            .NotEmpty();
    }
}