using FluentAssertions;
using MongoDB.Driver;
using Moq;
using RulesEnginePro.Models;
using RulesEnginePro.MongoDb;

namespace RulesEnginePro.Tests.MongoDb;

public class MongoWorkflowRepositoryMockTests
{
    private readonly Mock<IMongoCollection<WorkflowData>> _collectionMock = new();
    private readonly Mock<IMongoDatabase> _databaseMock = new();
    private readonly MongoWorkflowRepository _repository;

    public MongoWorkflowRepositoryMockTests()
    {
        _databaseMock.Setup(d => d.GetCollection<WorkflowData>(It.IsAny<string>(), null))
            .Returns(_collectionMock.Object);

        _repository = new MongoWorkflowRepository(_databaseMock.Object);
    }

    [Fact]
    public async Task CreateWorkflowAsync_ShouldInsertWorkflow()
    {
        // Arrange
        var workflow = new WorkflowData { WorkflowName = "test-flow" };

        // Act
        await _repository.CreateWorkflowAsync(workflow);

        // Assert
        _collectionMock.Verify(c => c.InsertOneAsync(workflow, null, default), Times.Once);
    }

    [Fact]
    public async Task GetWorkflowAsync_ShouldReturnMatchingWorkflow()
    {
        // Arrange
        var workflow = new WorkflowData { WorkflowName = "test-flow" };

        var cursorMock = new Mock<IAsyncCursor<WorkflowData>>();
        cursorMock.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(true)
                  .ReturnsAsync(false);
        cursorMock.Setup(c => c.Current).Returns(new[] { workflow });

        _collectionMock
            .Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<WorkflowData>>(),
                It.IsAny<FindOptions<WorkflowData, WorkflowData>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

        // Act
        var result = await _repository.GetWorkflowAsync("test-flow");

        // Assert
        result.Should().BeEquivalentTo(workflow);
    }

    [Fact]
    public async Task UpdateWorkflowAsync_ShouldCallReplaceOne()
    {
        // Arrange
        var workflow = new WorkflowData { WorkflowName = "flow-update" };

        // Act
        await _repository.UpdateWorkflowAsync(workflow);

        // Assert
        _collectionMock.Verify(c =>
            c.ReplaceOneAsync(
                It.IsAny<FilterDefinition<WorkflowData>>(),
                workflow,
                It.Is<ReplaceOptions>(o => o.IsUpsert == true),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteWorkflowAsync_ShouldCallDeleteOne()
    {
        // Arrange
        var workflowName = "flow-delete";

        // Act
        await _repository.DeleteWorkflowAsync(workflowName);

        // Assert
        _collectionMock.Verify(c =>
            c.DeleteOneAsync(
                It.IsAny<FilterDefinition<WorkflowData>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAllWorkflowsAsync_ShouldReturnMatchingWorkflows()
    {
        // Arrange
        var workflows = new List<WorkflowData>
    {
        new WorkflowData { WorkflowName = "flow-1" },
        new WorkflowData { WorkflowName = "flow-2" }
    };

        var cursorMock = new Mock<IAsyncCursor<WorkflowData>>();
        cursorMock.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(true)
                  .ReturnsAsync(false);
        cursorMock.Setup(x => x.Current).Returns(workflows);

        _collectionMock.Setup(c =>
                c.FindAsync(
                    It.IsAny<FilterDefinition<WorkflowData>>(),
                    It.IsAny<FindOptions<WorkflowData, WorkflowData>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

        // Act
        var result = new List<WorkflowData>();
        await foreach (var w in _repository.GetAllWorkflowsAsync("flow"))
        {
            result.Add(w);
        }

        // Assert
        result.Should().HaveCount(2);
        result[0].WorkflowName.Should().Be("flow-1");
    }
}