namespace TodoList.Api.UnitTests;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using TodoList.Api;
using TodoList.Api.Repository;
using TodoList.Api.Service;
using Xunit;

public class ToDoItemsServiceTests
{
    private readonly Mock<IToDoItemsRepository> _repositoryMock;
    private readonly Mock<ILogger<ToDoItemsService>> _loggerMock;

    public ToDoItemsServiceTests()
    {
        _repositoryMock = new Mock<IToDoItemsRepository>();
        _loggerMock = new Mock<ILogger<ToDoItemsService>>();
    }

    [Fact]
    public async Task GetTodoItems_ShouldCallRepositoryMethod()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetTodoItems()).ReturnsAsync(new List<TodoItem>());

        var service = new ToDoItemsService(_repositoryMock.Object, _loggerMock.Object);

        // Act
        var result = await service.GetTodoItems();

        // Assert
        _repositoryMock.Verify(r => r.GetTodoItems(), Times.Once);
    }

    [Fact]
    public async Task GetTodoItem_ShouldCallRepositoryMethodWithCorrectId()
    {
        // Arrange
        var testId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetTodoItem(testId)).ReturnsAsync(new TodoItem());

        var service = new ToDoItemsService(_repositoryMock.Object, _loggerMock.Object);

        // Act
        var result = await service.GetTodoItem(testId);

        // Assert
        _repositoryMock.Verify(r => r.GetTodoItem(testId), Times.Once);
    }

    [Fact]
    public void PostTodoItem_ShouldThrowException_WhenDescriptionIsEmpty()
    {
        // Arrange
        var service = new ToDoItemsService(_repositoryMock.Object, _loggerMock.Object);

        // Act & Assert
        Assert.Throws<ToDoItemException>(() => service.PostTodoItem(new TodoItem()));
    }

    [Fact]
    public async Task GetTodoItem_ShouldThrowException_WhenItemDoesNotExist()
    {
        // Arrange
        Guid nonExistentId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetTodoItem(nonExistentId)).ThrowsAsync(new ToDoItemException("Item not found"));

        var service = new ToDoItemsService(_repositoryMock.Object, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ToDoItemException>(() => service.GetTodoItem(nonExistentId));
    }

    [Fact]
    public void PutTodoItem_ShouldThrowException_WhenDescriptionIsEmpty()
    {
        // Arrange
        var testItem = new TodoItem { Id = Guid.NewGuid(), Description = "" };
        var service = new ToDoItemsService(_repositoryMock.Object, _loggerMock.Object);

        // Act & Assert
        Assert.Throws<ToDoItemException>(() => service.PutTodoItem(testItem.Id, testItem));
    }

    [Fact]
    public void PutTodoItem_ShouldUpdateItem_WhenDescriptionIsNotEmpty()
    {
        // Arrange
        var testItem = new TodoItem { Id = Guid.NewGuid(), Description = "Test Description" };
        _repositoryMock.Setup(r => r.PutTodoItem(testItem.Id, testItem));

        var service = new ToDoItemsService(_repositoryMock.Object, _loggerMock.Object);

        // Act
        service.PutTodoItem(testItem.Id, testItem);

        // Assert
        _repositoryMock.Verify(r => r.PutTodoItem(testItem.Id, testItem), Times.Once);
    }

    [Fact]
    public void PostTodoItem_ShouldThrowException_WhenDescriptionAlreadyExists()
    {
        // Arrange
        var testItem = new TodoItem { Description = "Existing Description" };
        _repositoryMock.Setup(r => r.TodoItemDescriptionExists(testItem.Description)).Returns(true);

        var service = new ToDoItemsService(_repositoryMock.Object, _loggerMock.Object);

        // Act & Assert
        Assert.Throws<ToDoItemException>(() => service.PostTodoItem(testItem));
    }

    [Fact]
    public void PostTodoItem_ShouldAddItem_WhenDescriptionIsUnique()
    {
        // Arrange
        var testItem = new TodoItem { Description = "Unique Description" };
        _repositoryMock.Setup(r => r.TodoItemDescriptionExists(testItem.Description)).Returns(false);
        _repositoryMock.Setup(r => r.PostTodoItem(testItem));

        var service = new ToDoItemsService(_repositoryMock.Object, _loggerMock.Object);

        // Act
        service.PostTodoItem(testItem);

        // Assert
        _repositoryMock.Verify(r => r.PostTodoItem(testItem), Times.Once);
    }
}