using Mapster;
using TaskManager.Application.Services.Interfaces;
using TaskManager.Contracts.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data.Repositories.Interfaces;

namespace TaskManager.Application.Services;

public class TaskItemService : ITaskItemService
{
    private readonly ITaskItemRepository _taskItemRepository;

    public TaskItemService(ITaskItemRepository taskItemRepository)
    {
        _taskItemRepository = taskItemRepository;
    }

    public async Task<Guid> CreateAsync(CreateTaskItemRequest dto)
    {
        var taskItem = new TaskItem(dto.Title, dto.Description, dto.DueDate);

        var taskValidate = await _taskItemRepository.GetByTitleAsync(taskItem.Title);

        if(taskValidate is not null)
        {
            throw new InvalidOperationException("Já existe uma tarefa com este mesmo título cadastrada.");
        }

        await _taskItemRepository.AddAsync(taskItem);
        return taskItem.Id;
    }

    public async Task<IEnumerable<TaskItemResponse>> GetAllAsync(GetTaskItemsQuery query)
    {
        var taskItems = await _taskItemRepository.GetAllAsync(query.Status, query.DueDate);

        return taskItems.Adapt<IEnumerable<TaskItemResponse>>();
    }

    public async Task<TaskItemResponse> GetByIdAsync(Guid id)
    {
        var taskItem = await _taskItemRepository.GetByIdAsync(id);

        if (taskItem is null)
        {
            throw new KeyNotFoundException("Tarefa não encontrada.");
        }

        return taskItem.Adapt<TaskItemResponse>();
    }

    public async Task UpdateAsync(Guid id, UpdateTaskItemRequest dto)
    {
        var taskItem = await _taskItemRepository.GetByIdAsync(id);

        if (taskItem is null)
        {
            throw new KeyNotFoundException("Tarefa não encontrada.");
        }

        taskItem.Update(dto.Title, dto.Description, dto.DueDate);

        if (taskItem.Status != dto.Status)
        {
            taskItem.ChangeStatus(dto.Status);
        }

        await _taskItemRepository.UpdateAsync(taskItem);
    }

    public async Task DeleteAsync(Guid id)
    {
        var taskItem = await _taskItemRepository.GetByIdAsync(id);

        if (taskItem is null)
        {
            throw new KeyNotFoundException("Tarefa não encontrada.");
        }

        await _taskItemRepository.DeleteAsync(taskItem.Id);
    }
}
