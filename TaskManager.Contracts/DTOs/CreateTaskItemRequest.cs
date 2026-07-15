namespace TaskManager.Contracts.DTOs;

public record CreateTaskItemRequest(
    string Title,
    string? Description,
    DateTime? DueDate
);