using FluentAssertions;
using RulesEnginePro.Core;

namespace RulesEnginePro.Tests.Core
{
    public class RulesEngineProExtensionTests
    {
        [Fact]
        public void GenerateSchema_Should_ReturnValidJsonSchema_ForFlatIdentifiers()
        {
            // Arrange
            var identifiers = new[] { "name", "age", "email" };

            // Act
            var schema = identifiers.GenerateSchema();

            // Assert
            schema["type"]!.ToString().Should().Be("object");
            var properties = schema["properties"]!.AsObject();

            properties.Should().ContainKey("name");
            properties["name"]!["title"]!.ToString().Should().Be("name");
        }

        [Fact]
        public void GenerateSchema_Should_SupportNestedIdentifiers()
        {
            // Arrange
            var identifiers = new[] { "user.name", "user.email", "user.address.city" };

            // Act
            var schema = identifiers.GenerateSchema();

            // Assert
            var user = schema["properties"]!["user"]!.AsObject();
            user["type"]!.ToString().Should().Be("object");

            var userProps = user["properties"]!.AsObject();
            userProps.Should().ContainKey("name");
            userProps["name"]!["type"]!.AsArray().Select(x=>x.ToString()).Should().Contain("null");

            var address = userProps["address"]!.AsObject();
            address["type"]!.ToString().Should().Be("object");

            var addressProps = address["properties"]!.AsObject();
            addressProps.Should().ContainKey("city");
            addressProps["city"]!["type"]!.AsArray().Select(x => x.ToString()).Should().Contain("null");
        }

        [Fact]
        public void GenerateSchema_Should_IgnoreEmptyOrWhitespaceIdentifiers()
        {
            // Arrange
            var identifiers = new[] { "", "   ", "user.name", null };

            // Act
            var schema = identifiers!.GenerateSchema();

            // Assert
            var properties = schema["properties"]!.AsObject();
            properties.Should().ContainKey("user");
        }
    }
}