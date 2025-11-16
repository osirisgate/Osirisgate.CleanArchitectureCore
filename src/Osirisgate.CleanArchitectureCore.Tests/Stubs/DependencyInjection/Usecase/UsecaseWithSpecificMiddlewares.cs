using Osirisgate.CleanArchitectureCore.Attributes.Type;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Middleware;
using UsecaseBase = Osirisgate.CleanArchitectureCore.Usecase.Usecase;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Usecase;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
[Usecase(PreMiddlewares = new[] { typeof(IAuthenticationMiddleware) })]
public sealed class UsecaseWithSpecificMiddlewares : UsecaseBase
{
    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
