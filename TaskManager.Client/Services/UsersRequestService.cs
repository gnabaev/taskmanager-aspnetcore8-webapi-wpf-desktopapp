using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using TaskManager.Client.Models;

namespace TaskManager.Client.Services
{
    public class UsersRequestService
    {
        private const string host = "http://localhost:39035/api/";

        private async Task<string> GetDataByUrl(string url, string userName, string password)
        {
            var client = new HttpClient();
            var authToken = Encoding.UTF8.GetBytes(userName + ":" + password);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            var response = await client.PostAsync(url, new StringContent(authToken.ToString()));
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public AuthToken GetToken(string userName, string password)
        {
            string url = host + "account/token";
            var resultStr = GetDataByUrl(url, userName, password);
            AuthToken token = JsonConvert.DeserializeObject<AuthToken>(resultStr.Result);
            return token;
        }
    }
}
