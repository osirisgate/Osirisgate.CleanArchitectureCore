# Osirisgate Clean Architecture Core

[![NuGet Version](https://img.shields.io/nuget/v/Osirisgate.CleanArchitectureCore.svg)](https://www.nuget.org/packages/Osirisgate.CleanArchitectureCore/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)

**Target Framework:** .NET 8.0+

**Author:** Ulrich Geraud A. | Software Engineer | developer@osirisgate.com

**Organization:** Osirisgate

---

## Overview

**Osirisgate Clean Architecture Core** is a comprehensive .NET library designed to provide a solid foundation for implementing Clean Architecture principles in your applications. It offers a structured, testable, and maintainable way to handle application logic by enforcing clear separation between requests, business rules, and responses.

Built with modern .NET practices, this library helps developers create scalable applications with well-defined boundaries, making it easier to maintain, test, and extend your codebase.

---

## Table of contents

1. [Why use this library?](#why-use-this-library)
2. [Key features](#key-features)
3. [Installation](#installation)
4. [Quick start](#quick-start)
5. [Core concepts](#core-concepts)
6. [Dependency injection](#dependency-injection)
7. [Middleware system](#middleware-system)
8. [Pagination](#pagination)
9. [Entity mapper](#entity-mapper)
10. [Status codes](#status-codes)
11. [Request extensions](#request-extensions)
12. [Response extensions](#response-extensions)
13. [Configuration options](#configuration-options)
14. [Attributes](#attributes)
15. [Exception handling](#exception-handling)
16. [Complete examples](#complete-examples)
17. [Best practices](#best-practices)
18. [API reference](#api-reference)

---

## Why use this library?

### The problem

Building maintainable enterprise applications is challenging. Common issues include:

- **Scattered validation logic**: Validation code spread across controllers, services, and models
- **Tight coupling**: Business logic tightly coupled to frameworks (ASP.NET, EF Core, etc.)
- **Difficult testing**: Controllers with complex dependencies are hard to unit test
- **Inconsistent responses**: Error responses vary across endpoints
- **Cross-cutting concerns**: Authentication, logging, and validation duplicated everywhere
- **Technical debt**: Code becomes harder to maintain as the application grows

### The solution

Osirisgate Clean Architecture Core provides a **battle-tested framework** that:

✅ **Enforces Clean Architecture** - Your business logic stays independent of frameworks

✅ **Centralizes Validation** - Declarative, reusable validation at the request level

✅ **Simplifies Testing** - Pure, isolated use cases that are trivial to test

✅ **Standardizes Responses** - Consistent error and success responses across your API

✅ **Enables Middleware** - Pre/post execution hooks for authentication, logging, caching, etc..

✅ **Environment-Aware Development** - Swap middleware between dev and production

✅ **Reduces Boilerplate** - Fluent API and conventions eliminate repetitive code

✅ **Automatic DI Registration** - Convention-based registration using attributes

---

## Key features

### 🎯 Core features

- **Clean Architecture Pattern**: Clear separation of concerns with Use Cases, Presenters, and Requests/Responses
- **Declarative Validation**: Define complex request validation rules, including nested objects
- **Pipeline Architecture**: Built-in middleware system for cross-cutting concerns
- **Fluent API**: Chain methods in a readable, logical flow
- **Asynchronous by Default**: Built with async-first approach with full `CancellationToken` support
- **Standardized Error Handling**: Centralized exception handling with consistent error responses

### 🔧 Advanced features

- **Automatic Dependency Injection**: Convention-based registration using attributes (`[Usecase]`, `[Service]`, etc.)
- **Environment-Specific Components**: Register components per environment (Dev, Test, Prod)
- **Dual Payload System**: Separate original and modified payloads for clean separation
- **Nested Field Access**: Use dot notation to access deeply nested fields (`user.address.city`)
- **Case-Insensitive Access**: Support for snake_case, PascalCase, and camelCase
- **Request/Response Validation**: Built-in validation with structure-based payload filtering
- **Pagination Support**: Comprehensive pagination utilities for collections
- **Entity Mapping**: Automatic mapping between domain and persistence entities
- **Global and Specific Middleware**: Middleware can be global or usecase-specific with priority support

---

## Installation

```bash
### .NET CLI
dotnet add package Osirisgate.CleanArchitectureCore

### Package manager
Install-Package Osirisgate.CleanArchitectureCore

### PackageReference
<PackageReference Include="Osirisgate.CleanArchitectureCore" Version="1.0.0" />
```

---

## Quick start

### Architecture Overview

To better understand the Clean Architecture flow before diving in, here's a simplified diagram:

```
┌─────────────────────────────────────────────────────────────────┐
│                         HTTP Endpoint                           │
│                  (ASP.NET Core Controller)                      │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             │ 1. Builds payload and creates business request
                             ▼
                    ┌────────────────────────┐
                    │ Business Request Class │  ← Validates structure
                    │  (required / optional  │     and business
                    │        fields)         │     constraints
                    └──────────┬─────────────┘
                               │
                               │ 2. Execute Usecase
                               │
                ┌──────────────┴──────────────┐
                │                             │
                │                             │
                ▼                             ▼
┌──────────────────────────────┐   ┌──────────────────────────────┐
│        Direct Execution      │   │     Pipeline Execution       │
│                              │   │                              │
│  usecase.ExecuteAsync()      │   │   Pipeline.ExecuteAsync()    │
│                              │   │                              │
│  • No middleware             │   │  ┌────────────────────────┐  │
│  • Pure business logic       │   │  │     Pre-Middlewares    │  │
│  • Useful for tests          │   │  │ (Auth, Validation, etc)│  │
└──────────────┬───────────────┘   │  └────────────┬───────────┘  │
               │                   │               │              │
               │                   │               ▼              │
               │                   │  ┌────────────────────────┐  │
               │                   │  │    Usecase Execution   │  │
               │                   │  │ • Domain logic         │  │
               │                   │  │ • Repositories         │  │
               │                   │  └────────────┬───────────┘  │
               │                   │               │              │
               │                   │               ▼              │
               │                   │  ┌────────────────────────┐  │
               │                   │  │    Post-Middlewares    │  │
               │                   │  │ (Audit, Logging, etc)  │  │
               │                   │  └────────────┬───────────┘  │
               │                   └───────────────┼─────────────-┘
               │                                   │
               └───────────────────────────────────┘
                               │
                               │ 3. Formats the response
                               ▼
                ┌────────────────────────┐
                │        Presenter       │  ← Builds standardized
                │                        │     API response (web, api, mobile, etc...)
                └──────────┬─────────────┘
                           │
                           │ 4. Returns formatted output
                           ▼
                ┌────────────────────────┐
                │        Response        │
                │  • status              │
                │  • code                │
                │  • message             │
                │  • data / details      │
                │  • meta                │
                └────────────────────────┘
```

**Key Concepts:**
- **Request**: Validates and structures input data
- **UseCase**: Contains pure business logic (isolated from frameworks)
- **Presenter**: Formats standardized response
- **Pipeline**: Allows adding middlewares (auth, logging, validation, etc.)


### 1. Register services in Program.cs

```csharp
using Osirisgate.CleanArchitectureCore.DependencyInjection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCleanArchitectureCore(
    environment: builder.Environment.EnvironmentName,
    configureOptions: options =>
    {
        options.Debug = builder.Environment.IsDevelopment();
    },
    assemblies: typeof(Program).Assembly)
);

var app = builder.Build();
app.Run();
```

### 2. Define a request

```csharp
using Osirisgate.CleanArchitectureCore.Request;

public sealed class CreateBookRequest(IReadOnlyDictionary<string, object> payload) : Request(payload)
{
    protected override IDictionary<string, object> RequestStructure => new Dictionary<string, object>
    {
        ["title"] = true,         // Required
        ["author"] = true,        // Required
        ["totalCopies"] = true,   // Required
        ["isbn"] = false,         // Optional
        ["publishedYear"] = false // Optional
    };
}
```

### 3. Implement a use case

```csharp
using Osirisgate.CleanArchitectureCore.Usecase;
using Osirisgate.CleanArchitectureCore.Attributes.Type;
using Osirisgate.CleanArchitectureCore.Response;

[Usecase]
public sealed class CreateBookUseCase : Usecase
{
    private readonly IBookRepository _bookRepository;

    public CreateBookUseCase(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var title = GetField<string>("title")!;
        var author = GetField<string>("author")!;
        var totalCopies = GetField<int>("totalCopies");

        var book = Book.Create(title, author, totalCopies);
        await _bookRepository.SaveAsync(book, cancellationToken);

        PresentResponse(Response.Response.Create(
            success: true,
            statusCode: StatusCode.Created,
            message: "book.created.successfully",
            data: new Dictionary<string, object>
            {
                ["id"] = book.Id,
                ["title"] = book.Title,
                ["author"] = book.Author
            }
        ));
    }
}
```

### 4. Create a presenter

```csharp
using Osirisgate.CleanArchitectureCore.Presenter;
using Osirisgate.CleanArchitectureCore.Attributes.Type;

[Presenter]
public sealed class ApiPresenter : Presenter
{
}
```

### 5. Create an endpoint

```csharp
using Osirisgate.CleanArchitectureCore.Pipeline;
using Osirisgate.CleanArchitectureCore.DependencyInjection;

app.MapPost("/api/books", async (
    [FromBody] CreateBookRequestModel requestModel,
    [FromServices] CreateBookUseCase useCase,
    [FromServices] ApiPresenter presenter,
    [FromServices] IPipeline pipeline,
    [FromServices] IMiddlewareResolver middlewareResolver,
    CancellationToken cancellationToken) =>
{
    var payload = new Dictionary<string, object>
    {
        ["title"] = requestModel.Title,
        ["author"] = requestModel.Author,
        ["totalCopies"] = requestModel.TotalCopies
    };

    if (!string.IsNullOrWhiteSpace(requestModel.Isbn))
        payload["isbn"] = requestModel.Isbn;

    var request = new CreateBookRequest(payload);

    useCase.WithRequest(request).WithPresenter(presenter);

    middlewareResolver.ConfigurePipeline(pipeline, typeof(CreateBookUseCase));

    await pipeline.ExecuteAsync(useCase, cancellationToken);

    var response = presenter.GetFormattedResponse();
    return Results.Json(response, statusCode: (int)response["code"]);
});
```

**Success response:**
```json
{
  "status": "success",
  "code": 201,
  "message": "book.created.successfully",
  "data": {
    "id": "urn:user:a1b2c3d4-e5f6-7890-1234-567890abcdef",
    "title": "Clean Code",
    "author": "Robert C. Martin"
  },
  "meta": {}
}
```

---

## Core concepts

### Request

The `Request` class validates and manages input data.

#### Features

- **Structure validation**: Define required/optional fields
- **Nested validation**: Support for nested objects
- **Case-insensitive access**: Works with snake_case, PascalCase, camelCase
- **Field modification**: Track changes via middleware
- **DTO mapping**: Convert to strongly-typed DTOs
- **Async validation**: Custom validation logic

#### Basic example

```csharp
public sealed class RegisterUserRequest(IReadOnlyDictionary<string, object> payload) : Request(payload)
{
    protected override IDictionary<string, object> RequestStructure => new Dictionary<string, object>
    {
        ["email"] = true,
        ["password"] = true,
        ["firstName"] = false,
        ["lastName"] = false
    };

    protected override async Task ApplyConstraintsOnRequestFieldsAsync(CancellationToken cancellationToken = default)
    {
        var email = GetField<string>("email");
        if (!email.Contains("@"))
        {
            throw new BadRequestContentException(new Dictionary<string, object>
            {
                ["message"] = "validation.failed",
                ["details"] = new Dictionary<string, object>
                {
                    ["email"] = "Invalid email format"
                }
            });
        }

        await Task.CompletedTask;
    }
}
```

#### Nested objects

```csharp
protected override IDictionary<string, object> RequestStructure => new Dictionary<string, object>
{
    ["user"] = new Dictionary<string, object>
    {
        ["name"] = true,
        ["email"] = true,
        ["address"] = new Dictionary<string, object>
        {
            ["street"] = true,
            ["city"] = false
        }
    }
};

var city = request.GetField<string>("user.address.city");
```

#### FluentValidation integration

The SDK integrates seamlessly with **FluentValidation** for powerful, declarative validation rules.

##### Installation

```bash
dotnet add package FluentValidation
```

##### Method 1: Using ToDto<TDto>() with separate DTO

This is the recommended approach for complex validation scenarios.

```csharp
using Osirisgate.CleanArchitectureCore.Request;
using Osirisgate.CleanArchitectureCore.Exception;
using FluentValidation;

public sealed class UserRegistrationDto
{
    public string? Email { get; set; }
    public int? Age { get; set; }
    public string? Password { get; set; }
}

public sealed class UserRegistrationValidator : AbstractValidator<UserRegistrationDto>
{
    public UserRegistrationValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.Age)
            .NotEmpty().WithMessage("Age is required")
            .InclusiveBetween(18, 120).WithMessage("Age must be between 18 and 120");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(100).WithMessage("Password must not exceed 100 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
    }
}

public sealed class UserRegistrationRequest(IReadOnlyDictionary<string, object> payload) : Request(payload)
{
    private static readonly UserRegistrationValidator _validator = new();

    protected override IDictionary<string, object> RequestStructure => new Dictionary<string, object>
    {
        ["email"] = true,
        ["age"] = true,
        ["password"] = true
    };

    protected override async Task ApplyConstraintsOnRequestFieldsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var dto = ToDto<UserRegistrationDto>();
        var validationResult = await _validator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = new Dictionary<string, object>();
            foreach (var error in validationResult.Errors)
            {
                if (!errors.ContainsKey(error.PropertyName))
                {
                    errors[error.PropertyName] = error.ErrorMessage;
                }
            }

            throw new BadRequestContentException(new Dictionary<string, object>
            {
                ["message"] = "validation.failed",
                ["details"] = errors
            });
        }
    }
}
```

**Error response:**
```json
{
  "status": "error",
  "code": 400,
  "message": "validation.failed",
  "details": {
    "Email": "Invalid email format",
    "Age": "Age must be between 18 and 120",
    "Password": "Password must contain at least one uppercase letter"
  },
  "meta": {}
}
```

##### Method 2: Using Request class as DTO

For simpler cases, you can use the Request class itself as the validation target.

```csharp
using Osirisgate.CleanArchitectureCore.Request;
using Osirisgate.CleanArchitectureCore.Exception;
using FluentValidation;

public sealed class CreateProductRequest : Request
{
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? Category { get; set; }

    private static readonly CreateProductValidator _validator = new();

    public CreateProductRequest(IReadOnlyDictionary<string, object> payload)
        : base(payload)
    {
        var dto = ToDto();
        Name = dto.TryGetValue("Name", out var name) ? name?.ToString() : null;
        Price = dto.TryGetValue("Price", out var price) && decimal.TryParse(price?.ToString(), out var priceValue)
            ? priceValue : null;
        Category = dto.TryGetValue("Category", out var category) ? category?.ToString() : null;
    }

    public CreateProductRequest() : base(new Dictionary<string, object>()) { }

    protected override IDictionary<string, object> RequestStructure => new Dictionary<string, object>
    {
        ["name"] = true,
        ["price"] = true,
        ["category"] = true
    };

    protected override async Task ApplyConstraintsOnRequestFieldsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validationResult = await _validator.ValidateAsync(this, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = new Dictionary<string, object>();
            foreach (var error in validationResult.Errors)
            {
                if (!errors.ContainsKey(error.PropertyName))
                {
                    errors[error.PropertyName] = error.ErrorMessage;
                }
            }

            throw new BadRequestContentException(new Dictionary<string, object>
            {
                ["message"] = "validation.failed",
                ["details"] = errors
            });
        }
    }
}

public sealed class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters");

        RuleFor(x => x.Price)
            .NotNull().WithMessage("Price is required")
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .LessThanOrEqualTo(1000000).WithMessage("Price must not exceed 1,000,000");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .Must(BeValidCategory).WithMessage("Invalid category");
    }

    private bool BeValidCategory(string? category)
    {
        var validCategories = new[] { "Electronics", "Books", "Clothing", "Food" };
        return category != null && validCategories.Contains(category);
    }
}
```

##### Method 3: Complex validation with nested objects

```csharp
public sealed class CreateOrderDto
{
    public string? CustomerId { get; set; }
    public List<OrderItemDto>? Items { get; set; }
    public ShippingAddressDto? ShippingAddress { get; set; }
}

public sealed class OrderItemDto
{
    public string? ProductId { get; set; }
    public int? Quantity { get; set; }
}

public sealed class ShippingAddressDto
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
}

public sealed class OrderItemValidator : AbstractValidator<OrderItemDto>
{
    public OrderItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product ID is required")
            .Must(BeValidGuid).WithMessage("Product ID must be a valid GUID");

        RuleFor(x => x.Quantity)
            .NotNull().WithMessage("Quantity is required")
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100");
    }

    private bool BeValidGuid(string? id)
    {
        return !string.IsNullOrEmpty(id) && Guid.TryParse(id, out _);
    }
}

public sealed class ShippingAddressValidator : AbstractValidator<ShippingAddressDto>
{
    public ShippingAddressValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required")
            .MaximumLength(200).WithMessage("Street must not exceed 200 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("Zip code is required")
            .Matches(@"^\d{5}(-\d{4})?$").WithMessage("Invalid zip code format");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .Length(2).WithMessage("Country must be a 2-letter code");
    }
}

public sealed class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required")
            .Must(BeValidGuid).WithMessage("Customer ID must be a valid GUID");

        RuleFor(x => x.Items)
            .NotNull().WithMessage("Order items are required")
            .NotEmpty().WithMessage("At least one item is required")
            .Must(items => items!.Count <= 50).WithMessage("Cannot order more than 50 items at once");

        RuleForEach(x => x.Items)
            .SetValidator(new OrderItemValidator());

        RuleFor(x => x.ShippingAddress)
            .NotNull().WithMessage("Shipping address is required")
            .SetValidator(new ShippingAddressValidator()!);
    }

    private bool BeValidGuid(string? id)
    {
        return !string.IsNullOrEmpty(id) && Guid.TryParse(id, out _);
    }
}

public sealed class CreateOrderRequest(IReadOnlyDictionary<string, object> payload) : Request(payload)
{
    private static readonly CreateOrderValidator _validator = new();

    protected override IDictionary<string, object> RequestStructure => new Dictionary<string, object>
    {
        ["customerId"] = true,
        ["items"] = true,
        ["shippingAddress"] = new Dictionary<string, object>
        {
            ["street"] = true,
            ["city"] = true,
            ["zipCode"] = true,
            ["country"] = true
        }
    };

    protected override async Task ApplyConstraintsOnRequestFieldsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var dto = ToDto<CreateOrderDto>();

        var validationResult = await _validator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = new Dictionary<string, object>();
            foreach (var error in validationResult.Errors)
            {
                if (!errors.ContainsKey(error.PropertyName))
                {
                    errors[error.PropertyName] = error.ErrorMessage;
                }
            }

            throw new BadRequestContentException(new Dictionary<string, object>
            {
                ["message"] = "validation.failed",
                ["details"] = errors
            });
        }
    }
}
```

##### Method 4: Async validation with database checks

```csharp
public sealed class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    private readonly IUserRepository _userRepository;

    public CreateUserValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MustAsync(BeUniqueEmail).WithMessage("Email is already registered");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores")
            .MustAsync(BeUniqueUsername).WithMessage("Username is already taken");
    }

    private async Task<bool> BeUniqueEmail(string? email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(email)) return true;
        return !await _userRepository.ExistsByEmailAsync(email, cancellationToken);
    }

    private async Task<bool> BeUniqueUsername(string? username, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(username)) return true;
        return !await _userRepository.ExistsByUsernameAsync(username, cancellationToken);
    }
}

public sealed class CreateUserRequest : Request
{
    private readonly CreateUserValidator _validator;

    public CreateUserRequest(
        IReadOnlyDictionary<string, object> payload,
        IUserRepository userRepository)
        : base(payload)
    {
        _validator = new CreateUserValidator(userRepository);
    }

    protected override IDictionary<string, object> RequestStructure => new Dictionary<string, object>
    {
        ["email"] = true,
        ["username"] = true,
        ["password"] = true
    };

    protected override async Task ApplyConstraintsOnRequestFieldsAsync(
        CancellationToken cancellationToken = default)
    {
        var dto = ToDto<CreateUserDto>();
        var validationResult = await _validator.ValidateAsync(dto, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = new Dictionary<string, object>();
            foreach (var error in validationResult.Errors)
            {
                if (!errors.ContainsKey(error.PropertyName))
                {
                    errors[error.PropertyName] = error.ErrorMessage;
                }
            }

            throw new BadRequestContentException(new Dictionary<string, object>
            {
                ["message"] = "validation.failed",
                ["details"] = errors
            });
        }
    }
}
```

---

### Response

The `Response` class provides standardized output.

#### Response format

**Success:**
```json
{
  "status": "success",
  "code": 200,
  "message": "operation.successful",
  "data": { ... },
  "meta": {}
}
```

**Error:**
```json
{
  "status": "error",
  "code": 400,
  "message": "validation.failed",
  "details": { ... },
  "meta": {}
}
```

#### Creating responses

```csharp
var response = Response.Response.Create(
    success: true,
    statusCode: StatusCode.Ok,
    message: "user.retrieved",
    data: new Dictionary<string, object>
    {
        ["id"] = userId,
        ["email"] = userEmail
    }
);

var errorResponse = Response.Response.Create(
    success: false,
    statusCode: StatusCode.NotFound,
    message: "user.not.found",
    data: new Dictionary<string, object>
    {
        ["userId"] = requestedId
    }
);
```

#### Working with responses

```csharp
bool isSuccess = response.IsSuccess();
int statusCode = response.GetStatusCode();
string email = response.Get<string>("email");

response.UpdateField("user.name", "John Doe");
response.UpdateMetaField("timestamp", DateTime.UtcNow);
```

---

### Usecase

The `Usecase` class encapsulates business logic.

#### Basic example

```csharp
[Usecase]
public sealed class GetBookUseCase : Usecase
{
    private readonly IBookRepository _bookRepository;

    public GetBookUseCase(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = GetField<string>("id")!;

        var book = await _bookRepository.GetByIdAsync(id, cancellationToken);

        if (book == null)
        {
            throw new BadRequestContentException(new Dictionary<string, object>
            {
                ["message"] = "book.not.found",
                ["details"] = new Dictionary<string, object>
                {
                    ["id"] = id
                }
            });
        }

        PresentResponse(Response.Response.Create(
            success: true,
            statusCode: StatusCode.Ok,
            message: "book.retrieved.successfully",
            data: book.ToDictionary()
        ));
    }
}
```

#### Accessing request data

```csharp
var email = GetField<string>("email");
var userId = GetModifiedField<string>("authenticatedUserId");

var payload = GetPayload();
var modifiedPayload = GetModifiedPayload();
```

---

### Presenter

The `Presenter` class formats responses.

```csharp
[Presenter]
public sealed class ApiPresenter : Presenter
{
}

var presenter = new ApiPresenter();
var formattedResponse = presenter.GetFormattedResponse();
```

---

### Pipeline

The `Pipeline` orchestrates middleware and usecase execution.

#### Basic usage

```csharp
var pipeline = new Pipeline()
    .WithPreMiddlewares(new List<IPreMiddleware>
    {
        new ValidationMiddleware(),
        new AuthMiddleware()
    })
    .WithPostMiddlewares(new List<IPostMiddleware>
    {
        new LoggingMiddleware()
    });

await pipeline.ExecuteAsync(usecase, cancellationToken);
```

#### With MiddlewareResolver (recommended)

```csharp
middlewareResolver.ConfigurePipeline(pipeline, typeof(CreateBookUseCase));
await pipeline.ExecuteAsync(usecase, cancellationToken);
```

---

## Dependency injection

### Automatic registration

```csharp
builder.Services.AddCleanArchitectureCore(
    environment: "Development",
    configureOptions: options =>
    {
        options.Debug = true;
        options.Logger = Console.WriteLine;
    },
    assemblies: typeof(Program).Assembly)
);
```

### Attributes

#### UsecaseAttribute

```csharp
[Usecase(
    Lifetime = ServiceLifetime.Scoped,
    AsInterface = typeof(IGetUserUsecase),
    PreMiddlewares = new[] { typeof(ValidationMiddleware) },
    PostMiddlewares = new[] { typeof(LoggingMiddleware) }
)]
public sealed class GetUserUsecase : Usecase, IGetUserUsecase
{
}
```

#### PresenterAttribute

```csharp
[Presenter(Lifetime = ServiceLifetime.Scoped)]
public sealed class ApiPresenter : Presenter { }
```

#### MiddlewareAttribute

```csharp
[Middleware(
    IsGlobal = true,      // Global middleware
    Priority = 100,       // Higher priority executes first (must be between 0 and 100, inclusive)
    Lifetime = ServiceLifetime.Scoped
)]
public class LoggingMiddleware : IPreMiddleware, IPostMiddleware
{
}
```

**Priority Constraint**: The `Priority` property must be between **0** and **100** (inclusive). Values outside this range will throw an `ArgumentOutOfRangeException`.

#### ServiceAttribute

```csharp
[Service(
    Lifetime = ServiceLifetime.Scoped,
    AsInterface = typeof(IUserService)
)]
public sealed class UserService : IUserService { }
```

#### RepositoryAttribute

```csharp
[Repository(
    Lifetime = ServiceLifetime.Scoped,
    AsInterface = typeof(IUserRepository)
)]
public sealed class UserRepository : IUserRepository { }
```

### Environment-specific registration

```csharp
[WhenDev]
[Middleware(IsGlobal = true)]
public sealed class FakeAuthMiddleware : IPreMiddleware
{
}

[WhenProd]
[Middleware(IsGlobal = true)]
public sealed class JwtAuthMiddleware : IPreMiddleware
{
}

[WhenTest]
[WhenDev]
[Service]
public sealed class TestDevService : IService
{
}
```

---

## Middleware system

### Global middleware

Global middlewares execute for all usecases:

```csharp
[Middleware(IsGlobal = true, Priority = 100)]
public sealed class LoggingMiddleware : IPreMiddleware, IPostMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(
        IRequest? request,
        IPresenter? presenter,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Executing ...");

        await next(cancellationToken);

        var response = presenter?.GetResponse();
        _logger.LogInformation("Completed with status: {Status}",
            response?.GetStatusCode());
    }
}
```

### Usecase-specific middleware

```csharp
[Usecase(PreMiddlewares = new[] { typeof(CustomValidationMiddleware) })]
public sealed class CreateOrderUsecase : Usecase
{
}
```

### Pre-Middleware

Executes before the usecase:

```csharp
[Middleware(IsGlobal = true, Priority = 90)]
public sealed class AuthMiddleware : IPreMiddleware
{
    public async Task InvokeAsync(
        IRequest? request,
        IPresenter? presenter,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        var token = request?.GetField<string>("token");

        if (string.IsNullOrEmpty(token))
        {
            presenter?.Present(Response.Response.Create(
                success: false,
                statusCode: StatusCode.Unauthorized,
                message: "authentication.required",
                data: null
            ));
            return;
        }

        request?.ModifiedPayload(new Dictionary<string, object>
        {
            ["authenticatedUserId"] = "user-123"
        });

        await next(cancellationToken);
    }
}
```

### Post-Middleware

Executes after the usecase:

```csharp
[Middleware(IsGlobal = true)]
public sealed class ResponseEnrichmentMiddleware : IPostMiddleware
{
    public async Task InvokeAsync(
        IRequest? request,
        IPresenter? presenter,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        var response = presenter?.GetResponse();
        if (response != null)
        {
            response.UpdateMetaField("timestamp", DateTime.UtcNow);
            response.UpdateMetaField("server", Environment.MachineName);
        }

        await next(cancellationToken);
    }
}
```

### Middleware priority

Higher priority executes first. The `Priority` property must be between **0** and **100** (inclusive):

```csharp
[Middleware(IsGlobal = true, Priority = 100)]  // Executes 1st (max priority)
public class Middleware1 : IPreMiddleware { }

[Middleware(IsGlobal = true, Priority = 50)]   // Executes 2nd
public class Middleware2 : IPreMiddleware { }

[Middleware(IsGlobal = true, Priority = 10)]   // Executes 3rd
public class Middleware3 : IPreMiddleware { }

[Middleware(IsGlobal = true, Priority = 0)]    // Executes last (min priority)
public class Middleware4 : IPreMiddleware { }
```

**Note**: Values outside the 0-100 range will throw an `ArgumentOutOfRangeException` when the attribute is instantiated.

### Using MiddlewareResolver

The `IMiddlewareResolver` automatically combines global and usecase-specific middlewares:

```csharp
middlewareResolver.ConfigurePipeline(pipeline, typeof(CreateBookUseCase));
await pipeline.ExecuteAsync(usecase, cancellationToken);
```

---

## Pagination

### PaginationRequest

```csharp
using Osirisgate.CleanArchitectureCore.Pagination;

var pagination = PaginationRequest.FromDictionary(new Dictionary<string, object>
{
    ["pageNumber"] = 1,
    ["pageSize"] = 10
});

var pagination = new PaginationRequest(pageNumber: 1, pageSize: 10);

var skip = pagination.Skip;
var take = pagination.Take;
```

### PagedResult

```csharp
var items = new List<Book> { /* ... */ };
var totalCount = 100;

var pagedResult = PagedResult<Book>.Create(
    items: items,
    pageNumber: 1,
    pageSize: 10,
    totalCount: totalCount
);

// Properties
pagedResult.Items;           // IReadOnlyList<Book>
pagedResult.PageNumber;      // 1
pagedResult.PageSize;        // 10
pagedResult.TotalCount;      // 100
pagedResult.TotalPages;      // 10
pagedResult.HasPreviousPage; // false
pagedResult.HasNextPage;     // true

// Convert to dictionary
var dict = pagedResult.ToDictionary();
```

### Pagination extensions

```csharp
using Osirisgate.CleanArchitectureCore.Pagination;

var books = new List<Book> { /* ... */ };
var pagination = new PaginationRequest(pageNumber: 1, pageSize: 10);

var pagedResult = books.ToPagedResult(pagination);

var pagedResult = books.ToPagedResult(pagination, totalCount: 150);

var pagedResult = books.ToPagedResult(
    pagination,
    totalCountFunc: () => repository.GetTotalCount() // books.Count
);

var pagedResult = await books.ToPagedResultAsync(
    pagination,
    totalCountFunc: async (ct) => await repository.GetTotalCountAsync(ct),
    cancellationToken: cancellationToken
);

var items = new List<Book> { /* already paginated */ }.AsReadOnly();
var pagedResult = items.ToPagedResult(pagination, totalCount: 150);

var emptyResult = pagination.EmptyPagedResult<Book>();
```

### Response extensions

```csharp
using Osirisgate.CleanArchitectureCore.Pagination;

var books = new List<Book> { /* ... */ };
var pagination = new PaginationRequest(pageNumber: 1, pageSize: 10);
var response = Response.Response.Create(
    success: true,
    statusCode: StatusCode.Ok,
    message: "Books retrieved",
    data: null
);

response.SetPagedData(books, pagination);

response.SetPagedData(books, pagination, totalCount: 150);

response.SetPagedData(books, pagination, totalCountFunc: () => repository.GetTotalCount());

await response.SetPagedDataAsync(
    books,
    pagination,
    totalCountFunc: async (ct) => await repository.GetTotalCountAsync(ct),
    cancellationToken: cancellationToken
);

var pagedResult = books.ToPagedResult(pagination);
response.SetPagedResult(pagedResult);

var retrievedResult = response.GetPagedResult<Book>();
```

---

## Entity mapper

### Automatic mapping

```csharp
using Osirisgate.CleanArchitectureCore.Mapper;

// Domain to Persistence
var domainUser = new User { Id = Guid.NewGuid(), Email = "user@example.com" };
var persistenceUser = EntityMapper.ToPersistence<User, UserDto>(domainUser);

// Persistence to Domain
var domainUser = EntityMapper.ToDomain<UserDto, User>(persistenceUser);

// Collections
var domainUsers = new List<User> { /* ... */ };
var persistenceUsers = EntityMapper.ToPersistenceList<User, UserDto>(domainUsers);

var persistenceUsers = new List<UserDto> { /* ... */ };
var persistenceUsers = EntityMapper.ToDomainList<UserDto, User>(persistenceUsers);
```

### Custom mapping

```csharp
var persistenceUser = EntityMapper.MapToPersistence<User, UserDto>(
    domainUser,
    domain => new UserDto
    {
        Id = domain.Id,
        EmailAddress = domain.Email,
        CreatedAt = DateTime.UtcNow
    }
);

var persistenceUser = EntityMapper.MapToDomain<UserDto, User>(
    persistenceUser,
    domain => new User
    {
        Id = domain.Id,
        EmailAddress = domain.Email,
        CreatedAt = DateTime.UtcNow
    }
);
```

### Update existing entities

```csharp
// Update persistence entity
var existingDto = new UserDto { Id = userId, Email = "old@example.com" };
EntityMapper.UpdatePersistence(domainUser, existingDto);

// Update domain entity
var existingDomain = new User { Id = userId, Email = "old@example.com" };
EntityMapper.UpdateDomain(persistenceUser, existingDomain);
```

---

## Status codes

The SDK provides a comprehensive `StatusCode` enum with all standard HTTP status codes.

### Available status codes

```csharp
using Osirisgate.CleanArchitectureCore.Response;

// 1xx Informational
StatusCode.Continue                    // 100
StatusCode.SwitchingProtocols          // 101
StatusCode.Processing                  // 102
StatusCode.EarlyHints                  // 103

// 2xx Success
StatusCode.Ok                          // 200
StatusCode.Created                     // 201
StatusCode.Accepted                    // 202
StatusCode.NoContent                   // 204
StatusCode.PartialContent              // 206

// 3xx Redirection
StatusCode.MovedPermanently            // 301
StatusCode.Found                       // 302
StatusCode.NotModified                 // 304
StatusCode.TemporaryRedirect           // 307
StatusCode.PermanentlyRedirect         // 308

// 4xx Client errors
StatusCode.BadRequest                  // 400
StatusCode.Unauthorized                // 401
StatusCode.Forbidden                   // 403
StatusCode.NotFound                    // 404
StatusCode.MethodNotAllowed            // 405
StatusCode.Conflict                    // 409
StatusCode.UnprocessableEntity         // 422
StatusCode.TooManyRequests             // 429

// 5xx Server errors
StatusCode.InternalServerError         // 500
StatusCode.NotImplemented              // 501
StatusCode.BadGateway                  // 502
StatusCode.ServiceUnavailable          // 503
StatusCode.GatewayTimeout              // 504
```

### Usage examples

```csharp
// Create response with status code
var response = Response.Response.Create(
    success: true,
    statusCode: StatusCode.Created,
    message: "resource.created",
    data: new Dictionary<string, object> { ["id"] = resourceId }
);

// Get status code value
int code = StatusCode.Ok.GetValue();  // Returns 200

// Update status code
response.SetStatusCode(StatusCode.Accepted);
```

---

## Request extensions

The SDK provides extension methods for `IRequest` to simplify field access and validation.

### Available extensions

| Method | Description | Returns |
|--------|-------------|---------|
| `GetRequired<T>(string fieldPath)` | Get required field (throws if missing/null) | `T` |
| `TryGetField<T>(string fieldPath, out T? value)` | Try get field (returns bool) | `bool` |
| `GetOrThrow<T>(string fieldPath)` | Get field or throw exception | `T` |
| `GetRequiredModified<T>(string fieldPath)` | Get required modified field | `T` |
| `TryGetModifiedField<T>(string fieldPath, out T? value)` | Try get modified field | `bool` |

### Usage examples

```csharp
using Osirisgate.CleanArchitectureCore.Request;

// Get required field (throws InvalidOperationException if missing)
var email = request.GetRequired<string>("email");
var userId = request.GetRequired<Guid>("userId");

// Try get field (safe, returns bool)
if (request.TryGetField<string>("optionalField", out var value))
{
    // Field exists and has a value
    Console.WriteLine($"Optional field: {value}");
}

// Get or throw (similar to GetRequired but different exception message)
var name = request.GetOrThrow<string>("name");

// Get required from modified payload (added by middleware)
var authenticatedUserId = request.GetRequiredModified<string>("authenticatedUserId");

// Try get from modified payload
if (request.TryGetModifiedField<string>("role", out var role))
{
    Console.WriteLine($"User role: {role}");
}
```

### Example in usecase

```csharp
[Usecase]
public sealed class UpdateProfileUseCase : Usecase
{
    private readonly IUserRepository _userRepository;

    public UpdateProfileUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var request = GetRequest()!;

        // Get required fields (throws if missing)
        var userId = request.GetRequired<Guid>("userId");
        var email = request.GetRequired<string>("email");

        // Try get optional fields
        string? phoneNumber = null;
        if (request.TryGetField<string>("phoneNumber", out var phone))
        {
            phoneNumber = phone;
        }

        // Get authenticated user from middleware
        var authenticatedUserId = request.GetRequiredModified<Guid>("authenticatedUserId");

        // Verify user can only update their own profile
        if (userId != authenticatedUserId)
        {
            throw new UnauthorizedException("Cannot update another user's profile");
        }

        await _userRepository.UpdateAsync(userId, email, phoneNumber, cancellationToken);

        PresentResponse(Response.Response.Create(
            success: true,
            statusCode: StatusCode.Ok,
            message: "profile.updated",
            data: new Dictionary<string, object>
            {
                ["userId"] = userId,
                ["email"] = email
            }
        ));
    }
}
```

---

## Response extensions

The SDK provides extension methods for `IResponse` to simplify data access and manipulation.

### Available extensions

| Method | Description | Returns |
|--------|-------------|---------|
| `GetRequired<T>(string fieldPath)` | Get required field (throws if missing/null) | `T` |
| `TryGet<T>(string fieldPath, out T? value)` | Try get field (returns bool) | `bool` |
| `GetOrThrow<T>(string fieldPath)` | Get field or throw exception | `T` |
| `SetFieldIfNotExists(string fieldPath, object? value)` | Set field only if it doesn't exist | `IResponse` |
| `MergeData(IDictionary<string, object> additionalData)` | Merge additional data into response | `IResponse` |
| `HasField(string fieldPath)` | Check if field exists | `bool` |

### Usage examples

```csharp
using Osirisgate.CleanArchitectureCore.Response;

var response = Response.Response.Create(
    success: true,
    statusCode: StatusCode.Ok,
    message: "user.retrieved",
    data: new Dictionary<string, object>
    {
        ["id"] = userId,
        ["email"] = "user@example.com"
    }
);

// Get required field (throws if missing)
var userId = response.GetRequired<Guid>("id");

// Try get field (safe, returns bool)
if (response.TryGet<string>("email", out var email))
{
    Console.WriteLine($"Email: {email}");
}

// Check if field exists
if (response.HasField("phoneNumber"))
{
    var phone = response.Get<string>("phoneNumber");
}

// Set field only if it doesn't exist
response.SetFieldIfNotExists("createdAt", DateTime.UtcNow);

// Merge additional data
var additionalData = new Dictionary<string, object>
{
    ["lastLogin"] = DateTime.UtcNow,
    ["loginCount"] = 42
};
response.MergeData(additionalData);

// Get or throw
var email = response.GetOrThrow<string>("email");
```

### Example in post-middleware

```csharp
[Middleware(IsGlobal = true, Priority = 50)]
public sealed class EnrichmentPostMiddleware : IPostMiddleware
{
    public async Task InvokeAsync(
        IRequest? request,
        IPresenter? presenter,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        await next(cancellationToken);

        var response = presenter?.GetResponse();
        if (response != null && response.IsSuccess())
        {
            // Add timestamp if not already present
            response.SetFieldIfNotExists("timestamp", DateTime.UtcNow);

            // Add server info
            response.MergeData(new Dictionary<string, object>
            {
                ["server"] = Environment.MachineName,
                ["version"] = "1.0.0"
            });

            // Check and enrich user data
            if (response.HasField("userId"))
            {
                var userId = response.GetRequired<Guid>("userId");
                // Add additional user info...
            }
        }
    }
}
```

---

## Configuration options

### CleanArchitectureOptions

Configure the SDK behavior when registering services.

```csharp
using Osirisgate.CleanArchitectureCore.DependencyInjection;

builder.Services.AddCleanArchitectureCore(
    environment: builder.Environment.EnvironmentName,
    configureOptions: options =>
    {
        // Enable debug logging
        options.Debug = true;

        // Custom logger (default: Console.WriteLine)
        options.Logger = message => _logger.LogInformation(message);
    },
    assemblies: typeof(Program).Assembly
);
```

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Debug` | `bool` | `false` | Enable detailed registration logging |
| `Logger` | `Action<string>?` | `Console.WriteLine` | Custom logging action |

#### Example with custom logger

```csharp
builder.Services.AddCleanArchitectureCore(
    environment: "Production",
    configureOptions: options =>
    {
        options.Debug = builder.Environment.IsDevelopment();
        options.Logger = message =>
        {
            if (options.Debug)
            {
                Console.WriteLine($"[CleanArchitecture] {message}");
            }
        };
    },
    typeof(Program).Assembly,
    typeof(SharedLibrary).Assembly  // Multiple assemblies
);
```

---

## Attributes

The SDK uses attributes for automatic dependency injection registration.

### Available attributes

| Attribute | Target | Description |
|-----------|--------|-------------|
| `[Usecase]` | Class | Register as usecase (Scoped) |
| `[Presenter]` | Class | Register as presenter (Scoped) |
| `[Service]` | Class | Register as service (Scoped by default) |
| `[Repository]` | Class | Register as repository (Scoped by default) |
| `[Middleware]` | Class | Register as middleware (Transient) |
| `[Pipeline]` | Class | Register as pipeline (Scoped) |
| `[WhenDev]` | Class | Register only in Development |
| `[WhenTest]` | Class | Register only in Test |
| `[WhenProd]` | Class | Register only in Production |

### Attribute properties

#### Service and Repository attributes

```csharp
[Service(
    Lifetime = ServiceLifetime.Singleton,  // Scoped, Transient, or Singleton
    AsInterface = typeof(IMyService)       // Register as interface
)]
public sealed class MyService : IMyService { }

[Repository(
    Lifetime = ServiceLifetime.Scoped,
    AsInterface = typeof(IUserRepository)
)]
public sealed class UserRepository : IUserRepository { }
```

#### Middleware attribute

```csharp
[Middleware(
    IsGlobal = true,      // Apply to all usecases
    Priority = 100        // Higher priority executes first (must be between 0 and 100, inclusive)
)]
public sealed class LoggingMiddleware : IPreMiddleware { }
```

**Priority Constraint**: The `Priority` property must be between **0** and **100** (inclusive). Values outside this range will throw an `ArgumentOutOfRangeException`.

### Usage examples

```csharp
// Usecase (always Scoped)
[Usecase]
public sealed class CreateUserUseCase : Usecase { }

// Presenter (always Scoped)
[Presenter]
public sealed class ApiPresenter : Presenter { }

// Service with custom lifetime
[Service(Lifetime = ServiceLifetime.Singleton)]
public sealed class CacheService : ICacheService { }

// Service registered as interface
[Service(AsInterface = typeof(IEmailService))]
public sealed class EmailService : IEmailService { }

// Repository
[Repository(AsInterface = typeof(IUserRepository))]
public sealed class UserRepository : IUserRepository { }

// Global middleware with priority
[Middleware(IsGlobal = true, Priority = 100)]
public sealed class AuthMiddleware : IPreMiddleware { }

// Usecase-specific middleware
[Middleware(IsGlobal = false, Priority = 50)]
public sealed class CacheMiddleware : IPreMiddleware { }

// Pipeline (custom implementation)
[Pipeline]
public sealed class CustomPipeline : IPipeline { }
```

### Environment-specific registration

```csharp
// Only in Development
[WhenDev]
[Service(AsInterface = typeof(IPaymentService))]
public sealed class FakePaymentService : IPaymentService { }

// Only in Production
[WhenProd]
[Service(AsInterface = typeof(IPaymentService))]
public sealed class StripePaymentService : IPaymentService { }

// Only in Test
[WhenTest]
[Service(AsInterface = typeof(IEmailService))]
public sealed class MockEmailService : IEmailService { }

// Multiple environments
[WhenDev]
[WhenTest]
[Middleware(IsGlobal = true, Priority = 90)]
public sealed class DebugMiddleware : IPreMiddleware { }
```

### Complete example

```csharp
// Domain layer
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User> CreateAsync(User user, CancellationToken cancellationToken);
}

// Infrastructure layer
[Repository(AsInterface = typeof(IUserRepository))]
public sealed class UserRepository : IUserRepository
{
    private readonly DbContext _context;

    public UserRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Users.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<User> SaveAsync(User user, CancellationToken cancellationToken)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }
}

// Application layer
[Usecase]
public sealed class CreateUserUseCase : Usecase
{
    private readonly IUserRepository _userRepository;

    public CreateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var request = GetRequest()!;
        var email = request.GetRequired<string>("email");
        var name = request.GetRequired<string>("name");

        var user = new User { Email = email, Name = name };
        var createdUser = await _userRepository.CreateAsync(user, cancellationToken);

        PresentResponse(Response.Response.Create(
            success: true,
            statusCode: StatusCode.Created,
            message: "user.created",
            data: new Dictionary<string, object>
            {
                ["id"] = createdUser.Id,
                ["email"] = createdUser.Email
            }
        ));
    }
}

// Presentation layer
[Presenter]
public sealed class ApiPresenter : Presenter { }

// Middleware
[Middleware(IsGlobal = true, Priority = 100)]
public sealed class LoggingMiddleware : IPreMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(
        IRequest? request,
        IPresenter? presenter,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Request started");
        await next(cancellationToken);
        _logger.LogInformation("Request completed");
    }
}
```

---

## Exception handling

### Built-in Exceptions

- `ArgumentException`: Invalid argument
- `ArgumentNullException`: Null argument
- `ArgumentOutOfRangeException`: Argument out of range
- `BadRequestContentException`: Bad request (400)
- `InvalidCastException`: Invalid type cast
- `InvalidOperationException`: Invalid operation
- `ObjectDisposedException`: Object disposed
- `RuntimeException`: Runtime error

### Exception format

```json
{
  "status": "error",
  "error_code": 400,
  "message": "validation.failed",
  "details": {
    "field": "email",
    "error": "invalid.format"
  }
}
```

### Custom exceptions

```csharp
public sealed class BookNotFoundException : BaseException
{
    protected override StatusCode ErrorCode { get; } = StatusCode.NotFound;

    public BookNotFoundException(string bookId)
        : base(new Dictionary<string, object>
        {
            ["message"] = "book.not.found",
            ["details"] = new Dictionary<string, object>
            {
                ["bookId"] = bookId,
                ["reason"] = "The requested book does not exist"
            }
        })
    { }
}

// Usage
if (book == null)
{
    throw new BookNotFoundException(bookId);
}
```

---

## Complete examples

### Example 1: Complete CRUD Endpoint

This example demonstrates a complete endpoint implementation. The `CreateBookRequest` is defined in the [Quick start](#quick-start) section above.

```csharp
// Usecase
[Usecase]
public sealed class CreateBookUseCase : Usecase
{
    private readonly IBookRepository _bookRepository;

    public CreateBookUseCase(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var title = GetField<string>("title")!;
        var author = GetField<string>("author")!;
        var totalCopies = GetField<int>("totalCopies");
        var isbn = GetField<string>("isbn");

        // Check if book exists
        var existingBook = await _bookRepository.GetByIsbnAsync(isbn, cancellationToken);
        if (existingBook != null)
        {
            throw new BadRequestContentException(new Dictionary<string, object>
            {
                ["message"] = "book.already.exists",
                ["details"] = new Dictionary<string, object>
                {
                    ["isbn"] = isbn
                }
            });
        }

        var book = Book.Create(title, author, totalCopies, isbn);
        await _bookRepository.SaveAsync(book, cancellationToken);

        PresentResponse(Response.Response.Create(
            success: true,
            statusCode: StatusCode.Created,
            message: "book.created.successfully",
            data: book.ToDictionary()
        ));
    }
}

// Presenter
[Presenter]
public sealed class ApiPresenter : Presenter { }

// Endpoint
app.MapPost("/api/books", async (
    [FromBody] CreateBookRequestModel requestModel,
    [FromServices] CreateBookUseCase useCase,
    [FromServices] ApiPresenter presenter,
    [FromServices] IPipeline pipeline,
    [FromServices] IMiddlewareResolver middlewareResolver,
    CancellationToken cancellationToken) =>
{
    var payload = new Dictionary<string, object>
    {
        ["title"] = requestModel.Title,
        ["author"] = requestModel.Author,
        ["totalCopies"] = requestModel.TotalCopies
    };

    if (!string.IsNullOrWhiteSpace(requestModel.Isbn))
        payload["isbn"] = requestModel.Isbn;

    var request = new CreateBookRequest(payload);

    useCase.WithRequest(request).WithPresenter(presenter);
    middlewareResolver.ConfigurePipeline(pipeline, typeof(CreateBookUseCase));
    await pipeline.ExecuteAsync(useCase, cancellationToken);

    var response = presenter.GetFormattedResponse();
    var statusCode = (int)response["code"];

    if (statusCode == 201 && response.TryGetValue("data", out var data)
        && data is Dictionary<string, object> dataDict
        && dataDict.TryGetValue("id", out var id))
    {
        return Results.Created($"/api/books/{id}", response);
    }

    return Results.Json(response, statusCode: statusCode);
});
```

### Example 2: Middleware Pipeline

See the [Middleware system](#middleware-system) section above for detailed middleware implementation examples. This example shows how to combine global and usecase-specific middlewares:

```csharp
// Usecase with specific middleware
[Usecase(PreMiddlewares = new[] { typeof(CustomAuthMiddleware) })]
public sealed class SecureUsecase : Usecase
{
    // Execution order:
    // 1. LoggingMiddleware (global, priority 100) - see Middleware system section
    // 2. ValidationMiddleware (global, priority 90) - see Middleware system section
    // 3. CustomAuthMiddleware (usecase-specific)
    // 4. SecureUsecase.ExecuteAsync()
    // 5. LoggingMiddleware (post-execution)
}
```

---

## Best practices

### 1. Request validation

```csharp
// ✅ Good: Clear structure with validation
public sealed class CreateUserRequest(IReadOnlyDictionary<string, object> payload) : Request
{
    protected override IDictionary<string, object> RequestStructure => new Dictionary<string, object>
    {
        ["email"] = true,
        ["password"] = true
    };

    protected override async Task ApplyConstraintsOnRequestFieldsAsync(CancellationToken cancellationToken = default)
    {
        var email = GetField<string>("email");
        if (!email.Contains("@"))
        {
            throw new BadRequestContentException(new Dictionary<string, object>
            {
                ["message"] = "validation.failed",
                ["details"] = new Dictionary<string, object> { ["email"] = "Invalid format" }
            });
        }

        await Task.CompletedTask;
    }
}
```

### 2. Usecase design

```csharp
// ✅ Good: Single responsibility, clear dependencies
[Usecase]
public sealed class CreateOrderUsecase : Usecase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryService _inventoryService;

    public CreateOrderUsecase(
        IOrderRepository orderRepository,
        IInventoryService inventoryService)
    {
        _orderRepository = orderRepository;
        _inventoryService = inventoryService;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Clear business logic
        var customerId = GetField<string>("customerId");
        var items = GetField<List<OrderItem>>("items");

        if (!await _inventoryService.IsAvailableAsync(items, cancellationToken))
        {
            throw new InsufficientInventoryException();
        }

        var order = Order.Create(customerId, items);
        await _orderRepository.CreateAsync(order, cancellationToken);

        PresentResponse(Response.Response.Create(
            success: true,
            statusCode: StatusCode.Created,
            message: "order.created",
            data: order.ToDictionary()
        ));
    }
}
```

### 3. Middleware design

```csharp
// ✅ Good: Focused, reusable
[Middleware(IsGlobal = true, Priority = 80)]
public sealed class AuthMiddleware : IPreMiddleware
{
    private readonly IAuthService _authService;

    public AuthMiddleware(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task InvokeAsync(
        IRequest? request,
        IPresenter? presenter,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        var token = request?.GetField<string>("token");

        if (string.IsNullOrEmpty(token))
        {
            presenter?.Present(Response.Response.Create(
                success: false,
                statusCode: StatusCode.Unauthorized,
                message: "authentication.required",
                data: null
            ));
            return;
        }

        var user = await _authService.ValidateAsync(token, cancellationToken);
        if (user == null)
        {
            presenter?.Present(Response.Response.Create(
                success: false,
                statusCode: StatusCode.Unauthorized,
                message: "invalid.token",
                data: null
            ));
            return;
        }

        request?.ModifiedPayload(new Dictionary<string, object>
        {
            ["authenticatedUserId"] = user.Id,
            ["authenticatedUserRole"] = user.Role
        });

        await next(cancellationToken);
    }
}
```

### 4. Error handling

```csharp
// ✅ Good: Specific, informative
public sealed class BookNotFoundException : BaseException
{
    protected override StatusCode ErrorCode { get; } = StatusCode.NotFound;

    public BookNotFoundException(string bookId)
        : base(new Dictionary<string, object>
        {
            ["message"] = "book.not.found",
            ["details"] = new Dictionary<string, object>
            {
                ["bookId"] = bookId,
                ["reason"] = "The requested book does not exist",
                ["suggestion"] = "Please verify the book ID and try again"
            }
        })
    { }
}
```

---

## API reference

### Usecase class

The `Usecase` abstract class provides core functionality for implementing business logic.

#### Public methods

| Method | Description | Returns |
|--------|-------------|---------|
| `abstract Task ExecuteAsync(CancellationToken)` | **[Abstract]** Execute business logic (must be implemented) | `Task` |
| `IUsecase WithRequest(IRequest request)` | Attach a request to the usecase | `IUsecase` |
| `IUsecase WithPresenter(IPresenter presenter)` | Attach a presenter to the usecase | `IUsecase` |
| `IRequest? GetRequest()` | Get the attached request | `IRequest?` |
| `IPresenter? GetPresenter()` | Get the attached presenter | `IPresenter?` |

#### Protected methods (available in derived classes)

| Method | Description | Returns |
|--------|-------------|---------|
| `void PresentResponse(IResponse response)` | Pass response to presenter | `void` |
| `T? GetField<T>(string fieldPath, object? defaultValue)` | Get field from request payload (supports dot notation) | `T?` |
| `T? GetModifiedField<T>(string fieldPath, object? defaultValue)` | Get field from modified payload | `T?` |
| `IReadOnlyDictionary<string, object> GetPayload()` | Get entire original payload | `IReadOnlyDictionary<string, object>` |
| `IDictionary<string, object> GetModifiedPayload()` | Get entire modified payload | `IDictionary<string, object>` |

#### Example: Using protected methods

```csharp
[Usecase]
public sealed class UpdateUserProfileUseCase : Usecase
{
    private readonly IUserRepository _userRepository;

    public UpdateUserProfileUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Using GetField<T> to access request data
        var userId = GetField<string>("userId")!;
        var email = GetField<string>("email");
        var firstName = GetField<string>("firstName");

        // Access nested fields with dot notation
        var city = GetField<string>("address.city");
        var zipCode = GetField<string>("address.zipCode");

        // Check if middleware modified the payload
        var modifiedPayload = GetModifiedPayload();
        if (modifiedPayload.Count > 0)
        {
            // Middleware added authentication info
            var authenticatedUserId = GetModifiedField<string>("authenticatedUserId");

            // Verify user can only update their own profile
            if (userId != authenticatedUserId)
            {
                throw new UnauthorizedException("Cannot update another user's profile");
            }
        }

        // Business logic
        var user = _userRepository.GetByIdAsync(userId, cancellationToken)
        user.Update(userId, email, firstName, city, zipCode);
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Using PresentResponse to send result
        PresentResponse(Response.Response.Create(
            success: true,
            statusCode: StatusCode.Ok,
            message: "user.profile.updated",
            data: new Dictionary<string, object>
            {
                ["userId"] = user.Id,
                ["email"] = user.Email,
                ["firstName"] = user.FirstName,
                ["updatedAt"] = user.UpdatedAt
            }
        ));
    }
}
```

---

### Request class

The `Request` abstract class validates and manages input data.

#### Public methods

| Method | Description | Returns |
|--------|-------------|---------|
| `IReadOnlyDictionary<string, object> GetPayload()` | Get original payload | `IReadOnlyDictionary<string, object>` |
| `T? GetField<T>(string fieldPath, object? defaultValue)` | Get field (supports dot notation, case-insensitive) | `T?` |
| `IDictionary<string, object> GetModifiedPayload()` | Get modified payload | `IDictionary<string, object>` |
| `T? GetModifiedField<T>(string fieldPath, object? defaultValue)` | Get modified field | `T?` |
| `IRequest ModifiedPayload(IDictionary<string, object>)` | Set/merge modified payload | `IRequest` |
| `virtual Task ValidateAsync(CancellationToken)` | Execute validation (calls `ApplyConstraintsOnRequestFieldsAsync`) | `Task` |

#### Protected properties (override in derived classes)

| Property | Description | Type |
|----------|-------------|------|
| `virtual IDictionary<string, object> RequestStructure` | Define required/optional fields structure | `IDictionary<string, object>` |

#### Protected methods (available in derived classes)

| Method | Description | Returns |
|--------|-------------|---------|
| `IDictionary<string, object> ToDto()` | Get normalized dictionary (PascalCase keys) | `IDictionary<string, object>` |
| `TDto ToDto<TDto>()` | Map payload to strongly-typed DTO | `TDto` |
| `IDictionary<string, object> ToModifiedDto()` | Get normalized modified payload | `IDictionary<string, object>` |
| `virtual Task ApplyConstraintsOnRequestFieldsAsync(CancellationToken)` | **[Override]** Custom validation logic | `Task` |

#### Example: Using protected methods

See the complete `UserRegistrationRequest` example in the [FluentValidation integration](#fluentvalidation-integration) section above. Here's a brief summary of the protected methods available:

```csharp
// Method 1: Use ToDto() to get normalized dictionary
var dto = ToDto();
// dto contains: { "Email": "...", "Password": "...", "Age": ..., "FirstName": "...", "LastName": "..." }

// Method 2: Use ToDto<TDto>() for strongly-typed validation
var userDto = ToDto<UserRegistrationDto>();

// Method 3: Access fields directly
var age = GetField<int>("age");

// Method 4: Get modified payload
var modifiedDto = ToModifiedDto();
```

---

### Response class

The `Response` sealed class represents standardized API responses.

#### Public static methods

| Method | Description | Returns |
|--------|-------------|---------|
| `static IResponse Create(bool success, StatusCode statusCode, string message, IDictionary<string, object>? data)` | Create a new response | `IResponse` |

#### Public instance methods

| Method | Description | Returns |
|--------|-------------|---------|
| `bool IsSuccess()` | Check if response is successful | `bool` |
| `int GetStatusCode()` | Get HTTP status code | `int` |
| `string GetMessage()` | Get response message | `string` |
| `IDictionary<string, object> GetData()` | Get response data | `IDictionary<string, object>` |
| `T? Get<T>(string fieldPath, object? defaultValue)` | Get data field (supports dot notation) | `T?` |
| `void UpdateField(string fieldPath, object? value)` | Update data field | `void` |
| `void SetSuccess(bool success)` | Set success status | `void` |
| `void SetMessage(string message)` | Set message | `void` |
| `void SetStatusCode(StatusCode statusCode)` | Set status code | `void` |
| `void ReplaceData(IDictionary<string, object> data)` | Replace entire data | `void` |
| `IDictionary<string, object> GetOutput()` | Get formatted output for API | `IDictionary<string, object>` |
| `IDictionary<string, object> GetMeta()` | Get metadata | `IDictionary<string, object>` |
| `T? GetMeta<T>(string fieldPath, object? defaultValue)` | Get metadata field | `T?` |
| `void SetMeta(IDictionary<string, object> meta)` | Set metadata | `void` |
| `void UpdateMetaField(string fieldPath, object? value)` | Update metadata field | `void` |

#### Example: Complete response usage

```csharp
// Create success response
var response = Response.Response.Create(
    success: true,
    statusCode: StatusCode.Ok,
    message: "user.retrieved.successfully",
    data: new Dictionary<string, object>
    {
        ["id"] = userId,
        ["email"] = "user@example.com",
        ["profile"] = new Dictionary<string, object>
        {
            ["firstName"] = "John",
            ["lastName"] = "Doe"
        }
    }
);

// Read data fields
var email = response.Get<string>("email");
var firstName = response.Get<string>("profile.firstName"); // Dot notation

// Update data fields
response.UpdateField("email", "newemail@example.com");
response.UpdateField("profile.lastName", "Smith");

// Add metadata
response.UpdateMetaField("requestId", Guid.NewGuid().ToString());
response.UpdateMetaField("timestamp", DateTime.UtcNow);
response.UpdateMetaField("version", "1.0.0");

// Get formatted output (ready for API)
var output = response.GetOutput();
/*
{
    "status": "success",
    "code": 200,
    "message": "user.retrieved.successfully",
    "data": {
        "id": "...",
        "email": "newemail@example.com",
        "profile": {
            "firstName": "John",
            "lastName": "Smith"
        }
    },
    "meta": {
        "requestId": "...",
        "timestamp": "...",
        "version": "1.0.0"
    }
}
*/

// Create error response
var errorResponse = Response.Response.Create(
    success: false,
    statusCode: StatusCode.BadRequest,
    message: "validation.failed",
    data: new Dictionary<string, object>
    {
        ["email"] = "Invalid email format",
        ["password"] = "Password too weak"
    }
);

var errorOutput = errorResponse.GetOutput();
/*
{
    "status": "error",
    "code": 400,
    "message": "validation.failed",
    "details": {  // Note: "details" instead of "data" for errors
        "email": "Invalid email format",
        "password": "Password too weak"
    },
    "meta": {
        "requestId": "...",
        "timestamp": "...",
        "version": "1.0.0"
    }
}
*/
```

---

### Presenter class

The `Presenter` abstract class handles response formatting.

#### Public methods

| Method | Description | Returns |
|--------|-------------|---------|
| `void Present(IResponse response)` | Set the response | `void` |
| `IResponse? GetResponse()` | Get the response | `IResponse?` |
| `IDictionary<string, object> GetFormattedResponse()` | Get formatted output (calls `response.GetOutput()`) | `IDictionary<string, object>` |

#### Example: Custom presenter

```csharp
[Presenter]
public sealed class ApiPresenter : Presenter
{
    // Default implementation is sufficient for most cases
    // GetFormattedResponse() returns the standardized format
}

// Usage in endpoint
var presenter = new ApiPresenter();
useCase.WithPresenter(presenter);
await pipeline.ExecuteAsync(useCase, cancellationToken);

var response = presenter.GetFormattedResponse();
return Results.Json(response, statusCode: (int)response["code"]);
```

---

### Pipeline class

The `Pipeline` sealed class orchestrates middleware and usecase execution.

#### Public methods

| Method | Description | Returns |
|--------|-------------|---------|
| `Task ExecuteAsync(IUsecase useCase, CancellationToken)` | Execute pipeline with middlewares and usecase | `Task` |
| `IPipeline WithPreMiddlewares(IReadOnlyList<IPreMiddleware>? preMiddlewares)` | Set pre-execution middlewares | `IPipeline` |
| `IPipeline WithPostMiddlewares(IReadOnlyList<IPostMiddleware>? postMiddlewares)` | Set post-execution middlewares | `IPipeline` |

#### Example: Manual pipeline configuration

```csharp
// Create pipeline
var pipeline = new Pipeline();

// Configure with middlewares
var preMiddlewares = new List<IPreMiddleware>
{
    new LoggingMiddleware(logger),
    new AuthMiddleware(authService),
    new ValidationMiddleware()
};

var postMiddlewares = new List<IPostMiddleware>
{
    new AuditMiddleware(auditService),
    new CacheMiddleware(cache)
};

pipeline
    .WithPreMiddlewares(preMiddlewares)
    .WithPostMiddlewares(postMiddlewares);

// Execute
await pipeline.ExecuteAsync(useCase, cancellationToken);
```

---

### Pagination classes

#### PaginationRequest

| Method/Property | Description | Type/Returns |
|-----------------|-------------|--------------|
| `PaginationRequest(int pageNumber, int pageSize)` | Constructor | - |
| `int PageNumber { get; }` | Current page number (starts at 1) | `int` |
| `int PageSize { get; }` | Items per page | `int` |
| `int Skip { get; }` | Calculated skip count | `int` |
| `int Take { get; }` | Calculated take count | `int` |
| `static PaginationRequest FromDictionary(IReadOnlyDictionary<string, object>)` | Create from dictionary (supports "pageNumber"/"page", "pageSize"/"limit") | `PaginationRequest` |
| `const int DefaultPageSize` | Default page size (10) | `int` |
| `const int MinPageNumber` | Minimum page number (1) | `int` |

#### PagedResult<T>

| Method/Property | Description | Type/Returns |
|-----------------|-------------|--------------|
| `static PagedResult<T> Create(IReadOnlyList<T> items, int pageNumber, int pageSize, int totalCount)` | Create paginated result | `PagedResult<T>` |
| `static PagedResult<T> Empty(int pageNumber, int pageSize)` | Create empty result | `PagedResult<T>` |
| `IReadOnlyList<T> Items { get; }` | Items in current page | `IReadOnlyList<T>` |
| `int PageNumber { get; }` | Current page number | `int` |
| `int PageSize { get; }` | Page size | `int` |
| `int TotalCount { get; }` | Total items count | `int` |
| `int TotalPages { get; }` | Total pages count | `int` |
| `bool HasPreviousPage { get; }` | Has previous page | `bool` |
| `bool HasNextPage { get; }` | Has next page | `bool` |
| `Dictionary<string, object> ToDictionary()` | Convert to dictionary | `Dictionary<string, object>` |

---

### EntityMapper class

The `EntityMapper` static class provides entity mapping utilities.

#### Public static methods

| Method | Description | Returns |
|--------|-------------|---------|
| `ToPersistence<TDomain, TPersistence>(TDomain domainEntity)` | Map domain to persistence (automatic) | `TPersistence` |
| `ToDomain<TPersistence, TDomain>(TPersistence persistenceEntity)` | Map persistence to domain (automatic) | `TDomain` |
| `ToPersistenceList<TDomain, TPersistence>(IEnumerable<TDomain>)` | Map domain list to persistence list | `List<TPersistence>` |
| `ToDomainList<TPersistence, TDomain>(IEnumerable<TPersistence>)` | Map persistence list to domain list | `List<TDomain>` |
| `MapToPersistence<TDomain, TPersistence>(TDomain, Func<TDomain, TPersistence>)` | Map domain to persistence (custom) | `TPersistence` |
| `MapToDomain<TPersistence, TDomain>(TPersistence, Func<TPersistence, TDomain>)` | Map persistence to domain (custom) | `TDomain` |
| `MapListToPersistence<TDomain, TPersistence>(IEnumerable<TDomain>, Func<TDomain, TPersistence>)` | Map domain list (custom) | `List<TPersistence>` |
| `MapListToDomain<TPersistence, TDomain>(IEnumerable<TPersistence>, Func<TPersistence, TDomain>)` | Map persistence list (custom) | `List<TDomain>` |
| `UpdatePersistence<TDomain, TPersistence>(TDomain, TPersistence)` | Update existing persistence entity | `void` |
| `UpdateDomain<TPersistence, TDomain>(TPersistence, TDomain)` | Update existing domain entity | `void` |

#### Example: Complete mapping scenarios

```csharp
// Domain entity
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// Persistence entity (DTO)
public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// Scenario 1: Automatic mapping (works with public properties + public setters)
var domainUser = new User
{
    Id = Guid.NewGuid(),
    Email = "user@example.com",
    FirstName = "John",
    LastName = "Doe",
    CreatedAt = DateTime.UtcNow
};

var persistenceUser = EntityMapper.ToPersistence<User, UserDto>(domainUser);
var backToDomain = EntityMapper.ToDomain<UserDto, User>(persistenceUser);

// Scenario 2: Mapping collections
var domainUsers = new List<User> { domainUser };
var persistenceUsers = EntityMapper.ToPersistenceList<User, UserDto>(domainUsers);
var domainUsersBack = EntityMapper.ToDomainList<UserDto, User>(persistenceUsers);

// Scenario 3: Custom mapping (for complex scenarios, private setters, factory methods)
var customPersistence = EntityMapper.MapToPersistence<User, UserDto>(
    domainUser,
    domain => new UserDto
    {
        Id = domain.Id,
        Email = domain.Email.ToLowerInvariant(), // Custom transformation
        FirstName = domain.FirstName,
        LastName = domain.LastName,
        CreatedAt = domain.CreatedAt
    }
);

// Scenario 4: Update existing entity (in-place update)
var existingDto = new UserDto { Id = domainUser.Id, Email = "old@example.com" };
EntityMapper.UpdatePersistence(domainUser, existingDto);
// existingDto now has updated values from domainUser

// Scenario 5: Custom mapping for collections
var customList = EntityMapper.MapListToPersistence<User, UserDto>(
    domainUsers,
    domain => new UserDto
    {
        Id = domain.Id,
        Email = domain.Email.ToLowerInvariant(),
        FirstName = domain.FirstName,
        LastName = domain.LastName,
        CreatedAt = domain.CreatedAt
    }
);
```

---

### Middleware interfaces

#### IPreMiddleware & IPostMiddleware

Both interfaces extend `IMiddlewareBase` and define middleware that runs before or after usecase execution.

| Method | Description | Returns |
|--------|-------------|---------|
| `Task InvokeAsync(IRequest? request, IPresenter? presenter, Func<CancellationToken, Task> next, CancellationToken cancellationToken)` | Execute middleware logic | `Task` |

#### Example: Custom middleware implementation

```csharp
// Pre-Middleware: Runs BEFORE usecase execution
[Middleware(IsGlobal = true, Priority = 100)]
public class LoggingPreMiddleware : IPreMiddleware
{
    private readonly ILogger<LoggingPreMiddleware> _logger;

    public LoggingPreMiddleware(ILogger<LoggingPreMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(
        IRequest? request,
        IPresenter? presenter,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting request processing");

        // Access request data
        if (request != null)
        {
            var payload = request.GetPayload();
            _logger.LogInformation("Request payload: {Payload}", payload);
        }

        // Continue to next middleware or usecase
        await next(cancellationToken);
    }
}

// Post-Middleware: Runs AFTER usecase execution
[Middleware(IsGlobal = true, Priority = 100)]
public class AuditPostMiddleware : IPostMiddleware
{
    private readonly IAuditService _auditService;

    public AuditPostMiddleware(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public async Task InvokeAsync(
        IRequest? request,
        IPresenter? presenter,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        // Continue to next middleware (or complete if last)
        await next(cancellationToken);

        // Access response after usecase execution
        if (presenter != null)
        {
            var response = presenter.GetResponse();
            if (response != null)
            {
                await _auditService.LogAsync(
                    action: "UseCase Executed",
                    success: response.IsSuccess(),
                    statusCode: response.GetStatusCode(),
                    cancellationToken: cancellationToken
                );
            }
        }
    }
}

// Middleware that short-circuits execution
[Middleware(IsGlobal = false, Priority = 90)]
public class AuthenticationMiddleware : IPreMiddleware
{
    private readonly IAuthService _authService;

    public AuthenticationMiddleware(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task InvokeAsync(
        IRequest? request,
        IPresenter? presenter,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        var token = request?.GetField<string>("token");

        if (string.IsNullOrEmpty(token))
        {
            // Short-circuit: Don't call next(), return error immediately
            presenter?.Present(Response.Response.Create(
                success: false,
                statusCode: StatusCode.Unauthorized,
                message: "authentication.required",
                data: null
            ));
            return; // Stop pipeline execution
        }

        var user = await _authService.ValidateTokenAsync(token, cancellationToken);
        if (user == null)
        {
            presenter?.Present(Response.Response.Create(
                success: false,
                statusCode: StatusCode.Unauthorized,
                message: "invalid.token",
                data: null
            ));
            return; // Stop pipeline execution
        }

        // Add authenticated user to modified payload
        request?.ModifiedPayload(new Dictionary<string, object>
        {
            ["authenticatedUserId"] = user.Id,
            ["authenticatedUserRole"] = user.Role
        });

        // Continue to next middleware or usecase
        await next(cancellationToken);
    }
}

// Usecase-specific middleware (not global)
[Middleware(IsGlobal = false, Priority = 50)]
public class CacheMiddleware : IPreMiddleware
{
    private readonly IMemoryCache _cache;

    public CacheMiddleware(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task InvokeAsync(
        IRequest? request,
        IPresenter? presenter,
        Func<CancellationToken, Task> next,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = request?.GetField<string>("cacheKey");

        if (!string.IsNullOrEmpty(cacheKey) && _cache.TryGetValue(cacheKey, out IResponse? cachedResponse))
        {
            // Return cached response, skip usecase execution
            presenter?.Present(cachedResponse!);
            return;
        }

        // Continue to usecase
        await next(cancellationToken);

        // Cache the response
        if (presenter != null && !string.IsNullOrEmpty(cacheKey))
        {
            var response = presenter.GetResponse();
            if (response != null && response.IsSuccess())
            {
                _cache.Set(cacheKey, response, TimeSpan.FromMinutes(5));
            }
        }
    }
}
```

---

### MiddlewareResolver class

The `MiddlewareResolver` resolves and combines global and usecase-specific middlewares.

#### Public methods

| Method | Description | Returns |
|--------|-------------|---------|
| `IReadOnlyList<IPreMiddleware> GetPreMiddlewares(Type usecaseType)` | Get pre-middlewares for a usecase (global + specific) | `IReadOnlyList<IPreMiddleware>` |
| `IReadOnlyList<IPostMiddleware> GetPostMiddlewares(Type usecaseType)` | Get post-middlewares for a usecase (global + specific) | `IReadOnlyList<IPostMiddleware>` |
| `IPipeline ConfigurePipeline(IPipeline pipeline, Type usecaseType)` | Configure pipeline with middlewares for a usecase | `IPipeline` |

#### Example: Using MiddlewareResolver

```csharp
// In your endpoint
app.MapPost("/api/books", async (
    [FromBody] CreateBookRequestModel requestModel,
    [FromServices] CreateBookUseCase useCase,
    [FromServices] ApiPresenter presenter,
    [FromServices] IPipeline pipeline,
    [FromServices] IMiddlewareResolver middlewareResolver, // Inject resolver
    CancellationToken cancellationToken) =>
{
    var payload = new Dictionary<string, object>
    {
        ["title"] = requestModel.Title,
        ["author"] = requestModel.Author,
        ["totalCopies"] = requestModel.TotalCopies
    };

    var request = new CreateBookRequest(payload);
    useCase.WithRequest(request).WithPresenter(presenter);

    // Automatically configure pipeline with global + usecase-specific middlewares
    middlewareResolver.ConfigurePipeline(pipeline, typeof(CreateBookUseCase));

    // Execute with all configured middlewares
    await pipeline.ExecuteAsync(useCase, cancellationToken);

    var response = presenter.GetFormattedResponse();
    return Results.Json(response, statusCode: (int)response["code"]);
});

// Manual middleware resolution (if needed)
var preMiddlewares = middlewareResolver.GetPreMiddlewares(typeof(CreateBookUseCase));
var postMiddlewares = middlewareResolver.GetPostMiddlewares(typeof(CreateBookUseCase));

pipeline
    .WithPreMiddlewares(preMiddlewares)
    .WithPostMiddlewares(postMiddlewares);
```

#### Middleware priority and execution order

Middlewares are executed in order of **priority (highest to lowest)**. The `Priority` property must be between **0** and **100** (inclusive):

```csharp
// Priority 100 - Executes first (maximum priority)
[Middleware(IsGlobal = true, Priority = 100)]
public class LoggingMiddleware : IPreMiddleware { }

// Priority 90 - Executes second
[Middleware(IsGlobal = true, Priority = 90)]
public class AuthenticationMiddleware : IPreMiddleware { }

// Priority 80 - Executes third
[Middleware(IsGlobal = true, Priority = 80)]
public class ValidationMiddleware : IPreMiddleware { }

// Priority 50 - Executes fourth (usecase-specific)
[Middleware(IsGlobal = false, Priority = 50)]
public class CacheMiddleware : IPreMiddleware { }

// Priority 0 - Executes last (minimum priority)
[Middleware(IsGlobal = false, Priority = 0)]
public class FinalMiddleware : IPreMiddleware { }

// Execution order: Logging → Authentication → Validation → Cache → Final → UseCase
```

**Priority Constraint**: The `Priority` value must be between **0** and **100** (inclusive). Attempting to set a value outside this range (e.g., -1 or 101) will throw an `ArgumentOutOfRangeException` at runtime.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## Support

For issues, questions, or contributions, please visit:
- **GitHub**: https://github.com/osirisgate/Osirisgate.CleanArchitectureCore
- **Email**: developer@osirisgate.com

---

## Acknowledgments

- Inspired by Clean Architecture principles by Robert C. Martin
- Built with modern .NET best practices
- Designed for developer productivity and code maintainability
