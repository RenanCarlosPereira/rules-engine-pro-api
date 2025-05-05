using FluentAssertions;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RulesEnginePro.MongoDb.Serializers;
using System.Text.Json;

namespace RulesEnginePro.Tests.MongoDb.Serializers;

public class DictionaryObjectSerializerTests
{
    private readonly DictionaryObjectSerializer _serializer = new();

    [Fact]
    public void SerializeAndDeserialize_ShouldPreserveDictionaryContent()
    {
        // Arrange
        var original = new Dictionary<string, object>
        {
            ["name"] = "Alice",
            ["age"] = 30,
            ["isActive"] = true,
            ["nested"] = new Dictionary<string, object>
            {
                ["score"] = 98.6
            }
        };

        using var stream = new MemoryStream();

        using (var writer = new BsonBinaryWriter(stream))
        {
            var serializationContext = BsonSerializationContext.CreateRoot(writer);
            _serializer.Serialize(serializationContext, original);
        }

        stream.Position = 0;

        Dictionary<string, object> result;

        using (var reader = new BsonBinaryReader(stream))
        {
            var deserializationContext = BsonDeserializationContext.CreateRoot(reader);
            result = _serializer.Deserialize(deserializationContext);
        }

        // Assert
        result.Should().NotBeNull();
        result["name"].As<JsonElement>().GetString().Should().Be("Alice");
        result["age"].As<JsonElement>().GetInt32().Should().Be(30);
        result["isActive"].As<JsonElement>().GetBoolean().Should().Be(true);

        result["nested"].Should().BeOfType<JsonElement>();
        var nested = (JsonElement)result["nested"];
        nested.GetProperty("score").GetDouble().Should().Be(98.6);
    }

    [Fact]
    public void SerializeAndDeserialize_EmptyDictionary_ShouldSucceed()
    {
        // Arrange
        var original = new Dictionary<string, object>();

        using var stream = new MemoryStream();

        using (var writer = new BsonBinaryWriter(stream))
        {
            var serializationContext = BsonSerializationContext.CreateRoot(writer);
            _serializer.Serialize(serializationContext, original);
        }

        stream.Position = 0;

        using var reader = new BsonBinaryReader(stream);
        var deserializationContext = BsonDeserializationContext.CreateRoot(reader);

        // Act
        var result = _serializer.Deserialize(deserializationContext);

        // Assert
        result.Should().BeEmpty();
    }
}