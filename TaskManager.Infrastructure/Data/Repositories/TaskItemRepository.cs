using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
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

    public async Task<TaskItem?> GetByIdAsync(Guid Id)
    {
        return await _context.TaskItems.FindAsync(Id);
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync(TaskItemStatus? status = null, DateTime? dueDate = null)
    {
        var query = _context.TaskItems.AsQueryable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (dueDate.HasValue)
            query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == dueDate.Value.Date);

        return await query.ToListAsync();
    }

    public async Task<TaskItem?> GetByTitleAsync(string title)
    {
        return await _context.TaskItems.SingleOrDefaultAsync(t => t.Title == title);
    }

    public async Task AddAsync(TaskItem taskItem)
    {
        await _context.TaskItems.AddAsync(taskItem);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TaskItem taskItem)
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
