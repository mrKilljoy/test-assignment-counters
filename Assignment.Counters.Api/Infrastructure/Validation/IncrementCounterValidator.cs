using Assignment.Counters.Application.Models.Requests;
using FluentValidation;

namespace Assignment.Counters.Api.Infrastructure.Validation;

public class IncrementCounterValidator : AbstractValidator<IncrementCounterRequest>
{
    public IncrementCounterValidator()
    {
        RuleFor(x => x.Value)
            .GreaterThan(0);

        RuleFor(x => x.CounterId)
            .NotEmpty();

        RuleFor(x => x.LastTimestamp)
            .NotEmpty();
    }
}