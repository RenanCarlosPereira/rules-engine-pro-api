using FluentAssertions;
using Moq;
using RulesEnginePro.Actions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace RulesEnginePro.Api.Tests.Endpoints;

[Collection("IntegrationTests")]
public class ActionEndpointsTests
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ActionEndpointsTests(TestFixture fixture)
    {
        _factory = fixture.Factory;

        _factory.ContextActionServiceMock
        .Setup(s => s.GetContextActions(It.IsAny<CancellationToken>()))
            .Returns(SampleContextActions);

        _client = fixture.CreateAuthenticatedClient();
    }

    [Fact]
    public async Task GetContextDefinitions_ReturnsExpectedResult()
    {
        var response = await _client.GetAsync("/action/context-definitions");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        var array = result.Deserialize<List<JsonElement>>();
        Assert.NotNull(array);
        array[0].GetProperty("key").GetString().Should().Be("SampleAction");
    }

    private static async IAsyncEnumerable<KeyValuePair<string, Dictionary<string, ContextActionBase.FieldDefinition>>> SampleContextActions()
    {
        yield return new KeyValuePair<string, Dictionary<string, ContextActionBase.FieldDefinition>>("SampleAction",
            new Dictionary<string, ContextActionBase.FieldDefinition>()
            {
                { "name", new ContextActionBase.FieldDefinition { Type = "string", Label = "test" } }
            });
    }
}