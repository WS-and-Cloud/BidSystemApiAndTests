namespace BidSystem.Data
{
    using System.Data.Entity;

    using BidSystem.Data.Models;
    using BidSystem.Models;

    using Microsoft.AspNet.Identity.EntityFramework;

    public class BidSystemDbContext : IdentityDbContext<User>
    {
        public BidSystemDbContext()
            : base("BidSystem")
        {
        }

        public virtual IDbSet<Offer> Offers { get; set; }

        public virtual IDbSet<Bid> Bids { get; set; }

        public static BidSystemDbContext Create()
        {
            return new BidSystemDbContext();
        }
    }
}
