using API.DebugDojo.Application.Enums;

namespace API.DebugDojo.Application.Entities;

public sealed class WorkItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OwnerUserId { get; set; }
    public User? OwnerUser { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public WorkItemStatus Status { get; set; } = WorkItemStatus.Todo;
    public WorkItemPriority Priority { get; set; } = WorkItemPriority.Medium;

    public DateTime? DueDateUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;

    // Concurrency (optimistic) para practicar 409 Conflict
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
