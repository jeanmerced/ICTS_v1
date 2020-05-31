using Microsoft.EntityFrameworkCore;

namespace ICTS_API_v1.Models
{
    /// <summary>
    /// DbContext is the primary class that is responsible for interacting with the ICTS database
    /// </summary>
    public class ICTS_Context : DbContext
    {
        /// <summary>
        /// Constructor for ICTS_Context
        /// </summary>
        /// <param name="options">The options to be used by a DbContext</param>
        /// <returns>ICTS_Context object</returns>
        public ICTS_Context(DbContextOptions<ICTS_Context> options)
            : base(options)
        {
        }

        //Entity set that can be used to query and save instances of Cart
        public DbSet<Cart> Carts
        {
            get;
            set;
        }

        //Entity set that can be used to query and save instances of Product
        public DbSet<Product> Products
        {
            get;
            set;
        }

        //Entity set that can be used to query and save instances of Site
        public DbSet<Site> Sites
        {
            get;
            set;
        }

        //Entity set that can be used to query and save instances of LocationHistory
        public DbSet<LocationHistory> LocationHistories
        {
            get;
            set;
        }
    }
}
