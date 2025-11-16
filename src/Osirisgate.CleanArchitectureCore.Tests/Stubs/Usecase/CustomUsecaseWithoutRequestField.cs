namespace Osirisgate.CleanArchitectureCore.Tests.Stubs.Usecase;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class CustomUsecaseWithoutRequestField : CleanArchitectureCore.Usecase.Usecase
{
    public override Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        GetField<int>("key", 10);
        return Task.CompletedTask;
    }
}
