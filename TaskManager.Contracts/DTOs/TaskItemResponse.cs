namespace TaskManager.Contracts.DTOs;

public record TaskItemResponse(
    Guid Id,
    string Title,
    string? Description,
    DateTime? DueDate,
    string Status,
    bool IsOverdue,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
