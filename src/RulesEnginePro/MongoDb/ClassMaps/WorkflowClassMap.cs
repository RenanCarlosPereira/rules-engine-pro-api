using MongoDB.Bson.Serialization;
using RulesEngine.Models;

namespace RulesEnginePro.MongoDb.ClassMaps;

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
    }
}