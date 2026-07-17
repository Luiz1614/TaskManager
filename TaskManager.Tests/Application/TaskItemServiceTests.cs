using Moq;
using TaskManager.Application.Services;
using TaskManager.Contracts.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data.Repositories.Interfaces;

namespace TaskManager.Tests.Application;

public class TaskItemServiceTests
{
    private readonly Mock<ITaskItemRepository> _repositoryMock;
    private readonly TaskItemService _service;

    public TaskItemServiceTests()
    {
        _repositoryMock = new Mock<ITaskItemRepository>();
        _service = new TaskItemService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateTitle_ThrowsInvalidOperationException()
    {
        var request = new CreateTaskItemRequest("Tarefa duplicada", null, null);
        var existingTask = new TaskItem("Tarefa duplicada", null, null);

        _repositoryMock
            .Setup(r => r.GetByTitleAsync(request.Title))
            .ReturnsAsync(existingTask);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAsync(request));

        Assert.Equal("Já existe uma tarefa com este mesmo título cadastrada.", exception.Message);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ReturnsGeneratedId()
    {
        var request = new CreateTaskItemRequest("Tarefa nova", "Descrição", null);

        _repositoryMock
            .Setup(r => r.GetByTitleAsync(request.Title))
            .ReturnsAsync((TaskItem)null!);

        var id = await _service.CreateAsync(request);

        Assert.NotEqual(Guid.Empty, id);
        _repositoryMock.Verify(
            r => r.AddAsync(It.Is<TaskItem>(t => t.Id == id && t.Title == request.Title)),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskDoesNotExist_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((TaskItem?)null);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.GetByIdAsync(id));

        Assert.Equal("Tarefa não encontrada.", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_WhenTaskDoesNotExist_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((TaskItem?)null);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.DeleteAsync(id));

        Assert.Equal("Tarefa não encontrada.", exception.Message);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenTaskDoesNotExist_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        var request = new UpdateTaskItemRequest("Título atualizado", null, null, default);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((TaskItem?)null);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.UpdateAsync(id, request));

        Assert.Equal("Tarefa não encontrada.", exception.Message);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
    }
}
