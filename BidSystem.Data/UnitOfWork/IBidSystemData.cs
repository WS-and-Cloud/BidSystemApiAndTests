namespace BidSystem.Data.UnitOfWork
{
    using BidSystem.Data.Models;
    using BidSystem.Data.Repositories;
    using BidSystem.Models;

    public interface IBidSystemData
    {
        IRepository<Bid> Bids { get; }

        IRepository<Offer> Offers { get; }

        IRepository<User> Users { get; }

        int SaveChanges();
    }
}