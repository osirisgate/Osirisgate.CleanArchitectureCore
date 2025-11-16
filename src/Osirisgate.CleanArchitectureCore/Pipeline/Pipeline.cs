using Osirisgate.CleanArchitectureCore.Attributes.Type;
using Osirisgate.CleanArchitectureCore.Middleware;
using Osirisgate.CleanArchitectureCore.Presenter;
using Osirisgate.CleanArchitectureCore.Request;
using Osirisgate.CleanArchitectureCore.Usecase;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;
using ObjectDisposedException = Osirisgate.CleanArchitectureCore.Exception.ObjectDisposedException;

namespace Osirisgate.CleanArchitectureCore.Pipeline;

/// <summary>
/// Default implementation of the pipeline that orchestrates middleware and use case execution.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
[Pipeline]
public sealed class Pipeline : IPipeline, IDisposable
{
    private bool _disposed = false;
    private IReadOnlyList<IPreMiddleware> _preMiddlewares;
    private IReadOnlyList<IPostMiddleware> _postMiddlewares;

    /// <summary>
    /// Initializes a new instance of the <see cref="Pipeline"/> class.
    /// </summary>
    public Pipeline()
    {
        _preMiddlewares = [];
        _postMiddlewares = [];
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(IUsecase useCase, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(useCase, nameof(useCase));
        cancellationToken.ThrowIfCancellationRequested();

        var request = useCase.GetRequest();
        var presenter = useCase.GetPresenter();

        await ExecuteMiddlewareChain(_preMiddlewares, 0, request, presenter, cancellationToken);

        if (presenter?.GetResponse() != null)
            return;

        if (request != null)
            useCase.WithRequest(request);

        if (presenter != null)
            useCase.WithPresenter(presenter);

        await useCase.ExecuteAsync(cancellationToken);

        await ExecuteMiddlewareChain(_postMiddlewares, 0, request, presenter, cancellationToken);
    }

    /// <inheritdoc/>
    public IPipeline WithPreMiddlewares(IReadOnlyList<IPreMiddleware>? preMiddlewares)
    {
        ThrowIfDisposed();
        _preMiddlewares = preMiddlewares ?? [];
        return this;
    }

    /// <inheritdoc/>
    public IPipeline WithPostMiddlewares(IReadOnlyList<IPostMiddleware>? postMiddlewares)
    {
        ThrowIfDisposed();
        _postMiddlewares = postMiddlewares ?? [];
        return this;
    }

    private static Task ExecuteMiddlewareChain<TMiddleware>(
        IReadOnlyList<TMiddleware> middlewares,
        int index,
        IRequest? request,
        IPresenter? presenter,
        CancellationToken cancellationToken
    ) where TMiddleware : IMiddlewareBase
    {
        if (index >= middlewares.Count)
            return Task.CompletedTask;

        cancellationToken.ThrowIfCancellationRequested();
        var middleware = middlewares[index];

        return middleware.InvokeAsync(
            request,
            presenter,
            ct => ExecuteMiddlewareChain(middlewares, index + 1, request, presenter, ct),
            cancellationToken
        );
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(Pipeline));
    }

    /// <summary>
    /// Releases the resources used by the Pipeline.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _preMiddlewares = [];
            _postMiddlewares = [];
            _disposed = true;
        }
    }
}
