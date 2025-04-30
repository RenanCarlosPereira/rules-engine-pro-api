using MongoDB.Driver;
using RulesEngine.Models;
using RulesEnginePro.Core;
using System.Runtime.CompilerServices;

namespace RulesEnginePro.MongoDb;

internal class MongoWorkflowRepository(IMongoDatabase database, string collectionName = "workflows") : IWorkflowRepository
{
    private readonly IMongoCollection<Workflow> _collection = database.GetCollection<Workflow>(collectionName);

    public async Task CreateWorkflowAsync(Workflow workflow, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(workflow, cancellationToken: cancellationToken);
    }

    public async Task<Workflow> GetWorkflowAsync(string workflowName, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Workflow>.Filter.Eq(w => w.WorkflowName, workflowName);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateWorkflowAsync(Workflow workflow, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Workflow>.Filter.Eq(w => w.WorkflowName, workflow.WorkflowName);
        await _collection.ReplaceOneAsync(filter, workflow, new ReplaceOptions { IsUpsert = true }, cancellationToken);
    }

    public async Task DeleteWorkflowAsync(string workflowName, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Workflow>.Filter.Eq(w => w.WorkflowName, workflowName);
        await _collection.DeleteOneAsync(filter, cancellationToken);
    }

    public async IAsyncEnumerable<Workflow> GetAllWorkflowsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var cursor = await _collection.FindAsync(Builders<Workflow>.Filter.Empty, cancellationToken: cancellationToken);

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var workflow in cursor.Current)
            {
                yield return workflow;
            }
        }
    }
}
