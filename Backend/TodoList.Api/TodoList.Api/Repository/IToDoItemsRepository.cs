namespace TodoList.Api.Repository;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IToDoItemsRepository
{
    Task<List<TodoItem>> GetTodoItems();
    Task<TodoItem> GetTodoItem(Guid id);
    void PutTodoItem(Guid id, TodoItem todoItem);
    void PostTodoItem(TodoItem todoItem);
    bool TodoItemDescriptionExists(string description);
}