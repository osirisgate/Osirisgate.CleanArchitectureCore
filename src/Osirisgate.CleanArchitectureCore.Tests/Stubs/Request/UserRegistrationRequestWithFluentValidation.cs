using Osirisgate.CleanArchitectureCore.Exception;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class UserRegistrationRequestWithFluentValidation(IReadOnlyDictionary<string, object> payload) : CleanArchitectureCore.Request.Request(payload)
{
    private static readonly UserRegistrationRequestDtoValidator _validator = new();

    protected override Dictionary<string, object> RequestStructure => new()
    {
        ["email"] = true,
        ["age"] = true,
        ["password"] = true
    };

    protected override Task ApplyConstraintsOnRequestFieldsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // Use ToDto<TDto>() to automatically map from RequestStructure to DTO
        // The mapping is based solely on RequestStructure fields (email, age, password)
        // Property names are matched case-insensitively - no need to modify existing DTOs
        var dto = ToDto<UserRegistrationRequestDto>();

        // Validate using FluentValidation
        var validationResult = _validator.Validate(dto);

        if (!validationResult.IsValid)
        {
            var errors = new Dictionary<string, object>();
            foreach (var error in validationResult.Errors)
            {
                if (!errors.TryGetValue(error.PropertyName, out object? _))
                {
                    errors[error.PropertyName] = error.ErrorMessage ?? "Validation failed";
                }
            }

            throw new BadRequestContentException(new Dictionary<string, object>
            {
                ["message"] = "validation.failed",
                ["details"] = errors
            });
        }

        return Task.CompletedTask;
    }
}

