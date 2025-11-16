using Osirisgate.CleanArchitectureCore.Attributes.Environment;
using Osirisgate.CleanArchitectureCore.Attributes.Type;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Service;

[WhenTest]
[Service(AsInterface = typeof(IMultipleEmailService))]
public sealed class SendGridEmailService : IMultipleEmailService
{
    public string GetProviderName() => "SendGrid";
}

