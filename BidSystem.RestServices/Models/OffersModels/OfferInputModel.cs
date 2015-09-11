namespace BidSystem.RestServices.Models.OffersModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class OfferInputModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal InitialPrice { get; set; }

        public DateTime ExpirationDateTime { get; set; }
    }
}