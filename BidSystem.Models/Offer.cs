namespace BidSystem.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using BidSystem.Data.Models;

    public class Offer
    {
        private ICollection<Bid> bids;

        public Offer()
        {
            this.bids = new HashSet<Bid>();
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string SellerId { get; set; }

        public virtual User Seller { get; set; }

        [Required]
        public DateTime PublishDate { get; set; }

        [Required]
        public decimal InitialPrice { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        public virtual ICollection<Bid> Bids
        {
            get
            {
                return this.bids;
            }

            set
            {
                this.bids = value;
            }
        }
    }
}
