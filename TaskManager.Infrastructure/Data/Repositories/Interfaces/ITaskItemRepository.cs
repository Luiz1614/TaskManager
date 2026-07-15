using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Data.Repositories.Interfaces
{
    public interface ITaskItemRepository
    {
        Task AddTaskItemAsync(TaskItem taskItem);
        Task DeleteAsync(Guid Id);
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<TaskItem> GetByIdAsync(Guid Id);
        Task UpdateTaskItemAsync(TaskItem taskItem);
    }
}