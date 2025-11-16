using Microsoft.Extensions.DependencyInjection;
using Osirisgate.CleanArchitectureCore.DependencyInjection.Middleware;
using Osirisgate.CleanArchitectureCore.Middleware;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Middleware;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Usecase;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;

namespace Osirisgate.CleanArchitectureCore.Tests.DependencyInjection;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class MiddlewareResolverTest
{
    [Fact]
    public void TestConstructorWithNullServiceProviderThrows()
    {
        Assert.Throws<ArgumentNullException>(() => new MiddlewareResolver(null!));
    }

    [Fact]
    public void TestGetPreMiddlewaresWithGlobalOnly()
    {
        var services = new ServiceCollection();
        services.AddScoped<GlobalLoggingMiddleware>();
        services.AddScoped<IPreMiddleware>(sp => sp.GetRequiredService<GlobalLoggingMiddleware>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPreMiddlewares(typeof(TestUsecase));

        Assert.Single(middlewares);
        Assert.IsType<GlobalLoggingMiddleware>(middlewares[0]);
    }

    [Fact]
    public void TestGetPreMiddlewaresWithSpecificOnly()
    {
        var services = new ServiceCollection();
        services.AddScoped<SpecificAuthMiddleware>();
        services.AddScoped<IAuthenticationMiddleware>(sp => sp.GetRequiredService<SpecificAuthMiddleware>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPreMiddlewares(typeof(UsecaseWithSpecificMiddlewares));

        Assert.Single(middlewares);
        Assert.IsType<SpecificAuthMiddleware>(middlewares[0]);
    }

    [Fact]
    public void TestGetPreMiddlewaresWithGlobalAndSpecific()
    {
        var services = new ServiceCollection();
        services.AddScoped<GlobalLoggingMiddleware>();
        services.AddScoped<SpecificAuthMiddleware>();
        services.AddScoped<IAuthenticationMiddleware>(sp => sp.GetRequiredService<SpecificAuthMiddleware>());
        services.AddScoped<IPreMiddleware>(sp => sp.GetRequiredService<GlobalLoggingMiddleware>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPreMiddlewares(typeof(UsecaseWithSpecificMiddlewares));

        Assert.Equal(2, middlewares.Count);
        Assert.Contains(middlewares, m => m is GlobalLoggingMiddleware);
        Assert.Contains(middlewares, m => m is SpecificAuthMiddleware);
    }

    [Fact]
    public void TestGetPreMiddlewaresCaching()
    {
        var services = new ServiceCollection();
        services.AddScoped<GlobalLoggingMiddleware>();
        services.AddScoped<IPreMiddleware>(sp => sp.GetRequiredService<GlobalLoggingMiddleware>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares1 = resolver.GetPreMiddlewares(typeof(TestUsecase));
        var middlewares2 = resolver.GetPreMiddlewares(typeof(TestUsecase));

        Assert.Equal(middlewares1.Count, middlewares2.Count);
        Assert.Equal(middlewares1[0].GetType(), middlewares2[0].GetType());
    }

    [Fact]
    public void TestGetPostMiddlewaresWithGlobalOnly()
    {
        var services = new ServiceCollection();
        services.AddScoped<GlobalPostMiddleware>();
        services.AddScoped<IPostMiddleware>(sp => sp.GetRequiredService<GlobalPostMiddleware>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPostMiddlewares(typeof(TestUsecase));

        Assert.Single(middlewares);
        Assert.IsType<GlobalPostMiddleware>(middlewares[0]);
    }

    [Fact]
    public void TestGetPostMiddlewaresWithSpecificOnly()
    {
        var services = new ServiceCollection();
        services.AddScoped<SpecificPostMiddleware>();
        services.AddScoped<IPostMiddleware>(sp => sp.GetRequiredService<SpecificPostMiddleware>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPostMiddlewares(typeof(UsecaseWithPostMiddlewares));

        Assert.Single(middlewares);
        Assert.IsType<SpecificPostMiddleware>(middlewares[0]);
    }

    [Fact]
    public void TestGetPostMiddlewaresWithGlobalAndSpecific()
    {
        var services = new ServiceCollection();
        services.AddScoped<GlobalPostMiddleware>();
        services.AddScoped<SpecificPostMiddleware>();
        services.AddScoped<IPostMiddleware>(sp => sp.GetRequiredService<GlobalPostMiddleware>());
        services.AddScoped<IPostMiddleware>(sp => sp.GetRequiredService<SpecificPostMiddleware>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPostMiddlewares(typeof(UsecaseWithPostMiddlewares));

        Assert.Equal(2, middlewares.Count);
        Assert.Contains(middlewares, m => m is GlobalPostMiddleware);
        Assert.Contains(middlewares, m => m is SpecificPostMiddleware);
    }

    [Fact]
    public void TestGetPostMiddlewaresCaching()
    {
        var services = new ServiceCollection();
        services.AddScoped<GlobalPostMiddleware>();
        services.AddScoped<IPostMiddleware>(sp => sp.GetRequiredService<GlobalPostMiddleware>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var postMiddlewares1 = resolver.GetPostMiddlewares(typeof(TestUsecase));
        var postMiddlewares2 = resolver.GetPostMiddlewares(typeof(TestUsecase));

        Assert.Equal(postMiddlewares1.Count, postMiddlewares2.Count);
        Assert.Equal(postMiddlewares1[0].GetType(), postMiddlewares2[0].GetType());
    }

    [Fact]
    public void TestGetPreMiddlewaresWithUsecaseWithoutMiddlewares()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPreMiddlewares(typeof(UsecaseWithoutMiddlewares));

        Assert.Empty(middlewares);
    }

    [Fact]
    public void TestGetPostMiddlewaresWithUsecaseWithoutMiddlewares()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPostMiddlewares(typeof(UsecaseWithoutMiddlewares));

        Assert.Empty(middlewares);
    }

    [Fact]
    public void TestGetPreMiddlewaresWithInterfaceMiddlewares()
    {
        var services = new ServiceCollection();
        services.AddScoped<SpecificAuthMiddleware>();
        services.AddScoped<IAuthenticationMiddleware>(sp => sp.GetRequiredService<SpecificAuthMiddleware>());
        services.AddScoped<IPreMiddleware>(sp => sp.GetRequiredService<IAuthenticationMiddleware>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPreMiddlewares(typeof(UsecaseWithInterfaceMiddlewares));

        Assert.Single(middlewares);
        Assert.IsType<SpecificAuthMiddleware>(middlewares[0]);
    }

    [Fact]
    public void TestGetPostMiddlewaresWithInterfaceMiddlewares()
    {
        var services = new ServiceCollection();
        services.AddScoped<SpecificPostMiddleware>();
        services.AddScoped<IPostMiddleware>(sp => sp.GetRequiredService<SpecificPostMiddleware>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPostMiddlewares(typeof(UsecaseWithPostMiddlewares));

        Assert.Single(middlewares);
        Assert.IsType<SpecificPostMiddleware>(middlewares[0]);
    }

    [Fact]
    public void TestConfigurePipeline()
    {
        var services = new ServiceCollection();
        services.AddScoped<GlobalLoggingMiddleware>();
        services.AddScoped<IPreMiddleware>(sp => sp.GetRequiredService<GlobalLoggingMiddleware>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var pipeline = new CleanArchitectureCore.Pipeline.Pipeline();

        var configuredPipeline = resolver.ConfigurePipeline(pipeline, typeof(TestUsecase));

        Assert.Same(pipeline, configuredPipeline);
    }

    [Fact]
    public void TestGetPreMiddlewaresWithNullUsecaseTypeThrows()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var resolver = new MiddlewareResolver(serviceProvider);

        Assert.Throws<ArgumentNullException>(() => resolver.GetPreMiddlewares(null!));
    }

    [Fact]
    public void TestGetPostMiddlewaresWithNullUsecaseTypeThrows()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var resolver = new MiddlewareResolver(serviceProvider);

        Assert.Throws<ArgumentNullException>(() => resolver.GetPostMiddlewares(null!));
    }

    [Fact]
    public void TestConfigurePipelineWithNullPipelineThrows()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var resolver = new MiddlewareResolver(serviceProvider);

        Assert.Throws<ArgumentNullException>(() => resolver.ConfigurePipeline(null!, typeof(TestUsecase)));
    }

    [Fact]
    public void TestConfigurePipelineWithNullUsecaseTypeThrows()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var resolver = new MiddlewareResolver(serviceProvider);
        var pipeline = new CleanArchitectureCore.Pipeline.Pipeline();

        Assert.Throws<ArgumentNullException>(() => resolver.ConfigurePipeline(pipeline, null!));
    }

    [Fact]
    public void TestGetPreMiddlewaresWithMultipleGlobalMiddlewaresOrdersByPriority()
    {
        var services = new ServiceCollection();
        services.AddScoped<GlobalLoggingMiddleware>();
        services.AddScoped<GlobalPreMiddlewareWithPriority>();
        services.AddScoped<IPreMiddleware>(sp => sp.GetRequiredService<GlobalLoggingMiddleware>());
        services.AddScoped<IPreMiddleware>(sp => sp.GetRequiredService<GlobalPreMiddlewareWithPriority>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPreMiddlewares(typeof(TestUsecase));

        Assert.Equal(2, middlewares.Count);
        Assert.IsType<GlobalPreMiddlewareWithPriority>(middlewares[0]);
        Assert.IsType<GlobalLoggingMiddleware>(middlewares[1]);
    }

    [Fact]
    public void TestGetPostMiddlewaresWithMultipleGlobalMiddlewaresOrdersByPriority()
    {
        var services = new ServiceCollection();
        services.AddScoped<GlobalPostMiddleware>();
        services.AddScoped<GlobalPostMiddlewareWithPriority>();
        services.AddScoped<IPostMiddleware>(sp => sp.GetRequiredService<GlobalPostMiddleware>());
        services.AddScoped<IPostMiddleware>(sp => sp.GetRequiredService<GlobalPostMiddlewareWithPriority>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPostMiddlewares(typeof(TestUsecase));

        Assert.Equal(2, middlewares.Count);
        Assert.IsType<GlobalPostMiddlewareWithPriority>(middlewares[0]);
        Assert.IsType<GlobalPostMiddleware>(middlewares[1]);
    }

    [Fact]
    public void TestGetPreMiddlewaresWithMiddlewareWithoutAttribute()
    {
        var services = new ServiceCollection();
        services.AddScoped<MiddlewareWithoutAttribute>();
        services.AddScoped<IPreMiddleware>(sp => sp.GetRequiredService<MiddlewareWithoutAttribute>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPreMiddlewares(typeof(TestUsecase));

        Assert.Empty(middlewares);
    }

    [Fact]
    public void TestGetPreMiddlewaresWithUsecaseWithoutUsecaseAttribute()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPreMiddlewares(typeof(UsecaseWithoutUsecaseAttribute));

        Assert.Empty(middlewares);
    }

    [Fact]
    public void TestGetPostMiddlewaresWithUsecaseWithoutUsecaseAttribute()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPostMiddlewares(typeof(UsecaseWithoutUsecaseAttribute));

        Assert.Empty(middlewares);
    }

    [Fact]
    public void TestGetPreMiddlewaresWithUnregisteredMiddlewareType()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPreMiddlewares(typeof(UsecaseWithUnregisteredMiddleware));

        Assert.Empty(middlewares);
    }

    [Fact]
    public void TestGetPostMiddlewaresWithUnregisteredMiddlewareType()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPostMiddlewares(typeof(UsecaseWithUnregisteredPostMiddleware));

        Assert.Empty(middlewares);
    }

    [Fact]
    public void TestGetPreMiddlewaresWithConcreteMiddlewareType()
    {
        var services = new ServiceCollection();
        services.AddScoped<ConcretePreMiddleware>();
        services.AddScoped<IPreMiddleware>(sp => sp.GetRequiredService<ConcretePreMiddleware>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPreMiddlewares(typeof(UsecaseWithConcretePreMiddleware));

        Assert.Single(middlewares);
        Assert.IsType<ConcretePreMiddleware>(middlewares[0]);
    }

    [Fact]
    public void TestGetPostMiddlewaresWithConcreteMiddlewareType()
    {
        var services = new ServiceCollection();
        services.AddScoped<ConcretePostMiddleware>();
        services.AddScoped<IPostMiddleware>(sp => sp.GetRequiredService<ConcretePostMiddleware>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPostMiddlewares(typeof(UsecaseWithConcretePostMiddleware));

        Assert.Single(middlewares);
        Assert.IsType<ConcretePostMiddleware>(middlewares[0]);
    }

    [Fact]
    public void TestGetPreMiddlewaresWithConcreteMiddlewareTypeNotInServiceProvider()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPreMiddlewares(typeof(UsecaseWithConcretePreMiddleware));

        Assert.Empty(middlewares);
    }

    [Fact]
    public void TestGetPostMiddlewaresWithConcreteMiddlewareTypeNotInServiceProvider()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPostMiddlewares(typeof(UsecaseWithConcretePostMiddleware));

        Assert.Empty(middlewares);
    }

    [Fact]
    public void TestGetPostMiddlewaresWithConcreteMiddlewareTypeInServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddScoped<ConcretePostMiddleware>();
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPostMiddlewares(typeof(UsecaseWithConcretePostMiddleware));

        Assert.Single(middlewares);
        Assert.IsType<ConcretePostMiddleware>(middlewares[0]);
    }

    [Fact]
    public void TestGetPostMiddlewaresWithInterfacePostMiddleware()
    {
        var services = new ServiceCollection();
        services.AddScoped<SpecificAuditMiddleware>();
        services.AddScoped<IAuditMiddleware>(sp => sp.GetRequiredService<SpecificAuditMiddleware>());
        services.AddScoped<IPostMiddleware>(sp => sp.GetRequiredService<IAuditMiddleware>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPostMiddlewares(typeof(UsecaseWithInterfacePostMiddlewares));

        Assert.Single(middlewares);
        Assert.IsType<SpecificAuditMiddleware>(middlewares[0]);
    }

    [Fact]
    public void TestGetPreMiddlewaresWithMiddlewareWithoutAttributePriority()
    {
        var services = new ServiceCollection();
        services.AddScoped<MiddlewareWithoutAttribute>();
        services.AddScoped<IPreMiddleware>(sp => sp.GetRequiredService<MiddlewareWithoutAttribute>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPreMiddlewares(typeof(TestUsecase));

        Assert.Empty(middlewares);
    }

    [Fact]
    public void TestGetPreMiddlewaresOrderByPriorityWithMiddlewareWithoutAttribute()
    {
        var services = new ServiceCollection();
        services.AddScoped<GlobalLoggingMiddleware>();
        services.AddScoped<MiddlewareWithoutAttribute>();
        services.AddScoped<IPreMiddleware>(sp => sp.GetRequiredService<GlobalLoggingMiddleware>());
        services.AddScoped<IPreMiddleware>(sp => sp.GetRequiredService<MiddlewareWithoutAttribute>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPreMiddlewares(typeof(TestUsecase));

        Assert.Single(middlewares);
        Assert.IsType<GlobalLoggingMiddleware>(middlewares[0]);
    }

    [Fact]
    public void TestGetPreMiddlewaresWithSpecificMiddlewareWithoutAttribute()
    {
        var services = new ServiceCollection();
        services.AddScoped<MiddlewareWithoutAttribute>();
        services.AddScoped<IPreMiddleware>(sp => sp.GetRequiredService<MiddlewareWithoutAttribute>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPreMiddlewares(typeof(UsecaseWithMiddlewareWithoutAttribute));

        Assert.Single(middlewares);
        Assert.IsType<MiddlewareWithoutAttribute>(middlewares[0]);
    }

    [Fact]
    public void TestGetPreMiddlewaresOrderByPriorityWithMiddlewareWithoutAttributeInSpecific()
    {
        var services = new ServiceCollection();
        services.AddScoped<MiddlewareWithoutAttribute>();
        services.AddScoped<IPreMiddleware>(sp => sp.GetRequiredService<MiddlewareWithoutAttribute>());
        var serviceProvider = services.BuildServiceProvider();

        var resolver = new MiddlewareResolver(serviceProvider);
        var middlewares = resolver.GetPreMiddlewares(typeof(UsecaseWithMiddlewareWithoutAttribute));

        Assert.Single(middlewares);
        Assert.IsType<MiddlewareWithoutAttribute>(middlewares[0]);
    }
}
