using Microsoft.EntityFrameworkCore;
using API.DebugDojo.Application.Contracts;
using API.DebugDojo.Application.Entities;
using API.DebugDojo.Application.Exceptions;
using API.DebugDojo.Application.Interfaces;

namespace API.DebugDojo.Infrastructure.Services;

public sealed class WorkItemService : IWorkItemService
{
    private readonly Data.AppDbContext _db;
    private readonly ICurrentUserAccessor _current;

    public WorkItemService(Data.AppDbContext db, ICurrentUserAccessor current)
    {
        _db = db;
        _current = current;
    }

    public async Task<PagedResponse<WorkItemResponse>> ListAsync(WorkItemQuery query, CancellationToken ct)
    {
        EnsureAuth();

        var q = _db.WorkItems.AsNoTracking().Where(w => w.OwnerUserId == _current.UserId);

        if (query.Status is not null)
            q = q.Where(w => w.Status == query.Status);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.Trim();
            q = q.Where(w => w.Title.Contains(s) || (w.Description != null && w.Description.Contains(s)));
        }

        q = ApplySorting(q, query.Sort);

        var total = await q.CountAsync(ct);

        var items = await q.Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(w => ToResponse(w))
            .ToListAsync(ct);

        var totalPages = (int)Math.Ceiling(total / (double)query.PageSize);
        return new PagedResponse<WorkItemResponse>(items, query.Page, query.PageSize, total, totalPages);
    }

    public async Task<WorkItemResponse> GetAsync(Guid id, CancellationToken ct)
    {
        EnsureAuth();

        var w = await _db.WorkItems.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id && x.OwnerUserId == _current.UserId, ct);

        if (w is null) throw new NotFoundException("WorkItem no encontrado.");
        return ToResponse(w);
    }

    public async Task<WorkItemResponse> CreateAsync(WorkItemCreateRequest request, CancellationToken ct)
    {
        EnsureAuth();

        var entity = new WorkItem
        {
            OwnerUserId = _current.UserId,
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Priority = request.Priority,
            Status = Application.Enums.WorkItemStatus.Todo,
            DueDateUtc = request.DueDate?.ToUniversalTime(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _db.WorkItems.Add(entity);
        await _db.SaveChangesAsync(ct);

        return await GetAsync(entity.Id, ct);
    }

    public async Task<WorkItemResponse> UpdateAsync(Guid id, WorkItemUpdateRequest request, CancellationToken ct)
    {
        EnsureAuth();

        var entity = await _db.WorkItems.SingleOrDefaultAsync(x => x.Id == id && x.OwnerUserId == _current.UserId, ct);
        if (entity is null) throw new NotFoundException("WorkItem no encontrado.");

        if (request.RowVersion is not null && request.RowVersion.Length > 0)
            _db.Entry(entity).Property(e => e.RowVersion).OriginalValue = request.RowVersion;

        entity.Title = request.Title.Trim();
        entity.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        entity.Priority = request.Priority;
        entity.DueDateUtc = request.DueDate?.ToUniversalTime();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await SaveWithConcurrencyHandling(ct);
        return await GetAsync(entity.Id, ct);
    }

    public async Task<WorkItemResponse> UpdateStatusAsync(Guid id, WorkItemStatusUpdateRequest request, CancellationToken ct)
    {
        EnsureAuth();

        var entity = await _db.WorkItems.SingleOrDefaultAsync(x => x.Id == id && x.OwnerUserId == _current.UserId, ct);
        if (entity is null) throw new NotFoundException("WorkItem no encontrado.");

        if (request.RowVersion is null || request.RowVersion.Length == 0)
            throw new ConflictException("RowVersion requerida.");

        _db.Entry(entity).Property(e => e.RowVersion).OriginalValue = request.RowVersion;

        entity.Status = request.Status;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await SaveWithConcurrencyHandling(ct);
        return await GetAsync(entity.Id, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        EnsureAuth();

        var entity = await _db.WorkItems.SingleOrDefaultAsync(x => x.Id == id && x.OwnerUserId == _current.UserId, ct);
        if (entity is null) throw new NotFoundException("WorkItem no encontrado.");

        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private void EnsureAuth()
    {
        if (!_current.IsAuthenticated || _current.UserId == Guid.Empty)
            throw new UnauthorizedAppException("No autenticado.");
    }

    private static IQueryable<WorkItem> ApplySorting(IQueryable<WorkItem> q, string sort) =>
        sort.ToLowerInvariant() switch
        {
            "createdat_asc" => q.OrderBy(x => x.CreatedAtUtc),
            "createdat_desc" => q.OrderByDescending(x => x.CreatedAtUtc),
            "due_asc" => q.OrderBy(x => x.DueDateUtc),
            "due_desc" => q.OrderByDescending(x => x.DueDateUtc),
            "priority_asc" => q.OrderBy(x => x.Priority),
            "priority_desc" => q.OrderByDescending(x => x.Priority),
            _ => q.OrderByDescending(x => x.CreatedAtUtc)
        };

    private static WorkItemResponse ToResponse(WorkItem w) =>
        new(w.Id, w.Title, w.Description, w.Status, w.Priority, w.DueDateUtc, w.CreatedAtUtc, w.UpdatedAtUtc, w.RowVersion);

    private async Task SaveWithConcurrencyHandling(CancellationToken ct)
    {
        try { await _db.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException("Concurrencia detectada (409). Recarga y reintenta.");
        }
    }
}
