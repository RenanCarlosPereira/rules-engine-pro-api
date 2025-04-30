using RulesEngine.Models;

namespace RulesEnginePro.Core;

public interface IWorkflowRepository
{
    Task CreateWorkflowAsync(Workflow workflow, CancellationToken cancellationToken = default);
    Task<Workflow> GetWorkflowAsync(string workflowName, CancellationToken cancellationToken = default);
    Task UpdateWorkflowAsync(Workflow workflow, CancellationToken cancellationToken = default);
    Task DeleteWorkflowAsync(string workflowName, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Workflow> GetAllWorkflowsAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default);

}