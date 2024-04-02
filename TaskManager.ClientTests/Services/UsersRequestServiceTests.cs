using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services.Tests
{
    [TestClass()]
    public class UsersRequestServiceTests
    {
        [TestMethod()]
        public void GetTokenTest()
        {
            var token = new UsersRequestService().GetToken("superadmin@gmail.com", "!Qwerty123456");

            Console.WriteLine(token);

            Assert.IsNotNull(token.access_token);
        }

        [TestMethod()]
        public void CreateUserTest()
        {
            var service = new UsersRequestService();

            var token = service.GetToken("superadmin@gmail.com", "!Qwerty123456");

            UserModel userTest = new UserModel("User", "User", "user@gmail.com", "!Qwerty123456", UserStatus.User, "1235345235");

            var result = service.CreateUser(token, userTest);

            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void GetUsersTest()
        {
            var service = new UsersRequestService();

            var token = service.GetToken("superadmin@gmail.com", "!Qwerty123456");

            var result = service.GetUsers(token);

            Console.WriteLine(result.Count);

            Assert.AreNotEqual(Array.Empty<UserModel>(), result.ToArray());
        }

        [TestMethod()]
        public void UpdateUserTest()
        {
            var service = new UsersRequestService();

            var token = service.GetToken("superadmin@gmail.com", "!Qwerty123456");

            UserModel userTest = new UserModel("User3", "User3", "user3@gmail.com", "!Qwerty123456", UserStatus.User, "34475878");
            userTest.Id = 3;

            var result = service.UpdateUser(token, userTest);

            Assert.AreEqual(HttpStatusCode.OK, result);
        }


        [TestMethod()]
        public void DeleteUserTest()
        {
            var service = new UsersRequestService();

            var token = service.GetToken("superadmin@gmail.com", "!Qwerty123456");

            var result = service.DeleteUser(token, 7);

            Assert.AreEqual(HttpStatusCode.OK, result);
        }
    }
}