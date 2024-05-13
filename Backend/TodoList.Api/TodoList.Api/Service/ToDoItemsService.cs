namespace TodoList.Api.Service;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TodoList.Api.Repository;

public class ToDoItemsService : IToDoItemsService
{
    private readonly IToDoItemsRepository _repository;
    private readonly ILogger<ToDoItemsService> _logger;
    
    public ToDoItemsService(IToDoItemsRepository repository, ILogger<ToDoItemsService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
        public async Task<List<TodoItem>> GetTodoItems()
        {
            return await _repository.GetTodoItems();
        }

        public async Task<TodoItem> GetTodoItem(Guid id)
        {
            return await _repository.GetTodoItem(id);
        }

        public void PutTodoItem(Guid id, TodoItem todoItem)
        {
            if (string.IsNullOrEmpty(todoItem.Description))
            {
                var message = StringResources.Description_is_required;
                _logger.LogInformation(message);
                throw new ToDoItemException(message);
            }
            
            _repository.PutTodoItem(id, todoItem);
        } 

        public void PostTodoItem(TodoItem todoItem)
        {
            ValidateItem(todoItem); 

            _repository.PostTodoItem(todoItem);
        }
        
        private void ValidateItem(TodoItem todoItem)
        {
            if (string.IsNullOrEmpty(todoItem.Description))
            {
                var message = StringResources.Description_is_required;
                _logger.LogInformation(message);
                throw new ToDoItemException(message);
            }

            if (_repository.TodoItemDescriptionExists(todoItem.Description))
            {
                var message = StringResources.Description_already_exists;
                _logger.LogInformation(message);
                throw new ToDoItemException(message);
            }
        }
}