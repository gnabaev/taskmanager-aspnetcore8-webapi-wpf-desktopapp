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

        public HttpStatusCode CreateUsers(AuthToken token, List<UserModel> users)
        {
            string userJson = JsonConvert.SerializeObject(users);
            var result = SendDataByUrl(HttpMethod.Post, usersControllerUrl + "/all", token, userJson);
            return result;
        }

        public UserModel GetUser(AuthToken token, int userId)
        {
            string response = GetDataByUrl(HttpMethod.Get, usersControllerUrl + $"/{userId}", token);
            UserModel user = JsonConvert.DeserializeObject<UserModel>(response);
            return user;
        }

        public UserModel GetCurrentUser(AuthToken token)
        {
            string response = GetDataByUrl(HttpMethod.Get, host + "account/info", token);
            UserModel user = JsonConvert.DeserializeObject<UserModel>(response);
            return user;
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

        public int? GetProjectUserAdmin(AuthToken token, int userId)
        {
            var result = GetDataByUrl(HttpMethod.Get, usersControllerUrl + $"/{userId}/admin", token);

            int adminId;

            bool parseResult = int.TryParse(result, out adminId);

            if (parseResult)
            {
                return adminId;
            }
            else
            {
                return null;
            }
        }

        public int GetWorkTimeMinutes(AuthToken token)
        {
            var result = GetDataByUrl(HttpMethod.Get, host + "account/workTime", token);
            int timeMinutes = 0;
            if (string.IsNullOrEmpty(result) == false)
            {
                bool parseResult = int.TryParse(result, out timeMinutes);
            }

            return timeMinutes;
        }
    }
}
