using API.DebugDojo.Application.Contracts;
using API.DebugDojo.Application.Enums;
using API.DebugDojo.Application.Interfaces;
using API.DebugDojo.Infrastructure.Data;
using API.DebugDojo.Infrastructure.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace API.DebugDojo.Tests;

public sealed class WorkItemServiceTests
{
    private sealed class FakeCurrentUser : ICurrentUserAccessor
    {
        public bool IsAuthenticated { get; init; } = true;
        public Guid UserId { get; init; } = Guid.NewGuid();
        public string? Email { get; init; } = "test@example.com";
    }

    private static AppDbContext CreateDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new AppDbContext(opts);
    }

    [Fact]
    public async Task Create_and_list_should_return_created_item()
    {
        using var db = CreateDb();
        var current = new FakeCurrentUser();
        var service = new WorkItemService(db, current);

        var created = await service.CreateAsync(new WorkItemCreateRequest(
            Title: "Test",
            Description: "Desc",
            Priority: WorkItemPriority.High,
            DueDate: DateTime.UtcNow.AddDays(1)
        ), CancellationToken.None);

        created.Title.Should().Be("Test");

        var page = await service.ListAsync(new WorkItemQuery(
            Status: null,
            Search: "Te",
            Page: 1,
            PageSize: 10,
            Sort: "createdAt_desc"
        ), CancellationToken.None);

        page.TotalItems.Should().Be(1);
        page.Items.Single().Id.Should().Be(created.Id);
    }
}
