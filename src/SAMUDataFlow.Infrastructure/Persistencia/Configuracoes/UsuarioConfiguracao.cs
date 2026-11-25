using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAMUDataFlow.Domain.Entidades;

namespace SAMUDataFlow.Infrastructure.Persistencia.Configuracoes;

public sealed class UsuarioConfiguracao : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> b)
    {
        b.ToTable("usuarios");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).IsRequired().HasMaxLength(100);
        b.Property(x => x.Setor).IsRequired().HasMaxLength(100);
        b.Ignore(x => x.Iniciais);
    }
}
