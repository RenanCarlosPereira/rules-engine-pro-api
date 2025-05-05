using FluentAssertions;
using Mongo2Go;
using MongoDB.Driver;
using RulesEnginePro.Models;
using RulesEnginePro.MongoDb;
using RulesEnginePro.MongoDb.ClassMaps;

namespace RulesEnginePro.Tests.MongoDb;

public class MongoWorkflowRepositoryTests : IAsyncLifetime
{
    private readonly MongoDbRunner _runner;
    private readonly IMongoDatabase _database;
    private readonly MongoWorkflowRepository _repository;

    public MongoWorkflowRepositoryTests()
    {
        WorkflowClassMap.Register();
        _runner = MongoDbRunner.Start(singleNodeReplSet: true);
        var client = new MongoClient(_runner.ConnectionString);
        _database = client.GetDatabase("test-db");
        _repository = new MongoWorkflowRepository(_database);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _runner?.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CreateAndGetWorkflowAsync_ShouldPersistAndReturnWorkflow()
    {
        // Arrange
        var workflow = new WorkflowData { WorkflowName = "test-flow" };

        // Act
        await _repository.CreateWorkflowAsync(workflow);
        var result = await _repository.GetWorkflowAsync("test-flow");

        // Assert
        result.Should().NotBeNull();
        result.WorkflowName.Should().Be("test-flow");
    }

    [Fact]
    public async Task UpdateWorkflowAsync_ShouldReplaceExistingWorkflow()
    {
        // Arrange
        var workflow = new WorkflowData { WorkflowName = "replace-me" };
        await _repository.CreateWorkflowAsync(workflow);

        var updated = new WorkflowData { WorkflowName = "replace-me", Rules = [new() { RuleName = "new-rule" }] };

        // Act
        await _repository.UpdateWorkflowAsync(updated);
        var result = await _repository.GetWorkflowAsync("replace-me");

        // Assert
        result.Should().NotBeNull();
        result.Rules.Should().ContainSingle(r => r.RuleName == "new-rule");
    }

    [Fact]
    public async Task DeleteWorkflowAsync_ShouldRemoveWorkflow()
    {
        // Arrange
        var workflow = new WorkflowData { WorkflowName = "delete-me" };
        await _repository.CreateWorkflowAsync(workflow);

        // Act
        await _repository.DeleteWorkflowAsync("delete-me");
        var result = await _repository.GetWorkflowAsync("delete-me");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllWorkflowsAsync_ShouldReturnFilteredResults()
    {
        // Arrange
        await _repository.CreateWorkflowAsync(new WorkflowData { WorkflowName = "flow1" });
        await _repository.CreateWorkflowAsync(new WorkflowData { WorkflowName = "flow2" });

        // Act
        var results = new List<WorkflowData>();
        await foreach (var w in _repository.GetAllWorkflowsAsync("flow"))
        {
            results.Add(w);
        }

        // Assert
        results.Should().HaveCount(2);
        results.Should().OnlyContain(w => w.WorkflowName.Contains("flow"));
    }

    [Fact]
    public async Task GetAllWorkflowsAsync_ShouldSupportPagination()
    {
        // Arrange
        for (int i = 0; i < 10; i++)
        {
            await _repository.CreateWorkflowAsync(new WorkflowData { WorkflowName = $"paginated-{i}" });
        }

        // Act
        var paged = new List<WorkflowData>();
        await foreach (var w in _repository.GetAllWorkflowsAsync("paginated", skip: 5, take: 3))
        {
            paged.Add(w);
        }

        // Assert
        paged.Should().HaveCount(3);
        paged[0].WorkflowName.Should().Contain("paginated");
    }
}