using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using TaskManager.Client.Models;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services.Tests
{
    [TestClass()]
    public class DesksRequestServiceTests
    {
        private AuthToken token;
        private DesksRequestService service;

        public DesksRequestServiceTests()
        {
            token = new UsersRequestService().GetToken("superadmin@gmail.com", "!Qwerty123456");
            service = new DesksRequestService();
        }

        [TestMethod()]
        public void GetUserDesksTest()
        {
            var result = service.GetUserDesks(token);

            Console.WriteLine(result.Count);

            Assert.AreNotEqual(Array.Empty<DeskModel>(), result.ToArray());
        }

        [TestMethod()]
        public void GetDeskTest()
        {
            var result = service.GetDesk(token, 1);

            Assert.AreNotEqual(null, result);
        }

        [TestMethod()]
        public void GetProjectDesksTest()
        {
            var desks = service.GetProjectDesks(token, 2);
            Assert.AreEqual(1, desks.Count);
        }

        [TestMethod()]
        public void CreateDeskTest()
        {
            var desk = new DeskModel("Desk 2", "Description", true, ["New", "In Progress", "In Review", "Completed"]);
            desk.ProjectId = 2;
            desk.AdminId = 1;

            var result = service.CreateDesk(token, desk);
            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        [TestMethod()]
        public void UpdateDeskTest()
        {
            var desk = new DeskModel("Desk 2", "Description", false, ["New", "In Progress", "In Review", "Completed"]);
            desk.ProjectId = 2;
            desk.AdminId = 1;
            desk.Id = 3;

            var result = service.UpdateDesk(token, desk);
            Assert.AreEqual(HttpStatusCode.OK, result);

        }

        [TestMethod()]
        public void DeleteDeskTest()
        {
            var result = service.DeleteDesk(token, 3);
            Assert.AreEqual(HttpStatusCode.OK, result);
        }
    }
}