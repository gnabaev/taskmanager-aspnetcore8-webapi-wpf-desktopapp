using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services
{
    public class UsersRequestService : CommonRequestService
    {
        private string usersControllerUrl = host + "users";

        public AuthToken GetToken(string userName, string password)
        {
            string url = host + "account/token";
            var resultStr = GetDataByUrl(HttpMethod.Post, url, null, userName, password);
            AuthToken token = JsonConvert.DeserializeObject<AuthToken>(resultStr);
            return token;
        }

        public HttpStatusCode CreateUser(AuthToken token, UserModel user)
        {
            string userJson = JsonConvert.SerializeObject(user);
            var result = SendDataByUrl(HttpMethod.Post, usersControllerUrl, token, userJson);
            return result;
        }

        public List<UserModel> GetUsers(AuthToken token)
        {
            string response = GetDataByUrl(HttpMethod.Get, usersControllerUrl, token);
            List<UserModel> users = JsonConvert.DeserializeObject<List<UserModel>>(response);
            return users;
        }

        public HttpStatusCode DeleteUser(AuthToken token, int userId)
        {
            var result = DeleteDataByUrl(usersControllerUrl + $"/{userId}", token);
            return result;
        }

        public HttpStatusCode UpdateUser(AuthToken token, UserModel user)
        {
            string userJson = JsonConvert.SerializeObject(user);
            var result = SendDataByUrl(HttpMethod.Put, usersControllerUrl + $"/{user.Id}", token, userJson);
            return result;
        }
    }
}
