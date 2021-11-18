using FluentValidation;

namespace Application.Validation;

internal static class ValidatorExtensions
{
    private const int MIN_PASSWORD_LENGTH = 8;
    private const int MAX_PASSWORD_LENGTH = 16;

    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        var options = ruleBuilder
            .NotEmpty()
            .MinimumLength(MIN_PASSWORD_LENGTH).WithMessage($"Password must have at least {MIN_PASSWORD_LENGTH} characters.")
            .MaximumLength(MAX_PASSWORD_LENGTH).WithMessage($"Password must be shorter than {MAX_PASSWORD_LENGTH + 1}.");

        return options;
    }
}
