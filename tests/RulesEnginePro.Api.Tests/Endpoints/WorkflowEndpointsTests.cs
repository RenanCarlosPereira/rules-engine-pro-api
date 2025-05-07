using Moq;
using RulesEnginePro.Models;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace RulesEnginePro.Api.Tests.Endpoints;

[Collection("IntegrationTests")]
public class WorkflowEndpointTests(TestFixture fixture)
{
    private readonly HttpClient _client = fixture.CreateAuthenticatedClient();
    private readonly CustomWebApplicationFactory _factory = fixture.Factory;

    [Fact]
    public async Task CreateWorkflow_ReturnsCreated()
    {
        var workflow = new WorkflowData { WorkflowName = "TestWorkflow" };
        _factory.WorkflowRepositoryMock
            .Setup(r => r.CreateWorkflowAsync(It.IsAny<WorkflowData>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await _client.PostAsJsonAsync("/workflows", workflow);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var returned = await response.Content.ReadFromJsonAsync<WorkflowData>();
        Assert.Equal("TestWorkflow", returned!.WorkflowName);
    }

    [Fact]
    public async Task GetWorkflowByName_ReturnsWorkflow()
    {
        var workflow = new WorkflowData { WorkflowName = "MyWorkflow" };
        _factory.WorkflowRepositoryMock
            .Setup(r => r.GetWorkflowAsync("MyWorkflow", It.IsAny<CancellationToken>()))
            .ReturnsAsync(workflow);

        var response = await _client.GetAsync("/workflows/MyWorkflow");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<WorkflowData>();
        Assert.Equal("MyWorkflow", result!.WorkflowName);
    }

    [Fact]
    public async Task UpdateWorkflow_ReturnsNoContent_WhenSuccessful()
    {
        var workflow = new WorkflowData { WorkflowName = "MyWorkflow" };
        _factory.WorkflowRepositoryMock
            .Setup(r => r.UpdateWorkflowAsync(It.IsAny<WorkflowData>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await _client.PutAsJsonAsync("/workflows/MyWorkflow", workflow);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteWorkflow_ReturnsNoContent()
    {
        _factory.WorkflowRepositoryMock
            .Setup(r => r.DeleteWorkflowAsync("ToDelete", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await _client.DeleteAsync("/workflows/ToDelete");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}