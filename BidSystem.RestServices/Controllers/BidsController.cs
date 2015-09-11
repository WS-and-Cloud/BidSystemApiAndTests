namespace BidSystem.RestServices.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;

    using BidSystem.Data;
    using BidSystem.Data.Providers;
    using BidSystem.Data.UnitOfWork;
    using BidSystem.Models;
    using BidSystem.RestServices.Models.BidsModels;

    using Microsoft.AspNet.Identity;

    public class BidsController : BaseApiController
    {
        public BidsController()
            : this(new BidSystemData(new BidSystemDbContext()), new AspNetUserIdProvider())
        {
        }

        public BidsController(IBidSystemData data, IUserIdProvider idProvider)
            : base(data, idProvider)
        {
        }

        // GET /api/bids/won
        [HttpGet]
        [Route("api/bids/won")]
        public IHttpActionResult GetWonBids()
        {
            var loggedUserId = this.UserIdProvider.GetUserId();
            var user = this.BidSystemData.Users.Find(loggedUserId);
            if (user == null)
            {
                return this.Unauthorized();
            }

            var userWonBids =
                this.BidSystemData.Bids.All()
                    .OrderByDescending(b => b.DateOfBid)
                    .Where(b => b.BidderId == user.Id)
                    .Select(BidOutputModel.CreateBid);

            return this.Ok(userWonBids);
        }

        // POST /api/offers/117/bid
        [HttpPost]
        [Route("api/offers/{id}/bid")]
        public IHttpActionResult PostBidToExistingOffer(int id, [FromBody]BidInputModel model)
        {
            var loggedUserId = this.UserIdProvider.GetUserId();
            var user = this.BidSystemData.Users.Find(loggedUserId);
            if (user == null)
            {
                return this.Unauthorized();
            }

            if (model == null)
            {
                return this.BadRequest("Model cannot be null");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var offer = this.BidSystemData.Offers.Find(id);
            if (offer == null)
            {
                return this.NotFound();
            }

            if (offer.ExpirationDate <= DateTime.Now)
            {
                return this.BadRequest("{\"Message\":\"Offer has expired.\"}");
            }

            if (model.BidPrice == offer.InitialPrice)
            {

                return this.BadRequest(string.Format("Message:Your bid should be > " + offer.InitialPrice));
            }

            if (offer.Bids.Any())
            {
                foreach (var offerBid in offer.Bids)
                {
                    
                    if (model.BidPrice <= offerBid.BidPrice)
                    {
                        return this.BadRequest(string.Format("Message:Your bid should be > " + offerBid.BidPrice));
                    }
                }
            }

            var bid = new Bid()
                      {
                          BidPrice = model.BidPrice,
                          Comment = model.Comment,
                          Offer = offer,
                          Bidder = user,
                          DateOfBid = DateTime.Now,
                      };

            offer.Bids.Add(bid);
            this.BidSystemData.SaveChanges();

            return this.Ok(new { Id = bid.Id, Seller = user.UserName, Message = "Bid created" });
        }

        // GET /api/bids/my
        [HttpGet]
        [Route("api/bids/my")]
        public IHttpActionResult MyBids()
        {
            var loggedUserId = this.UserIdProvider.GetUserId();
            var user = this.BidSystemData.Users.Find(loggedUserId);
            if (user == null)
            {
                return this.Unauthorized();
            }

            var myBids =
                this.BidSystemData.Bids.All()
                    .OrderByDescending(b => b.DateOfBid)
                    .ThenBy(b => b.Id)
                    .Where(b => b.Bidder.Id == user.Id)
                    .Select(BidOutputModel.CreateBid);

            return this.Ok(myBids);
        }
    }
}