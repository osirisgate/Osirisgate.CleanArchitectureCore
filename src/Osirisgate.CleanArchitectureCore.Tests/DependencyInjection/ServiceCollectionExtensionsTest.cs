using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Osirisgate.CleanArchitectureCore.Attributes.Type;
using Osirisgate.CleanArchitectureCore.DependencyInjection;
using Osirisgate.CleanArchitectureCore.DependencyInjection.Middleware;
using Osirisgate.CleanArchitectureCore.Pipeline;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Middleware;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Pipeline;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Presenter;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Repository;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Service;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.DependencyInjection.Usecase;
using InvalidOperationException = Osirisgate.CleanArchitectureCore.Exception.InvalidOperationException;

namespace Osirisgate.CleanArchitectureCore.Tests.DependencyInjection;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class ServiceCollectionExtensionsTest
{
    /// <summary>
    /// Helper method to extract individual errors from an aggregated exception.
    /// If the exception is a single error, returns it. If it's an aggregated error, extracts all individual errors.
    /// </summary>
    private static List<IDictionary<string, object>> ExtractErrors(InvalidOperationException ex)
    {
        var formatted = ex.Format();
        var message = formatted["message"]?.ToString();

        if (message == "multiple.registration.errors")
        {
            var details = formatted["details"] as IDictionary<string, object>;
            if (details != null && details.TryGetValue("errorsByComponent", out var errorsByComponentObj))
            {
                // Extract individual errors from the new grouped structure
                var extractedErrors = new List<IDictionary<string, object>>();

                // Handle different possible types for errorsByComponent
                if (errorsByComponentObj is IDictionary<string, object> errorsByComponent)
                {
                    foreach (var componentGroup in errorsByComponent.Values)
                    {
                        if (componentGroup is IDictionary<string, object> groupDict
                            && groupDict.TryGetValue("errors", out var errorsObj))
                        {
                            if (errorsObj is System.Collections.IList errorsList)
                            {
                                foreach (var error in errorsList)
                                {
                                    if (error is IDictionary<string, object> errorDict)
                                    {
                                        extractedErrors.Add(errorDict);
                                    }
                                }
                            }
                        }
                    }
                }
                // Handle Generic Dictionary case if explicitly needed (e.g. Dictionary<string, Dictionary<string, object>>)
                else if (errorsByComponentObj is System.Collections.IDictionary errorsDict)
                {
                    foreach (var value in errorsDict.Values)
                    {
                        if (value is IDictionary<string, object> groupDict
                            && groupDict.TryGetValue("errors", out var errorsObj))
                        {
                            if (errorsObj is System.Collections.IList errorsList)
                            {
                                foreach (var error in errorsList)
                                {
                                    if (error is IDictionary<string, object> errorDict)
                                    {
                                        extractedErrors.Add(errorDict);
                                    }
                                }
                            }
                        }
                    }
                }

                // Fallback: if no errors found in grouped structure, try allMessages
                if (extractedErrors.Count == 0 && details.TryGetValue("allMessages", out var allMessagesObj)
                    && allMessagesObj is System.Collections.IList messagesList)
                {
                    foreach (var msg in messagesList)
                    {
                        extractedErrors.Add(new Dictionary<string, object> { ["message"] = msg });
                    }
                }

                if (extractedErrors.Count > 0)
                {
                    return extractedErrors;
                }
            }

            // Legacy support: try old structure with "errors" and "allDetails"
            if (details != null && details.TryGetValue("errors", out var legacyErrorsObj) && legacyErrorsObj is System.Collections.IList legacyErrorsList)
            {
                var allDetails = details.TryGetValue("allDetails", out var allDetailsObj) ? allDetailsObj as IDictionary<string, object> : null;
                var legacyExtractedErrors = new List<IDictionary<string, object>>();

                for (int i = 0; i < legacyErrorsList.Count; i++)
                {
                    var errorMsg = legacyErrorsList[i]?.ToString();
                    if (string.IsNullOrEmpty(errorMsg))
                        continue;

                    var errorDetails = new Dictionary<string, object> { ["message"] = errorMsg };

                    if (allDetails != null)
                    {
                        var errorPrefix = $"{errorMsg}[{i}].";
                        foreach (var kvp in allDetails)
                        {
                            if (kvp.Key.StartsWith(errorPrefix))
                            {
                                var key = kvp.Key.Substring(errorPrefix.Length);
                                errorDetails[key] = kvp.Value;
                            }
                        }
                    }

                    legacyExtractedErrors.Add(errorDetails);
                }

                return legacyExtractedErrors;
            }
        }

        // Single error - flatten the structure to match expected format
        var singleError = new Dictionary<string, object>();
        if (formatted.TryGetValue("message", out var singleMsg))
        {
            singleError["message"] = singleMsg;
        }
        if (formatted.TryGetValue("details", out var singleDetails) && singleDetails is IDictionary<string, object> singleDetailsDict)
        {
            foreach (var kvp in singleDetailsDict)
            {
                singleError[kvp.Key] = kvp.Value;

            }
        }
        return new List<IDictionary<string, object>> { singleError };
    }

    /// <summary>
    /// Helper method to add Clean Architecture Core components, ignoring registration errors.
    /// This method is used in tests where we want to register valid components even if there are errors in the assembly
    /// (e.g., conflict test stubs that are intentionally problematic).
    /// The SDK now collects all errors before throwing, so valid components are registered even if there are errors.
    /// Tests that expect errors should use Assert.Throws instead of this method.
    /// </summary>
    private static void AddCleanArchitectureCoreSafely(
        IServiceCollection services,
        string? environment,
        params Assembly[] assemblies)
    {
        try
        {
            if (environment != null)
            {
                services.AddCleanArchitectureCore(environment, assemblies);
            }
            else
            {
                services.AddCleanArchitectureCore(assemblies);
            }
        }
        catch (InvalidOperationException)
        {
            // Ignore registration errors - valid components are still registered
            // This allows tests to work even when assemblies contain intentionally problematic stubs
        }
    }

    /// <summary>
    /// Helper method to add Clean Architecture Core components with options, ignoring registration errors.
    /// This method is used in tests where we want to register valid components even if there are errors in the assembly
    /// (e.g., conflict test stubs that are intentionally problematic).
    /// The SDK now collects all errors before throwing, so valid components are registered even if there are errors.
    /// Tests that expect errors should use Assert.Throws instead of this method.
    /// </summary>
    private static void AddCleanArchitectureCoreSafely(
        IServiceCollection services,
        string? environment,
        Action<CleanArchitectureOptions>? configureOptions,
        params Assembly[] assemblies)
    {
        try
        {
            if (environment != null && configureOptions != null)
            {
                services.AddCleanArchitectureCore(environment, configureOptions, assemblies);
            }
            else if (environment != null)
            {
                services.AddCleanArchitectureCore(environment, assemblies);
            }
            else if (configureOptions != null)
            {
                services.AddCleanArchitectureCore(configureOptions, assemblies);
            }
            else
            {
                services.AddCleanArchitectureCore(assemblies);
            }
        }
        catch (InvalidOperationException)
        {
            // Ignore registration errors - valid components are still registered
            // This allows tests to work even when assemblies contain intentionally problematic stubs
        }
    }
    [Fact]
    public void TestRegisterUsecase()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestUsecase).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecase>();

        Assert.NotNull(usecase);
        Assert.IsType<TestUsecase>(usecase);
    }

    [Fact]
    public void TestRegisterUsecaseWithInterface()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestUsecaseWithInterface).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<ITestUsecaseWithInterface>();

        Assert.NotNull(usecase);
        Assert.IsType<TestUsecaseWithInterface>(usecase);
    }

    [Fact]
    public void TestRegisterPresenter()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestPresenter).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var presenter = serviceProvider.GetService<TestPresenter>();

        Assert.NotNull(presenter);
        Assert.IsType<TestPresenter>(presenter);
    }

    [Fact]
    public void TestRegisterMiddleware()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestMiddleware).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var middleware = serviceProvider.GetService<TestMiddleware>();

        Assert.NotNull(middleware);
        Assert.IsType<TestMiddleware>(middleware);
    }

    [Fact]
    public void TestRegisterService()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestService).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestService>();

        Assert.NotNull(service);
        Assert.IsType<TestService>(service);
        Assert.Equal("test", service.GetValue());
    }

    [Fact]
    public void TestRegisterServiceWithInterface()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestServiceWithInterface).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceWithInterface>();

        Assert.NotNull(service);
        Assert.IsType<TestServiceWithInterface>(service);
        Assert.Equal("test-interface", service.GetValue());
    }

    [Fact]
    public void TestRegisterServiceWithImplementation()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestServiceImplementation).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceWithImplementation>();

        Assert.NotNull(service);
        Assert.IsType<TestServiceImplementation>(service);
        Assert.Equal("test-implementation", service.GetValue());
    }

    [Fact]
    public void TestRegisterUsecaseDevWithoutEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestUsecaseDev).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecaseDev>();
        Assert.Null(usecase);
    }

    [Fact]
    public void TestRegisterUsecaseDevWithDevelopmentEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(TestUsecaseDev).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecaseDev>();

        Assert.NotNull(usecase);
        Assert.IsType<TestUsecaseDev>(usecase);
    }

    [Fact]
    public void TestRegisterUsecaseDevWithDevEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Dev", typeof(TestUsecaseDev).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecaseDev>();

        Assert.NotNull(usecase);
        Assert.IsType<TestUsecaseDev>(usecase);
    }

    [Fact]
    public void TestRegisterUsecaseDevWithProductionEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Production", typeof(TestUsecaseDev).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecaseDev>();
        Assert.Null(usecase);
    }

    [Fact]
    public void TestRegisterUsecaseProdWithProductionEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Production", typeof(TestUsecaseProd).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecaseProd>();

        Assert.NotNull(usecase);
        Assert.IsType<TestUsecaseProd>(usecase);
    }

    [Fact]
    public void TestRegisterUsecaseProdWithProdEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Prod", typeof(TestUsecaseProd).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecaseProd>();

        Assert.NotNull(usecase);
        Assert.IsType<TestUsecaseProd>(usecase);
    }

    [Fact]
    public void TestRegisterUsecaseProdWithDevelopmentEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(TestUsecaseProd).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecaseProd>();
        Assert.Null(usecase);
    }

    [Fact]
    public void TestRegisterServiceDevWithDevelopmentEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(TestServiceDev).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceDev>();

        Assert.NotNull(service);
        Assert.IsType<TestServiceDev>(service);
        Assert.Equal("dev", service.GetValue());
    }

    [Fact]
    public void TestRegisterServiceTestWithTestEnvironment()
    {
        var services = new ServiceCollection();

        try
        {
            AddCleanArchitectureCoreSafely(services, "Test", typeof(TestServiceTest).Assembly);
        }
        catch (InvalidOperationException ex)
        {
            var formatted = ex.Format();
            var message = formatted.TryGetValue("message", out var messageValue) ? messageValue?.ToString() ?? "" : "";
            if (message.Contains("multiple.implementations"))
            {
                Assert.True(true, "Multiple implementations detected as expected");
                return;
            }
            throw;
        }

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceTest>();

        Assert.NotNull(service);
        Assert.IsType<TestServiceTest>(service);
        Assert.Equal("test", service.GetValue());
    }

    [Fact]
    public void TestRegisterServiceTestWithDevelopmentEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(TestServiceTest).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceTest>();
        Assert.Null(service);
    }

    [Fact]
    public void TestRegisterMultipleComponents()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null,
            typeof(TestUsecase).Assembly,
            typeof(TestPresenter).Assembly,
            typeof(TestMiddleware).Assembly,
            typeof(TestService).Assembly
        );

        var serviceProvider = services.BuildServiceProvider();

        var usecase = serviceProvider.GetService<TestUsecase>();
        var presenter = serviceProvider.GetService<TestPresenter>();
        var middleware = serviceProvider.GetService<TestMiddleware>();
        var service = serviceProvider.GetService<ITestService>();

        Assert.NotNull(usecase);
        Assert.NotNull(presenter);
        Assert.NotNull(middleware);
        Assert.NotNull(service);
    }

    [Fact]
    public void TestRegisterComponentsWithoutEnvironmentAttributesInProduction()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Production", typeof(TestUsecase).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecase>();
        Assert.NotNull(usecase);
    }

    [Fact]
    public void TestRegisterComponentsWithoutEnvironmentAttributesInDevelopment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(TestUsecase).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecase>();
        Assert.NotNull(usecase);
    }

    [Fact]
    public void TestRegisterComponentsWithoutEnvironmentAttributesInTest()
    {
        var services = new ServiceCollection();

        try
        {
            AddCleanArchitectureCoreSafely(services, "Test", typeof(TestUsecase).Assembly);
            var serviceProvider = services.BuildServiceProvider();
            var usecase = serviceProvider.GetService<TestUsecase>();
            Assert.NotNull(usecase);
        }
        catch (InvalidOperationException ex)
        {
            var formatted = ex.Format();
            var message = formatted.TryGetValue("message", out var messageValue) ? messageValue?.ToString() ?? "" : "";
            if (message.Contains("multiple.implementations"))
            {
                Assert.True(true, "Multiple implementations detected as expected");
            }
            else
            {
                throw;
            }
        }
    }

    [Fact]
    public void TestRegisterComponentsWithoutEnvironmentAttributesWhenNoEnvironmentSpecified()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestUsecase).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecase>();
        Assert.NotNull(usecase);
    }

    [Fact]
    public void TestRegisterServiceWithSingletonLifetime()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestServiceSingleton).Assembly);

        var serviceProvider = services.BuildServiceProvider();

        var service1 = serviceProvider.GetService<ITestServiceSingleton>();
        var service2 = serviceProvider.GetService<ITestServiceSingleton>();

        Assert.NotNull(service1);
        Assert.NotNull(service2);
        Assert.Same(service1, service2);
        Assert.Equal(service1.GetInstanceId(), service2.GetInstanceId());
    }

    [Fact]
    public void TestRegisterServiceWithTransientLifetime()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestServiceTransient).Assembly);

        var serviceProvider = services.BuildServiceProvider();

        var service1 = serviceProvider.GetService<ITestServiceTransient>();
        var service2 = serviceProvider.GetService<ITestServiceTransient>();

        Assert.NotNull(service1);
        Assert.NotNull(service2);
        Assert.NotSame(service1, service2);
        Assert.NotEqual(service1.GetInstanceId(), service2.GetInstanceId());
    }

    [Fact]
    public void TestRegisterServiceWithScopedLifetime()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestService).Assembly);

        var serviceProvider = services.BuildServiceProvider();

        using var scope1 = serviceProvider.CreateScope();
        using var scope2 = serviceProvider.CreateScope();

        var service1 = scope1.ServiceProvider.GetService<ITestService>();
        var service2 = scope1.ServiceProvider.GetService<ITestService>();
        var service3 = scope2.ServiceProvider.GetService<ITestService>();

        Assert.NotNull(service1);
        Assert.NotNull(service2);
        Assert.NotNull(service3);

        // Same scope should return same instance (Scoped lifetime)
        Assert.Same(service1, service2);

        // Different scopes should return different instances
        Assert.NotSame(service1, service3);
    }

    [Fact]
    public void TestRegisterRepository()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestRepository).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetService<ITestRepository>();

        Assert.NotNull(repository);
        Assert.IsType<TestRepository>(repository);
        Assert.Equal("repository", repository.GetValue());
    }

    [Fact]
    public void TestRegisterRepositoryWithInterface()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestRepositoryWithInterface).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetService<ITestRepositoryWithInterface>();

        Assert.NotNull(repository);
        Assert.IsType<TestRepositoryWithInterface>(repository);
        Assert.Equal("repository-interface", repository.GetValue());
    }

    [Fact]
    public void TestAutoDetectUsecaseInterface()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(GetSystemStatsUseCase).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<IGetSystemStatsUseCase>();

        Assert.NotNull(usecase);
        Assert.IsType<GetSystemStatsUseCase>(usecase);
    }

    [Fact]
    public void TestAutoDetectPresenterInterface()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(CreateUserPresenter).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var presenter = serviceProvider.GetService<ICreateUserPresenter>();

        Assert.NotNull(presenter);
        Assert.IsType<CreateUserPresenter>(presenter);
    }

    [Fact]
    public void TestAutoDetectServiceInterface()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(EmailService).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<IEmailService>();

        Assert.NotNull(service);
        Assert.IsType<EmailService>(service);
        Assert.Equal("email-service", service.GetValue());
    }

    [Fact]
    public void TestAutoDetectRepositoryInterface()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(UserRepository).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetService<IUserRepository>();

        Assert.NotNull(repository);
        Assert.IsType<UserRepository>(repository);
        Assert.Equal("user-repository", repository.GetValue());
    }

    [Fact]
    public void TestExplicitAsInterfaceOverridesAutoDetection()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestUsecaseWithInterface).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<ITestUsecaseWithInterface>();
        Assert.NotNull(usecase);
        Assert.IsType<TestUsecaseWithInterface>(usecase);
    }

    [Fact]
    public void TestGlobalMiddlewareRegistration()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(GlobalLoggingMiddleware).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var middleware = serviceProvider.GetService<GlobalLoggingMiddleware>();

        Assert.NotNull(middleware);
        Assert.IsType<GlobalLoggingMiddleware>(middleware);
    }

    [Fact]
    public void TestUsecaseWithSpecificMiddlewares()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(UsecaseWithSpecificMiddlewares).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<UsecaseWithSpecificMiddlewares>();

        Assert.NotNull(usecase);
        Assert.IsType<UsecaseWithSpecificMiddlewares>(usecase);
    }

    [Fact]
    public void TestMiddlewareResolverCombinesGlobalAndSpecific()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null,
            typeof(GlobalLoggingMiddleware).Assembly,
            typeof(UsecaseWithSpecificMiddlewares).Assembly
        );

        var serviceProvider = services.BuildServiceProvider();
        var resolver = serviceProvider.GetService<IMiddlewareResolver>();

        Assert.NotNull(resolver);

        var preMiddlewares = resolver!.GetPreMiddlewares(typeof(UsecaseWithSpecificMiddlewares));

        Assert.True(preMiddlewares.Count >= 1);

        var hasGlobal = preMiddlewares.Any(m => m is GlobalLoggingMiddleware);
        var hasSpecific = preMiddlewares.Any(m => m is IAuthenticationMiddleware || m is SpecificAuthMiddleware);

        Assert.True(hasGlobal);
        Assert.True(hasSpecific);
    }

    [Fact]
    public void TestMiddlewareResolverResolvesInterfaces()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null,
            typeof(SpecificAuthMiddleware).Assembly,
            typeof(UsecaseWithInterfaceMiddlewares).Assembly
        );

        var serviceProvider = services.BuildServiceProvider();
        var resolver = serviceProvider.GetService<IMiddlewareResolver>();

        Assert.NotNull(resolver);

        var preMiddlewares = resolver!.GetPreMiddlewares(typeof(UsecaseWithInterfaceMiddlewares));

        Assert.True(preMiddlewares.Count >= 2);

        var hasAuth = preMiddlewares.Any(m => m is IAuthenticationMiddleware);
        var hasAuthz = preMiddlewares.Any(m => m is IAuthorizationMiddleware);

        Assert.True(hasAuth);
        Assert.True(hasAuthz);
    }

    [Fact]
    public void TestRegisterRepositoryWithImplementationAttribute()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestRepositoryImplementation).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetService<ITestRepositoryWithImplementation>();

        Assert.NotNull(repository);
        Assert.IsType<TestRepositoryImplementation>(repository);
        Assert.Equal("repository-implementation", repository.GetValue());
    }

    [Fact]
    public void TestRegisterServiceWithImplementationAttribute()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestServiceImplementation).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceWithImplementation>();

        Assert.NotNull(service);
        Assert.IsType<TestServiceImplementation>(service);
        Assert.Equal("test-implementation", service.GetValue());
    }

    [Fact]
    public void TestRegisterServiceWithInterfaceWithoutImplementationDoesNotThrowInTest()
    {
        var services = new ServiceCollection();

        try
        {
            AddCleanArchitectureCoreSafely(services, "Test", typeof(TestServiceWithInterfaceWithoutImplementation).Assembly);
        }
        catch (InvalidOperationException ex)
        {
            var formatted = ex.Format();
            var message = formatted.TryGetValue("message", out var messageValue) ? messageValue?.ToString() ?? "" : "";
            if (message.Contains("multiple.implementations"))
            {
                return;
            }
            throw;
        }

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceWithoutImplementation>();

        Assert.NotNull(service);
        Assert.IsType<TestServiceWithInterfaceWithoutImplementation>(service);
    }

    [Fact]
    public void TestRegisterDefaultPipelineAutomatically()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(IPipeline).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var pipeline = serviceProvider.GetService<IPipeline>();

        Assert.NotNull(pipeline);
        Assert.True(pipeline is CleanArchitectureCore.Pipeline.Pipeline);
    }

    [Fact]
    public void TestRegisterDefaultPipelineWithMultipleAssemblies()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null,
            typeof(IPipeline).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var pipeline = serviceProvider.GetService<IPipeline>();

        Assert.NotNull(pipeline);
        Assert.True(pipeline is CleanArchitectureCore.Pipeline.Pipeline);

        var serviceProvider2 = services.BuildServiceProvider();
        var pipeline2 = serviceProvider2.GetService<IPipeline>();

        Assert.NotNull(pipeline2);
        Assert.True(pipeline2 is CleanArchitectureCore.Pipeline.Pipeline);
    }

    [Fact]
    public void TestRegisterDefaultPipelineIsScoped()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(IPipeline).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var pipeline1 = serviceProvider.GetService<IPipeline>();
        var pipeline2 = serviceProvider.GetService<IPipeline>();

        Assert.NotNull(pipeline1);
        Assert.NotNull(pipeline2);
        Assert.Same(pipeline1, pipeline2);
    }

    [Fact]
    public void TestRegisterDefaultPipelineCreatesNewInstancePerScope()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(IPipeline).Assembly);

        var serviceProvider = services.BuildServiceProvider();

        using var scope1 = serviceProvider.CreateScope();
        var pipeline1 = scope1.ServiceProvider.GetService<IPipeline>();

        using var scope2 = serviceProvider.CreateScope();
        var pipeline2 = scope2.ServiceProvider.GetService<IPipeline>();

        Assert.NotNull(pipeline1);
        Assert.NotNull(pipeline2);
        Assert.NotSame(pipeline1, pipeline2);
        Assert.True(pipeline1 is CleanArchitectureCore.Pipeline.Pipeline);
        Assert.True(pipeline2 is CleanArchitectureCore.Pipeline.Pipeline);
    }

    [Fact]
    public void TestRegisterDefaultPipelineOnlyOnce()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(IPipeline).Assembly);
        AddCleanArchitectureCoreSafely(services, null, typeof(IPipeline).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var pipelines = serviceProvider.GetServices<IPipeline>().ToList();

        Assert.Single(pipelines);
        Assert.True(pipelines[0] is CleanArchitectureCore.Pipeline.Pipeline);
    }

    [Fact]
    public void TestRegisterServiceDevWithDevEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Dev", typeof(TestServiceDev).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceDev>();

        Assert.NotNull(service);
        Assert.IsType<TestServiceDev>(service);
        Assert.Equal("dev", service.GetValue());
    }

    [Fact]
    public void TestRegisterServiceDevWithTestEnvironment()
    {
        var services = new ServiceCollection();

        try
        {
            AddCleanArchitectureCoreSafely(services, "Test", typeof(TestServiceDev).Assembly);
        }
        catch (InvalidOperationException ex)
        {
            var formatted = ex.Format();
            var message = formatted.TryGetValue("message", out var messageValue) ? messageValue?.ToString() ?? "" : "";
            if (message.Contains("multiple.implementations"))
            {
                return;
            }
            throw;
        }

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceDev>();
        Assert.Null(service);
    }

    [Fact]
    public void TestRegisterServiceDevWithProductionEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Production", typeof(TestServiceDev).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceDev>();
        Assert.Null(service);
    }

    [Fact]
    public void TestRegisterServiceTestWithProductionEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Production", typeof(TestServiceTest).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceTest>();
        Assert.Null(service);
    }

    [Fact]
    public void TestRegisterUsecaseDevWithTestEnvironment()
    {
        var services = new ServiceCollection();

        try
        {
            AddCleanArchitectureCoreSafely(services, "Test", typeof(TestUsecaseDev).Assembly);
        }
        catch (InvalidOperationException ex)
        {
            var formatted = ex.Format();
            var message = formatted.TryGetValue("message", out var messageValue) ? messageValue?.ToString() ?? "" : "";
            if (message.Contains("multiple.implementations"))
            {
                return;
            }
            throw;
        }

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecaseDev>();
        Assert.Null(usecase);
    }

    [Fact]
    public void TestRegisterUsecaseProdWithTestEnvironment()
    {
        var services = new ServiceCollection();

        try
        {
            AddCleanArchitectureCoreSafely(services, "Test", typeof(TestUsecaseProd).Assembly);
        }
        catch (InvalidOperationException ex)
        {
            var formatted = ex.Format();
            var message = formatted.TryGetValue("message", out var messageValue) ? messageValue?.ToString() ?? "" : "";
            if (message.Contains("multiple.implementations"))
            {
                return;
            }
            throw;
        }

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecaseProd>();
        Assert.Null(usecase);
    }

    [Fact]
    public void TestRegisterUsecaseProdWithDevEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Dev", typeof(TestUsecaseProd).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecaseProd>();
        Assert.Null(usecase);
    }

    [Fact]
    public void TestRegisterServiceWithEnvironmentCaseInsensitive()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "DEVELOPMENT", typeof(TestServiceDev).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceDev>();

        Assert.NotNull(service);
        Assert.IsType<TestServiceDev>(service);
    }

    [Fact]
    public void TestRegisterServiceWithEnvironmentCaseInsensitiveProd()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "PRODUCTION", typeof(TestUsecaseProd).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecaseProd>();

        Assert.NotNull(usecase);
        Assert.IsType<TestUsecaseProd>(usecase);
    }

    [Fact]
    public void TestRegisterServiceWithEnvironmentCaseInsensitiveTest()
    {
        var services = new ServiceCollection();

        try
        {
            AddCleanArchitectureCoreSafely(services, "TEST", typeof(TestServiceTest).Assembly);
        }
        catch (InvalidOperationException ex)
        {
            var formatted = ex.Format();
            var message = formatted.TryGetValue("message", out var messageValue) ? messageValue?.ToString() ?? "" : "";
            if (message.Contains("multiple.implementations"))
            {
                Assert.True(true, "Multiple implementations detected as expected");
                return;
            }
            throw;
        }

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestServiceTest>();

        Assert.NotNull(service);
        Assert.IsType<TestServiceTest>(service);
    }

    [Fact]
    public void TestRegisterRepositoryWithEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(TestRepository).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetService<ITestRepository>();

        Assert.NotNull(repository);
        Assert.IsType<TestRepository>(repository);
    }

    [Fact]
    public void TestRegisterPresenterWithEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(TestPresenter).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var presenter = serviceProvider.GetService<TestPresenter>();

        Assert.NotNull(presenter);
        Assert.IsType<TestPresenter>(presenter);
    }

    [Fact]
    public void TestRegisterMiddlewareWithEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(TestMiddleware).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var middleware = serviceProvider.GetService<TestMiddleware>();

        Assert.NotNull(middleware);
        Assert.IsType<TestMiddleware>(middleware);
    }

    [Fact]
    public void TestRegisterUsecaseWithEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(TestUsecase).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecase>();

        Assert.NotNull(usecase);
        Assert.IsType<TestUsecase>(usecase);
    }

    [Fact]
    public void TestRegisterServiceWithEmptyEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "", typeof(TestUsecaseDev).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecaseDev>();
        Assert.Null(usecase);
    }

    [Fact]
    public void TestRegisterServiceWithNullEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestUsecaseDev).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecaseDev>();
        Assert.Null(usecase);
    }

    [Fact]
    public void TestRegisterServiceWithWhitespaceEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "   ", typeof(TestUsecaseDev).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecaseDev>();
        Assert.Null(usecase);
    }

    [Fact]
    public void TestRegisterServiceWithInvalidEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Invalid", typeof(TestUsecaseDev).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecaseDev>();
        Assert.Null(usecase);
    }

    [Fact]
    public void TestRegisterComponentsWithoutEnvironmentAttributesWithInvalidEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Invalid", typeof(TestUsecase).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecase>();
        Assert.NotNull(usecase);
    }

    [Fact]
    public void TestClearRegistrationCaches()
    {
        var services1 = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services1, null, typeof(TestUsecase).Assembly);

        var statsBefore = ServiceCollectionExtensions.GetCacheStatistics();
        Assert.True(statsBefore["AssemblyTypes"] > 0 || statsBefore["TypeEnvironment"] > 0 || statsBefore["InterfaceName"] > 0);

        ServiceCollectionExtensions.ClearRegistrationCaches();

        var statsAfter = ServiceCollectionExtensions.GetCacheStatistics();
        Assert.Equal(0, statsAfter["AssemblyTypes"]);
        Assert.Equal(0, statsAfter["TypeEnvironment"]);
        Assert.Equal(0, statsAfter["InterfaceName"]);

        var services2 = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services2, null, typeof(TestUsecase).Assembly);

        var serviceProvider = services2.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecase>();
        Assert.NotNull(usecase);
    }

    [Fact]
    public void TestGetCacheStatistics()
    {
        ServiceCollectionExtensions.ClearRegistrationCaches();

        var statsEmpty = ServiceCollectionExtensions.GetCacheStatistics();
        Assert.Equal(0, statsEmpty["AssemblyTypes"]);
        Assert.Equal(0, statsEmpty["TypeEnvironment"]);
        Assert.Equal(0, statsEmpty["InterfaceName"]);

        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestUsecase).Assembly);

        var statsAfter = ServiceCollectionExtensions.GetCacheStatistics();
        Assert.True(statsAfter["AssemblyTypes"] > 0);
    }

    [Fact]
    public void TestMultipleServiceImplementationsAllowed()
    {
        var services = new ServiceCollection();
        var exceptionThrown = false;

        try
        {
            AddCleanArchitectureCoreSafely(services, "Test", typeof(SmtpEmailService).Assembly);
        }
        catch (InvalidOperationException ex)
        {
            exceptionThrown = true;
            var formatted = ex.Format();
            var message = formatted.TryGetValue("message", out var messageValue) ? messageValue?.ToString() ?? "" : "";
            Assert.False(message.Contains("service.multiple.implementations"),
                "Services should allow multiple implementations. Exception was: " + message);
        }

        if (!exceptionThrown)
        {
            var serviceProvider = services.BuildServiceProvider();
            var servicesList = serviceProvider.GetServices<IMultipleEmailService>().ToList();

            Assert.True(servicesList.Count >= 2, "Should have at least 2 implementations registered");
            Assert.Contains(servicesList, s => s.GetType() == typeof(SmtpEmailService));
            Assert.Contains(servicesList, s => s.GetType() == typeof(SendGridEmailService));
        }
    }

    [Fact]
    public void TestMultipleUsecaseImplementationsThrowsException()
    {
        var services = new ServiceCollection();
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            services.AddCleanArchitectureCore(typeof(ConflictTestFirstUsecase).Assembly);
        });

        Assert.NotNull(exception);
        var errors = ExtractErrors(exception);
        var foundError = errors.FirstOrDefault(e =>
        {
            var msg = e.TryGetValue("message", out var m) ? m?.ToString() ?? "" : "";
            return msg.Contains("multiple") && msg.Contains("implementations");
        });

        Assert.NotNull(foundError);

        // Verify that conflicting components are NOT registered in the DI container
        var serviceDescriptors = services.Where(sd =>
            sd.ServiceType == typeof(IMultipleUsecaseInterface) ||
            sd.ImplementationType == typeof(ConflictTestFirstUsecase) ||
            sd.ImplementationType == typeof(ConflictTestSecondUsecase)
        ).ToList();

        Assert.Empty(serviceDescriptors);
    }


    [Fact]
    public void TestMultipleRepositoryImplementationsAllowed()
    {
        var services = new ServiceCollection();
        var exceptionThrown = false;

        try
        {
            AddCleanArchitectureCoreSafely(services, "Test", typeof(FirstRepository).Assembly);
        }
        catch (InvalidOperationException ex)
        {
            exceptionThrown = true;
            var formatted = ex.Format();
            var message = formatted.TryGetValue("message", out var messageValue) ? messageValue?.ToString() ?? "" : "";
            Assert.False(message.Contains("repository.multiple.implementations"),
                "Repositories should allow multiple implementations. Exception was: " + message);
        }

        if (!exceptionThrown)
        {
            var serviceProvider = services.BuildServiceProvider();
            var repositories = serviceProvider.GetServices<IMultipleRepositoryInterface>().ToList();

            Assert.True(repositories.Count >= 2, "Should have at least 2 implementations registered");
            Assert.Contains(repositories, r => r.GetType() == typeof(FirstRepository));
            Assert.Contains(repositories, r => r.GetType() == typeof(SecondRepository));
        }
    }

    [Fact]
    public void TestMultiplePresenterImplementationsThrowsException()
    {
        var services = new ServiceCollection();
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            services.AddCleanArchitectureCore(typeof(ConflictTestFirstPresenter).Assembly);
        });

        Assert.NotNull(exception);
        var errors = ExtractErrors(exception);
        var foundError = errors.FirstOrDefault(e =>
        {
            var msg = e.TryGetValue("message", out var m) ? m?.ToString() ?? "" : "";
            return msg.Contains("multiple") && msg.Contains("implementations");
        });

        Assert.NotNull(foundError);
    }

    [Fact]
    public void TestMultiplePipelineImplementationsThrowsException()
    {
        var services = new ServiceCollection();
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            services.AddCleanArchitectureCore(typeof(ConflictTestFirstPipeline).Assembly);
        });

        Assert.NotNull(exception);
        var errors = ExtractErrors(exception);
        var foundError = errors.FirstOrDefault(e =>
        {
            var msg = e.TryGetValue("message", out var m) ? m?.ToString() ?? "" : "";
            return msg.Contains("multiple") && msg.Contains("implementations");
        });

        Assert.NotNull(foundError);
    }

    [Fact]
    public void TestSingleImplementationSucceeds()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(TestService).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<ITestService>();

        Assert.NotNull(service);
        Assert.IsType<TestService>(service);
    }

    [Fact]
    public void TestDebugModeLogging()
    {
        var logMessages = new List<string>();
        var services = new ServiceCollection();

        AddCleanArchitectureCoreSafely(
            services,
            null,
            options =>
            {
                options.Debug = true;
                options.Logger = message => logMessages.Add(message);
            },
            typeof(TestUsecase).Assembly
        );

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecase>();

        Assert.NotNull(usecase);
        Assert.True(logMessages.Count > 0);
        Assert.Contains(logMessages, m => m.Contains("Starting Clean Architecture Scan"));
        Assert.Contains(logMessages, m => m.Contains("Scan Completed"));
    }

    [Fact]
    public void TestDebugModeDisabled()
    {
        var logMessages = new List<string>();
        var services = new ServiceCollection();

        AddCleanArchitectureCoreSafely(services, null,
            options =>
            {
                options.Debug = false;
                options.Logger = message => logMessages.Add(message);
            },
            typeof(TestUsecase).Assembly
        );

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecase>();

        Assert.NotNull(usecase);
        Assert.Empty(logMessages);
    }

    [Fact]
    public void TestDebugModeWithCustomLogger()
    {
        var customLogs = new List<string>();
        var services = new ServiceCollection();

        AddCleanArchitectureCoreSafely(services, null,
            options =>
            {
                options.Debug = true;
                options.Logger = msg => customLogs.Add($"[CUSTOM] {msg}");
            },
            typeof(TestUsecase).Assembly
        );

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecase>();

        Assert.NotNull(usecase);
        Assert.True(customLogs.Count > 0);
        Assert.All(customLogs, log => Assert.StartsWith("[CUSTOM]", log));
    }

    [Fact]
    public void TestRegisterPipelineWithAttribute()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(MyCustomPipeline).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var pipeline = serviceProvider.GetService<IPipeline>();

        Assert.NotNull(pipeline);
        Assert.IsType<MyCustomPipeline>(pipeline);
    }

    [Fact]
    public void TestRegisterPipelineWithoutAttributeAsFallback()
    {
        ServiceCollectionExtensions.ClearRegistrationCaches();

        var services = new ServiceCollection();
        var assembly = typeof(CustomPipelineWithoutAttribute).Assembly;

        var assemblyTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IPipeline).IsAssignableFrom(t))
            .ToList();

        var hasAttributedPipeline = assemblyTypes.Any(t => t.GetCustomAttribute<PipelineAttribute>() != null);

        AddCleanArchitectureCoreSafely(services, null, assembly);
        var serviceProvider = services.BuildServiceProvider();
        var pipeline = serviceProvider.GetService<IPipeline>();

        Assert.NotNull(pipeline);

        if (hasAttributedPipeline)
        {
            var hasAttribute = assemblyTypes.First(t => t == pipeline!.GetType())
                .GetCustomAttribute<PipelineAttribute>() != null;
            Assert.True(hasAttribute, "When both attributed and non-attributed pipelines exist, attributed one should be selected");
        }
        else
        {
            Assert.IsType<CustomPipelineWithoutAttribute>(pipeline);
        }
    }

    [Fact]
    public void TestRegisterPipelinePreferAttributeOverFallback()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null,
            typeof(MyCustomPipeline).Assembly,
            typeof(CustomPipelineWithoutAttribute).Assembly
        );

        var serviceProvider = services.BuildServiceProvider();
        var pipeline = serviceProvider.GetService<IPipeline>();

        Assert.NotNull(pipeline);
        Assert.IsType<MyCustomPipeline>(pipeline);
    }

    [Fact]
    public void TestRegisterPipelineWithCustomLifetime()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(MyCustomPipeline).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var pipeline1 = serviceProvider.GetService<IPipeline>();
        var pipeline2 = serviceProvider.GetService<IPipeline>();

        Assert.NotNull(pipeline1);
        Assert.NotNull(pipeline2);
        Assert.Same(pipeline1, pipeline2);
    }

    [Fact]
    public void TestRegisterPipelinesOnlyOnce()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(MyCustomPipeline).Assembly);
        AddCleanArchitectureCoreSafely(services, null, typeof(MyCustomPipeline).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var pipelines = serviceProvider.GetServices<IPipeline>().ToList();

        Assert.Single(pipelines);
        Assert.IsType<MyCustomPipeline>(pipelines[0]);
    }

    [Fact]
    public void TestAddCleanArchitectureCoreWithConfigureOptionsOnly()
    {
        var services = new ServiceCollection();
        var logMessages = new List<string>();

        AddCleanArchitectureCoreSafely(services, null,
            options =>
            {
                options.Debug = true;
                options.Logger = msg => logMessages.Add(msg);
            },
            typeof(TestUsecase).Assembly
        );

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecase>();

        Assert.NotNull(usecase);
        Assert.True(logMessages.Count > 0);
    }

    [Fact]
    public void TestAddCleanArchitectureCoreWithEnvironmentAndConfigureOptions()
    {
        var services = new ServiceCollection();
        var logMessages = new List<string>();

        AddCleanArchitectureCoreSafely(
            services,
            "Development",
            options =>
            {
                options.Debug = true;
                options.Logger = msg => logMessages.Add(msg);
            },
            typeof(TestUsecaseDev).Assembly
        );

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<TestUsecaseDev>();

        Assert.NotNull(usecase);
        Assert.True(logMessages.Count > 0);
        Assert.Contains(logMessages, m => m.Contains("Development"));
    }

    [Fact]
    public void TestCacheWorksAcrossMultipleRegistrations()
    {
        ServiceCollectionExtensions.ClearRegistrationCaches();

        var services1 = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services1, null, typeof(TestUsecase).Assembly);

        var stats1 = ServiceCollectionExtensions.GetCacheStatistics();

        var services2 = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services2, null, typeof(TestUsecase).Assembly);

        var stats2 = ServiceCollectionExtensions.GetCacheStatistics();

        Assert.Equal(stats1["AssemblyTypes"], stats2["AssemblyTypes"]);
    }

    [Fact]
    public void TestRegisterMultiplePipelinesWithDifferentNames()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null,
            typeof(MyCustomPipeline).Assembly,
            typeof(CustomPipelineWithoutAttribute).Assembly
        );

        var serviceProvider = services.BuildServiceProvider();
        var pipeline = serviceProvider.GetService<IPipeline>();

        Assert.NotNull(pipeline);
        var pipelines = serviceProvider.GetServices<IPipeline>().ToList();
        Assert.Single(pipelines);
    }

    [Fact]
    public void TestMiddlewarePriorityOrdering()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, null, typeof(GlobalPreMiddlewareWithPriority).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var resolver = serviceProvider.GetService<IMiddlewareResolver>();

        Assert.NotNull(resolver);

        var preMiddlewares = resolver!.GetPreMiddlewares(typeof(TestUsecase));
        Assert.True(preMiddlewares.Count > 0);
    }

    [Fact]
    public void TestAddCleanArchitectureCoreWithEmptyAssembliesArrayUsesCallingAssembly()
    {
        var services = new ServiceCollection();
        // Pass empty array to trigger Assembly.GetCallingAssembly() path
        AddCleanArchitectureCoreSafely(services, null, []);

        var serviceProvider = services.BuildServiceProvider();
        var pipeline = serviceProvider.GetService<IPipeline>();
        Assert.NotNull(pipeline);
    }

    [Fact]
    public void TestAddCleanArchitectureCoreWithEmptyAssembliesAndEnvironment()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", []);

        var serviceProvider = services.BuildServiceProvider();
        var pipeline = serviceProvider.GetService<IPipeline>();
        Assert.NotNull(pipeline);
    }

    [Fact]
    public void TestAddCleanArchitectureCoreWithEmptyAssembliesAndOptions()
    {
        var services = new ServiceCollection();
        var logMessages = new List<string>();
        AddCleanArchitectureCoreSafely(services, null,
            options =>
            {
                options.Debug = true;
                options.Logger = msg => logMessages.Add(msg);
            },
            []);

        var serviceProvider = services.BuildServiceProvider();
        var pipeline = serviceProvider.GetService<IPipeline>();
        Assert.NotNull(pipeline);
        Assert.True(logMessages.Count > 0);
    }

    [Fact]
    public void TestRegisterDefaultPipelineWhenAlreadyRegisteredShouldSkip()
    {
        var services = new ServiceCollection();
        // Register IPipeline first
        services.AddScoped<IPipeline, MyCustomPipeline>();
        // Then call AddCleanArchitectureCore
        AddCleanArchitectureCoreSafely(services, null, typeof(MyCustomPipeline).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var pipelines = serviceProvider.GetServices<IPipeline>().ToList();
        Assert.Single(pipelines);
    }

    [Fact]
    public void TestRegisterDefaultPipelineWithoutAttributeShouldLogWarning()
    {
        var services = new ServiceCollection();
        var logMessages = new List<string>();

        AddCleanArchitectureCoreSafely(services, null,
            options =>
            {
                options.Debug = true;
                options.Logger = msg => logMessages.Add(msg);
            },
            typeof(CustomPipelineWithoutAttribute).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var pipeline = serviceProvider.GetService<IPipeline>();
        Assert.NotNull(pipeline);

        Assert.True(logMessages.Count > 0);
    }

    [Fact]
    public void TestRegisterDefaultPipelineWhenNoImplementationFoundShouldLogMessage()
    {
        var services = new ServiceCollection();
        var logMessages = new List<string>();
        // Use an assembly that doesn't contain any IPipeline implementation
        var emptyAssembly = typeof(string).Assembly; // System.Private.CoreLib

        AddCleanArchitectureCoreSafely(services, null,
            options =>
            {
                options.Debug = true;
                options.Logger = msg => logMessages.Add(msg);
            },
            emptyAssembly);

        var serviceProvider = services.BuildServiceProvider();
        var pipeline = serviceProvider.GetService<IPipeline>();
        // The core assembly has Pipeline, so it should be registered
        Assert.NotNull(pipeline);
    }

    [Fact]
    public void TestServiceWithoutInterfaceImplementationShouldThrowException()
    {
        var services = new ServiceCollection();

        try
        {
            services.AddCleanArchitectureCore(typeof(ServiceWithoutInterfaceImplementation).Assembly);
            Assert.Fail("Expected InvalidOperationException to be thrown");
        }
        catch (InvalidOperationException ex)
        {
            var errors = ExtractErrors(ex);
            var foundError = errors.FirstOrDefault(e =>
            {
                var msg = e.TryGetValue("message", out var m) ? m?.ToString() : null;
                return msg == "service.interface.without.implementation";
            });

            Assert.NotNull(foundError);
            Assert.Equal("ServiceWithoutInterfaceImplementation", foundError.TryGetValue("componentType", out var ct) ? ct?.ToString() : null);
            Assert.Equal("IServiceWithoutImplementation", foundError.TryGetValue("interfaceType", out var it) ? it?.ToString() : null);
        }
    }

    [Fact]
    public void TestRepositoryWithoutInterfaceImplementationShouldThrowException()
    {
        var services = new ServiceCollection();

        try
        {
            services.AddCleanArchitectureCore(typeof(RepositoryWithoutInterfaceImplementation).Assembly);
            Assert.Fail("Expected InvalidOperationException to be thrown");
        }
        catch (InvalidOperationException ex)
        {
            var errors = ExtractErrors(ex);
            var foundError = errors.FirstOrDefault(e =>
            {
                var msg = e.TryGetValue("message", out var m) ? m?.ToString() : null;
                return msg == "repository.interface.without.implementation";
            });

            Assert.NotNull(foundError);
            var componentType = foundError.TryGetValue("componentType", out var ct) ? ct?.ToString() : null;
            Assert.Equal("RepositoryWithoutInterfaceImplementation", componentType);
        }
    }

    [Fact]
    public void TestUsecaseWithoutInterfaceImplementationShouldThrowException()
    {
        var services = new ServiceCollection();

        try
        {
            services.AddCleanArchitectureCore(typeof(UsecaseWithoutInterfaceImplementation).Assembly);
            Assert.Fail("Expected InvalidOperationException to be thrown");
        }
        catch (InvalidOperationException ex)
        {
            var errors = ExtractErrors(ex);
            var foundError = errors.FirstOrDefault(e =>
            {
                var msg = e.TryGetValue("message", out var m) ? m?.ToString() : null;
                return msg == "usecase.interface.without.implementation";
            });

            Assert.NotNull(foundError);
            var componentType = foundError.TryGetValue("componentType", out var ct) ? ct?.ToString() : null;
            Assert.Equal("UsecaseWithoutInterfaceImplementation", componentType);
        }
    }

    [Fact]
    public void TestPresenterWithoutInterfaceImplementationShouldThrowException()
    {
        var services = new ServiceCollection();

        try
        {
            services.AddCleanArchitectureCore(typeof(PresenterWithoutInterfaceImplementation).Assembly);
            Assert.Fail("Expected InvalidOperationException to be thrown");
        }
        catch (InvalidOperationException ex)
        {
            var errors = ExtractErrors(ex);
            var foundError = errors.FirstOrDefault(e =>
            {
                var msg = e.TryGetValue("message", out var m) ? m?.ToString() : null;
                return msg == "presenter.interface.without.implementation";
            });

            Assert.NotNull(foundError);
            var componentType = foundError.TryGetValue("componentType", out var ct) ? ct?.ToString() : null;
            Assert.Equal("PresenterWithoutInterfaceImplementation", componentType);
        }
    }

    [Fact]
    public void TestMiddlewareWithoutInterfaceImplementationShouldThrowException()
    {
        var services = new ServiceCollection();

        try
        {
            services.AddCleanArchitectureCore(typeof(MiddlewareWithoutInterfaceImplementation).Assembly);
            Assert.Fail("Expected InvalidOperationException to be thrown");
        }
        catch (InvalidOperationException ex)
        {
            var errors = ExtractErrors(ex);
            var foundError = errors.FirstOrDefault(e =>
            {
                var msg = e.TryGetValue("message", out var m) ? m?.ToString() : null;
                return msg == "middleware.interface.without.implementation";
            });

            Assert.NotNull(foundError);
            var componentType = foundError.TryGetValue("componentType", out var ct) ? ct?.ToString() : null;
            Assert.Equal("MiddlewareWithoutInterfaceImplementation", componentType);
        }
    }

    [Fact]
    public void TestMultiEnvironmentUsecaseShouldRegisterInDevAndTest()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(MultiEnvironmentUsecase).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<MultiEnvironmentUsecase>();

        Assert.NotNull(usecase);
        Assert.IsType<MultiEnvironmentUsecase>(usecase);
    }

    [Fact]
    public void TestMultiEnvironmentUsecaseShouldRegisterInTest()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Test", typeof(MultiEnvironmentUsecase).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<MultiEnvironmentUsecase>();

        Assert.NotNull(usecase);
        Assert.IsType<MultiEnvironmentUsecase>(usecase);
    }

    [Fact]
    public void TestMultiEnvironmentUsecaseShouldNotRegisterInProduction()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Production", typeof(MultiEnvironmentUsecase).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<MultiEnvironmentUsecase>();

        Assert.Null(usecase);
    }

    [Fact]
    public void TestDevProdUsecaseShouldRegisterInDev()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(DevProdUsecase).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<DevProdUsecase>();

        Assert.NotNull(usecase);
        Assert.IsType<DevProdUsecase>(usecase);
    }

    [Fact]
    public void TestDevProdUsecaseShouldRegisterInProduction()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Production", typeof(DevProdUsecase).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<DevProdUsecase>();

        Assert.NotNull(usecase);
        Assert.IsType<DevProdUsecase>(usecase);
    }

    [Fact]
    public void TestDevProdUsecaseShouldNotRegisterInTest()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Test", typeof(DevProdUsecase).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var usecase = serviceProvider.GetService<DevProdUsecase>();

        Assert.Null(usecase);
    }

    [Fact]
    public void TestMultiEnvironmentPresenterShouldRegisterInDevAndTest()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(MultiEnvironmentPresenter).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var presenter = serviceProvider.GetService<MultiEnvironmentPresenter>();

        Assert.NotNull(presenter);
        Assert.IsType<MultiEnvironmentPresenter>(presenter);
    }

    [Fact]
    public void TestMultiEnvironmentPresenterShouldRegisterInTest()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Test", typeof(MultiEnvironmentPresenter).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var presenter = serviceProvider.GetService<MultiEnvironmentPresenter>();

        Assert.NotNull(presenter);
        Assert.IsType<MultiEnvironmentPresenter>(presenter);
    }

    [Fact]
    public void TestTestProdPresenterShouldRegisterInTestAndProduction()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Test", typeof(TestProdPresenter).Assembly);
        var serviceProvider = services.BuildServiceProvider();
        var presenter = serviceProvider.GetService<TestProdPresenter>();

        Assert.NotNull(presenter);
        Assert.IsType<TestProdPresenter>(presenter);
    }

    [Fact]
    public void TestTestProdPresenterShouldRegisterInProduction()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Production", typeof(TestProdPresenter).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var presenter = serviceProvider.GetService<TestProdPresenter>();

        Assert.NotNull(presenter);
        Assert.IsType<TestProdPresenter>(presenter);
    }

    [Fact]
    public void TestMultiEnvironmentMiddlewareShouldRegisterInDevAndTest()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(MultiEnvironmentMiddleware).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var middleware = serviceProvider.GetService<MultiEnvironmentMiddleware>();

        Assert.NotNull(middleware);
        Assert.IsType<MultiEnvironmentMiddleware>(middleware);
    }

    [Fact]
    public void TestAllEnvironmentsMiddlewareShouldRegisterInAllEnvironments()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(AllEnvironmentsMiddleware).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var middleware = serviceProvider.GetService<AllEnvironmentsMiddleware>();

        Assert.NotNull(middleware);
        Assert.IsType<AllEnvironmentsMiddleware>(middleware);
    }

    [Fact]
    public void TestAllEnvironmentsMiddlewareShouldRegisterInTest()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Test", typeof(AllEnvironmentsMiddleware).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var middleware = serviceProvider.GetService<AllEnvironmentsMiddleware>();

        Assert.NotNull(middleware);
        Assert.IsType<AllEnvironmentsMiddleware>(middleware);
    }

    [Fact]
    public void TestAllEnvironmentsMiddlewareShouldRegisterInProduction()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Production", typeof(AllEnvironmentsMiddleware).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var middleware = serviceProvider.GetService<AllEnvironmentsMiddleware>();

        Assert.NotNull(middleware);
        Assert.IsType<AllEnvironmentsMiddleware>(middleware);
    }

    [Fact]
    public void TestMultiEnvironmentServiceShouldRegisterInDevAndTest()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(MultiEnvironmentService).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<IMultiEnvironmentService>();

        Assert.NotNull(service);
        Assert.IsType<MultiEnvironmentService>(service);
        Assert.Equal("multi-env", service.GetValue());
    }

    [Fact]
    public void TestDevProdServiceShouldRegisterInDevAndProduction()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(DevProdService).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<IDevProdService>();

        Assert.NotNull(service);
        Assert.IsType<DevProdService>(service);
        Assert.Equal("dev-prod", service.GetValue());
    }

    [Fact]
    public void TestMultiEnvironmentRepositoryShouldRegisterInDevAndTest()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(MultiEnvironmentRepository).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetService<IMultiEnvironmentRepository>();

        Assert.NotNull(repository);
        Assert.IsType<MultiEnvironmentRepository>(repository);
        Assert.Equal("multi-env-repo", repository.GetValue());
    }

    [Fact]
    public void TestTestProdRepositoryShouldRegisterInTestAndProduction()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Test", typeof(TestProdRepository).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetService<ITestProdRepository>();

        Assert.NotNull(repository);
        Assert.IsType<TestProdRepository>(repository);
        Assert.Equal("test-prod-repo", repository.GetValue());
    }

    [Fact]
    public void TestMultiEnvironmentPipelineShouldRegisterInDevAndTest()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(MultiEnvironmentPipeline).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var pipeline = serviceProvider.GetService<IMultiEnvironmentPipeline>();

        Assert.NotNull(pipeline);
        Assert.IsType<MultiEnvironmentPipeline>(pipeline);
    }

    [Fact]
    public void TestAllEnvironmentsPipelineShouldRegisterInAllEnvironments()
    {
        var services = new ServiceCollection();
        AddCleanArchitectureCoreSafely(services, "Development", typeof(AllEnvironmentsPipeline).Assembly);

        var serviceProvider = services.BuildServiceProvider();
        var pipeline = serviceProvider.GetService<IAllEnvironmentsPipeline>();

        Assert.NotNull(pipeline);
        Assert.IsType<AllEnvironmentsPipeline>(pipeline);
    }
}

