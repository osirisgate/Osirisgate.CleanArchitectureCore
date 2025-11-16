using Osirisgate.CleanArchitectureCore.Tests.Stubs.Request;
using ArgumentException = Osirisgate.CleanArchitectureCore.Exception.ArgumentException;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;
using InvalidCastException = Osirisgate.CleanArchitectureCore.Exception.InvalidCastException;

namespace Osirisgate.CleanArchitectureCore.Tests.Helper;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class HelperAdvancedTest
{
    [Fact]
    public void TestGetWithNullDataThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CleanArchitectureCore.Helper.Helper.Get<string>(null!, "key"));
    }

    [Fact]
    public void TestGetWithNullOrWhiteSpaceFieldPathThrowsArgumentException()
    {
        var data = new Dictionary<string, object>();

        Assert.Throws<ArgumentException>(() => CleanArchitectureCore.Helper.Helper.Get<string>(data, null!));
        Assert.Throws<ArgumentException>(() => CleanArchitectureCore.Helper.Helper.Get<string>(data, ""));
        Assert.Throws<ArgumentException>(() => CleanArchitectureCore.Helper.Helper.Get<string>(data, "   "));
    }

    [Fact]
    public void TestGetCaseInsensitiveWithNullDataThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CleanArchitectureCore.Helper.Helper.GetCaseInsensitive<string>(null!, "key"));
    }

    [Fact]
    public void TestGetCaseInsensitiveWithNullOrWhiteSpaceFieldPathThrowsArgumentException()
    {
        var data = new Dictionary<string, object>();

        Assert.Throws<ArgumentException>(() => CleanArchitectureCore.Helper.Helper.GetCaseInsensitive<string>(data, null!));
        Assert.Throws<ArgumentException>(() => CleanArchitectureCore.Helper.Helper.GetCaseInsensitive<string>(data, ""));
        Assert.Throws<ArgumentException>(() => CleanArchitectureCore.Helper.Helper.GetCaseInsensitive<string>(data, "   "));
    }

    [Fact]
    public void TestUpdateFieldWithNullDataThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CleanArchitectureCore.Helper.Helper.UpdateField(null!, "key", "value"));
    }

    [Fact]
    public void TestUpdateFieldWithNullOrWhiteSpaceFieldPathThrowsArgumentException()
    {
        var data = new Dictionary<string, object>();

        Assert.Throws<ArgumentException>(() => CleanArchitectureCore.Helper.Helper.UpdateField(data, null!, "value"));
        Assert.Throws<ArgumentException>(() => CleanArchitectureCore.Helper.Helper.UpdateField(data, "", "value"));
        Assert.Throws<ArgumentException>(() => CleanArchitectureCore.Helper.Helper.UpdateField(data, "   ", "value"));
    }

    [Fact]
    public void TestUpdateFieldWithNullValueUpdatesCorrectly()
    {
        var data = new Dictionary<string, object> { ["key"] = "value" };

        CleanArchitectureCore.Helper.Helper.UpdateField(data, "key", null);

        Assert.Null(data["key"]);
    }

    [Fact]
    public void TestMergeDictionariesWithNullTargetThrowsArgumentNullException()
    {
        var source = new Dictionary<string, object>();

        Assert.Throws<ArgumentNullException>(() => CleanArchitectureCore.Helper.Helper.MergeDictionaries(null!, source));
    }

    [Fact]
    public void TestMergeDictionariesWithNullSourceThrowsArgumentNullException()
    {
        var target = new Dictionary<string, object>();

        Assert.Throws<ArgumentNullException>(() => CleanArchitectureCore.Helper.Helper.MergeDictionaries(target, null!));
    }

    [Fact]
    public void TestCastToWithNullValueThrowsInvalidCastException()
    {
        Assert.Throws<InvalidCastException>(() => CleanArchitectureCore.Helper.Helper.CastTo<string>(null!));
    }

    [Fact]
    public void TestCastToWithInvalidTypeThrowsInvalidCastException()
    {
        Assert.Throws<InvalidCastException>(() => CleanArchitectureCore.Helper.Helper.CastTo<int>("string"));
    }

    [Fact]
    public void TestConvertSnakeCaseToPascalCaseWithNullThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CleanArchitectureCore.Helper.Helper.ConvertSnakeCaseToPascalCase(null!));
    }

    [Fact]
    public void TestNormalizePayloadKeysWithNullThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CleanArchitectureCore.Helper.Helper.NormalizePayloadKeys(null!));
    }

    [Fact]
    public void TestFilterPayloadByStructureWithNullSourcePayloadThrowsArgumentNullException()
    {
        var structure = new Dictionary<string, object>();
        var target = new Dictionary<string, object>();

        Assert.Throws<ArgumentNullException>(() => CleanArchitectureCore.Helper.Helper.FilterPayloadByStructure(null!, structure, target));
    }

    [Fact]
    public void TestFilterPayloadByStructureWithNullStructureThrowsArgumentNullException()
    {
        var source = new Dictionary<string, object>();
        var target = new Dictionary<string, object>();

        Assert.Throws<ArgumentNullException>(() => CleanArchitectureCore.Helper.Helper.FilterPayloadByStructure(source, null!, target));
    }

    [Fact]
    public void TestFilterPayloadByStructureWithNullTargetThrowsArgumentNullException()
    {
        var source = new Dictionary<string, object>();
        var structure = new Dictionary<string, object>();

        Assert.Throws<ArgumentNullException>(() => CleanArchitectureCore.Helper.Helper.FilterPayloadByStructure(source, structure, null!));
    }

    [Fact]
    public void TestMapToDtoWithNullPayloadThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CleanArchitectureCore.Helper.Helper.MapToDto<UserRegistrationDto>(null!));
    }

    [Fact]
    public void TestGetWithDeepNestedPathReturnsValue()
    {
        var data = new Dictionary<string, object>
        {
            ["level1"] = new Dictionary<string, object>
            {
                ["level2"] = new Dictionary<string, object>
                {
                    ["level3"] = new Dictionary<string, object>
                    {
                        ["value"] = "found"
                    }
                }
            }
        };

        var result = CleanArchitectureCore.Helper.Helper.Get<string>(data, "level1.level2.level3.value");

        Assert.Equal("found", result);
    }

    [Fact]
    public void TestGetCaseInsensitiveWithMixedCaseKeysReturnsValue()
    {
        var data = new Dictionary<string, object>
        {
            ["UserName"] = "John",
            ["UserEmail"] = "john@example.com"
        };

        Assert.Equal("John", CleanArchitectureCore.Helper.Helper.GetCaseInsensitive<string>(data, "username"));
        Assert.Equal("John", CleanArchitectureCore.Helper.Helper.GetCaseInsensitive<string>(data, "user_name"));
        Assert.Equal("john@example.com", CleanArchitectureCore.Helper.Helper.GetCaseInsensitive<string>(data, "UserEmail"));
    }

    [Fact]
    public void TestUpdateFieldWithDeepNestedPathCreatesStructure()
    {
        var data = new Dictionary<string, object>();

        CleanArchitectureCore.Helper.Helper.UpdateField(data, "a.b.c.d", "value");

        var result = CleanArchitectureCore.Helper.Helper.Get<string>(data, "a.b.c.d");
        Assert.Equal("value", result);
    }

    [Fact]
    public void TestNormalizePayloadKeysWithComplexStructureNormalizesRecursively()
    {
        var payload = new Dictionary<string, object>
        {
            ["user_name"] = "John",
            ["user_email"] = "john@example.com",
            ["address"] = new Dictionary<string, object>
            {
                ["street_name"] = "Main St",
                ["city_name"] = "New York",
                ["nested"] = new Dictionary<string, object>
                {
                    ["deep_field"] = "value"
                }
            }
        };

        var normalized = CleanArchitectureCore.Helper.Helper.NormalizePayloadKeys(payload);

        Assert.Equal("John", normalized["UserName"]);
        Assert.Equal("john@example.com", normalized["UserEmail"]);
        var address = normalized["Address"] as Dictionary<string, object>;
        Assert.NotNull(address);
        Assert.Equal("Main St", address["StreetName"]);
        Assert.Equal("New York", address["CityName"]);
        var nested = address["Nested"] as Dictionary<string, object>;
        Assert.NotNull(nested);
        Assert.Equal("value", nested["DeepField"]);
    }

    [Fact]
    public void TestGetCaseInsensitiveWithNonDictionaryDataReturnsDefaultValue()
    {
        var nonDictionaryData = "not a dictionary";

        var result = CleanArchitectureCore.Helper.Helper.GetCaseInsensitive<string>(nonDictionaryData, "key", "default");

        Assert.Equal("default", result);
    }

    [Fact]
    public void TestNormalizePayloadKeysWithIReadOnlyDictionary()
    {
        var readOnlyNested = new Dictionary<string, object>
        {
            ["nested_key"] = "nested_value"
        }.AsReadOnly();

        var payload = new Dictionary<string, object>
        {
            ["top_key"] = readOnlyNested
        };

        var normalized = CleanArchitectureCore.Helper.Helper.NormalizePayloadKeys(payload);

        Assert.True(normalized.TryGetValue("TopKey", out var nestedValue));
        var nested = nestedValue as Dictionary<string, object>;
        Assert.NotNull(nested);
        Assert.Equal("nested_value", nested["NestedKey"]);
    }

    [Fact]
    public void TestGetCaseInsensitiveWithNestedNonDictionaryInPath()
    {
        // Test when a value in the path is not a dictionary
        var data = new Dictionary<string, object>
        {
            ["key"] = "not a dictionary"
        };

        // Try to access "key.nested" where "key" is a string, not a dictionary
        var result = CleanArchitectureCore.Helper.Helper.GetCaseInsensitive<string>(data, "key.nested", "default");

        Assert.Equal("default", result);
    }

    [Fact]
    public void TestConvertSnakeCaseToPascalCaseWithEmptyString()
    {
        var result = CleanArchitectureCore.Helper.Helper.ConvertSnakeCaseToPascalCase("");
        Assert.Equal("", result);
    }

    [Fact]
    public void TestConvertSnakeCaseToPascalCaseWithSingleCharacter()
    {
        var result = CleanArchitectureCore.Helper.Helper.ConvertSnakeCaseToPascalCase("a_b_c");
        Assert.Equal("ABC", result);
    }

    [Fact]
    public void TestConvertSnakeCaseToPascalCaseWithSingleCharPart()
    {
        var result = CleanArchitectureCore.Helper.Helper.ConvertSnakeCaseToPascalCase("a_bc_d");
        Assert.Equal("ABcD", result);
    }
}
