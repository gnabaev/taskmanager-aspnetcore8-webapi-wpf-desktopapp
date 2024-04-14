using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services.Tests
{
    [TestClass()]
    public class TasksRequestServiceTests
    {
        private AuthToken token;
        private TasksRequestService service;

        public TasksRequestServiceTests()
        {
            token = new UsersRequestService().GetToken("superadmin@gmail.com", "!Qwerty123456");
            service = new TasksRequestService();
        }

        [TestMethod()]
        public void GetUserTasksTest()
        {
            var result = service.GetUserTasks(token);
            Console.WriteLine(result.Count);
            Assert.AreNotEqual(Array.Empty<TaskModel>(), result.ToArray());
        }

        [TestMethod()]
        public void GetTaskTest()
        {
            var result = service.GetTask(token, 1);
            Assert.AreNotEqual(null, result);
        }

        [TestMethod()]
        public void GetDeskTasksTest()
        {
            var desks = service.GetDeskTasks(token, 1);
            Assert.AreEqual(1, desks.Count);
        }

        [TestMethod()]
        public void CreateTaskTest()
        {
            var task = new TaskModel("Task 2", "Description", DateTime.Now, DateTime.Now, "New");
            task.DeskId = 1;
            task.CreatorId = 1;
            task.ExecutorId = 1;

            var result = service.CreateTask(token, task);
            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void UpdateTaskTest()
        {
            var task = new TaskModel("Task 2", "Description", DateTime.Now, DateTime.Now, "InProgress");
            task.Id = 3;
            task.ExecutorId = 1;

            var result = service.UpdateTask(token, task);
            Assert.AreEqual(HttpStatusCode.OK, result);

        }

        [TestMethod()]
        public void DeleteTaskTest()
        {
            var result = service.DeleteTask(token, 3);
            Assert.AreEqual(HttpStatusCode.OK, result);
        }
    }
}