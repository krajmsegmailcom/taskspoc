using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tasks.Core.Entities.Interfaces;
using Tasks.Application.Services;
using Tasks.Infrastructure.Repositories;
using Tasks.Core.Entities;
using Microsoft.VisualBasic;
using System.Drawing;

namespace Tasks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost("add")] // Endpoint for adding a task
        public IActionResult CreateTask([FromBody] TaskCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request body");
            }

            try
            {
                var newTask = _taskService.CreateTask(request.TaskName, request.Tags, request.DueDate, request.Color, request.AssignedTo);
                return Ok(newTask);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpPost("delete/{taskId}")] // Endpoint for deleting a task
        public IActionResult DeleteTask(int taskId)
        {
            try
            {
                bool isDeleted = _taskService.DeleteTask(taskId);
                if (!isDeleted)
                {
                    return NotFound($"Task with ID {taskId} not found");
                }
                return Ok($"Task with ID {taskId} deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }




        [HttpPost("update/{taskId}")]
        public IActionResult UpdateTask(int taskId, [FromBody] TaskUpdateRequest request)
        {
            try
            {
                var existingTask = _taskService.GetTaskById(taskId);
                if (existingTask == null)
                {
                    return NotFound($"Task with ID {taskId} not found");
                }

                existingTask.TaskName = request.TaskName;
                existingTask.Color = request.Color;
                existingTask.AssignedTo = request.AssignedTo;
                var updatedTask = _taskService.UpdateTask(taskId, request.TaskName, request.DueDate, request.Color, request.AssignedTo,request.status);

                return Ok(existingTask);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("add-activity/{taskId}")]
        public IActionResult AddActivity(int taskId, [FromBody] ActivityRequest request)
        {
            try
            {
                var addedActivity = _taskService.AddActivity(taskId, request.ActivityDate, request.DoneBy, request.Description);

                return Ok(addedActivity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("get-activities/{taskId}")]
        public async Task<IActionResult> GetTaskActivities(int taskId)
        {
            try
            {
                var activities = await _taskService.GetTaskActivities(taskId);

                return Ok(activities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }




        [HttpGet("get-all")]
        public IActionResult GetAllTasks()
        {
            try
            {
                var allTasks = _taskService.GetAll();
                return Ok(allTasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPost("search")]
        public async Task<IActionResult> SearchTasks([FromBody] TaskSearchCriteria criteria)
        {
            try
            {
                var tasks = await _taskService.SearchTasks(criteria); // Await the asynchronous operation
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("get-task/{taskId}")]
        public IActionResult GetTaskById(int taskId)
        {
            try
            {
                var task = _taskService.GetTaskById(taskId); // Assuming _taskService is available and has a GetTaskById method

                if (task == null)
                {
                    return NotFound($"Task with ID {taskId} not found.");
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("validate-user")]
        public IActionResult ValidateUser(string username, string password)
        {
            try
            {
                bool isValidUser = _taskService.Validuser(username, password);

                if (!isValidUser)
                {
                    return Unauthorized("Invalid username or password.");
                }

                return Ok("User is valid.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }





    public class TaskUpdateRequest
    {
        public string TaskName { get; set; }
        public List<string> Tags { get; set; } // Assuming Tag is a string
        public DateTime DueDate { get; set; }
        public string Color { get; set; }
        public string AssignedTo { get; set; }
        public string status { get; set; }
    }

    public class DateRangeRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}