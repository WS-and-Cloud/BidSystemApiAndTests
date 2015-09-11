namespace BidSystem.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using BidSystem.Data.Models;

    public class Bid
    {
        public int Id { get; set; }

        public int? OfferId { get; set; }

        public virtual Offer Offer { get; set; }

        public decimal BidPrice { get; set; }

        [Required]
        public string BidderId { get; set; }

        public virtual User Bidder { get; set; }

        public DateTime DateOfBid { get; set; }

        public string Comment { get; set; }
    }
}
