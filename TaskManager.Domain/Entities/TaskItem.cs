using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime? DueDate { get; private set; }
    public TaskItemStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public TaskItem(Guid id, string title, string? description, DateTime? dueDate, TaskItemStatus status, DateTime createdAt, DateTime updatedAt)
    {
        ValidateTitle(title);
        ValidateDueDate(dueDate);

        Id = id;
        Title = title;
        Description = description;
        DueDate = dueDate;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }


    public void Update(string title, string description, DateTime dueDate)
    {
        if(Status == TaskItemStatus.Completed)
        {
            throw new InvalidOperationException("Não é possível editar uma tarefa já concluída.");
        }

        Title = title;
        Description = description;
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }


    public void ChangeStatus(TaskItemStatus newStatus)
    {
        if(Status == newStatus)
        {
            return;
        }

        if (Status == TaskItemStatus.Completed && newStatus == TaskItemStatus.Pending)
            throw new InvalidOperationException("Uma tarefa concluída não pode voltar para Pendente.");

        if (Status == TaskItemStatus.Pending && newStatus == TaskItemStatus.Completed)
            throw new InvalidOperationException("A tarefa precisa estar Em Progresso antes de ser Concluída.");

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsOverdue()
    {
        if (!DueDate.HasValue)
            return false;

        if (Status == TaskItemStatus.Completed)
            return false;

        return DueDate.Value < DateTime.UtcNow;
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("O título é obrigatório");
        }

        if(title.Length > 200)
        {
            throw new ArgumentException("O título não pode ser maior que 200 caracteres.");
        }
    }

    private static void ValidateDueDate(DateTime? dueDate)
    {
        if(dueDate.HasValue && dueDate.Value.Date < DateTime.UtcNow)
        {
            throw new ArgumentException("A data de vencimento não pode ser menor do que a data atual.");
        }
    }
}
