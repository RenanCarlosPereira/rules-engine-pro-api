using MongoDB.Bson.Serialization;
using RulesEngine.Models;
using RulesEnginePro.Models;
using System.Diagnostics.CodeAnalysis;

namespace RulesEnginePro.MongoDb.ClassMaps;

[ExcludeFromCodeCoverage]
public static class WorkflowClassMap
{
    public static void Register()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Workflow)))
        {
            BsonClassMap.RegisterClassMap<Workflow>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(x => x.WorkflowName);
                cm.SetIgnoreExtraElements(true);
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(WorkflowData)))
        {
            BsonClassMap.RegisterClassMap<WorkflowData>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }
    }
}