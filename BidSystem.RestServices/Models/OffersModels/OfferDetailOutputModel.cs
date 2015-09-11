namespace BidSystem.RestServices.Models.OffersModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using BidSystem.Models;
    using BidSystem.RestServices.Models.BidsModels;

    public class OfferDetailOutputModel
    {
        public static Expression<Func<Offer, OfferDetailOutputModel>> CreateOffer
        {
            get
            {
                return o => new OfferDetailOutputModel
                {
                    Id = o.Id,
                    Title = o.Title,
                    Description = o.Description,
                    Seller = o.Seller.UserName,
                    DatePublished = o.PublishDate,
                    InitialPrice = o.InitialPrice,
                    ExpirationDateTime = o.ExpirationDate,
                    IsExpired = o.ExpirationDate <= DateTime.Now,
                    BidWinner =
                        o.Bids.Any() && o.ExpirationDate <= DateTime.Now
                            ? o.Bids.OrderByDescending(b => b.BidPrice).FirstOrDefault().Bidder.UserName
                            : null,
                    Bids = o.Bids.AsQueryable().OrderByDescending(b => b.Id).Select(BidOutputModel.CreateBid)
                };
            }
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Seller { get; set; }

        public DateTime DatePublished { get; set; }

        public decimal InitialPrice { get; set; }

        public DateTime ExpirationDateTime { get; set; }

        public bool IsExpired { get; set; }

        public string BidWinner { get; set; }

        public IEnumerable<BidOutputModel> Bids { get; set; } 
    }
}