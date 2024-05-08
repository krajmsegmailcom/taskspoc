using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Task.Blazor.Components.Pages;
using System.Net.Http.Json;

namespace Task.Blazor.Services
{
    public class TaskService
    {
        private readonly HttpClient _httpClient;

        public TaskService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CreateTaskAsync(string name, List<string> tags, DateTime dueDate, string color, string assignedTo)
        {

            var task = new
            {
                TaskName = name,
                Tags = tags,
                DueDate = dueDate,
                Color = color,
                AssignedTo = assignedTo
            };

            var json = JsonConvert.SerializeObject(task);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7266/api/Task/add", content);
            Console.WriteLine(response);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            var response = await _httpClient.PostAsync($"https://localhost:7266/api/Task/delete/{taskId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<TaskModel>> SearchByNameAsync(string searchTerm)
        {
            var content = new StringContent(JsonConvert.SerializeObject(searchTerm), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7266/api/Task/search/Name", content);

            response.EnsureSuccessStatusCode();

            var tasksJson = await response.Content.ReadAsStringAsync();
            var tasks = JsonConvert.DeserializeObject<List<TaskModel>>(tasksJson);

            return tasks;
        }


        public async Task<List<TaskModel>> SearchByTagsAsync(List<string> tags)
        {
            var content = new StringContent(JsonConvert.SerializeObject(tags), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7266/api/Task/search/Tags", content);

            response.EnsureSuccessStatusCode(); // Ensure the request was successful

            var tasksJson = await response.Content.ReadAsStringAsync(); // Read response content as string
            var tasks = JsonConvert.DeserializeObject<List<TaskModel>>(tasksJson); // Deserialize JSON to List<TaskModel>

            return tasks;
        }

        public async Task<List<TaskModel>> SearchByDueDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var dateRange = new
            {
                StartDate = startDate,
                EndDate = endDate
            };

            var content = new StringContent(JsonConvert.SerializeObject(dateRange), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7266/api/Task/search/DueDate", content);

            response.EnsureSuccessStatusCode(); // Ensure the request was successful

            var tasksJson = await response.Content.ReadAsStringAsync(); // Read response content as string
            var tasks = JsonConvert.DeserializeObject<List<TaskModel>>(tasksJson); // Deserialize JSON to List<TaskModel>

            return tasks;
        }


        public async Task<List<TaskModel>> SearchByStatusAsync(List<string> statuses)
        {
            var content = new StringContent(JsonConvert.SerializeObject(statuses), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7266/api/Task/search/Status", content);

            response.EnsureSuccessStatusCode(); // Ensure the request was successful

            var tasksJson = await response.Content.ReadAsStringAsync(); // Read response content as string
            var tasks = JsonConvert.DeserializeObject<List<TaskModel>>(tasksJson); // Deserialize JSON to List<TaskModel>

            return tasks;
        }


        public async Task<bool> UpdateTaskAsync(int taskId, string name, List<string> tags, DateTime dueDate, string color, string assignedTo, string status)
        {
            var task = new
            {
                TaskName = name,
                DueDate = dueDate,
                Color = color,
                AssignedTo = assignedTo,
                Tags = tags,
                Status = status
            };

            var json = JsonConvert.SerializeObject(task);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"https://localhost:7266/api/Task/update/{taskId}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AddActivityAsync(int taskId, DateTime activityDate, string doneBy, string description)
        {
            var activity = new
            {
                ActivityDate = activityDate,
                DoneBy = doneBy,
                Description = description
            };

            var json = JsonConvert.SerializeObject(activity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"https://localhost:7266/api/Task/add-activity/{taskId}", content);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> AddActivity(ActivityModel activity, int taskId)
        {
            var json = JsonConvert.SerializeObject(activity);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"https://localhost:7266/api/Task/add-activity/{taskId}", content);
            return response.IsSuccessStatusCode;
        }
      

        public async Task<List<ActivityModel>> GetTaskActivitiesAsync(int taskId)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7266/api/Task/get-activities/{taskId}");

            response.EnsureSuccessStatusCode();

            var activitiesJson = await response.Content.ReadAsStringAsync();
            var activities = JsonConvert.DeserializeObject<List<ActivityModel>>(activitiesJson);

            return activities;
        }

        public async Task<string> SearchTasksAsync(TaskSearchCriteria criteria)
        {
            // Serialize the criteria object to a JSON string
            var criteriaJson = JsonConvert.SerializeObject(criteria);
            var content = new StringContent(criteriaJson, Encoding.UTF8, "application/json");

            // Send a POST request to the search endpoint
            var response = await _httpClient.PostAsync("https://localhost:7266/api/Task/search", content);

            // Ensure the request was successful
            response.EnsureSuccessStatusCode();

            // Read the response content as a raw JSON string
            var tasksJson = await response.Content.ReadAsStringAsync();

            return tasksJson;
        }


        public async Task<List<TaskModel>> GetAllTasks()
        {
            var response = await _httpClient.GetAsync("https://localhost:7266/api/Task/get-all");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<TaskModel>>(result);
            }
            else
            {
                Console.Out.WriteLine("Failed to retrieve all tasks: " + response.StatusCode);
                return new List<TaskModel>();
            }
        }

        public async Task<List<TaskModel>> GetAll()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7266/api/Task/get-all");
                response.EnsureSuccessStatusCode(); // Ensure the request was successful

                var tasks = await response.Content.ReadFromJsonAsync<List<TaskModel>>(); // Deserialize JSON to List<TaskModel>

                return tasks;
            }
            catch (HttpRequestException ex)
            {
                // Log or handle the exception
                throw new Exception("Failed to retrieve tasks.", ex);
            }
        }

        public async Task<string> GetAllTasksRawJson()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://localhost:7266/api/Task/get-all");
                response.EnsureSuccessStatusCode(); // Ensure the request was successful

                var json = await response.Content.ReadAsStringAsync(); // Read the response content as string

                return json;
            }
            catch (HttpRequestException ex)
            {
                // Log or handle the exception
                throw new Exception("Failed to retrieve tasks.", ex);
            }
        }
        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var credentials = new
            {
                Username = username,
                Password = password
            };

            var json = JsonConvert.SerializeObject(credentials);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.GetAsync($"https://localhost:7266/api/Task/validate-user?username={Uri.EscapeDataString(username)}&password={Uri.EscapeDataString(password)}");
            return response.IsSuccessStatusCode;
        }


    }
}
