using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using TaskManager.Api.Models;
using TaskManager.Client.Models;

namespace TaskManager.Client.Services
{
    public class DesksRequestService : CommonRequestService
    {
        private string desksControllerUrl = host + "desks";

        public List<DeskModel> GetUserDesks(AuthToken token)
        {
            string response = GetDataByUrl(HttpMethod.Get, desksControllerUrl, token);
            List<DeskModel> desks = JsonConvert.DeserializeObject<List<DeskModel>>(response);
            return desks;
        }

        public DeskModel GetDesk(AuthToken token, int deskId)
        {
            string response = GetDataByUrl(HttpMethod.Get, desksControllerUrl + $"/{deskId}", token);
            DeskModel desk = JsonConvert.DeserializeObject<DeskModel>(response);
            return desk;
        }

        public List<DeskModel> GetProjectDesks(AuthToken token, int projectId)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("projectId", projectId.ToString());
            string response = GetDataByUrl(HttpMethod.Get, desksControllerUrl + "/project", token, null, null, parameters);
            List<DeskModel> desks = JsonConvert.DeserializeObject<List<DeskModel>>(response);
            return desks;
        }

        public HttpStatusCode CreateDesk(AuthToken token, DeskModel desk)
        {
            string deskJson = JsonConvert.SerializeObject(desk);
            var result = SendDataByUrl(HttpMethod.Post, desksControllerUrl, token, deskJson);
            return result;
        }

        public HttpStatusCode UpdateDesk(AuthToken token, DeskModel desk)
        {
            string desksJson = JsonConvert.SerializeObject(desk);
            var result = SendDataByUrl(HttpMethod.Put, desksControllerUrl + $"/{desk.Id}", token, desksJson);
            return result;
        }

        public HttpStatusCode DeleteDesk(AuthToken token, int deskId)
        {
            var result = DeleteDataByUrl(desksControllerUrl + $"/{deskId}", token);
            return result;
        }
    }
}
