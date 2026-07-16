using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Infrastructure.Data.Repositories.Interfaces
{
    public interface ITaskItemRepository
    {
        Task AddAsync(TaskItem taskItem);
        Task DeleteAsync(Guid Id);
        Task<IEnumerable<TaskItem>> GetAllAsync(TaskItemStatus? status = null, DateTime? dueDate = null);
        Task<TaskItem?> GetByIdAsync(Guid Id);
        Task<TaskItem> GetByTitleAsync(string title);
        Task UpdateAsync(TaskItem taskItem);
    }
}