using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services
{
    public class UsersRequestService
    {
        private const string host = "http://localhost:39035/api/";

        private string usersControllerUrl = host + "users";

        private string GetDataByUrl(HttpMethod method, string url, AuthToken token, string userName = null, string password = null)
        {
            string result = string.Empty;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = method.Method;

            if (userName != null && password != null)
            {
                string encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(userName + ":" + password));
                request.Headers.Add("Authorization", "Basic " + encoded);
            }
            else if (token != null)
            {
                request.Headers.Add("Authorization", "Bearer " + token.access_token);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                string responseString = reader.ReadToEnd();
                result = responseString;
            }

            return result;

            //var client = new HttpClient();
            //var authToken = Encoding.UTF8.GetBytes(userName + ":" + password);
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            //var response = await client.PostAsync(url, new StringContent(authToken.ToString()));
            //var content = await response.Content.ReadAsStringAsync();
            //return content;
        }

        private HttpStatusCode SendDataByUrl(HttpMethod method, string url, AuthToken token, string data)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);

            var content = new StringContent(data, Encoding.UTF8, "application/json");

            if (method == HttpMethod.Post)
            {
                result = client.PostAsync(url, content).Result;
            }

            if (method == HttpMethod.Put)
            {
                result = client.PutAsync(url, content).Result;
            }

            return result.StatusCode;
        }

        private HttpStatusCode DeleteDataByUrl(string url, AuthToken token)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);
            result = client.DeleteAsync(url).Result;

            return result.StatusCode;
        }

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
