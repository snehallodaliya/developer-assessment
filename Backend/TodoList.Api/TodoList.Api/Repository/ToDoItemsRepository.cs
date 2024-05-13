namespace TodoList.Api.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class ToDoItemsRepository : IToDoItemsRepository
{
    private readonly TodoContext _context;
    private readonly ILogger<ToDoItemsRepository> _logger;
    
    public ToDoItemsRepository(TodoContext context, ILogger<ToDoItemsRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
        public async Task<List<TodoItem>> GetTodoItems()
        {
            var results = await _context.TodoItems.Where(x => !x.IsCompleted).ToListAsync();
            return results;
        }

        public async Task<TodoItem> GetTodoItem(Guid id)
        {
            var result = await _context.TodoItems.FindAsync(id);

            if (result == null)
            {
                var message = "Item not found";
                _logger.LogInformation(message);
                throw new ToDoItemException(message);
            }

            return result;
        }

        public async void PutTodoItem(Guid id, TodoItem todoItem)
        {
            var existing = await _context.TodoItems.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (existing != null)
            {
                existing.IsCompleted = true;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ToDoItemException("Invalid request");
            }
        } 

        public async void PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            
            await _context.SaveChangesAsync();
        }

        public bool TodoItemDescriptionExists(string description)
        {
            // string comparison instead of ==
            // consider moving IsCompleted elsewhere or rename method for clarity
            return _context.TodoItems
                   .Any(x => String.Equals(x.Description, description, StringComparison.InvariantCultureIgnoreCase) && !x.IsCompleted);
        }
}