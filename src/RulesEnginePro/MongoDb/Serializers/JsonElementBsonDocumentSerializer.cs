using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace RulesEnginePro.MongoDb.Serializers;

public class JsonElementBsonDocumentSerializer : SerializerBase<JsonElement>
{
    private static readonly JsonWriterSettings JsonWriterSettins = new() { OutputMode = JsonOutputMode.RelaxedExtendedJson };

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JsonElement value)
    {
        var bsonValue = value.JsonElementToBsonValue();
        BsonSerializer.Serialize(context.Writer, bsonValue);
    }

    public override JsonElement Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonValue = BsonSerializer.Deserialize<BsonValue>(context.Reader);

        var json = bsonValue.ToJson(JsonWriterSettins);

        var doc = JsonDocument.Parse(json);

        return doc.RootElement;
    }
}