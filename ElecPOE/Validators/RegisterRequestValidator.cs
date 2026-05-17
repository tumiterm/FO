using FluentValidation;
using ForekOnline.Domain.ViewModels;

namespace ElecPOE.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is required.");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.");

            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.IdNumber) || !string.IsNullOrWhiteSpace(x.PassportNumber))
                .WithMessage("Either IdNumber or PassportNumber is required.");

             RuleFor(x => x.IdNumber).Matches(@"^\d{13}$").When(x => !string.IsNullOrWhiteSpace(x.IdNumber)).WithMessage("Invalid ID Number format.");
        }
    }
}
