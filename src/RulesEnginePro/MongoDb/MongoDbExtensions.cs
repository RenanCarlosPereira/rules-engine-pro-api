using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using RulesEnginePro.Core;
using RulesEnginePro.MongoDb.ClassMaps;
using RulesEnginePro.MongoDb.Serializers;
using System.Text.Json;

namespace RulesEnginePro.MongoDb;

internal static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
        BsonSerializer.RegisterSerializer(new GlobalObjectSerializer());
        BsonSerializer.RegisterSerializer(new DictionaryObjectSerializer());
        BsonSerializer.RegisterSerializer(new JsonElementBsonDocumentSerializer());

        var conventionPack = new ConventionPack
        {
            new IgnoreExtraElementsConvention(true),
            new EnumRepresentationConvention(BsonType.String),
            new IgnoreIfDefaultConvention(true)
        };

        ConventionRegistry.Register("Conventions", conventionPack, _ => true);

        services.ConfigureOptions<MongoDbConfigureOptions>();

        services.AddSingleton(provider =>
        {
            var options = provider.GetRequiredService<IOptions<MongoDbOptions>>().Value;

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
                throw new InvalidOperationException("MongoDb ConnectionString is missing.");
            if (string.IsNullOrWhiteSpace(options.DatabaseName))
                throw new InvalidOperationException("MongoDb DatabaseName is missing.");

            var client = new MongoClient(options.ConnectionString);
            return client.GetDatabase(options.DatabaseName);
        });

        WorkflowClassMap.Register();
        services.AddSingleton<IWorkflowRepository, MongoWorkflowRepository>();

        return services;
    }

    internal static BsonValue JsonElementToBsonValue(this JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var document = new BsonDocument();
                foreach (var property in element.EnumerateObject())
                {
                    document[property.Name] = JsonElementToBsonValue(property.Value);
                }
                return document;

            case JsonValueKind.Array:
                var array = new BsonArray();
                foreach (var item in element.EnumerateArray())
                {
                    array.Add(JsonElementToBsonValue(item));
                }
                return array;

            case JsonValueKind.String:
                return new BsonString(element.GetString());

            case JsonValueKind.Number:
                if (element.TryGetInt32(out var intValue))
                {
                    return new BsonInt32(intValue);
                }
                if (element.TryGetInt64(out var longValue))
                {
                    return new BsonInt64(longValue);
                }
                return new BsonDouble(element.GetDouble());

            case JsonValueKind.True:
            case JsonValueKind.False:
                return new BsonBoolean(element.GetBoolean());

            case JsonValueKind.Null:
                return BsonNull.Value;

            default:
                throw new NotSupportedException($"Unsupported JsonValueKind: {element.ValueKind}");
        }
    }
}