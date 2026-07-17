using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Tests.Domain;

public class TaskItemTests
{
    private static DateTime FutureDate => DateTime.UtcNow.AddDays(7);

    private static TaskItem CreateValidTask(DateTime? dueDate = null)
        => new("Estudar testes unitários", "Descrição da tarefa", dueDate);

    [Fact]
    public void Constructor_WithValidTitle_CreatesTask()
    {
        var dueDate = FutureDate;

        var task = new TaskItem("Estudar testes unitários", "Descrição da tarefa", dueDate);

        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.Equal("Estudar testes unitários", task.Title);
        Assert.Equal("Descrição da tarefa", task.Description);
        Assert.Equal(dueDate, task.DueDate);
        Assert.Equal(TaskItemStatus.Pending, task.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithoutTitle_ThrowsArgumentException(string? title)
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new TaskItem(title!, null, null));

        Assert.Equal("O título é obrigatório", exception.Message);
    }

    [Fact]
    public void Constructor_WithTitleOverMaxLength_ThrowsArgumentException()
    {
        var longTitle = new string('a', 201);

        var exception = Assert.Throws<ArgumentException>(
            () => new TaskItem(longTitle, null, null));

        Assert.Equal("O título não pode ser maior que 200 caracteres.", exception.Message);
    }

    [Fact]
    public void Constructor_WithPastDueDate_ThrowsArgumentException()
    {
        var pastDate = DateTime.UtcNow.AddDays(-1);

        var exception = Assert.Throws<ArgumentException>(
            () => new TaskItem("Tarefa válida", null, pastDate));

        Assert.Equal("A data de vencimento não pode ser menor do que a data atual.", exception.Message);
    }

    [Fact]
    public void ChangeStatus_WithValidTransition_ChangesStatus()
    {
        var task = CreateValidTask();

        task.ChangeStatus(TaskItemStatus.InProgress);
        Assert.Equal(TaskItemStatus.InProgress, task.Status);

        task.ChangeStatus(TaskItemStatus.Completed);
        Assert.Equal(TaskItemStatus.Completed, task.Status);
    }

    [Fact]
    public void ChangeStatus_FromPendingToCompleted_ThrowsInvalidOperationException()
    {
        var task = CreateValidTask();

        var exception = Assert.Throws<InvalidOperationException>(
            () => task.ChangeStatus(TaskItemStatus.Completed));

        Assert.Equal("A tarefa precisa estar Em Progresso antes de ser Concluída.", exception.Message);
    }

    [Fact]
    public void ChangeStatus_FromCompletedToPending_ThrowsInvalidOperationException()
    {
        var task = CreateValidTask();
        task.ChangeStatus(TaskItemStatus.InProgress);
        task.ChangeStatus(TaskItemStatus.Completed);

        var exception = Assert.Throws<InvalidOperationException>(
            () => task.ChangeStatus(TaskItemStatus.Pending));

        Assert.Equal("Uma tarefa concluída não pode voltar para Pendente.", exception.Message);
    }

    [Fact]
    public void ChangeStatus_WithSameStatus_ThrowsInvalidOperationException()
    {
        var task = CreateValidTask();

        var exception = Assert.Throws<InvalidOperationException>(
            () => task.ChangeStatus(TaskItemStatus.Pending));

        Assert.Equal("A tarefa já está com este status.", exception.Message);
    }

    [Fact]
    public void Update_WhenCompleted_ThrowsInvalidOperationException()
    {
        var task = CreateValidTask();
        task.ChangeStatus(TaskItemStatus.InProgress);
        task.ChangeStatus(TaskItemStatus.Completed);

        var exception = Assert.Throws<InvalidOperationException>(
            () => task.Update("Novo título", "Nova descrição", null));

        Assert.Equal("Não é possível editar uma tarefa já concluída.", exception.Message);
    }

    [Fact]
    public void IsOverdue_WithPastDueDateAndNotCompleted_ReturnsTrue()
    {
        var task = CreateValidTask(FutureDate);
        task.Update(task.Title, task.Description, DateTime.UtcNow.AddDays(-1));

        Assert.True(task.IsOverdue());
    }

    [Fact]
    public void IsOverdue_WhenCompleted_ReturnsFalse()
    {
        var task = CreateValidTask(FutureDate);
        task.Update(task.Title, task.Description, DateTime.UtcNow.AddDays(-1));
        task.ChangeStatus(TaskItemStatus.InProgress);
        task.ChangeStatus(TaskItemStatus.Completed);

        Assert.False(task.IsOverdue());
    }

    [Fact]
    public void IsOverdue_WithoutDueDate_ReturnsFalse()
    {
        var task = CreateValidTask();

        Assert.False(task.IsOverdue());
    }
}
