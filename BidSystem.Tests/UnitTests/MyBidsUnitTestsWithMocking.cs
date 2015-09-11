namespace BidSystem.Tests.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;

    using BidSystem.Data.Providers;
    using BidSystem.Data.UnitOfWork;
    using BidSystem.RestServices.Controllers;
    using BidSystem.RestServices.Models.BidsModels;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class MyBidsUnitTestsWithMocking
    {
        private MockContainer mockContainer;

        [TestInitialize]
        public void InitMocks()
        {
            this.mockContainer = new MockContainer();
            this.mockContainer.PrepareMocks();
        }

        [TestMethod]
        public void GetMyBids_WithCorrectUserToken_ShouldReturnOnlyUsersBidsAnd200Ok()
        {
            // Arrange
            var fakeBids = this.mockContainer.BidRepositoryMock.Object.All();
            var fakeUsers = this.mockContainer.UserRepositoryMock.Object.All();
            var fakeUser = fakeUsers.FirstOrDefault();
            if (fakeUser == null)
            {
                Assert.Fail("Cannot perform test no users available.");
            }

            var mockData = new Mock<IBidSystemData>();
            mockData.Setup(r => r.Bids.All()).Returns(fakeBids.AsQueryable());
            mockData.Setup(r => r.Users.Find(It.IsAny<string>())).Returns(
                (string id) =>
                {
                    return fakeUsers.FirstOrDefault(u => u.Id == id);
                });
            var mockProvider = new Mock<IUserIdProvider>();
            mockProvider.Setup(r => r.GetUserId()).Returns(fakeUser.Id);

            // Act
            var bidsController = new BidsController(mockData.Object, mockProvider.Object);
            this.SetupController(bidsController);
            var httpGetResponse = bidsController.MyBids().ExecuteAsync(CancellationToken.None).Result;
            var result = httpGetResponse.Content.ReadAsAsync<IEnumerable<BidOutputModel>>().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpGetResponse.StatusCode);
            foreach (var bidOutputModel in result)
            {
                Assert.AreEqual(fakeUser.UserName, bidOutputModel.Bidder);
            }
        }

        [TestMethod]
        public void GetMyBids_WithIncorrectUserToken_ShouldReturn401NotAuthorized()
        {
            // Arrange
            var fakeBids = this.mockContainer.BidRepositoryMock.Object.All();
            var fakeUsers = this.mockContainer.UserRepositoryMock.Object.All();
            var fakeUser = fakeUsers.FirstOrDefault();
            if (fakeUser == null)
            {
                Assert.Fail("Cannot perform test no users available.");
            }

            var mockData = new Mock<IBidSystemData>();
            mockData.Setup(r => r.Bids.All()).Returns(fakeBids.AsQueryable());
            mockData.Setup(r => r.Users.Find(It.IsAny<string>())).Returns(
                (string id) =>
                {
                    return fakeUsers.FirstOrDefault(u => u.Id == id);
                });
            var mockProvider = new Mock<IUserIdProvider>();

            // Act
            var bidsController = new BidsController(mockData.Object, mockProvider.Object);
            this.SetupController(bidsController);
            var httpGetResponse = bidsController.MyBids().ExecuteAsync(CancellationToken.None).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, httpGetResponse.StatusCode);
        }


        private void SetupController(ApiController controller)
        {
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();
        }
    }
}