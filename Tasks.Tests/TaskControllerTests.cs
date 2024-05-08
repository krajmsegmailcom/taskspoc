using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tasks.API.Controllers;
using Tasks.Core.Entities.Enum;
using Tasks.Core.Entities.Interfaces;
using Xunit;

namespace Tasks.Tests
{
    public class TaskControllerTests
    {
        [Fact]
        public async void CreateTask_ReturnsOkResult_WithValidRequest()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>();
            mockTaskService.Setup(service => service.CreateTask(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new Tasks.Core.Entities.Task { /* properties with expected return values */ });

            var controller = new TaskController(mockTaskService.Object);

            var request = new TaskCreateRequest
            {
                TaskName = "Sample Task",
                Tags = new List<string> { "Tag1", "Tag2" },
                DueDate = DateTime.Now.AddDays(7),
                Color = "Red",
                AssignedTo = "User1"
            };

            // Act
            var result = controller.CreateTask(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Tasks.Core.Entities.Task>(okResult.Value);
            Assert.NotNull(returnValue);
            // ... additional assertions for the returned task properties ...
        }

        // ... additional tests for other scenarios like invalid request, server error, etc. ...





        [Fact]
        public async Task UpdateTask_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>();
            var taskId = 1;
            var request = new TaskUpdateRequest
            {
                // Properties with values for the update request
            };

            mockTaskService.Setup(service => service.GetTaskById(taskId)).Returns((Tasks.Core.Entities.Task)null);

            var controller = new TaskController(mockTaskService.Object);

            // Act
            var result = controller.UpdateTask(taskId, request);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        // ... additional tests for other scenarios like server error, etc. ...


        [Fact]
        public void DeleteTask_ReturnsOkResult_WhenTaskIsDeleted()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>();
            var taskId = 1;
            mockTaskService.Setup(service => service.DeleteTask(taskId)).Returns(true);

            var controller = new TaskController(mockTaskService.Object);

            // Act
            var result = controller.DeleteTask(taskId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal($"Task with ID {taskId} deleted successfully", okResult.Value);
        }

        [Fact]
        public void DeleteTask_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>();
            var taskId = 1;
            mockTaskService.Setup(service => service.DeleteTask(taskId)).Returns(false);

            var controller = new TaskController(mockTaskService.Object);

            // Act
            var result = controller.DeleteTask(taskId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Task with ID {taskId} not found", notFoundResult.Value);
        }

        [Fact]
        public void DeleteTask_ReturnsServerError_OnException()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>();
            var taskId = 1;
            mockTaskService.Setup(service => service.DeleteTask(taskId)).Throws(new Exception("Internal server error"));

            var controller = new TaskController(mockTaskService.Object);

            // Act
            var result = controller.DeleteTask(taskId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.StartsWith("An error occurred:", statusCodeResult.Value.ToString());
        }

        [Fact]
        public void AddActivity_ReturnsOkResult_WithValidRequest()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>();
            var taskId = 1;
            var activityDate = DateTime.Now;
            var doneBy = "User1";
            var description = "Completed task";

            var activity = new Core.Entities.Activity
            {
                Id = 1,
                ActivityDate = activityDate,
                DoneBy = doneBy,
                Description = description
            };

            var request = new ActivityRequest
            {
                Id = 1,
                ActivityDate = activityDate,
                DoneBy = doneBy,
                Description = description
            };

            mockTaskService.Setup(service => service.AddActivity(taskId, activityDate, doneBy, description))
.Returns(activity);

            var controller = new TaskController(mockTaskService.Object);

            // Act
            var result = controller.AddActivity(taskId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Tasks.Core.Entities.Activity>(okResult.Value);
            Assert.NotNull(returnValue);
            Assert.Equal(activityDate, returnValue.ActivityDate);
            Assert.Equal(doneBy, returnValue.DoneBy);
            Assert.Equal(description, returnValue.Description);
        }

        [Fact]
        public void AddActivity_ReturnsServerError_OnException()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>();
            var taskId = 1;
            var activityDate = DateTime.Now;
            var doneBy = "User1";
            var description = "Completed task";

            var request = new ActivityRequest
            {
                ActivityDate = activityDate,
                DoneBy = doneBy,
                Description = description
            };

            mockTaskService.Setup(service => service.AddActivity(taskId, activityDate, doneBy, description))
            .Throws(new Exception("Internal server error"));

            var controller = new TaskController(mockTaskService.Object);

            // Act
            var result = controller.AddActivity(taskId, request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.StartsWith("An error occurred:", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task GetTaskActivities_ReturnsOkObjectResult_WithActivities()
        {
            // Arrange
            var taskId = 1; // Example task ID
            var mockTaskService = new Mock<ITaskService>();
            var activities = new List<Core.Entities.Activity>
{
new Core.Entities.Activity { Id = 1,
TaskId = taskId, // Assuming 'taskId' is defined elsewhere in your test
Description = "Complete the initial design",
 },
new Core.Entities.Activity {TaskId = taskId, // Assuming 'taskId' is defined elsewhere in your test
Description = "Complete the design", }
};

            mockTaskService.Setup(service => service.GetTaskActivities(taskId))
            .ReturnsAsync(activities);

            var controller = new TaskController(mockTaskService.Object);

            // Act
            var result = await controller.GetTaskActivities(taskId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedActivities = Assert.IsType<List<Core.Entities.Activity>>(okResult.Value);
            Assert.Equal(activities.Count, returnedActivities.Count);
        }

        [Fact]
        public async Task GetTaskActivities_ReturnsStatusCode500_WhenExceptionThrown()
        {
            // Arrange
            var taskId = 1; // Example task ID
            var mockTaskService = new Mock<ITaskService>();

            mockTaskService.Setup(service => service.GetTaskActivities(taskId))
            .ThrowsAsync(new Exception("Test exception"));

            var controller = new TaskController(mockTaskService.Object);

            // Act & Assert
            var result = await controller.GetTaskActivities(taskId);
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public void GetTaskById_ReturnsNotFoundResult_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = 1; // Example task ID that does not exist
            var mockTaskService = new Mock<ITaskService>();
            mockTaskService.Setup(service => service.GetTaskById(taskId)).Returns((Core.Entities.Task)null);

            var controller = new TaskController(mockTaskService.Object);

            // Act
            var result = controller.GetTaskById(taskId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetTaskById_ReturnsOkObjectResult_WithTask_WhenTaskExists()
        {
            // Arrange
            var taskId = 1; // Example task ID that exists
            var mockTaskService = new Mock<ITaskService>();
            var task = new Core.Entities.Task { Id = taskId, TaskName = "Test Task", /* other properties */ };
            mockTaskService.Setup(service => service.GetTaskById(taskId)).Returns(task);

            var controller = new TaskController(mockTaskService.Object);

            // Act
            var result = controller.GetTaskById(taskId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTask = Assert.IsType<Core.Entities.Task>(okResult.Value);
            Assert.Equal(taskId, returnedTask.Id);
        }

        [Fact]
        public void GetTaskById_ReturnsStatusCode500_WhenExceptionIsThrown()
        {
            // Arrange
            var taskId = 1; // Example task ID
            var mockTaskService = new Mock<ITaskService>();
            mockTaskService.Setup(service => service.GetTaskById(taskId)).Throws(new Exception("Test exception"));

            var controller = new TaskController(mockTaskService.Object);

            // Act
            var result = controller.GetTaskById(taskId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

    }
}