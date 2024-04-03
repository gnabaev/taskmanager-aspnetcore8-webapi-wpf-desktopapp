using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using TaskManager.Api.Models;
using TaskManager.Client.Models;

namespace TaskManager.Client.Services
{
    public class ProjectsRequestService : CommonRequestService
    {
        private string projectsControllerUrl = host + "projects";

        public List<ProjectModel> GetProjects(AuthToken token)
        {
            string response = GetDataByUrl(HttpMethod.Get, projectsControllerUrl, token);
            List<ProjectModel> projects = JsonConvert.DeserializeObject<List<ProjectModel>>(response);
            return projects;
        }

        public ProjectModel GetProject(AuthToken token, int projectId)
        {
            string response = GetDataByUrl(HttpMethod.Get, projectsControllerUrl + $"/{projectId}", token);
            ProjectModel project = JsonConvert.DeserializeObject<ProjectModel>(response);
            return project;
        }

        public HttpStatusCode CreateProject(AuthToken token, ProjectModel project)
        {
            string projectJson = JsonConvert.SerializeObject(project);
            var result = SendDataByUrl(HttpMethod.Post, projectsControllerUrl, token, projectJson);
            return result;
        }

        public HttpStatusCode UpdateProject(AuthToken token, ProjectModel project)
        {
            string projectJson = JsonConvert.SerializeObject(project);
            var result = SendDataByUrl(HttpMethod.Put, projectsControllerUrl + $"/{project.Id}", token, projectJson);
            return result;
        }

        public HttpStatusCode DeleteProject(AuthToken token, int projectId)
        {
            var result = DeleteDataByUrl(projectsControllerUrl + $"/{projectId}", token);
            return result;
        }

        public HttpStatusCode AddUsersToProject(AuthToken token, int projectId, List<int> userIds)
        {
            string userIdsJson = JsonConvert.SerializeObject(userIds);
            var result = SendDataByUrl(HttpMethod.Put, projectsControllerUrl + $"/{projectId}/users", token, userIdsJson);
            return result;
        }

        public HttpStatusCode RemoveUsersFromProject(AuthToken token, int projectId, List<int> userIds)
        {
            string userIdsJson = JsonConvert.SerializeObject(userIds);
            var result = SendDataByUrl(HttpMethod.Put, projectsControllerUrl + $"/{projectId}/users/remove", token, userIdsJson);
            return result;
        }
    }
}
