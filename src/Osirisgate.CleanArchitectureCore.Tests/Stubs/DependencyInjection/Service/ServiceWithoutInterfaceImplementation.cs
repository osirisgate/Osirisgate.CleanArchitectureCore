using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Service;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
/// <summary>
/// This class specifies an interface in the attribute but does not implement it.
/// Used to test the path if (!componentImplementsInterface) in ValidateInterfaceHasImplementation.
/// </summary>
[Service(AsInterface = typeof(IServiceWithoutImplementation))]
public sealed class ServiceWithoutInterfaceImplementation
{
    public string GetValue() => "test";
}

