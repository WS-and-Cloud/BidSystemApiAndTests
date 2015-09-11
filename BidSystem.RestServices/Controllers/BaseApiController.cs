namespace BidSystem.RestServices.Controllers
{
    using System.Web.Http;

    using BidSystem.Data;
    using BidSystem.Data.Providers;
    using BidSystem.Data.UnitOfWork;

    public class BaseApiController : ApiController
    {
        private IBidSystemData bidSystemData;

        private IUserIdProvider userIdProvider;

        public BaseApiController()
            : this(new BidSystemData(new BidSystemDbContext()), new AspNetUserIdProvider())
        {
        }

        public BaseApiController(IBidSystemData data, IUserIdProvider userIdProvider)
        {
            this.BidSystemData = data;
            this.UserIdProvider = userIdProvider;
        }

        protected IBidSystemData BidSystemData
        {
            get
            {
                return this.bidSystemData;
            }

            private set
            {
                this.bidSystemData = value;
            }
        }

        protected IUserIdProvider UserIdProvider
        {
            get
            {
                return this.userIdProvider;
            }

            private set
            {
                this.userIdProvider = value;
            }
        }
    }
}