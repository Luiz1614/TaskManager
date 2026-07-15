using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data.Context;
using TaskManager.Infrastructure.Data.Repositories.Interfaces;

namespace TaskManager.Infrastructure.Data.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly ApplicationDbContext _context;

    public TaskItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem> GetByIdAsync(Guid Id)
    {
        return await _context.TaskItems.FindAsync(Id);
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await _context.TaskItems.AsNoTracking().ToListAsync();
    }

    public async Task AddTaskItemAsync(TaskItem taskItem)
    {
        await _context.TaskItems.AddAsync(taskItem);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTaskItemAsync(TaskItem taskItem)
    {
        _context.TaskItems.Update(taskItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid Id)
    {
        var taskItem = await _context.TaskItems.FindAsync(Id);

        if (taskItem is not null)
        {
            _context.Remove(taskItem);
            await _context.SaveChangesAsync();
        }
    }
}
