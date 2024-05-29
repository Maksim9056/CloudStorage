using Microsoft.EntityFrameworkCore;
using CloudStorageClass.CloudStorageModel;
namespace CloudStorageWebAPI.Data
{
    public class DB : DbContext
    {
        public DB(DbContextOptions<DB> options) : base(options)
        {
            try
            {
                Database.EnsureCreated();

            }
            catch (Exception ex)
            {

            }
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Filles> Filles { get; set; } = null!;
    }
}
