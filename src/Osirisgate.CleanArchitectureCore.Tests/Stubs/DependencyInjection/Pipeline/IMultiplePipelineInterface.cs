using Osirisgate.CleanArchitectureCore.Pipeline;

namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Pipeline;

public interface IMultiplePipelineInterface : IPipeline
{
    public string GetName();
}

