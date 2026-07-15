using TaskManager.Domain.Enums;

namespace TaskManager.Contracts.DTOs;

public record UpdateTaskItemRequest(
    string Title,
    string? Description,
    DateTime? DueDate,
    TaskItemStatus Status
);
