using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Services.Interfaces;
using TaskManager.Contracts.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de tarefas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TaskItemController : ControllerBase
{
    private readonly ITaskItemService _taskItemService;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="TaskItemController"/>.
    /// </summary>
    /// <param name="taskItemService">Serviço de tarefas.</param>
    public TaskItemController(ITaskItemService taskItemService)
    {
        _taskItemService = taskItemService;
    }

    /// <summary>
    /// Retorna todas as tarefas, com filtros opcionais por status e data de vencimento.
    /// </summary>
    /// <param name="status">Status da tarefa para filtrar (opcional).</param>
    /// <param name="dueDate">Data de vencimento para filtrar (opcional).</param>
    /// <returns>Lista de tarefas encontradas.</returns>
    /// <response code="200">Retorna a lista de tarefas.</response>
    [HttpGet]
    public async Task<IActionResult> GetAllTaskItems([FromQuery] TaskItemStatus? status, [FromQuery] DateTime? dueDate)
    {
        var query = new GetTaskItemsQuery(status, dueDate);

        var taskItems = await _taskItemService.GetAllAsync(query);

        return Ok(taskItems);
    }

    /// <summary>
    /// Retorna uma tarefa pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador da tarefa.</param>
    /// <returns>A tarefa correspondente ao identificador informado.</returns>
    /// <response code="200">Retorna a tarefa encontrada.</response>
    /// <response code="404">Nenhuma tarefa encontrada com o identificador informado.</response>
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

    /// <summary>
    /// Cria uma nova tarefa.
    /// </summary>
    /// <param name="dto">Dados da tarefa a ser criada.</param>
    /// <returns>Mensagem de confirmação com a tarefa criada.</returns>
    /// <response code="200">Tarefa criada com sucesso.</response>
    [HttpPost]
    public async Task<IActionResult> AddTaskItem(CreateTaskItemRequest dto)
    {
        var taskItem = await _taskItemService.CreateAsync(dto);

        return Ok($"Tarefa Criada com sucesso: {taskItem}");
    }

    /// <summary>
    /// Atualiza uma tarefa existente.
    /// </summary>
    /// <param name="id">Identificador da tarefa a ser atualizada.</param>
    /// <param name="dto">Novos dados da tarefa.</param>
    /// <returns>Nenhum conteúdo.</returns>
    /// <response code="204">Tarefa atualizada com sucesso.</response>
    [HttpPut]
    public async Task<IActionResult> UpdateTaskItem([FromQuery] Guid id, [FromBody] UpdateTaskItemRequest dto)
    {
        await _taskItemService.UpdateAsync(id, dto);

        return NoContent();
    }

    /// <summary>
    /// Exclui uma tarefa.
    /// </summary>
    /// <param name="id">Identificador da tarefa a ser excluída.</param>
    /// <returns>Nenhum conteúdo.</returns>
    /// <response code="204">Tarefa excluída com sucesso.</response>
    [HttpDelete]
    public async Task<IActionResult> DeleteTaskItem(Guid id)
    {
        await _taskItemService.DeleteAsync(id);

        return NoContent();
    }
}
