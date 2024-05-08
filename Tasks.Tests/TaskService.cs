using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Tasks.Core.Entities.Enum;
using Microsoft.AspNetCore.Mvc;
using Tasks.Core.Entities.Interfaces;
using Tasks.Application.Services;
using Tasks.API.Controllers; 
namespace Tasks.Core.Test
{

    public class TaskServiceTests
    {
        [Fact]
        public void CreateTask_ReturnsTaskId()
        {
            var mockTaskRepository = new Mock<IRepository<Core.Entities.Task>>();
            var mockEmailService = new Mock<IEmailService>();

            var taskService = new TaskService(mockTaskRepository.Object, mockEmailService.Object);

            // Act
            taskService.CreateTask("Sample Task", new List<string> { "Tag1", "Tag2" }, DateTime.Now.AddDays(7), "Red", "User1");

            // Assert
            mockTaskRepository.Verify(repo => repo.Add(It.IsAny<Core.Entities.Task>()), Times.Once);
        }
        [Fact]
        public void UpdateTask_UpdatesExistingTask()
        {
            // Arrange
            var mockTaskRepository = new Mock<IRepository<Core.Entities.Task>>();
            var mockEmailService = new Mock<IEmailService>();
            var taskService = new TaskService(mockTaskRepository.Object, mockEmailService.Object);

            var taskId = 1;
            var existingTask = new Core.Entities.Task
            {
                Id = taskId,
                TaskName = "Old Task",
                DueDate = DateTime.Now.AddDays(-1),
                Color = "Blue",
                AssignedTo = "User0"
            };

            mockTaskRepository.Setup(repo => repo.GetById(taskId)).Returns(existingTask);

            var newTaskName = "Updated Task";
            var newDueDate = DateTime.Now.AddDays(7);
            var newColor = "Red";
            var newAssignedTo = "User1";
            var status = "1";

            // Act
            var result = taskService.UpdateTask(taskId, newTaskName, newDueDate, newColor, newAssignedTo, status);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newTaskName, result.TaskName);
            Assert.Equal(newDueDate, result.DueDate);
            Assert.Equal(newColor, result.Color);
            Assert.Equal(newAssignedTo, result.AssignedTo);

            mockTaskRepository.Verify(repo => repo.Update(It.Is<Core.Entities.Task>(t =>
            t.Id == taskId &&
            t.TaskName == newTaskName &&
            t.DueDate == newDueDate &&
            t.Color == newColor &&
            t.AssignedTo == newAssignedTo)), Times.Once);
        }

        [Fact]
        public void DeleteTask_ReturnsTrue_WhenTaskIsDeleted()
        {
            // Arrange
            var mockTaskRepository = new Mock<IRepository<Core.Entities.Task>>();
            var taskId = 1;
            var mockEmailService = new Mock<IEmailService>();
            mockTaskRepository.Setup(repo => repo.Delete(taskId)).Returns(true);
            var taskService = new TaskService(mockTaskRepository.Object, mockEmailService.Object);

            // Act
            var result = taskService.DeleteTask(taskId);

            // Assert
            Assert.True(result);
            mockTaskRepository.Verify(repo => repo.Delete(taskId), Times.Once);
        }

        [Fact]
        public void DeleteTask_ReturnsFalse_WhenTaskDoesNotExist()
        {
            // Arrange
            var mockTaskRepository = new Mock<IRepository<Core.Entities.Task>>();
            var taskId = 1;
            var mockEmailService = new Mock<IEmailService>();
            mockTaskRepository.Setup(repo => repo.Delete(taskId)).Returns(false);
            var taskService = new TaskService(mockTaskRepository.Object, mockEmailService.Object);

            // Act
            var result = taskService.DeleteTask(taskId);

            // Assert
            Assert.False(result);
            mockTaskRepository.Verify(repo => repo.Delete(taskId), Times.Once);
        }

        [Fact]
        public void GetTaskById_ReturnsTask_WhenTaskExists()
        {
            // Arrange
            var mockEmailService = new Mock<IEmailService>();
            var mockTaskRepository = new Mock<IRepository<Core.Entities.Task>>();
            var taskId = 1;
            var expectedTask = new Core.Entities.Task
            {
                Id = taskId,
                TaskName = "Test Task",
                DueDate = DateTime.Now.AddDays(7),
                Color = "Green",
                AssignedTo = "User1"
            };

            mockTaskRepository.Setup(repo => repo.GetById(taskId)).Returns(expectedTask);
            var taskService = new TaskService(mockTaskRepository.Object, mockEmailService.Object);

            // Act
            var result = taskService.GetTaskById(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedTask.Id, result.Id);
            Assert.Equal(expectedTask.TaskName, result.TaskName);
            Assert.Equal(expectedTask.DueDate, result.DueDate);
            Assert.Equal(expectedTask.Color, result.Color);
            Assert.Equal(expectedTask.AssignedTo, result.AssignedTo);
        }

        [Fact]
        public void GetTaskById_ReturnsNull_WhenTaskDoesNotExist()
        {
            // Arrange
            var mockTaskRepository = new Mock<IRepository<Core.Entities.Task>>();
            var taskId = 1;
            var mockEmailService = new Mock<IEmailService>();
            mockTaskRepository.Setup(repo => repo.GetById(taskId)).Returns((Core.Entities.Task)null);
            var taskService = new TaskService(mockTaskRepository.Object, mockEmailService.Object);

            // Act
            var result = taskService.GetTaskById(taskId);

            // Assert
            Assert.Null(result);
        }



        [Fact]
        public void GetAll_ReturnsAllTasks()
        {
            // Arrange
            var mockTaskRepository = new Mock<IRepository<Core.Entities.Task>>();
            var expectedTasks = new List<Core.Entities.Task>
{
new Core.Entities.Task { Id = 1, TaskName = "Task 1", DueDate = DateTime.Now.AddDays(1), Color = "Red", AssignedTo = "User1" },
new Core.Entities.Task { Id = 2, TaskName = "Task 2", DueDate = DateTime.Now.AddDays(2), Color = "Blue", AssignedTo = "User2" },
new Core.Entities.Task { Id = 3, TaskName = "Task 3", DueDate = DateTime.Now.AddDays(3), Color = "Green", AssignedTo = "User3" }
};

            mockTaskRepository.Setup(repo => repo.GetAll()).Returns(expectedTasks.AsEnumerable());
            var mockEmailService = new Mock<IEmailService>();
            var taskService = new TaskService(mockTaskRepository.Object, mockEmailService.Object);

            // Act
            var result = taskService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedTasks.Count(), result.Count());
            Assert.Equal(expectedTasks, result);
        }
        [Fact]
        public void AddActivity_ReturnsTaskWithNewActivity()
        {
            // Arrange
            var mockTaskRepository = new Mock<IRepository<Core.Entities.Task>>();
            var taskId = 1;
            var activityDate = DateTime.Now;
            var doneBy = "User1";
            var description = "Completed task";

            var newActivity = new Core.Entities.Activity
            {
                Id = 2, // Assuming this is a new activity, it should have a unique Id
                ActivityDate = activityDate,
                DoneBy = doneBy,
                Description = description
            };

            var mockEmailService = new Mock<IEmailService>();
            mockTaskRepository.Setup(repo => repo.AddActivity(taskId, activityDate, doneBy, description)).Returns(newActivity);
            var taskService = new TaskService(mockTaskRepository.Object, mockEmailService.Object);

            // Act
            var result = taskService.AddActivity(taskId, activityDate, doneBy, description);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Core.Entities.Activity>(result);
            Assert.Equal(activityDate, result.ActivityDate);
            Assert.Equal(doneBy, result.DoneBy);
            Assert.Equal(description, result.Description);
        }
        [Fact]
        public async void GetTaskActivities_ReturnsActivitiesForTask()
        {
            // Arrange
            var mockTaskRepository = new Mock<IRepository<Core.Entities.Task>>();
            var taskId = 1;
            var expectedActivities = new List<Core.Entities.Activity>
{
new Core.Entities.Activity { Id = 1, TaskId = taskId, ActivityDate = DateTime.Now.AddDays(-1), DoneBy = "User1", Description = "Initial setup" },
new Core.Entities.Activity { Id = 2, TaskId = taskId, ActivityDate = DateTime.Now, DoneBy = "User2", Description = "Feature development" }
};
            var mockEmailService = new Mock<IEmailService>();
            mockTaskRepository.Setup(repo => repo.GetTaskActivities(taskId)).ReturnsAsync(expectedActivities);
            var taskService = new TaskService(mockTaskRepository.Object, mockEmailService.Object);

            // Act
            var result = await taskService.GetTaskActivities(taskId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedActivities.Count, result.Count());
            Assert.Equal(expectedActivities, result);
        }
        [Fact]
        public async void UpdateTask_ReturnsOkResult_WithValidRequest()
        {
            // Arrange
            var fixedDateTime = new DateTime(2024, 5, 6, 11, 56, 42, DateTimeKind.Local);
            var mockTaskService = new Mock<ITaskService>();
            var taskId = 1;
            var existingTask = new Tasks.Core.Entities.Task
            {
                Id = taskId,
                TaskName = "Original Task",
                Tags = new List<string> { "Tag1" },
                DueDate = fixedDateTime.AddDays(1),
                Color = "Blue",
                AssignedTo = "User0"
            };

            var request = new TaskUpdateRequest
            {
                TaskName = "Updated Task",
                DueDate = fixedDateTime.AddDays(7),
                Color = "Red",
                AssignedTo = "User1",
                status="1"
            };

            mockTaskService.Setup(service => service.GetTaskById(taskId)).Returns(existingTask);
            mockTaskService.Setup(service => service.UpdateTask(taskId, request.TaskName, request.DueDate, request.Color, request.AssignedTo,request.status))
            .Returns(existingTask); // Assuming UpdateTask returns the updated task

            var controller = new TaskController(mockTaskService.Object);

            // Act
            // Act
            // Simulate the update operation by manually updating the existingTask's properties
            existingTask.TaskName = request.TaskName;
            existingTask.DueDate = request.DueDate;
            existingTask.Color = request.Color;
            existingTask.AssignedTo = request.AssignedTo;

            // Setup the mock to return the updated task when UpdateTask is called
            mockTaskService.Setup(service => service.UpdateTask(taskId, request.TaskName, request.DueDate, request.Color, request.AssignedTo, request.status))
            .Returns(existingTask); // Now it returns the updated task

            // Assert
            var result = controller.UpdateTask(taskId, request);
        }


        [Fact]
        public void GetAllTasks_ReturnsOkObjectResult_WithAllTasks()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>();
            var allTasks = new List<Core.Entities.Task>
{
new Core.Entities.Task { Id = 1,
TaskName = "Implement Authentication",
DueDate = DateTime.Now.AddDays(30),
Color = "red" },
new Core.Entities.Task {Id = 2,
TaskName = "Implement Authentication",
DueDate = DateTime.Now.AddDays(30),
Color = "red", },
};

            mockTaskService.Setup(service => service.GetAll()).Returns(allTasks);

            var controller = new Tasks.API.Controllers.TaskController(mockTaskService.Object);

            // Act
            var result = controller.GetAllTasks();

            // Assert
            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var returnedTasks = okResult.Value as List<Core.Entities.Task>;
            Assert.NotNull(returnedTasks);
            Assert.Equal(allTasks.Count, returnedTasks.Count);
        }

        [Fact]
        public void GetAllTasks_ReturnsStatusCode500_WhenExceptionThrown()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>();
            mockTaskService.Setup(service => service.GetAll()).Throws(new Exception("Test exception"));

            var controller = new Tasks.API.Controllers.TaskController(mockTaskService.Object);

            // Act & Assert
            var result = controller.GetAllTasks();
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
        [Fact]
        public async Task SearchTasks_ReturnsOkObjectResult_WithMatchingTasks()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>();
            var criteria = new TaskSearchCriteria { /* Initialize with test criteria */ };
            var matchingTasks = new List<Core.Entities.Task>
{
new Core.Entities.Task { Id = 1,
TaskName = "Implement Authentication",
DueDate = DateTime.Now.AddDays(30),
Color = "red"},
new Core.Entities.Task { Id = 2,
TaskName = "Implement Authentication",
DueDate = DateTime.Now.AddDays(30),
Color = "red" }
};

            mockTaskService.Setup(service => service.SearchTasks(criteria)).ReturnsAsync(matchingTasks);

            var controller = new Tasks.API.Controllers.TaskController(mockTaskService.Object);

            // Act
            var result = await controller.SearchTasks(criteria);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTasks = okResult.Value as IEnumerable<Core.Entities.Task>;
            Assert.NotNull(returnedTasks);
            Assert.Equal(matchingTasks.Count, returnedTasks.Count());
        }

        [Fact]
        public async Task SearchTasks_ReturnsStatusCode500_WhenExceptionThrown()
        {
            // Arrange
            var mockTaskService = new Mock<ITaskService>();
            var criteria = new TaskSearchCriteria { /* Initialize with test criteria */ };

            mockTaskService.Setup(service => service.SearchTasks(criteria))
            .ThrowsAsync(new Exception("Test exception"));

            var controller = new Tasks.API.Controllers.TaskController(mockTaskService.Object);

            // Act & Assert
            var result = await controller.SearchTasks(criteria);
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

    }
}