using Osirisgate.CleanArchitectureCore.Exception;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class UserRegistrationRequestAsDto : CleanArchitectureCore.Request.Request
{
    // Properties that match RequestStructure fields (email, age, password) - these become the DTO
    public string? Email { get; set; }
    public int? Age { get; set; }
    public string? Password { get; set; }

    private static readonly UserRegistrationRequestAsDtoValidator Validator = new();

    public UserRegistrationRequestAsDto(IReadOnlyDictionary<string, object> payload) : base(payload)
    {
        // Map RequestStructure fields to properties using ToDto() which returns normalized dictionary
        var dtoData = ToDto();
        Email = dtoData.TryGetValue("Email", out var email) ? email?.ToString() : null;
        Age = dtoData.TryGetValue("Age", out var age) && int.TryParse(age?.ToString(), out var ageInt) ? ageInt : null;
        Password = dtoData.TryGetValue("Password", out var pwd) ? pwd?.ToString() : null;
    }

    // Parameterless constructor for ToDto<TDto>() constraint
    public UserRegistrationRequestAsDto() : base(new Dictionary<string, object>())
    {
    }

    protected override Dictionary<string, object> RequestStructure => new()
    {
        ["email"] = true,
        ["age"] = true,
        ["password"] = true
    };

    protected override Task ApplyConstraintsOnRequestFieldsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // Use the Request itself (this) as DTO - no separate DTO class needed!
        var validationResult = Validator.Validate(this);

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

