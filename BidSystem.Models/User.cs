namespace BidSystem.Data.Models
{
    using System.Collections.Generic;
    using System.Security.AccessControl;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using BidSystem.Models;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class User : IdentityUser
    {
        private ICollection<Bid> bids;
        private ICollection<Offer> offers;

        public User()
        {
            this.bids = new HashSet<Bid>();
            this.offers = new HashSet<Offer>();
        }

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

        public virtual ICollection<Offer> Offers
        {
            get
            {
                return this.offers;
            }

            set
            {
                this.offers = value;
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(
            UserManager<User> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            return userIdentity;
        }
    }
}
