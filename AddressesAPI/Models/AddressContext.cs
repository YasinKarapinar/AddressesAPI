using Microsoft.EntityFrameworkCore;

namespace AddressesAPI.Models
{
    public class AddressContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }

        public AddressContext(DbContextOptions<AddressContext> options) : base(options)
        {
        }
    }
}
