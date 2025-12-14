using API.DebugDojo.Application.Contracts;

namespace API.DebugDojo.Application.Interfaces;

public interface IWorkItemService
{
    Task<PagedResponse<WorkItemResponse>> ListAsync(WorkItemQuery query, CancellationToken ct);
    Task<WorkItemResponse> GetAsync(Guid id, CancellationToken ct);
    Task<WorkItemResponse> CreateAsync(WorkItemCreateRequest request, CancellationToken ct);
    Task<WorkItemResponse> UpdateAsync(Guid id, WorkItemUpdateRequest request, CancellationToken ct);
    Task<WorkItemResponse> UpdateStatusAsync(Guid id, WorkItemStatusUpdateRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
