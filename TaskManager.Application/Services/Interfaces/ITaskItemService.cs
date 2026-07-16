using TaskManager.Contracts.DTOs;

namespace TaskManager.Application.Services.Interfaces
{
    public interface ITaskItemService
    {
        Task<Guid> CreateAsync(CreateTaskItemRequest dto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<TaskItemResponse>> GetAllAsync(GetTaskItemsQuery query);
        Task<TaskItemResponse> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, UpdateTaskItemRequest dto);
    }
}