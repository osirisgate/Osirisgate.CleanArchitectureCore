using Osirisgate.CleanArchitectureCore.Exception;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class UserRegistrationRequestWithSimpleValidation(IReadOnlyDictionary<string, object> payload) : CleanArchitectureCore.Request.Request(payload)
{
    protected override Dictionary<string, object> RequestStructure => new()
    {
        ["email"] = true,
        ["age"] = true,
        ["password"] = true
    };

    protected override Task ApplyConstraintsOnRequestFieldsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var email = GetField<string>("email");
        var age = GetField<int?>("age");
        var password = GetField<string>("password");

        var errors = new Dictionary<string, object>();

        // Email validation
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            errors["email"] = "Invalid email format";
        }

        // Age validation
        if (age.HasValue && (age.Value < 18 || age.Value > 120))
        {
            errors["age"] = "Age must be between 18 and 120";
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            errors["password"] = "Password must be at least 8 characters long";
        }

        if (errors.Count > 0)
        {
            throw new BadRequestContentException(new Dictionary<string, object>
            {
                ["message"] = "validation.failed",
                ["details"] = errors
            });
        }

        return Task.CompletedTask;
    }
}

