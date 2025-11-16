using Osirisgate.CleanArchitectureCore.Enums;
using Osirisgate.CleanArchitectureCore.Exception;
using Osirisgate.CleanArchitectureCore.Response;
using Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;

namespace Osirisgate.CleanArchitectureCore.Tests.Request;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class RequestTest
{
    [Fact]
    public void TestCanCreateNewRequest()
    {
        CleanArchitectureCore.Request.Request customRequest = new CustomRequest(new Dictionary<string, object>());
        Assert.IsType<CustomRequest>(customRequest);
    }

    [Fact]
    public void TestCanCreateNewRequestWithPayload()
    {
        Dictionary<string, object> payload = new()
        {
            ["key"] = "value"
        };
        CleanArchitectureCore.Request.Request request = new CustomRequestWithPayload(payload);

        Assert.NotEmpty(request.GetPayload());
        Assert.Equal(payload, request.GetPayload());
    }

    [Fact]
    public void TestCanGetExistingRequestFieldFromPayload()
    {
        Dictionary<string, object> payload = new()
        {
            ["key"] = "value"
        };
        CleanArchitectureCore.Request.Request request = new CustomRequestWithPayload(payload);

        var field = request.GetField<string>("key");
        Assert.IsType<string>(field);
        Assert.Equal("value", field);
    }

    [Fact]
    public void TestCanGetNonExistingRequestFieldFromPayload()
    {
        Dictionary<string, object> payload = new()
        {
            ["key"] = "value"
        };
        CleanArchitectureCore.Request.Request request = new CustomRequestWithPayload(payload);

        var unknownField = request.GetField<string>("unknownField");
        Assert.Null(unknownField);
    }

    [Fact]
    public void TestCanGetDefaultValueForNonExistingRequestField()
    {
        Dictionary<string, object> payload = new()
        {
            ["key"] = "value"
        };
        CleanArchitectureCore.Request.Request request = new CustomRequestWithPayload(payload);

        var defaultFieldValue = request.GetField<string>("unknownField", "default");
        Assert.Equal("default", defaultFieldValue);
    }

    [Fact]
    public void TestCanGetDeepExistingRequestFieldFromPayload()
    {
        Dictionary<string, object> address = new()
        {
            ["city"] = "London",
        };

        Dictionary<string, object> payload = new()
        {
            ["user"] = new Dictionary<string, object>()
            {
                ["name"] = "Jean",
                ["age"] = 18,
                ["address"] = address
            }
        };
        CleanArchitectureCore.Request.Request request = new CustomRequestWithDeepPayload(payload);

        var name = request.GetField<string>("user.name");
        Assert.Equal("Jean", name);

        var age = request.GetField<int>("user.age");
        Assert.Equal(18, age);

        var agesNull = request.GetField<object>("user.ages");
        Assert.Null(agesNull);

        var addressField = request.GetField<Dictionary<string, object>>("user.address");
        Assert.NotNull(addressField);
        Assert.Equal("London", addressField["City"]?.ToString());

        var addressCity = request.GetField<string>("user.address.city");
        Assert.Equal("London", addressCity);
    }

    [Fact]
    public void TestCanNotCreateRequestWithMissingField()
    {
        try
        {
            Dictionary<string, object> payload = new()
            {
                ["firstname"] = "ulrich"
            };
            CleanArchitectureCore.Request.Request request = new CustomRequestWithMissingField(payload);
        }
        catch (BaseException ex)
        {
            Assert.Equal(StatusCode.BadRequest.GetValue(), ex.GetErrorCode());
            Assert.Equal("missing.required.fields", ex.GetMessage());
            Assert.Equal(
                new Dictionary<string, object>() { ["missing_fields"] = new Dictionary<string, object> { ["lastname"] = "required" } },
                ex.GetDetails()
            );
        }
    }

    [Fact]
    public void TestCanNotCreateRequestWithDeepDictionnaryMissingField()
    {
        try
        {
            Dictionary<string, object> payload = new()
            {
                ["user"] = "ulrich"
            };
            CleanArchitectureCore.Request.Request customRequest = new CustomRequestWithDeepMissingField(payload);
        }
        catch (BaseException ex)
        {
            Assert.Equal(
                new Dictionary<string, object>()
                {
                    ["missing_fields"] = new Dictionary<string, object>
                    {
                        ["user"] = "required field type not matching [Dictionary<string, object>]"
                    }
                },
                ex.GetDetails()
            );
        }
    }

    [Fact]
    public void TestCanNotCreateRequestWithDeepMissingField()
    {
        try
        {
            Dictionary<string, object> payload = new()
            {
                ["user"] = new Dictionary<string, object>()
                {
                    ["lastname"] = "ulrich",
                    ["address"] = "London"
                }
            };
            CleanArchitectureCore.Request.Request request = new CustomRequestWithDeepMissingField(payload);
        }
        catch (BaseException ex)
        {
            Assert.Equal(
                new Dictionary<string, object>()
                {
                    ["missing_fields"] = new Dictionary<string, object>
                    {
                        ["user.firstname"] = "required",
                        ["user.address"] = "required field type not matching [Dictionary<string, object>]"
                    }
                },
                ex.GetDetails()
            );
        }
    }

    [Fact]
    public void TestCanNotCreateRequestWithNotAllowedField()
    {
        try
        {
            Dictionary<string, object> payload = new()
            {
                ["firstname"] = "Ulrich",
                ["lastname"] = "Geraud",
                ["age"] = "18"
            };
            CleanArchitectureCore.Request.Request request = new CustomRequestWithMissingField(payload);
        }
        catch (BaseException ex)
        {
            Assert.Equal(StatusCode.BadRequest.GetValue(), ex.GetErrorCode());
            Assert.Equal("illegal.fields", ex.GetMessage());
            Assert.Equal(
                new Dictionary<string, object>() { ["unrequired_fields"] = new List<string>(["age"]) },
                ex.GetDetails()
            );
        }
    }

    [Fact]
    public void TestCanNotCreateRequestWithDeepNotAllowedField()
    {
        try
        {
            Dictionary<string, object> payload = new()
            {
                ["user"] = new Dictionary<string, object>()
                {
                    ["firstname"] = "ulrich",
                    ["lastname"] = "London",
                    ["address"] = new Dictionary<string, object>()
                    {
                        ["city"] = "London",
                        ["age"] = 20,
                    }
                }
            };
            CleanArchitectureCore.Request.Request request = new CustomRequestWithDeepMissingField(payload);
        }
        catch (BaseException ex)
        {
            Assert.Equal(
                new Dictionary<string, object>() { ["unrequired_fields"] = new List<string>(["user.address.age"]) },
                ex.GetDetails()
            );
        }
    }

    [Fact]
    public async Task TestCanNotCreateRequestWithErrorsConstraints()
    {
        try
        {
            Dictionary<string, object> payload = new()
            {
                ["user"] = new Dictionary<string, object>()
                {
                    ["firstname"] = "ulrich",
                    ["lastname"] = "London",
                    ["address"] = new Dictionary<string, object>()
                    {
                        ["city"] = "London",
                    }
                }
            };
            CleanArchitectureCore.Request.Request request = new CustomRequestWithApplyingConstraint(payload);
            await request.ValidateAsync();
        }
        catch (BaseException ex)
        {
            Assert.IsType<BadRequestContentException>(ex);
            Assert.Equal(
                new Dictionary<string, object>
                {
                    ["status"] = Status.Error.GetValue(),
                    ["error_code"] = StatusCode.BadRequest.GetValue(),
                    ["message"] = "constraint.applying.failed",
                    ["details"] = new Dictionary<string, object>
                    {
                        ["firstname"] = "required",
                        ["lastname"] = "required",
                    }
                },
                ex.Format()
            );
        }
    }

    [Fact]
    public async Task TestValidateAsyncWithSuccessfulValidation()
    {
        Dictionary<string, object> payload = new()
        {
            ["key"] = "value"
        };

        var request = new CustomRequestWithPayload(payload);
        await request.ValidateAsync();

        Assert.NotEmpty(request.GetPayload());
        Assert.Equal(payload, request.GetPayload());
    }

    [Fact]
    public async Task TestValidateAsyncThrowsExceptionOnFailedValidation()
    {
        try
        {
            Dictionary<string, object> payload = new()
            {
                ["user"] = new Dictionary<string, object>()
                {
                    ["firstname"] = "ulrich",
                    ["lastname"] = "London",
                    ["address"] = new Dictionary<string, object>()
                    {
                        ["city"] = "London",
                    }
                }
            };

            var request = new CustomRequestWithApplyingConstraint(payload);
            await request.ValidateAsync();

            Assert.Fail("Expected BadRequestContentException to be thrown");
        }
        catch (BadRequestContentException ex)
        {
            Assert.Equal("constraint.applying.failed", ex.GetMessage());
            Assert.Equal(
                new Dictionary<string, object>
                {
                    ["firstname"] = "required",
                    ["lastname"] = "required",
                },
                ex.GetDetails()
            );
        }
    }

    [Fact]
    public async Task TestValidateAsyncWithSuccessfulAsyncValidation()
    {
        Dictionary<string, object> payload = new()
        {
            ["email"] = "test@example.com"
        };

        var request = new CustomRequestWithSuccessfulAsyncValidation(payload);
        await request.ValidateAsync();

        Assert.NotEmpty(request.GetPayload());
        Assert.Equal(payload, request.GetPayload());
        var email = request.GetField<string>("email");
        Assert.Equal("test@example.com", email);
    }

    [Fact]
    public void TestModifiedPayloadReturnsRequestInstance()
    {
        var payload = new Dictionary<string, object> { ["key"] = "value" };
        var request = new CustomRequestWithPayload(payload);

        var modifiedPayload = new Dictionary<string, object> { ["modified"] = "data" };
        var result = request.ModifiedPayload(modifiedPayload);

        Assert.Same(request, result);
        Assert.Contains("modified", request.GetModifiedPayload().Keys);
        Assert.Equal("data", request.GetModifiedPayload()["modified"]);
    }

    [Fact]
    public void TestModifiedPayloadWithEmptyDictionary()
    {
        var payload = new Dictionary<string, object> { ["key"] = "value" };
        var request = new CustomRequestWithPayload(payload);

        var emptyPayload = new Dictionary<string, object>();
        var result = request.ModifiedPayload(emptyPayload);

        Assert.Same(request, result);
        Assert.Empty(request.GetModifiedPayload());
    }

    [Fact]
    public void TestGetModifiedFieldUsesNormalizedKeys()
    {
        var payload = new Dictionary<string, object> { ["key"] = "value" };
        var request = new CustomRequestWithPayload(payload);

        var modifiedPayload = new Dictionary<string, object>
        {
            ["modified"] = "data",
            ["user_name"] = "test_user"
        };
        request.ModifiedPayload(modifiedPayload);

        var modifiedValue = request.GetModifiedField<string>("modified");
        Assert.Equal("data", modifiedValue);

        // "user_name" becomes "UserName" in normalized dictionary
        var userName = request.GetModifiedField<string>("user_name");
        Assert.Equal("test_user", userName);

        var modifiedValuePascal = request.GetModifiedField<string>("Modified");
        Assert.Equal("data", modifiedValuePascal);

        var userNamePascal = request.GetModifiedField<string>("UserName");
        Assert.Equal("test_user", userNamePascal);
    }

    [Fact]
    public void TestGetModifiedFieldWithNestedFields()
    {
        var payload = new Dictionary<string, object> { ["key"] = "value" };
        var request = new CustomRequestWithPayload(payload);

        var modifiedPayload = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["first_name"] = "John",
                ["last_name"] = "Doe"
            }
        };
        request.ModifiedPayload(modifiedPayload);

        var firstName = request.GetModifiedField<string>("user.first_name");
        Assert.Equal("John", firstName);

        var lastName = request.GetModifiedField<string>("user.last_name");
        Assert.Equal("Doe", lastName);

        var firstNamePascal = request.GetModifiedField<string>("user.FirstName");
        Assert.Equal("John", firstNamePascal);

        var lastNamePascal = request.GetModifiedField<string>("user.LastName");
        Assert.Equal("Doe", lastNamePascal);
    }

    [Fact]
    public async Task TestSimpleValidationPasses()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "user@example.com",
            ["age"] = 25,
            ["password"] = "SecurePass123"
        };

        var request = new UserRegistrationRequestWithSimpleValidation(payload);
        await request.ValidateAsync();

        Assert.Equal("user@example.com", request.GetField<string>("email"));
        Assert.Equal(25, request.GetField<int?>("age"));
    }

    [Fact]
    public async Task TestSimpleValidationFailsOnInvalidEmail()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "invalid-email",
            ["age"] = 25,
            ["password"] = "SecurePass123"
        };

        var request = new UserRegistrationRequestWithSimpleValidation(payload);

        var exception = await Assert.ThrowsAsync<BadRequestContentException>(async () =>
        {
            await request.ValidateAsync();
        });

        Assert.Equal("validation.failed", exception.GetMessage());
        var details = exception.GetDetails();
        Assert.True(details.TryGetValue("email", out var emailError));
        Assert.Equal("Invalid email format", emailError);
    }

    [Fact]
    public async Task TestSimpleValidationFailsOnInvalidAge()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "user@example.com",
            ["age"] = 15,
            ["password"] = "SecurePass123"
        };

        var request = new UserRegistrationRequestWithSimpleValidation(payload);

        var exception = await Assert.ThrowsAsync<BadRequestContentException>(async () =>
        {
            await request.ValidateAsync();
        });

        Assert.Equal("validation.failed", exception.GetMessage());
        var details = exception.GetDetails();
        Assert.True(details.TryGetValue("age", out var ageError));
        Assert.Equal("Age must be between 18 and 120", ageError);
    }

    [Fact]
    public async Task TestSimpleValidationFailsOnShortPassword()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "user@example.com",
            ["age"] = 25,
            ["password"] = "short"
        };

        var request = new UserRegistrationRequestWithSimpleValidation(payload);

        var exception = await Assert.ThrowsAsync<BadRequestContentException>(async () =>
        {
            await request.ValidateAsync();
        });

        Assert.Equal("validation.failed", exception.GetMessage());
        var details = exception.GetDetails();
        Assert.True(details.TryGetValue("password", out var passwordError));
        Assert.Equal("Password must be at least 8 characters long", passwordError);
    }

    [Fact]
    public async Task TestFluentValidationPasses()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "user@example.com",
            ["age"] = 25,
            ["password"] = "SecurePass123"
        };

        var request = new UserRegistrationRequestWithFluentValidation(payload);
        await request.ValidateAsync();

        Assert.Equal("user@example.com", request.GetField<string>("email"));
        Assert.Equal(25, request.GetField<int?>("age"));
    }

    [Fact]
    public async Task TestFluentValidationFailsOnInvalidEmail()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "invalid-email",
            ["age"] = 25,
            ["password"] = "SecurePass123"
        };

        var request = new UserRegistrationRequestWithFluentValidation(payload);

        var exception = await Assert.ThrowsAsync<BadRequestContentException>(async () =>
        {
            await request.ValidateAsync();
        });

        Assert.Equal("validation.failed", exception.GetMessage());
        var details = exception.GetDetails();
        Assert.True(details.TryGetValue("Email", out var emailError));
        Assert.Equal("Invalid email format", emailError);
    }

    [Fact]
    public async Task TestFluentValidationFailsOnMissingUppercase()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "user@example.com",
            ["age"] = 25,
            ["password"] = "securepass123"
        };

        var request = new UserRegistrationRequestWithFluentValidation(payload);

        var exception = await Assert.ThrowsAsync<BadRequestContentException>(async () =>
        {
            await request.ValidateAsync();
        });

        Assert.Equal("validation.failed", exception.GetMessage());
        var details = exception.GetDetails();
        Assert.True(details.TryGetValue("Password", out var passwordError));
        Assert.Equal("Password must contain at least one uppercase letter", passwordError);
    }

    [Fact]
    public async Task TestFluentValidationFailsOnMultipleErrors()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "invalid",
            ["age"] = 15, // Too young
            ["password"] = "short" // Too short and missing requirements
        };

        var request = new UserRegistrationRequestWithFluentValidation(payload);

        var exception = await Assert.ThrowsAsync<BadRequestContentException>(async () =>
        {
            await request.ValidateAsync();
        });

        Assert.Equal("validation.failed", exception.GetMessage());
        var details = exception.GetDetails();

        Assert.True(details.TryGetValue("Email", out _));
        Assert.True(details.TryGetValue("Age", out _));
        Assert.True(details.TryGetValue("Password", out _));
    }

    [Fact]
    public void TestToDtoMapsFieldsFromRequestStructure()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "user@example.com",
            ["age"] = 25,
            ["password"] = "SecurePass123"
        };

        var request = new TestRequestForDtoMapping(payload);
        var dto = request.GetDto<UserRegistrationDto>();

        Assert.Equal("user@example.com", dto.Email);
        Assert.Equal(25, dto.Age);
        Assert.Equal("SecurePass123", dto.Password);
    }

    [Fact]
    public void TestToDtoHandlesCaseInsensitiveMapping()
    {
        var payload = new Dictionary<string, object>
        {
            ["Email"] = "user@example.com",
            ["Age"] = 25,
            ["Password"] = "SecurePass123"
        };

        var request = new TestRequestForDtoMappingOptional(payload);
        var dto = request.GetDto<UserRegistrationDto>();

        Assert.Equal("user@example.com", dto.Email);
        Assert.Equal(25, dto.Age);
        Assert.Equal("SecurePass123", dto.Password);
    }

    [Fact]
    public void TestToDtoHandlesSnakeCaseToPascalCase()
    {
        var payload = new Dictionary<string, object>
        {
            ["first_name"] = "John",
            ["last_name"] = "Doe",
            ["email_address"] = "john@example.com"
        };

        var request = new CustomRequestWithSnakeCaseFields(payload);
        var dto = request.GetDto<UserNameDto>();

        Assert.Equal("John", dto.FirstName);
        Assert.Equal("Doe", dto.LastName);
        Assert.Equal("john@example.com", dto.EmailAddress);
    }

    [Fact]
    public void TestToDtoHandlesNullValues()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "user@example.com",
            ["age"] = null!, // Null value
            ["password"] = "SecurePass123"
        };

        var request = new TestRequestForDtoMappingOptional(payload);
        var dto = request.GetDto<UserRegistrationDto>();

        Assert.Equal("user@example.com", dto.Email);
        Assert.Null(dto.Age); // Nullable int should accept null
        Assert.Equal("SecurePass123", dto.Password);
    }

    [Fact]
    public void TestToDtoHandlesMissingFields()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "user@example.com"
        };

        var request = new TestRequestForDtoMappingOptional(payload);
        var dto = request.GetDto<UserRegistrationDto>();

        Assert.Equal("user@example.com", dto.Email);
        Assert.Null(dto.Age);
        Assert.Null(dto.Password);
    }

    [Fact]
    public void TestToDtoWorksWithFluentValidation()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "user@example.com",
            ["age"] = 25,
            ["password"] = "SecurePass123"
        };

        var request = new UserRegistrationRequestWithFluentValidation(payload);
        var validationTask = request.ValidateAsync();
        Assert.True(validationTask.IsCompletedSuccessfully);
    }

    [Fact]
    public void TestToDtoReturnsNormalizedDictionary()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "user@example.com",
            ["age"] = 25,
            ["password"] = "SecurePass123"
        };

        var request = new TestRequestForDtoMapping(payload);
        var dtoData = request.GetDtoDictionary();

        Assert.True(dtoData.TryGetValue("Email", out var email));
        Assert.True(dtoData.TryGetValue("Age", out var age));
        Assert.True(dtoData.TryGetValue("Password", out var password));
        Assert.Equal("user@example.com", email);
        Assert.Equal(25, age);
        Assert.Equal("SecurePass123", dtoData["Password"]);
    }

    [Fact]
    public void TestRequestCanBeUsedAsDto()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "user@example.com",
            ["age"] = 25,
            ["password"] = "SecurePass123"
        };

        var request = new UserRegistrationRequestAsDto(payload);

        Assert.Equal("user@example.com", request.Email);
        Assert.Equal(25, request.Age);
        Assert.Equal("SecurePass123", request.Password);
        var validationTask = request.ValidateAsync();
        Assert.True(validationTask.IsCompletedSuccessfully);
    }

    [Fact]
    public void TestToDtoWorksWithValidMapping()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "user@example.com",
            ["age"] = 25,
            ["password"] = "SecurePass123"
        };

        var request = new TestRequestForDtoMapping(payload);
        var dto = request.GetDto<UserRegistrationDto>();

        Assert.NotNull(dto);
        Assert.Equal("user@example.com", dto.Email);
        Assert.Equal(25, dto.Age);
        Assert.Equal("SecurePass123", dto.Password);
    }

    [Fact]
    public void TestToDtoWithEmptyPayloadStillCreatesDto()
    {
        var payload = new Dictionary<string, object>();
        var request = new TestRequestForDtoMappingOptional(payload);
        var dto = request.GetDto<UserRegistrationDto>();

        Assert.NotNull(dto);
        Assert.Null(dto.Email);
        Assert.Null(dto.Age);
        Assert.Null(dto.Password);
    }

    [Fact]
    public void TestToDtoNormalizesKeysCorrectly()
    {
        var payload = new Dictionary<string, object>
        {
            ["first_name"] = "John",
            ["last_name"] = "Doe",
            ["user_email"] = "john@example.com"
        };

        var request = new TestRequestForDtoMappingOptional(payload);
        var dtoData = request.GetDtoDictionary();

        Assert.True(dtoData.TryGetValue("FirstName", out var firstName));
        Assert.True(dtoData.TryGetValue("LastName", out var lastName));
        Assert.True(dtoData.TryGetValue("UserEmail", out var userEmail));
        Assert.Equal("John", firstName);
        Assert.Equal("Doe", lastName);
        Assert.Equal("john@example.com", dtoData["UserEmail"]);
    }

    [Fact]
    public void TestToDtoWithNestedStructures()
    {
        var payload = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = "John Doe",
                ["contact"] = new Dictionary<string, object>
                {
                    ["email"] = "john@example.com",
                    ["phone"] = "123-456-7890"
                }
            }
        };

        var request = new TestRequestForDtoMappingOptional(payload);
        var dtoData = request.GetDtoDictionary();

        Assert.True(dtoData.TryGetValue("User", out var userValue));
        var user = userValue as IDictionary<string, object>;
        Assert.NotNull(user);
        Assert.Equal("John Doe", user["Name"]);

        var contact = user["Contact"] as IDictionary<string, object>;
        Assert.NotNull(contact);
        Assert.Equal("john@example.com", contact["Email"]);
        Assert.Equal("123-456-7890", contact["Phone"]);
    }

    [Fact]
    public void TestToModifiedDtoReturnsNormalizedDictionary()
    {
        var payload = new Dictionary<string, object> { ["key"] = "value" };
        var request = new CustomRequestWithToModifiedDto(payload);

        var modifiedPayload = new Dictionary<string, object>
        {
            ["modified_key"] = "modified_value",
            ["user_name"] = "test_user"
        };
        request.ModifiedPayload(modifiedPayload);

        var normalizedDto = request.GetToModifiedDto();

        Assert.True(normalizedDto.TryGetValue("ModifiedKey", out var modifiedValue));
        Assert.True(normalizedDto.TryGetValue("UserName", out var userName));
        Assert.Equal("modified_value", modifiedValue);
        Assert.Equal("test_user", userName);
    }

    [Fact]
    public void TestToModifiedDtoWithEmptyModifiedPayload()
    {
        var payload = new Dictionary<string, object> { ["key"] = "value" };
        var request = new CustomRequestWithToModifiedDto(payload);
        var normalizedDto = request.GetToModifiedDto();

        Assert.NotNull(normalizedDto);
        Assert.Empty(normalizedDto);
    }

    [Fact]
    public void TestToModifiedDtoWithNestedStructures()
    {
        var payload = new Dictionary<string, object> { ["key"] = "value" };
        var request = new CustomRequestWithToModifiedDto(payload);

        var modifiedPayload = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["first_name"] = "John",
                ["last_name"] = "Doe"
            }
        };
        request.ModifiedPayload(modifiedPayload);

        var normalizedDto = request.GetToModifiedDto();

        Assert.True(normalizedDto.TryGetValue("User", out var userValue));
        var user = userValue as IDictionary<string, object>;
        Assert.NotNull(user);
        Assert.True(user.TryGetValue("FirstName", out var firstName));
        Assert.True(user.TryGetValue("LastName", out var lastName));
        Assert.Equal("John", firstName);
        Assert.Equal("Doe", lastName);
    }

    [Fact]
    public void TestFindMissingFieldsWithNestedRequiredDictionaryFieldMissing()
    {
        try
        {
            var payload = new Dictionary<string, object>
            {
                ["user"] = new Dictionary<string, object>
                {
                    ["firstname"] = "John",
                    ["lastname"] = "Doe"
                }
            };

            _ = new CustomRequestWithDeepMissingField(payload);
            Assert.Fail("Expected BadRequestContentException to be thrown");
        }
        catch (BadRequestContentException ex)
        {
            Assert.Equal("missing.required.fields", ex.GetMessage());

            Assert.Equal(
                new Dictionary<string, object>()
                {
                    ["missing_fields"] = new Dictionary<string, object>
                    {
                        ["user.address"] = "required field type not matching [Dictionary<string, object>]"
                    }
                },
                ex.GetDetails()
            );
        }
    }

    [Fact]
    public void TestToDtoHandlesValidMapping()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "user@example.com",
            ["age"] = 25
        };

        var request = new CustomRequestWithToDtoExposed(payload);
        var dto = request.GetDto<UserRegistrationDto>();

        Assert.NotNull(dto);
        Assert.Equal("user@example.com", dto.Email);
        Assert.Equal(25, dto.Age);
    }

    [Fact]
    public void TestRequestConstructorWithEmptyStructure()
    {
        var payload = new Dictionary<string, object>
        {
            ["any"] = "field",
            ["allowed"] = "here"
        };

        var request = new CustomRequestWithEmptyStructure(payload);

        Assert.NotNull(request);
        Assert.Equal(payload, request.GetPayload());
    }

    [Fact]
    public void TestRequestConstructorWithEmptyStructureAndAnyPayload()
    {
        var payload = new Dictionary<string, object>
        {
            ["nested"] = new Dictionary<string, object>
            {
                ["deep"] = "value"
            },
            ["simple"] = "value"
        };

        var request = new CustomRequestWithEmptyStructure(payload);
        Assert.NotNull(request);
        Assert.Equal(payload, request.GetPayload());
    }

    [Fact]
    public void TestModifiedPayloadWithEmptyDictionaryShouldReturnEarly()
    {
        var payload = new Dictionary<string, object> { ["key"] = "value" };
        var request = new CustomRequestWithPayload(payload);

        var result = request.ModifiedPayload(new Dictionary<string, object>());

        Assert.Same(request, result);
        Assert.Empty(request.GetModifiedPayload());
    }

    [Fact]
    public void TestModifiedPayloadWithNonEmptyDictionaryShouldMerge()
    {
        var payload = new Dictionary<string, object> { ["key"] = "value" };
        var request = new CustomRequestWithPayload(payload);

        var modified = new Dictionary<string, object> { ["key"] = "modifiedValue" };
        var result = request.ModifiedPayload(modified);

        Assert.Same(request, result);
        Assert.Equal("modifiedValue", request.GetModifiedField<string>("key"));
    }
}
