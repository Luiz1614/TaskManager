using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Services.Interfaces;
using TaskManager.Contracts.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskItemController : ControllerBase
{
    private readonly ITaskItemService _taskItemService;

    public TaskItemController(ITaskItemService taskItemService)
    {
        _taskItemService = taskItemService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTaskItems([FromQuery] TaskItemStatus? status, [FromQuery] DateTime? dueDate)
    {
        var query = new GetTaskItemsQuery(status, dueDate);

        var taskItems = await _taskItemService.GetAllAsync(query);

        return Ok(taskItems);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskItemById(Guid id)
    {
        var taskItem = await _taskItemService.GetByIdAsync(id);

        if(taskItem is null)
        {
            return NotFound("Nenhuma tarefa encontrada.");
        }

        return Ok(taskItem);
    }

    [HttpPost]
    public async Task<IActionResult> AddTaskItem(CreateTaskItemRequest dto)
    {
        var taskItem = await _taskItemService.CreateAsync(dto);

        return Ok($"Tarefa Criada com sucesso: {taskItem}");
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTaskItem([FromQuery] Guid id, [FromBody] UpdateTaskItemRequest dto)
    {
        await _taskItemService.UpdateAsync(id, dto);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTaskItem(Guid id)
    {
        await _taskItemService.DeleteAsync(id);

        return NoContent();
    }
}
