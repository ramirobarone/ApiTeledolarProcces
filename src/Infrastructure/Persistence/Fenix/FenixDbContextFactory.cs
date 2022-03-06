using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Fenix
{
    public class FenixDbContextFactory : DesignTimeDbContextFactoryBase<FenixDbContext>
    {
        protected override FenixDbContext CreateNewInstance(DbContextOptions<FenixDbContext> options)
        {
            return new FenixDbContext(options);
        }
    }
}
