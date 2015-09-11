namespace BidSystem.Tests.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BidSystem.Data.Models;
    using BidSystem.Data.Repositories;
    using BidSystem.Models;

    using Moq;

    public class MockContainer
    {
        public Mock<IRepository<Offer>> OfferRepositoryMock { get; set; }

        public Mock<IRepository<Bid>> BidRepositoryMock { get; set; }

        public Mock<IRepository<User>> UserRepositoryMock { get; set; }

        public void PrepareMocks()
        {
            this.SetupFakeUsers();
            this.SetupFakeOffers();
            this.SetupFakeBids();
        }

        private void SetupFakeBids()
        {
            var bidder = new User() { Id = "123", UserName = "Pesho#1" };
            var fakeBids = new List<Bid>()
                           {
                               new Bid()
                               {
                                   Id = 1,
                                   BidPrice = 220m,
                                   DateOfBid = DateTime.UtcNow,
                                   Bidder = bidder
                               },
                               new Bid()
                               {
                                   Id = 1,
                                   BidPrice = 220m,
                                   DateOfBid = DateTime.UtcNow,
                                   Bidder = bidder
                               },
                               new Bid()
                               {
                                   Id = 1,
                                   BidPrice = 220m,
                                   DateOfBid = DateTime.UtcNow,
                                   Bidder = bidder
                               },
                           };

            this.BidRepositoryMock = new Mock<IRepository<Bid>>();
            this.BidRepositoryMock.Setup(r => r.All()).Returns(fakeBids.AsQueryable());
            this.BidRepositoryMock.Setup(r => r.Find(It.IsAny<int>())).Returns(
                (int id) =>
                {
                    return fakeBids.FirstOrDefault(o => o.Id == id);
                });
        }

        private void SetupFakeOffers()
        {
            var seller = new User() { Id = "123", UserName = "Pesho#1" };
            var fakeOffers = new List<Offer>()
                            {
                                new Offer()
                                {
                                    Id = 1, Title = "offer #1", Description = "Desc#1",
                                    PublishDate = DateTime.UtcNow,
                                    ExpirationDate = DateTime.UtcNow.AddDays(10),
                                    InitialPrice = 200m,
                                    Seller = seller
                                },
                                new Offer()
                                {
                                    Id = 2, Title = "offer #2", Description = "Desc#2",
                                    PublishDate = DateTime.UtcNow,
                                    ExpirationDate = DateTime.UtcNow.AddDays(10),
                                    InitialPrice = 200m,
                                    Seller = seller
                                },
                            };
            this.OfferRepositoryMock = new Mock<IRepository<Offer>>();
            this.OfferRepositoryMock.Setup(r => r.All()).Returns(fakeOffers.AsQueryable());
            this.OfferRepositoryMock.Setup(r => r.Find(It.IsAny<int>())).Returns(
                (int id) =>
                {
                    return fakeOffers.FirstOrDefault(o => o.Id == id);
                });
        }

        private void SetupFakeUsers()
        {
            var fakeUsers = new List<User>()
                            {
                                new User() { Id = "123", UserName = "Pesho#1" },
                                new User() { Id = "232", UserName = "Pesho#2" },
                                new User() { Id = "323", UserName = "Pesho#3" },
                            };
            this.UserRepositoryMock = new Mock<IRepository<User>>();
            this.UserRepositoryMock.Setup(r => r.All()).Returns(fakeUsers.AsQueryable());
            this.UserRepositoryMock.Setup(r => r.Find(It.IsAny<string>())).Returns(
                (string id) =>
                {
                    return fakeUsers.FirstOrDefault(u => u.Id == id);
                });
        }
    }
}