namespace BidSystem.RestServices.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;

    using BidSystem.Data;
    using BidSystem.Data.Providers;
    using BidSystem.Data.UnitOfWork;
    using BidSystem.Models;
    using BidSystem.RestServices.Models.OffersModels;

    using Microsoft.AspNet.Identity;

    [RoutePrefix("api/offers")]
    public class OffersController : BaseApiController
    {
        public OffersController()
            : this(new BidSystemData(new BidSystemDbContext()), new AspNetUserIdProvider())
        {
        }

        public OffersController(IBidSystemData data, IUserIdProvider idProvider)
            : base(data, idProvider)
        {
        }

        // GET /api/offers/all
        [HttpGet]
        [Route("all")]
        public IHttpActionResult All()
        {
            var allOffers = 
                this.BidSystemData.Offers.All()
                .OrderByDescending(o => o.PublishDate)
                .Select(OffersOutputModel.CreateOffer);

            return this.Ok(allOffers);
        }

        // GET /api/offers/active
        [HttpGet]
        [Route("active")]
        public IHttpActionResult Active()
        {
            var activeOffers =
                this.BidSystemData.Offers.All()
                    .OrderByDescending(o => o.ExpirationDate)
                    .Where(o => o.ExpirationDate >= DateTime.Now)
                    .Select(OffersOutputModel.CreateOffer);
            return this.Ok(activeOffers);
        }

        // GET /api/offers/expired
        [HttpGet]
        [Route("expired")]
        public IHttpActionResult Expired()
        {
            var activeOffers =
                this.BidSystemData.Offers.All()
                    .OrderByDescending(o => o.ExpirationDate)
                    .Where(o => o.ExpirationDate <= DateTime.Now)
                    .Select(OffersOutputModel.CreateOffer);
            return this.Ok(activeOffers);
        }

        // GET /api/offers/details/{id}
        [HttpGet]
        [Route("details/{id}")]
        public IHttpActionResult GetOfferDetails(int id)
        {
            var offer =
                this.BidSystemData.Offers.All()
                    .Where(o => o.Id == id)
                    .Select(OfferDetailOutputModel.CreateOffer)
                    .FirstOrDefault();

            if (offer == null)
            {
                return this.NotFound();
            }

            return this.Ok(offer);
        }

        // GET /api/offers/my
        [HttpGet]
        [Route("my")]
        public IHttpActionResult GetMyOffers()
        {
            var loggedUserId = this.UserIdProvider.GetUserId();
            var user = this.BidSystemData.Users.Find(loggedUserId);
            if (user == null)
            {
                return this.Unauthorized();
            }

            var usersOffers =
                this.BidSystemData.Offers.All()
                    .OrderByDescending(o => o.PublishDate)
                    .ThenBy(o => o.Id)
                    .Where(o => o.SellerId == user.Id)
                    .Select(OffersOutputModel.CreateOffer);

            return this.Ok(usersOffers);
        }

        // POST /api/offers
        [HttpPost]
        public IHttpActionResult CreateOffer(OfferInputModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Model cannot be null");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var loggedUserId = this.UserIdProvider.GetUserId();
            var user = this.BidSystemData.Users.Find(loggedUserId);
            if (user == null)
            {
                return this.Unauthorized();
            }

            var offer = new Offer()
                        {
                            Title = model.Title,
                            Description = model.Description,
                            PublishDate = DateTime.Now,
                            InitialPrice = model.InitialPrice,
                            SellerId = user.Id,
                            ExpirationDate = model.ExpirationDateTime
                        };
        
            this.BidSystemData.Offers.Add(offer);
            this.BidSystemData.SaveChanges();

            return this.CreatedAtRoute(
                "DefaultApi",
                new { id = offer.Id },
                new { Id = offer.Id, Seller = offer.Seller.UserName, Message = "Offer created" });
        }
    }
}
