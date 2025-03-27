using Assignment.Counters.Application.Models.Requests;
using FluentValidation;

namespace Assignment.Counters.Api.Infrastructure.Validation;

public class NewCounterValidator : AbstractValidator<NewCounterRequest>
{
    public NewCounterValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty();

        RuleFor(x => x.UserName)
            .NotEmpty();
    }
}