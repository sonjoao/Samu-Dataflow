using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAMUDataFlow.Domain.Entidades;

namespace SAMUDataFlow.Infrastructure.Persistencia.Configuracoes;

public sealed class EventoHistoricoConfiguracao : IEntityTypeConfiguration<EventoHistorico>
{
    public void Configure(EntityTypeBuilder<EventoHistorico> b)
    {
        b.ToTable("eventos_historicos");
        b.HasKey(x => x.Id);
        b.Property(x => x.TipoRegistro).IsRequired().HasMaxLength(40);
        b.Property(x => x.CodigoOcorrencia).HasMaxLength(100);
        b.Property(x => x.Acao).IsRequired().HasMaxLength(60);
        b.Property(x => x.Responsavel).IsRequired().HasMaxLength(100);
        b.Property(x => x.Detalhes).IsRequired().HasMaxLength(500);
        b.Property(x => x.CriadoEmUtc).HasColumnType("timestamp with time zone");
        b.HasIndex(x => x.CriadoEmUtc);
        b.HasIndex(x => x.CodigoOcorrencia);
    }
}
