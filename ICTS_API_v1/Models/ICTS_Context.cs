using Microsoft.EntityFrameworkCore;

namespace ICTS_API_v1.Models
{
    public class ICTS_Context : DbContext
    {
        public ICTS_Context(DbContextOptions<ICTS_Context> options)
            : base(options)
        {
        }

        public DbSet<Cart> Carts
        {
            get;
            set;
        }

        public DbSet<Product> Products
        {
            get;
            set;
        }

        public DbSet<Site> Sites
        {
            get;
            set;
        }

        public DbSet<LocationHistory> LocationHistories
        {
            get;
            set;
        }
    }
}