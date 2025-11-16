using Osirisgate.CleanArchitectureCore.Response;
using ArgumentException = Osirisgate.CleanArchitectureCore.Exception.ArgumentException;
using ArgumentNullException = Osirisgate.CleanArchitectureCore.Exception.ArgumentNullException;
using InvalidOperationException = Osirisgate.CleanArchitectureCore.Exception.InvalidOperationException;

namespace Osirisgate.CleanArchitectureCore.Tests.Response;

public sealed class ResponseExtensionsTest
{
    [Fact]
    public void TestGetRequiredWithExistingFieldReturnsValue()
    {
        var data = new Dictionary<string, object> { ["email"] = "test@example.com" };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        var result = response.GetRequired<string>("email");

        Assert.Equal("test@example.com", result);
    }

    [Fact]
    public void TestGetRequiredWithMissingFieldThrowsInvalidOperationException()
    {
        var data = new Dictionary<string, object> { ["key"] = "value" };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        var exception = Assert.Throws<InvalidOperationException>(() => response.GetRequired<string>("email"));
        var formatted = exception.Format();

        Assert.Contains("Required field 'email' is missing or null", formatted["message"]?.ToString() ?? "");
    }

    [Fact]
    public void TestGetRequiredWithNullResponseThrowsArgumentNullException()
    {
        IResponse? response = null;

        Assert.Throws<ArgumentNullException>(() => response!.GetRequired<string>("email"));
    }

    [Fact]
    public void TestGetRequiredWithNullOrWhiteSpaceFieldPathThrowsArgumentException()
    {
        var data = new Dictionary<string, object> { ["email"] = "test@example.com" };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        Assert.Throws<ArgumentException>(() => response.GetRequired<string>(null!));
        Assert.Throws<ArgumentException>(() => response.GetRequired<string>(""));
        Assert.Throws<ArgumentException>(() => response.GetRequired<string>("   "));
    }

    [Fact]
    public void TestTryGetWithExistingFieldReturnsTrue()
    {
        var data = new Dictionary<string, object> { ["email"] = "test@example.com" };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        var result = response.TryGet<string>("email", out var value);

        Assert.True(result);
        Assert.Equal("test@example.com", value);
    }

    [Fact]
    public void TestTryGetWithMissingFieldReturnsFalse()
    {
        var data = new Dictionary<string, object>();
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        var result = response.TryGet<string>("email", out var value);

        Assert.False(result);
        Assert.Null(value);
    }

    [Fact]
    public void TestTryGetWithNullResponseThrowsArgumentNullException()
    {
        IResponse? response = null;

        Assert.Throws<ArgumentNullException>(() => response!.TryGet<string>("email", out _));
    }

    [Fact]
    public void TestTryGetWithNullOrWhiteSpaceFieldPathThrowsArgumentException()
    {
        var data = new Dictionary<string, object> { ["email"] = "test@example.com" };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        Assert.Throws<ArgumentException>(() => response.TryGet<string>(null!, out _));
        Assert.Throws<ArgumentException>(() => response.TryGet<string>("", out _));
        Assert.Throws<ArgumentException>(() => response.TryGet<string>("   ", out _));
    }

    [Fact]
    public void TestGetOrThrowWithExistingFieldReturnsValue()
    {
        var data = new Dictionary<string, object> { ["email"] = "test@example.com" };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        var result = response.GetOrThrow<string>("email");

        Assert.Equal("test@example.com", result);
    }

    [Fact]
    public void TestGetOrThrowWithMissingFieldThrowsInvalidOperationException()
    {
        var data = new Dictionary<string, object> { ["key"] = "value" };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            response.GetOrThrow<string>("email"));

        var formatted = exception.Format();
        Assert.Equal("Field 'email' is missing or null.", formatted["message"]?.ToString());
    }

    [Fact]
    public void TestGetOrThrowWithNullResponseThrowsArgumentNullException()
    {
        IResponse? response = null;

        Assert.Throws<ArgumentNullException>(() => response!.GetOrThrow<string>("email"));
    }

    [Fact]
    public void TestGetRequiredWithNestedFieldReturnsValue()
    {
        var data = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object> { ["email"] = "test@example.com" }
        };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        var result = response.GetRequired<string>("user.email");

        Assert.Equal("test@example.com", result);
    }

    [Fact]
    public void TestSetFieldIfNotExistsSetsFieldWhenNotExists()
    {
        var data = new Dictionary<string, object>();
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        var result = response.SetFieldIfNotExists("email", "test@example.com");

        Assert.Same(response, result);
        Assert.Equal("test@example.com", response.Get<string>("email"));
    }

    [Fact]
    public void TestSetFieldIfNotExistsDoesNotSetFieldWhenExists()
    {
        var data = new Dictionary<string, object> { ["email"] = "existing@example.com" };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        response.SetFieldIfNotExists("email", "new@example.com");

        Assert.Equal("existing@example.com", response.Get<string>("email"));
    }

    [Fact]
    public void TestSetFieldIfNotExistsWithNullResponseThrowsArgumentNullException()
    {
        IResponse? response = null;

        Assert.Throws<ArgumentNullException>(() => response!.SetFieldIfNotExists("email", "value"));
    }

    [Fact]
    public void TestSetFieldIfNotExistsWithNullOrWhiteSpaceFieldPathThrowsArgumentException()
    {
        var data = new Dictionary<string, object>();
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        Assert.Throws<ArgumentException>(() => response.SetFieldIfNotExists(null!, "value"));
        Assert.Throws<ArgumentException>(() => response.SetFieldIfNotExists("", "value"));
        Assert.Throws<ArgumentException>(() => response.SetFieldIfNotExists("   ", "value"));
    }

    [Fact]
    public void TestMergeDataMergesAdditionalData()
    {
        var data = new Dictionary<string, object> { ["key1"] = "value1" };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);
        var additionalData = new Dictionary<string, object>
        {
            ["key2"] = "value2",
            ["key3"] = "value3"
        };

        var result = response.MergeData(additionalData);

        Assert.Same(response, result);
        Assert.Equal("value1", response.Get<string>("key1"));
        Assert.Equal("value2", response.Get<string>("key2"));
        Assert.Equal("value3", response.Get<string>("key3"));
    }

    [Fact]
    public void TestMergeDataOverwritesExistingFields()
    {
        var data = new Dictionary<string, object> { ["key1"] = "original" };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);
        var additionalData = new Dictionary<string, object> { ["key1"] = "updated" };

        response.MergeData(additionalData);

        Assert.Equal("updated", response.Get<string>("key1"));
    }

    [Fact]
    public void TestMergeDataWithNullResponseThrowsArgumentNullException()
    {
        IResponse? response = null;
        var additionalData = new Dictionary<string, object>();

        Assert.Throws<ArgumentNullException>(() => response!.MergeData(additionalData));
    }

    [Fact]
    public void TestMergeDataWithNullAdditionalDataThrowsArgumentNullException()
    {
        var data = new Dictionary<string, object>();
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        Assert.Throws<ArgumentNullException>(() => response.MergeData(null!));
    }

    [Fact]
    public void TestHasFieldReturnsTrueWhenFieldExists()
    {
        var data = new Dictionary<string, object> { ["email"] = "test@example.com" };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        var result = response.HasField("email");

        Assert.True(result);
    }

    [Fact]
    public void TestHasFieldReturnsFalseWhenFieldDoesNotExist()
    {
        var data = new Dictionary<string, object> { ["key"] = "value" };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        var result = response.HasField("email");

        Assert.False(result);
    }

    [Fact]
    public void TestHasFieldWithNestedField()
    {
        var data = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object> { ["email"] = "test@example.com" }
        };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        Assert.True(response.HasField("user.email"));
        Assert.False(response.HasField("user.name"));
        Assert.False(response.HasField("nonexistent.field"));
    }

    [Fact]
    public void TestHasFieldWithNullResponseThrowsArgumentNullException()
    {
        IResponse? response = null;

        Assert.Throws<ArgumentNullException>(() => response!.HasField("email"));
    }

    [Fact]
    public void TestHasFieldWithNullOrWhiteSpaceFieldPathThrowsArgumentException()
    {
        var data = new Dictionary<string, object>();
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        Assert.Throws<ArgumentException>(() => response.HasField(null!));
        Assert.Throws<ArgumentException>(() => response.HasField(""));
        Assert.Throws<ArgumentException>(() => response.HasField("   "));
    }

    [Fact]
    public void TestGetRequiredWithIntegerField()
    {
        var data = new Dictionary<string, object> { ["age"] = 25 };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        var result = response.GetRequired<int>("age");

        Assert.Equal(25, result);
    }

    [Fact]
    public void TestTryGetWithIntegerField()
    {
        var data = new Dictionary<string, object> { ["age"] = 30 };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        var result = response.TryGet<int>("age", out var value);

        Assert.True(result);
        Assert.Equal(30, value);
    }

    [Fact]
    public void TestSetFieldIfNotExistsWithNestedField()
    {
        var data = new Dictionary<string, object>();
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);

        response.SetFieldIfNotExists("user.email", "test@example.com");

        Assert.Equal("test@example.com", response.Get<string>("user.email"));
    }

    [Fact]
    public void TestMergeDataWithNestedFields()
    {
        var data = new Dictionary<string, object>();
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);
        var additionalData = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = "John",
                ["age"] = 30
            }
        };

        response.MergeData(additionalData);

        var user = response.Get<Dictionary<string, object>>("user");
        Assert.NotNull(user);
        Assert.Equal("John", response.Get<string>("user.name"));
    }

    [Fact]
    public void TestMergeDataWithNestedDictionariesMergesRecursively()
    {
        var data = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = "John",
                ["profile"] = new Dictionary<string, object>
                {
                    ["email"] = "john@example.com",
                    ["age"] = 30
                }
            }
        };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);
        var additionalData = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["surname"] = "Doe",
                ["profile"] = new Dictionary<string, object>
                {
                    ["phone"] = "123-456-7890",
                    ["age"] = 31
                }
            }
        };

        response.MergeData(additionalData);

        Assert.Equal("John", response.Get<string>("user.name"));
        Assert.Equal("Doe", response.Get<string>("user.surname"));
        Assert.Equal("john@example.com", response.Get<string>("user.profile.email"));
        Assert.Equal("123-456-7890", response.Get<string>("user.profile.phone"));
        Assert.Equal(31, response.Get<int>("user.profile.age"));
    }

    [Fact]
    public void TestMergeDataWithMultipleLevelNestedDictionaries()
    {
        var data = new Dictionary<string, object>
        {
            ["level1"] = new Dictionary<string, object>
            {
                ["level2"] = new Dictionary<string, object>
                {
                    ["level3"] = new Dictionary<string, object>
                    {
                        ["value"] = "original"
                    }
                }
            }
        };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);
        var additionalData = new Dictionary<string, object>
        {
            ["level1"] = new Dictionary<string, object>
            {
                ["level2"] = new Dictionary<string, object>
                {
                    ["level3"] = new Dictionary<string, object>
                    {
                        ["newValue"] = "added",
                        ["value"] = "updated"
                    },
                    ["level2Value"] = "added"
                },
                ["level1Value"] = "added"
            }
        };

        response.MergeData(additionalData);

        Assert.Equal("updated", response.Get<string>("level1.level2.level3.value"));
        Assert.Equal("added", response.Get<string>("level1.level2.level3.newValue"));
        Assert.Equal("added", response.Get<string>("level1.level2.level2Value"));
        Assert.Equal("added", response.Get<string>("level1.level1Value"));
    }

    [Fact]
    public void TestMergeDataWithNestedDictionaryOverwritesNonDictionaryValue()
    {
        var data = new Dictionary<string, object>
        {
            ["user"] = "string_value"
        };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);
        var additionalData = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = "John",
                ["age"] = 30
            }
        };

        response.MergeData(additionalData);

        Assert.Equal("John", response.Get<string>("user.name"));
        Assert.Equal(30, response.Get<int>("user.age"));
    }

    [Fact]
    public void TestMergeDataWithNestedDictionaryDoesNotOverwriteWhenValueIsNotDictionary()
    {
        var data = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = "John"
            }
        };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);
        var additionalData = new Dictionary<string, object>
        {
            ["user"] = "string_value"
        };

        response.MergeData(additionalData);

        Assert.Equal("string_value", response.Get<string>("user"));
    }

    [Fact]
    public void TestMergeDataWithEmptyNestedDictionary()
    {
        var data = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>
            {
                ["name"] = "John"
            }
        };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);
        var additionalData = new Dictionary<string, object>
        {
            ["user"] = new Dictionary<string, object>()
        };

        response.MergeData(additionalData);

        Assert.Equal("John", response.Get<string>("user.name"));
    }

    [Fact]
    public void TestMergeDataWithComplexNestedStructure()
    {
        var data = new Dictionary<string, object>
        {
            ["company"] = new Dictionary<string, object>
            {
                ["name"] = "Acme Corp",
                ["employees"] = new Dictionary<string, object>
                {
                    ["count"] = 100,
                    ["departments"] = new Dictionary<string, object>
                    {
                        ["engineering"] = new Dictionary<string, object>
                        {
                            ["count"] = 50
                        }
                    }
                }
            }
        };
        var response = CleanArchitectureCore.Response.Response.Create(true, StatusCode.Ok, "Success", data);
        var additionalData = new Dictionary<string, object>
        {
            ["company"] = new Dictionary<string, object>
            {
                ["location"] = "Paris",
                ["employees"] = new Dictionary<string, object>
                {
                    ["count"] = 150,
                    ["departments"] = new Dictionary<string, object>
                    {
                        ["sales"] = new Dictionary<string, object>
                        {
                            ["count"] = 30
                        },
                        ["engineering"] = new Dictionary<string, object>
                        {
                            ["lead"] = "Alice"
                        }
                    }
                }
            }
        };

        response.MergeData(additionalData);

        Assert.Equal("Acme Corp", response.Get<string>("company.name"));
        Assert.Equal("Paris", response.Get<string>("company.location"));
        Assert.Equal(150, response.Get<int>("company.employees.count"));
        Assert.Equal(50, response.Get<int>("company.employees.departments.engineering.count"));
        Assert.Equal("Alice", response.Get<string>("company.employees.departments.engineering.lead"));
        Assert.Equal(30, response.Get<int>("company.employees.departments.sales.count"));
    }
}

