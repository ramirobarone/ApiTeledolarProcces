using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.EfectivoYaDo
{
    public class EfectivoYaDoDbContextFactory : DesignTimeDbContextFactoryBase<EfectivoYaDoDbContext>
    {
        protected override EfectivoYaDoDbContext CreateNewInstance(DbContextOptions<EfectivoYaDoDbContext> options)
        {
            return new EfectivoYaDoDbContext(options);
        }
    }
}
