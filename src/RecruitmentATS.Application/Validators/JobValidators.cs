using FluentValidation;
using RecruitmentATS.Application.DTOs;

namespace RecruitmentATS.Application.Validators;

public class CreateJobRequestValidator : AbstractValidator<CreateJobRequest>
{
    public CreateJobRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Requirements).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.SalaryRangeMin)
            .GreaterThanOrEqualTo(0).When(x => x.SalaryRangeMin.HasValue);
        RuleFor(x => x.SalaryRangeMax)
            .GreaterThanOrEqualTo(x => x.SalaryRangeMin ?? 0)
            .When(x => x.SalaryRangeMax.HasValue);
    }
}

public class ApplyToJobRequestValidator : AbstractValidator<ApplyToJobRequest>
{
    public ApplyToJobRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Phone).MaximumLength(20).When(x => !string.IsNullOrEmpty(x.Phone));
        RuleFor(x => x.LinkedInUrl).MaximumLength(500).When(x => !string.IsNullOrEmpty(x.LinkedInUrl));
    }
}
