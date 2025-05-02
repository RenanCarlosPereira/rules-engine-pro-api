using RulesEngine.Models;
using RulesEnginePro.Models;

namespace RulesEnginePro.Core;

public interface IWorkflowRepository
{
    Task CreateWorkflowAsync(WorkflowData workflow, CancellationToken cancellationToken = default);

    Task<WorkflowData> GetWorkflowAsync(string workflowName, CancellationToken cancellationToken = default);

    Task UpdateWorkflowAsync(WorkflowData workflow, CancellationToken cancellationToken = default);

    Task DeleteWorkflowAsync(string workflowName, CancellationToken cancellationToken = default);

    IAsyncEnumerable<WorkflowData> GetAllWorkflowsAsync(string? workflowName = default, int skip = 0, int take = 50, CancellationToken cancellationToken = default);
}