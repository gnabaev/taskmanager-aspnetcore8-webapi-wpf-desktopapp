using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using TaskManager.Api.Models;
using TaskManager.Client.Models;

namespace TaskManager.Client.Services
{
    public class TasksRequestService : CommonRequestService
    {
        private string tasksControllerUrl = host + "tasks";

        public List<TaskModel> GetUserTasks(AuthToken token)
        {
            string response = GetDataByUrl(HttpMethod.Get, tasksControllerUrl, token);
            List<TaskModel> tasks = JsonConvert.DeserializeObject<List<TaskModel>>(response);
            return tasks;
        }

        public TaskModel GetTask(AuthToken token, int taskId)
        {
            string response = GetDataByUrl(HttpMethod.Get, tasksControllerUrl + $"/{taskId}", token);
            TaskModel task = JsonConvert.DeserializeObject<TaskModel>(response);
            return task;
        }

        public List<TaskModel> GetDeskTasks(AuthToken token, int deskId)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("deskId", deskId.ToString());
            string response = GetDataByUrl(HttpMethod.Get, tasksControllerUrl + "/desk", token, null, null, parameters);
            List<TaskModel> desks = JsonConvert.DeserializeObject<List<TaskModel>>(response);
            return desks;
        }

        public HttpStatusCode CreateTask(AuthToken token, TaskModel task)
        {
            string taskJson = JsonConvert.SerializeObject(task);
            var result = SendDataByUrl(HttpMethod.Post, tasksControllerUrl, token, taskJson);
            return result;
        }

        public HttpStatusCode UpdateTask(AuthToken token, TaskModel task)
        {
            string tasksJson = JsonConvert.SerializeObject(task);
            var result = SendDataByUrl(HttpMethod.Put, tasksControllerUrl + $"/{task.Id}", token, tasksJson);
            return result;
        }

        public HttpStatusCode DeleteTask(AuthToken token, int taskId)
        {
            var result = DeleteDataByUrl(tasksControllerUrl + $"/{taskId}", token);
            return result;
        }
    }
}
