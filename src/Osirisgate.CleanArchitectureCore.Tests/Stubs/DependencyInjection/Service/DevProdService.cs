using Osirisgate.CleanArchitectureCore.Attributes.Environment;
using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Service;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public interface IDevProdService
{
    public string GetValue();
}

[WhenDev]
[WhenProd]
[Service(AsInterface = typeof(IDevProdService))]
public sealed class DevProdService : IDevProdService
{
    public string GetValue() => "dev-prod";
}

