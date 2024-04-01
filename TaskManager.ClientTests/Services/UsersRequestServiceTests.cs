using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}