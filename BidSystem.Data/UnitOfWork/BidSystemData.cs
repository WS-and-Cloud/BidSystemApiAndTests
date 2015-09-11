namespace BidSystem.Data.UnitOfWork
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;

    using BidSystem.Data.Models;
    using BidSystem.Data.Repositories;
    using BidSystem.Models;

    public class BidSystemData : IBidSystemData
    {
        private readonly DbContext context;
        private readonly IDictionary<Type, object> repositories;

        public BidSystemData(DbContext context)
        {
            this.context = context;
            this.repositories = new Dictionary<Type, object>();
        }

        public IRepository<Bid> Bids
        {
            get
            {
                return this.GetRepository<Bid>();
            }
        }

        public IRepository<Offer> Offers
        {
            get
            {
                return this.GetRepository<Offer>();
            }
        }

        public IRepository<User> Users
        {
            get
            {
                return this.GetRepository<User>();
            }
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        private IRepository<T> GetRepository<T>() where T : class
        {
            var typeOfRepository = typeof(T);
            if (!this.repositories.ContainsKey(typeOfRepository))
            {
                var newRepository = Activator.CreateInstance(typeof(GenericRepository<T>), this.context);
                this.repositories.Add(typeOfRepository, newRepository);
            }

            return (IRepository<T>)this.repositories[typeOfRepository];
        } 
    }
}