using Microsoft.EntityFrameworkCore;
using SAMUDataFlow.Domain.Entidades;

namespace SAMUDataFlow.Infrastructure.Persistencia;

public sealed class SamuDbContext(DbContextOptions<SamuDbContext> opcoes) : DbContext(opcoes)
{
    public DbSet<Ocorrencia> Ocorrencias => Set<Ocorrencia>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SamuDbContext).Assembly);
}
