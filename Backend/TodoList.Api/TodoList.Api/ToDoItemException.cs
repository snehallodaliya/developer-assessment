namespace TodoList.Api;

using System;

public class ToDoItemException : Exception
{
    public ToDoItemException(string message) : base(message)
    {
    }
}
