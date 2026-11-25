using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAMUDataFlow.Domain.Entidades;

namespace SAMUDataFlow.Infrastructure.Persistencia.Configuracoes;

public sealed class OcorrenciaConfiguracao : IEntityTypeConfiguration<Ocorrencia>
{
    public void Configure(EntityTypeBuilder<Ocorrencia> b)
    {
        b.ToTable("ocorrencias");
        b.HasKey(x => x.Id);

        b.Property(x => x.CodigoExterno).IsRequired().HasMaxLength(100);
        b.Property(x => x.Distrito).IsRequired().HasMaxLength(100);
        b.Property(x => x.Bairro).IsRequired().HasMaxLength(100);

        // Enums gravados como texto: legíveis no banco e imunes a mudança de ordem.
        b.Property(x => x.TipoAgravo).HasConversion<string>().HasMaxLength(30);
        b.Property(x => x.Gravidade).HasConversion<string>().HasMaxLength(30);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);

        b.Property(x => x.DataChamado).HasColumnType("timestamp with time zone");
        b.Property(x => x.DataChegada).HasColumnType("timestamp with time zone");
        b.Property(x => x.RegistradoEm).HasColumnType("timestamp with time zone");

        b.HasIndex(x => x.CodigoExterno).IsUnique();
        b.HasIndex(x => x.DataChamado);
        b.HasIndex(x => x.Bairro);
    }
}
