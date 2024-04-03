using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using TaskManager.Api.Models;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services.Tests
{
    [TestClass()]
    public class ProjectsRequestServiceTests
    {
        private AuthToken token;
        private ProjectsRequestService service;

        public ProjectsRequestServiceTests()
        {
            token = new UsersRequestService().GetToken("superadmin@gmail.com", "!Qwerty123456");
            service = new ProjectsRequestService();
        }

        [TestMethod()]
        public void GetProjectsTest()
        {
            var result = service.GetProjects(token);

            Console.WriteLine(result.Count);

            Assert.AreNotEqual(Array.Empty<ProjectModel>(), result.ToArray());
        }

        [TestMethod()]
        public void GetProjectTest()
        {
            var result = service.GetProject(token, 2);

            Assert.AreNotEqual(null, result);
        }

        [TestMethod()]
        public void CreateProjectTest()
        {
            ProjectModel project = new ProjectModel("Project 2", "Description", ProjectStatus.InProgress);
            project.AdminId = 1;
            
            var result = service.CreateProject(token, project);

            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void UpdateProjectTest()
        {
            ProjectModel project = new ProjectModel("Project 1", "Description", ProjectStatus.InProgress);
            project.Id = 2;

            var result = service.UpdateProject(token, project);

            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void DeleteProjectTest()
        {
            var result = service.DeleteProject(token, 5);

            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void AddUsersToProjectTest()
        {
            var result = service.AddUsersToProject(token, 2, new List<int>() { 2, 3 });
            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void RemoveUsersFromProjectTest()
        {
            var result = service.RemoveUsersFromProject(token, 2, new List<int>() { 2 });
            Assert.AreEqual(HttpStatusCode.OK, result);
        }
    }
}