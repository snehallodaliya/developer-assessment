using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace TodoList.Api.Controllers
{
    using TodoList.Api.Service;

    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly IToDoItemsService _service;
        private readonly ILogger<TodoItemsController> _logger;

        public TodoItemsController(IToDoItemsService service, ILogger<TodoItemsController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<IActionResult> GetTodoItems()
        {
            var results = await _service.GetTodoItems();
            return Ok(results);
        }

        // GET: api/TodoItems/...
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoItem(Guid id)
        {
            TodoItem result;
            try
            {
                 result = await _service.GetTodoItem(id);
            }
            catch (Exception e)
            {
                if (e is ToDoItemException)
                {
                    _logger.LogInformation(e, e.Message);
                    return NotFound();
                }
                _logger.LogError(e, e.Message);
                throw;
            }

            return Ok(result);
        }

        // PUT: api/TodoItems/... 
        [HttpPut("{id}")]
        public IActionResult PutTodoItem(Guid id, [FromBody] TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            try
            {
                _service.PutTodoItem(id, todoItem);
            } 
            catch (Exception e)
            {
                if (e is ToDoItemException)
                {
                    _logger.LogInformation(e, e.Message);
                    return NotFound();
                }
                _logger.LogError(e, e.Message);
                throw;
            }

            return NoContent();
        } 

        // POST: api/TodoItems 
        [HttpPost]
        public IActionResult PostTodoItem([FromBody] string description)
        {
            var todoItem = new TodoItem()
            {
                Id = new Guid(),
                Description = description,
                IsCompleted = false
            };
            try
            {
                _service.PostTodoItem(todoItem);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        } 

    }
}
