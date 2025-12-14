using API.DebugDojo.Application.Enums;

namespace API.DebugDojo.Application.Contracts;

public sealed record WorkItemCreateRequest(
    string Title,
    string? Description,
    WorkItemPriority Priority,
    DateTime? DueDate);

public sealed record WorkItemUpdateRequest(
    string Title,
    string? Description,
    WorkItemPriority Priority,
    DateTime? DueDate,
    byte[]? RowVersion);

public sealed record WorkItemStatusUpdateRequest(
    WorkItemStatus Status,
    byte[]? RowVersion);

public sealed record WorkItemResponse(
    Guid Id,
    string Title,
    string? Description,
    WorkItemStatus Status,
    WorkItemPriority Priority,
    DateTime? DueDateUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    byte[] RowVersion);

public sealed record WorkItemQuery(
    WorkItemStatus? Status,
    string? Search,
    int Page = 1,
    int PageSize = 20,
    string Sort = "createdAt_desc");

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);
