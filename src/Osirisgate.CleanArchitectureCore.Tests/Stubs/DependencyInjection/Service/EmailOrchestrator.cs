namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Service;

public sealed class EmailOrchestrator(IEnumerable<IMultipleEmailService> emailServices)
{
    private readonly IEnumerable<IMultipleEmailService> _emailServices = emailServices;

    public List<string> GetAllProviderNames()
    {
        return [.. _emailServices.Select(s => s.GetProviderName())];
    }
}

