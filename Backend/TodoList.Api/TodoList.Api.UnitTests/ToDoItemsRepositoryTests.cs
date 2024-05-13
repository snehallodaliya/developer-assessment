namespace TodoList.Api.UnitTests;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TodoList.Api.Repository;
using Xunit;

public class ToDoItemsRepositoryTests
{
    private readonly Mock<ILogger<ToDoItemsRepository>> _loggerMock;
    private readonly TodoContext _context;

    public ToDoItemsRepositoryTests()
    {
        // Mocking the ILogger
        _loggerMock = new Mock<ILogger<ToDoItemsRepository>>();
        
        // Using InMemory database for testing
        DbContextOptions<TodoContext> options = ((DbContextOptionsBuilder)new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())).Options as DbContextOptions<TodoContext>;
        _context = new TodoContext(options);
    }

    [Fact]
    public async Task GetTodoItem_ShouldReturnItem_WhenItemExists()
    {
        // Arrange
        var todo = new TodoItem { Id = Guid.NewGuid(), Description = "Test", IsCompleted = false };
        _context.TodoItems.Add(todo);
        await _context.SaveChangesAsync();
        
        var repo = new ToDoItemsRepository(_context, _loggerMock.Object);

        // Act
        var result = await repo.GetTodoItem(todo.Id);

        // Assert
        Assert.Equal(todo.Description, result.Description);
    }

    [Fact]
    public async Task GetTodoItem_ShouldThrowException_WhenItemDoesNotExist()
    {
        // Arrange
        var repo = new ToDoItemsRepository(_context, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ToDoItemException>(async () => await repo.GetTodoItem(Guid.NewGuid()));
    }
}
