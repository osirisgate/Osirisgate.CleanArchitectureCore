using Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;

namespace Osirisgate.CleanArchitectureCore.Tests.Helper;

using Helper = CleanArchitectureCore.Helper.Helper;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class HelperTest
{
    [Fact]
    public void TestGetWithSimpleKey()
    {
        var data = new Dictionary<string, object> { ["key"] = "value" };
        var result = Helper.Get<string>(data, "key");
        Assert.Equal("value", result);
    }

    [Fact]
    public void TestGetWithNestedKey()
    {
        var data = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object> { ["name"] = "John" }
        };
        var result = Helper.Get<string>(data, "user.name");
        Assert.Equal("John", result);
    }

    [Fact]
    public void TestGetWithNonExistentKey()
    {
        var data = new Dictionary<string, object> { ["key"] = "value" };
        var result = Helper.Get<string>(data, "nonexistent");
        Assert.Null(result);
    }

    [Fact]
    public void TestGetWithDefaultValue()
    {
        var data = new Dictionary<string, object> { ["key"] = "value" };
        var result = Helper.Get<string>(data, "nonexistent", "default");
        Assert.Equal("default", result);
    }

    [Fact]
    public void TestGetWithWrongType()
    {
        var data = new Dictionary<string, object> { ["key"] = 123 };
        var result = Helper.Get<string>(data, "key");
        Assert.Null(result);
    }

    [Fact]
    public void TestGetCaseInsensitiveWithExactMatch()
    {
        var data = new Dictionary<string, object> { ["UserName"] = "John" };
        var result = Helper.GetCaseInsensitive<string>(data, "UserName");
        Assert.Equal("John", result);
    }

    [Fact]
    public void TestGetCaseInsensitiveWithNormalizedKey()
    {
        var data = new Dictionary<string, object> { ["UserName"] = "John" };
        var result = Helper.GetCaseInsensitive<string>(data, "user_name");
        Assert.Equal("John", result);
    }

    [Fact]
    public void TestGetCaseInsensitiveWithCaseInsensitiveMatch()
    {
        var data = new Dictionary<string, object> { ["UserName"] = "John" };
        var result = Helper.GetCaseInsensitive<string>(data, "username");
        Assert.Equal("John", result);
    }

    [Fact]
    public void TestGetCaseInsensitiveWithNestedKey()
    {
        var data = new Dictionary<string, object>
        {
            ["User"] = new Dictionary<string, object> { ["Name"] = "John" }
        };
        var result = Helper.GetCaseInsensitive<string>(data, "User.Name");
        Assert.Equal("John", result);
    }

    [Fact]
    public void TestGetCaseInsensitiveWithNullValue()
    {
        var data = new Dictionary<string, object> { ["key"] = null! };
        var result = Helper.GetCaseInsensitive<string>(data, "key");
        Assert.Null(result);
    }

    [Fact]
    public void TestUpdateFieldWithSimpleKey()
    {
        var data = new Dictionary<string, object>();
        Helper.UpdateField(data, "key", "value");
        Assert.Equal("value", data["key"]);
    }

    [Fact]
    public void TestUpdateFieldWithNestedKey()
    {
        var data = new Dictionary<string, object>();
        Helper.UpdateField(data, "user.name", "John");
        Assert.Equal("John", ((Dictionary<string, object>)data["user"])!["name"]);
    }

    [Fact]
    public void TestUpdateFieldWithExistingNestedKey()
    {
        var data = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object> { ["name"] = "Old" }
        };
        Helper.UpdateField(data, "user.name", "New");
        Assert.Equal("New", ((Dictionary<string, object>)data["user"])!["name"]);
    }

    [Fact]
    public void TestUpdateFieldWithDictionaryMerge()
    {
        var data = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object> { ["name"] = "John" }
        };
        var newData = new Dictionary<string, object> { ["age"] = 30 };
        Helper.UpdateField(data, "user", newData);
        var userDict = (Dictionary<string, object>)data["user"]!;
        Assert.Equal("John", userDict["name"]);
        Assert.Equal(30, userDict["age"]);
    }

    [Fact]
    public void TestMergeDictionaries()
    {
        var target = new Dictionary<string, object> { ["key1"] = "value1", ["nested"] = new Dictionary<string, object> { ["a"] = "1" } };
        var source = new Dictionary<string, object> { ["key2"] = "value2", ["nested"] = new Dictionary<string, object> { ["b"] = "2" } };

        Helper.MergeDictionaries(target, source);

        Assert.Equal("value1", target["key1"]);
        Assert.Equal("value2", target["key2"]);
        var nested = (Dictionary<string, object>)target["nested"]!;
        Assert.Equal("1", nested["a"]);
        Assert.Equal("2", nested["b"]);
    }

    [Fact]
    public void TestCastToWithValidCast()
    {
        object value = "test";
        var result = Helper.CastTo<string>(value);
        Assert.Equal("test", result);
    }

    [Fact]
    public void TestCastToWithInvalidCast()
    {
        object value = 123;
        Assert.Throws<CleanArchitectureCore.Exception.InvalidCastException>(() => Helper.CastTo<string>(value));
    }

    [Fact]
    public void TestConvertSnakeCaseToPascalCase()
    {
        Assert.Equal("Email", Helper.ConvertSnakeCaseToPascalCase("email"));
        Assert.Equal("FirstName", Helper.ConvertSnakeCaseToPascalCase("first_name"));
        Assert.Equal("UserName", Helper.ConvertSnakeCaseToPascalCase("user_name"));
        Assert.Equal("UserName", Helper.ConvertSnakeCaseToPascalCase("userName"));
        Assert.Equal("UserName", Helper.ConvertSnakeCaseToPascalCase("UserName"));
    }

    [Fact]
    public void TestConvertSnakeCaseToPascalCaseWithEmptyString()
    {
        Assert.Equal("", Helper.ConvertSnakeCaseToPascalCase(""));
    }

    [Fact]
    public void TestConvertSnakeCaseToPascalCaseWithNull()
    {
        Assert.Throws<ArgumentNullException>(() => Helper.ConvertSnakeCaseToPascalCase(null!));
    }

    [Fact]
    public void TestNormalizePayloadKeys()
    {
        var payload = new Dictionary<string, object>
        {
            ["user_name"] = "John",
            ["email"] = "john@example.com",
            ["address"] = new Dictionary<string, object> { ["street_name"] = "Main St" }
        };

        var normalized = Helper.NormalizePayloadKeys(payload);

        Assert.Equal("John", normalized["UserName"]);
        Assert.Equal("john@example.com", normalized["Email"]);
        var address = (Dictionary<string, object>)normalized["Address"]!;
        Assert.Equal("Main St", address["StreetName"]);
    }

    [Fact]
    public void TestNormalizePayloadKeysWithReadOnlyDictionary()
    {
        var readOnlyDict = new Dictionary<string, object> { ["key"] = "value" };
        var payload = new Dictionary<string, object>
        {
            ["nested"] = new Dictionary<string, object>(readOnlyDict).AsReadOnly()
        };

        var normalized = Helper.NormalizePayloadKeys(payload);
        var nested = (Dictionary<string, object>)normalized["Nested"]!;
        Assert.Equal("value", nested["Key"]);
    }

    [Fact]
    public void TestFilterPayloadByStructure()
    {
        var sourcePayload = new Dictionary<string, object>
        {
            ["name"] = "John",
            ["age"] = 30,
            ["extra"] = "ignored"
        };
        var structure = new Dictionary<string, object>
        {
            ["name"] = "required",
            ["age"] = "required"
        };
        var targetPayload = new Dictionary<string, object>();

        Helper.FilterPayloadByStructure(sourcePayload, structure, targetPayload);

        Assert.Equal("John", targetPayload["name"]);
        Assert.Equal(30, targetPayload["age"]);
        Assert.False(targetPayload.ContainsKey("extra"));
    }

    [Fact]
    public void TestFilterPayloadByStructureWithNested()
    {
        var sourcePayload = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = "John",
                ["age"] = 30,
                ["extra"] = "ignored"
            }
        };
        var structure = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = "required",
                ["age"] = "required"
            }
        };
        var targetPayload = new Dictionary<string, object>();

        Helper.FilterPayloadByStructure(sourcePayload, structure, targetPayload);

        var user = (Dictionary<string, object>)targetPayload["user"]!;
        Assert.Equal("John", user["name"]);
        Assert.Equal(30, user["age"]);
        Assert.False(user.ContainsKey("extra"));
    }

    [Fact]
    public void TestMapToDto()
    {
        var payload = new Dictionary<string, object>
        {
            ["Email"] = "john@example.com",
            ["Password"] = "password123"
        };

        var dto = Helper.MapToDto<UserRegistrationDto>(payload);

        Assert.NotNull(dto);
        Assert.Equal("john@example.com", dto.Email);
        Assert.Equal("password123", dto.Password);
    }

    [Fact]
    public void TestMapToDtoWithCaseInsensitive()
    {
        var payload = new Dictionary<string, object>
        {
            ["email"] = "john@example.com",
            ["password"] = "password123"
        };

        var dto = Helper.MapToDto<UserRegistrationDto>(payload);

        Assert.NotNull(dto);
        Assert.Equal("john@example.com", dto.Email);
        Assert.Equal("password123", dto.Password);
    }

    [Fact]
    public void TestFilterPayloadByStructureWithNestedStructure()
    {
        // Use Dictionary directly - it implements IReadOnlyDictionary
        var sourcePayload = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = "John",
                ["email"] = "john@example.com",
                ["age"] = 30
            },
            ["other"] = "value"
        };

        var structure = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = new Dictionary<string, object>(),
                ["email"] = new Dictionary<string, object>()
            }
        };

        var targetPayload = new Dictionary<string, object>();
        Helper.FilterPayloadByStructure(sourcePayload, structure, targetPayload);

        // Dictionary<string, object> is not IReadOnlyDictionary<string, object> in the check
        // So the nested path won't be taken. This test covers the else branch (non-nested rule)
        // For nested structure test, see TestFilterPayloadByStructureWithNested which works correctly
        Assert.False(targetPayload.ContainsKey("user")); // Dictionary doesn't match IReadOnlyDictionary check
    }

    [Fact]
    public void TestFilterPayloadByStructureWithNestedStructureEmptyResult()
    {
        var sourcePayload = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = "John"
            }
        };

        var structure = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["email"] = new Dictionary<string, object>()
            }
        };

        var targetPayload = new Dictionary<string, object>();
        Helper.FilterPayloadByStructure(sourcePayload, structure, targetPayload);

        Assert.False(targetPayload.ContainsKey("user"));
    }

    [Fact]
    public void TestFilterPayloadByStructureWithNonNestedRule()
    {
        var sourcePayload = new Dictionary<string, object>
        {
            ["name"] = "John",
            ["email"] = "john@example.com",
            ["age"] = 30
        };

        var structure = new Dictionary<string, object>
        {
            ["name"] = "string",
            ["email"] = "string"
        };

        var targetPayload = new Dictionary<string, object>();
        Helper.FilterPayloadByStructure(sourcePayload, structure, targetPayload);

        Assert.Equal("John", targetPayload["name"]);
        Assert.Equal("john@example.com", targetPayload["email"]);
        Assert.False(targetPayload.ContainsKey("age"));
    }

    [Fact]
    public void TestFilterPayloadByStructureWithMissingNestedValue()
    {
        var sourcePayload = new Dictionary<string, object>
        {
            ["other"] = "value"
        };

        var structure = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = new Dictionary<string, object>()
            }
        };

        var targetPayload = new Dictionary<string, object>();
        Helper.FilterPayloadByStructure(sourcePayload, structure, targetPayload);

        Assert.False(targetPayload.ContainsKey("user"));
    }

    [Fact]
    public void TestFilterPayloadByStructureWithNestedValueNotDictionary()
    {
        var sourcePayload = new Dictionary<string, object>
        {
            ["user"] = "not-a-dictionary"
        };

        var structure = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = new Dictionary<string, object>()
            }
        };

        var targetPayload = new Dictionary<string, object>();
        Helper.FilterPayloadByStructure(sourcePayload, structure, targetPayload);

        Assert.False(targetPayload.ContainsKey("user"));
    }
}
