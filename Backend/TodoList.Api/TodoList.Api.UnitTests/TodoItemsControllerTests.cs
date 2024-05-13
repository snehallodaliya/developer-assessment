using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoList.Api.Controllers;
using Xunit;

namespace TodoList.Api.UnitTests;

using System.Threading.Tasks;
using TodoList.Api.Service;

public class TodoItemsControllerTests
{
    private readonly Mock<IToDoItemsService> _serviceMock;
    private readonly Mock<ILogger<TodoItemsController>> _loggerMock;

    public TodoItemsControllerTests()
    {
        _serviceMock = new Mock<IToDoItemsService>();
        _loggerMock = new Mock<ILogger<TodoItemsController>>();
    }

    [Fact]
    public async Task GetTodoItems_ReturnsOk()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetTodoItems()).ReturnsAsync(new List<TodoItem>());
        var controller = new TodoItemsController(_serviceMock.Object, _loggerMock.Object);

        // Act
        var result = await controller.GetTodoItems();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetTodoItem_ReturnsNotFound_WhenItemDoesNotExist()
    {
        // Arrange
        Guid testId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetTodoItem(testId)).Throws(new ToDoItemException("Not Found"));
        var controller = new TodoItemsController(_serviceMock.Object, _loggerMock.Object);

        // Act
        var result = await controller.GetTodoItem(testId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetTodoItem_ReturnsOk_WhenItemExists()
    {
        // Arrange
        Guid testId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetTodoItem(testId)).ReturnsAsync(new TodoItem { Id = testId, Description = "Test" });
        var controller = new TodoItemsController(_serviceMock.Object, _loggerMock.Object);

        // Act
        var result = await controller.GetTodoItem(testId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<TodoItem>(okResult.Value);
        Assert.Equal(testId, returnValue.Id);
    }

    [Fact]
    public void PutTodoItem_ReturnsBadRequest_WhenIdsDoNotMatch()
    {
        // Arrange
        var testItem = new TodoItem { Id = Guid.NewGuid(), Description = "Test" };
        var controller = new TodoItemsController(_serviceMock.Object, _loggerMock.Object);

        // Act
        var result = controller.PutTodoItem(Guid.NewGuid(), testItem); // different Ids

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void PutTodoItem_ReturnsNotFound_WhenItemDoesNotExist()
    {
        // Arrange
        var testItem = new TodoItem { Id = Guid.NewGuid(), Description = "Test" };
        _serviceMock.Setup(s => s.PutTodoItem(testItem.Id, testItem)).Throws(new ToDoItemException("Not Found"));
        var controller = new TodoItemsController(_serviceMock.Object, _loggerMock.Object);

        // Act
        var result = controller.PutTodoItem(testItem.Id, testItem);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }


    [Fact]
    public void PostTodoItem_ReturnsCreatedAtAction_WhenSuccessful()
    {
        // Arrange
        var description = "Test Description";
        var controller = new TodoItemsController(_serviceMock.Object, _loggerMock.Object);

        // Act
        var result = controller.PostTodoItem(description);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnValue = Assert.IsType<TodoItem>(createdAtActionResult.Value);
        Assert.Equal(description, returnValue.Description);
    }
    
}