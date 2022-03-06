using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IEfectivoYaDoDbContext
    {
        DbSet<User> Users { get; set; }


        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        EntityEntry Entry([NotNullAttribute] object entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
    }
}
