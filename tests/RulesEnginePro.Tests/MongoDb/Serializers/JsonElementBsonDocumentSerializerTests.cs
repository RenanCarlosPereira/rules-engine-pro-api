using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RulesEnginePro.MongoDb.Serializers;
using System.Text.Json;

namespace RulesEnginePro.Tests.MongoDb.Serializers;

public class JsonElementBsonDocumentSerializerTests
{
    private readonly JsonElementBsonDocumentSerializer _serializer = new();

    public JsonElementBsonDocumentSerializerTests()
    {
        // Register the custom serializes for demonstration. In a real application, be cautious with global registrations
        // to avoid side effects. This is for unit testing purposes only.
        if (BsonSerializer.SerializerRegistry.GetSerializer<JsonElement>() == null)
        {
            BsonSerializer.RegisterSerializer(new JsonElementBsonDocumentSerializer());
        }
    }

    [Theory]
    [InlineData("{\"string\":\"text\"}", "string", "text")]
    [InlineData("{\"integer\":123}", "integer", 123)]
    [InlineData("{\"long\":9223372036854775807}", "long", 9223372036854775807L)]
    [InlineData("{\"boolean\":true}", "boolean", true)]
    [InlineData("{\"null\":null}", "null", null)]
    public void Serialize_JsonElement_ShouldCreateEquivalentBsonDocument(string json, string key, object? expectedValue)
    {
        // Arrange
        var element = JsonDocument.Parse(json).RootElement;
        var memoryStream = new MemoryStream();

        // Act
        var context = BsonSerializationContext.CreateRoot(new BsonBinaryWriter(memoryStream));
        _serializer.Serialize(context, element);

        // Reset stream position to read
        memoryStream.Position = 0;
        var bsonDocument = BsonSerializer.Deserialize<BsonDocument>(memoryStream);

        // Assert
        bsonDocument.Contains(key).Should().BeTrue();
        var actualValue = bsonDocument[key];
        if (expectedValue == null)
        {
            actualValue.Should().Be(BsonNull.Value);
        }
        else
        {
            // Comparing as strings to handle different BSON types transparently
            actualValue.ToString()?.ToLower().Should().Be(expectedValue.ToString()?.ToLower());
        }
    }

    [Theory]
    [InlineData("{\"string\":\"text\"}", "string", "text")]
    [InlineData("{\"integer\":123}", "integer", 123)]
    [InlineData("{\"long\":9223372036854775807}", "long", 9223372036854775807L)]
    [InlineData("{\"boolean\":true}", "boolean", true)]
    [InlineData("{\"null\":null}", "null", null)]
    public void Deserialize_BsonDocument_ShouldCreateEquivalentJsonElement(string json, string key, object? expectedValue)
    {
        // Arrange
        var bsonDocument = BsonDocument.Parse(json);
        var bsonBytes = bsonDocument.ToBson();
        var memoryStream = new MemoryStream(bsonBytes);

        // Act
        var jsonElement = _serializer.Deserialize(BsonDeserializationContext.CreateRoot(new BsonBinaryReader(memoryStream)), default);

        // Assert
        var actualValue = jsonElement.GetProperty(key);
        if (expectedValue == null)
        {
            actualValue.ValueKind.Should().Be(JsonValueKind.Null);
        }
        else
        {
            // Using ToString comparisons to avoid issues with different numeric types (int vs. long)
            actualValue.ToString().Should().Be(expectedValue.ToString());
        }
    }

    [Fact]
    public void Serialize_JsonArray_ShouldCreateEquivalentBsonArray()
    {
        // Arrange
        const string json = "{\"tags\":[\"mongodb\", \"csharp\", \"json\"]}";
        var element = JsonDocument.Parse(json).RootElement;
        var memoryStream = new MemoryStream();

        // Act
        var context = BsonSerializationContext.CreateRoot(new BsonBinaryWriter(memoryStream));
        _serializer.Serialize(context, element);

        // Reset stream position to read
        memoryStream.Position = 0;
        var bsonDocument = BsonSerializer.Deserialize<BsonDocument>(memoryStream);

        // Assert
        bsonDocument.Contains("tags").Should().BeTrue();
        var bsonArray = bsonDocument["tags"].AsBsonArray;
        bsonArray.Count.Should().Be(3);
        bsonArray[0].AsString.Should().Be("mongodb");
        bsonArray[1].AsString.Should().Be("csharp");
        bsonArray[2].AsString.Should().Be("json");
    }

    [Fact]
    public void Deserialize_BsonArray_ShouldCreateEquivalentJsonArray()
    {
        // Arrange
        var bson = new BsonDocument
        {
            { "tags", new BsonArray(new[] {"mongodb", "csharp", "json"}) }
        };

        // Convert BsonDocument to byte array and then to MemoryStream for reading
        var bsonBytes = bson.ToBson();
        var memoryStream = new MemoryStream(bsonBytes);

        // Act
        var reader = new BsonBinaryReader(memoryStream);
        var context = BsonDeserializationContext.CreateRoot(reader);
        var jsonElement = _serializer.Deserialize(context, default);

        // Assert
        var jsonArray = jsonElement.GetProperty("tags");
        jsonArray.GetArrayLength().Should().Be(3);
        jsonArray[0].GetString().Should().Be("mongodb");
        jsonArray[1].GetString().Should().Be("csharp");
        jsonArray[2].GetString().Should().Be("json");
    }

    [Fact]
    public void Serialize_JsonNestedDocument_ShouldCreateEquivalentBsonDocument()
    {
        // Arrange
        const string json = "{\"user\":{\"name\":\"John\", \"age\":30}}";
        var element = JsonDocument.Parse(json).RootElement;
        var memoryStream = new MemoryStream();

        // Act
        var context = BsonSerializationContext.CreateRoot(new BsonBinaryWriter(memoryStream));
        _serializer.Serialize(context, element);

        // Reset stream position to read
        memoryStream.Position = 0;
        var bsonDocument = BsonSerializer.Deserialize<BsonDocument>(memoryStream);

        // Assert
        bsonDocument.Contains("user").Should().BeTrue();
        var userDoc = bsonDocument["user"].AsBsonDocument;
        userDoc["name"].AsString.Should().Be("John");
        userDoc["age"].AsInt32.Should().Be(30);
    }

    [Fact]
    public void Deserialize_BsonNestedDocument_ShouldCreateEquivalentJsonElement()
    {
        // Arrange
        var bson = new BsonDocument
        {
            { "user", new BsonDocument { { "name", "John" }, { "age", 30 } } }
        };

        // Convert BsonDocument to byte array and then to MemoryStream for reading
        var bsonBytes = bson.ToBson();
        var memoryStream = new MemoryStream(bsonBytes);

        // Act
        var reader = new BsonBinaryReader(memoryStream);
        var context = BsonDeserializationContext.CreateRoot(reader);
        var jsonElement = _serializer.Deserialize(context, default);

        // Assert
        var userElement = jsonElement.GetProperty("user");
        userElement.GetProperty("name").GetString().Should().Be("John");
        userElement.GetProperty("age").GetInt32().Should().Be(30);
    }

    [Fact]
    public void Serialize_JsonObjectId_ShouldCreateEquivalentBsonDocument()
    {
        // Arrange
        var objectId = ObjectId.GenerateNewId();
        var json = $"{{\"_id\":\"{objectId}\"}}";
        var element = JsonDocument.Parse(json).RootElement;
        var memoryStream = new MemoryStream();

        // Act
        var context = BsonSerializationContext.CreateRoot(new BsonBinaryWriter(memoryStream));
        _serializer.Serialize(context, element);

        // Reset stream position to read
        memoryStream.Position = 0;
        var bsonDocument = BsonSerializer.Deserialize<BsonDocument>(memoryStream);

        // Assert
        bsonDocument["_id"].AsString.Should().Be(objectId.ToString());
    }
}