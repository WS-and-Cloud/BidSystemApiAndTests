namespace BidSystem.Tests.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;

    using BidSystem.RestServices.Models.OffersModels;
    using BidSystem.Tests.Models;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class OfferDetailsIntegrationTests
    {
        [TestMethod]
        public void GetOffers_ActionAll_ShouldReturnOffersAnd200Ok()
        {
            // Arrange -> clean the database and register new user
            TestingEngine.CleanDatabase();
            var userSession = TestingEngine.RegisterUser("peter", "pAssW@rd#123456");

            // Act -> create a few offers
            CreateFourOfferModelsInDatabase(userSession);

            var httpResponse = TestingEngine.GetOffers(userSession.Access_Token, "all", null);
            var result = httpResponse.Content.ReadAsAsync<IList<OffersOutputModel>>().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.AreEqual(4, result.Count);
        }

        [TestMethod]
        public void GetOffers_ActionActive_ShouldReturnOnlyActiveOffersAnd200Ok()
        {
            // Arrange -> clean the database and register new user
            TestingEngine.CleanDatabase();
            var userSession = TestingEngine.RegisterUser("peter", "pAssW@rd#123456");

            // Act -> create a few offers
            CreateFourOfferModelsInDatabase(userSession);

            var httpResponse = TestingEngine.GetOffers(userSession.Access_Token, "active", null);
            var result = httpResponse.Content.ReadAsAsync<IList<OffersOutputModel>>().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
            foreach (var offersOutputModel in result)
            {
                Assert.AreEqual(false, offersOutputModel.IsExpired);
            }
        }

        [TestMethod]
        public void GetOffers_ActionExpired_ShouldReturnOnlyExpiredOffersAnd200Ok()
        {
            // Arrange -> clean the database and register new user
            TestingEngine.CleanDatabase();
            var userSession = TestingEngine.RegisterUser("peter", "pAssW@rd#123456");

            // Act -> create a few offers
            CreateFourOfferModelsInDatabase(userSession);

            var httpResponse = TestingEngine.GetOffers(userSession.Access_Token, "expired", null);
            var result = httpResponse.Content.ReadAsAsync<IList<OffersOutputModel>>().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
            foreach (var offersOutputModel in result)
            {
                Assert.AreEqual(true, offersOutputModel.IsExpired);
            }
        }

        [TestMethod]
        public void GetOffer_ActionDetails_ForExistingOffer_ShouldReturnOneOfferAnd200Ok()
        {
            // Arrange -> clean the database and register new user
            TestingEngine.CleanDatabase();
            var userSession = TestingEngine.RegisterUser("peter", "pAssW@rd#123456");

            // Act -> create a few offers
            var httpCreateOfferResponse = TestingEngine.CreateOfferHttpPost(userSession.Access_Token, "Offer #101", "Description #101", 550m, DateTime.Now.AddDays(10));
            if (!httpCreateOfferResponse.IsSuccessStatusCode)
            {
                Assert.Fail("Unable to create offer.");
            }

            var offer = httpCreateOfferResponse.Content.ReadAsAsync<OffersOutputModel>().Result;

            var httpResponse = TestingEngine.GetOffers(userSession.Access_Token, "details", offer.Id);
            var result = httpResponse.Content.ReadAsAsync<OffersOutputModel>().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.AreEqual("Offer #101", result.Title);
            Assert.AreEqual(550m, result.InitialPrice);
        }

        [TestMethod]
        public void GetOffer_ActionDetails_ForNonExistingOffer_ShouldReturn404NotFound()
        {
            // Arrange -> clean the database and register new user
            TestingEngine.CleanDatabase();
            var userSession = TestingEngine.RegisterUser("peter", "pAssW@rd#123456");

            // Act -> create a few offers
            var httpResponse = TestingEngine.GetOffers(userSession.Access_Token, "details", -1);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, httpResponse.StatusCode);
        }

        [TestMethod]
        public void GetOffers_ActionMy_WithCorrectData_ShouldReturnUserOffersAnd200Ok()
        {
            // Arrange -> clean the database and register new user
            TestingEngine.CleanDatabase();
            var userSession = TestingEngine.RegisterUser("peter", "pAssW@rd#123456");

            // Act -> create a few offers
            CreateFourOfferModelsInDatabase(userSession);

            var httpResponse = TestingEngine.GetOffers(userSession.Access_Token, "my", null);
            var result = httpResponse.Content.ReadAsAsync<IList<OffersOutputModel>>().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
            foreach (var offersOutputModel in result)
            {
                Assert.AreEqual("peter", offersOutputModel.Seller);
            }
        }

        [TestMethod]
        public void GetOffer_ActionMy_WithIncorrectUserToken_ShouldReturn401Unauthorized()
        {
            // Arrange -> clean the database and register new user
            TestingEngine.CleanDatabase();
            var userSession = TestingEngine.RegisterUser("peter", "pAssW@rd#123456");

            // Act -> create a few offers
            CreateFourOfferModelsInDatabase(userSession);

            var httpResponse = TestingEngine.GetOffers(null, "my", null);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        }

        private static void CreateFourOfferModelsInDatabase(UserSessionModel userSession)
        {
            var offersToAdds = new OfferModel[]
                               {
                                   new OfferModel()
                                   {
                                       Title = "First Offer (Expired)",
                                       Description = "Description",
                                       InitialPrice = 200,
                                       ExpirationDateTime = DateTime.Now.AddDays(-5)
                                   },
                                   new OfferModel()
                                   {
                                       Title = "Another Offer (Expired)",
                                       InitialPrice = 15.50m,
                                       ExpirationDateTime = DateTime.Now.AddDays(-1)
                                   },
                                   new OfferModel()
                                   {
                                       Title = "Second Offer (Active 3 months)",
                                       Description = "Description",
                                       InitialPrice = 500,
                                       ExpirationDateTime = DateTime.Now.AddMonths(3)
                                   },
                                   new OfferModel()
                                   {
                                       Title = "Third Offer (Active 6 months)",
                                       InitialPrice = 120,
                                       ExpirationDateTime = DateTime.Now.AddMonths(6)
                                   },
                               };

            foreach (var offer in offersToAdds)
            {
                var httpResult = TestingEngine.CreateOfferHttpPost(
                    userSession.Access_Token,
                    offer.Title,
                    offer.Description,
                    offer.InitialPrice,
                    offer.ExpirationDateTime);
                Assert.AreEqual(HttpStatusCode.Created, httpResult.StatusCode);
            }
        }
    }
}