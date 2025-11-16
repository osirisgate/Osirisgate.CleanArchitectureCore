using Osirisgate.CleanArchitectureCore.Exception;
using Osirisgate.CleanArchitectureCore.Response;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Usecase;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class UserAlreadyExistsException : BaseException
{
    protected override StatusCode ErrorCode { get; } = StatusCode.Conflict;

    public UserAlreadyExistsException(string email)
        : base(new Dictionary<string, object>
        {
            ["message"] = "user.already.exists",
            ["details"] = new Dictionary<string, object>
            {
                ["email"] = email
            }
        })
    { }
}
