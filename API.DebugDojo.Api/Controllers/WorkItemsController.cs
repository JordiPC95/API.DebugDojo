using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DebugDojo.Application.Contracts;
using API.DebugDojo.Application.Interfaces;

namespace API.DebugDojo.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/workitems")]
public class WorkItemsController : ControllerBase
{
    private readonly IWorkItemService _service;
    public WorkItemsController(IWorkItemService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<PagedResponse<WorkItemResponse>>> List([FromQuery] WorkItemQuery query, CancellationToken ct)
        => Ok(await _service.ListAsync(query, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkItemResponse>> Get(Guid id, CancellationToken ct)
        => Ok(await _service.GetAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<WorkItemResponse>> Create(WorkItemCreateRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<WorkItemResponse>> Update(Guid id, WorkItemUpdateRequest request, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, request, ct));

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<WorkItemResponse>> UpdateStatus(Guid id, WorkItemStatusUpdateRequest request, CancellationToken ct)
        => Ok(await _service.UpdateStatusAsync(id, request, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}
