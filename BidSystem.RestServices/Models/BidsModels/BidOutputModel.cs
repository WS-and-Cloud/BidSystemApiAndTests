namespace BidSystem.RestServices.Models.BidsModels
{
    using System;
    using System.Linq.Expressions;

    using BidSystem.Models;

    public class BidOutputModel
    {
        public static Expression<Func<Bid, BidOutputModel>> CreateBid
        {
            get
            {
                return b => new BidOutputModel()
                {
                    Id = b.Id,
                    OfferId = b.OfferId,
                    DateCreated = b.DateOfBid,
                    Bidder = b.Bidder.UserName,
                    OfferedPrice = b.BidPrice,
                    Comment = b.Comment
                };
            }
        }

        public int Id { get; set; }

        public int? OfferId { get; set; }

        public DateTime DateCreated { get; set; }

        public string Bidder { get; set; }

        public decimal OfferedPrice { get; set; }

        public string Comment { get; set; }
    }
}