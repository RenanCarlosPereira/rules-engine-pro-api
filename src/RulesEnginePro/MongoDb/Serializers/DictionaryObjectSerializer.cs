using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace RulesEnginePro.MongoDb.Serializers;

public class DictionaryObjectSerializer : SerializerBase<Dictionary<string, object>>
{
    private readonly JsonElementBsonDocumentSerializer _jsonElementBsonDocumentSerializer = new();

    public override Dictionary<string, object> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var element = _jsonElementBsonDocumentSerializer.Deserialize(context, args);
        return element.Deserialize<Dictionary<string, object>>()!;
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Dictionary<string, object> value)
    {
        var jsonElement = JsonSerializer.SerializeToElement(value);
        _jsonElementBsonDocumentSerializer.Serialize(context, args, jsonElement);
    }
}