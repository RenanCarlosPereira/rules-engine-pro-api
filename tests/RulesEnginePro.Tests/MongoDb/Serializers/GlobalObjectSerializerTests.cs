using Xunit;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Text.Json;
using RulesEnginePro.MongoDb.Serializers;

namespace RulesEnginePro.Tests.MongoDb.Serializers;

public class GlobalObjectSerializerTests
{
    private readonly GlobalObjectSerializer _serializer = new();

    public record Person(string Name, int Age);

    [Fact]
    public void SerializeAndDeserialize_ShouldPreserveSimpleObject()
    {
        // Arrange
        var original = new Person("Alice", 30);
        var bsonWriterSettings = new BsonBinaryWriterSettings();
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream, bsonWriterSettings);
        var context = BsonSerializationContext.CreateRoot(writer);

        // Act
        _serializer.Serialize(context, original);

        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        var deserializeContext = BsonDeserializationContext.CreateRoot(reader);
        var deserialized = _serializer.Deserialize(deserializeContext);

        // Assert
        deserialized.Should().BeOfType<JsonElement>();
        var json = ((JsonElement)deserialized).GetRawText();
        var rehydrated = JsonSerializer.Deserialize<Person>(json);
        rehydrated.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void Serialize_ShouldHandleJsonElementInput()
    {
        // Arrange
        var json = JsonSerializer.SerializeToElement(new { Fruit = "Banana", Count = 3 });

        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        var context = BsonSerializationContext.CreateRoot(writer);

        // Act
        _serializer.Serialize(context, json);

        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        var deserializeContext = BsonDeserializationContext.CreateRoot(reader);
        var result = _serializer.Deserialize(deserializeContext);

        // Assert
        result.Should().BeOfType<JsonElement>();
        var resultJson = ((JsonElement)result).GetProperty("Fruit").GetString();
        resultJson.Should().Be("Banana");
    }
}