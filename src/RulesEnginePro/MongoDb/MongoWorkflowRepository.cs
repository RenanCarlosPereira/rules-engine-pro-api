using MongoDB.Bson;
using MongoDB.Driver;
using RulesEngine.Models;
using RulesEnginePro.Core;
using RulesEnginePro.Models;
using System.Runtime.CompilerServices;

namespace RulesEnginePro.MongoDb;

internal class MongoWorkflowRepository(IMongoDatabase database, string collectionName = "workflows") : IWorkflowRepository
{
    private readonly IMongoCollection<WorkflowData> _collection = database.GetCollection<WorkflowData>(collectionName);

    public async Task CreateWorkflowAsync(WorkflowData workflow, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(workflow, cancellationToken: cancellationToken);
    }

    public async Task<WorkflowData> GetWorkflowAsync(string workflowName, CancellationToken cancellationToken = default)
    {
        var filter = Builders<WorkflowData>.Filter.Eq(w => w.WorkflowName, workflowName);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateWorkflowAsync(WorkflowData workflow, CancellationToken cancellationToken = default)
    {
        var filter = Builders<WorkflowData>.Filter.Eq(w => w.WorkflowName, workflow.WorkflowName);
        await _collection.ReplaceOneAsync(filter, workflow, new ReplaceOptions { IsUpsert = true }, cancellationToken);
    }

    public async Task DeleteWorkflowAsync(string workflowName, CancellationToken cancellationToken = default)
    {
        var filter = Builders<WorkflowData>.Filter.Eq(w => w.WorkflowName, workflowName);
        await _collection.DeleteOneAsync(filter, cancellationToken);
    }

    public async IAsyncEnumerable<WorkflowData> GetAllWorkflowsAsync(string? workflowName = default, int skip = 0, int take = 50, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Build the filter
        var filter = string.IsNullOrWhiteSpace(workflowName) ? FilterDefinition<WorkflowData>.Empty
            : Builders<WorkflowData>.Filter.Regex(x => x.WorkflowName, new BsonRegularExpression(workflowName, "i"));

        var options = new FindOptions<WorkflowData>
        {
            Skip = skip,
            Limit = take
        };

        using var cursor = await _collection.FindAsync(filter, options, cancellationToken);

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var workflow in cursor.Current)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return workflow;
            }
        }
    }
}