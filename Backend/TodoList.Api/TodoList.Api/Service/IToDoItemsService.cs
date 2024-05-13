namespace TodoList.Api.Service;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IToDoItemsService
{
    Task<List<TodoItem>> GetTodoItems();
    Task<TodoItem> GetTodoItem(Guid id);
    void PutTodoItem(Guid id, TodoItem todoItem);
    void PostTodoItem(TodoItem todoItem);
}