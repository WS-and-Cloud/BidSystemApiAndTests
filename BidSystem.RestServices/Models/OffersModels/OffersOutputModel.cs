namespace BidSystem.RestServices.Models.OffersModels
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using BidSystem.Models;

    public class OffersOutputModel
    {
        public static Expression<Func<Offer, OffersOutputModel>> CreateOffer
        {
            get
            {
                return o => new OffersOutputModel
                {
                    Id = o.Id,
                    Title = o.Title,
                    Description = o.Description,
                    Seller = o.Seller.UserName,
                    DatePublished = o.PublishDate,
                    InitialPrice = o.InitialPrice,
                    ExpirationDateTime = o.ExpirationDate,
                    IsExpired = o.ExpirationDate <= DateTime.Now,
                    BidsCount = o.Bids.Count(),
                    BidWinner = o.Bids.Any() && o.ExpirationDate >= DateTime.Now ? o.Bids.OrderByDescending(b => b.BidPrice).FirstOrDefault().Bidder.UserName : null
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

        public int BidsCount { get; set; }

        public string BidWinner { get; set; }
    }
}