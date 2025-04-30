using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Text.Json;

namespace RulesEnginePro.MongoDb.Serializers;


public class GlobalObjectSerializer : SerializerBase<object>
{
    private readonly JsonElementBsonDocumentSerializer _jsonElementBsonDocumentSerializer = new();

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        if (value is JsonElement jElement)
            _jsonElementBsonDocumentSerializer.Serialize(context, args, jElement);
        else
            _jsonElementBsonDocumentSerializer.Serialize(context, args, JsonSerializer.SerializeToElement(value));
    }

    public override object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return _jsonElementBsonDocumentSerializer.Deserialize(context, args);
    }
}