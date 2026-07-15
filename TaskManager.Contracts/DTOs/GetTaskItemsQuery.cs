using TaskManager.Domain.Enums;

namespace TaskManager.Contracts.DTOs;

public record GetTaskItemsQuery(
    TaskItemStatus? Status,
    DateTime? DueDate
);
