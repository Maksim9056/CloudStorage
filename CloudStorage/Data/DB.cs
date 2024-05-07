using Microsoft.EntityFrameworkCore;
using CloudStorageClass.CloudStorageModel;
namespace CloudStorageWebAPI.Data
{
    public class DB : DbContext
    {
        public DB(DbContextOptions<DB> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Filles> Filles { get; set; } = null!;
    }
}
