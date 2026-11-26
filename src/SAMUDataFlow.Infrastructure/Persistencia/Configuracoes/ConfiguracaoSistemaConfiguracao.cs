using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAMUDataFlow.Domain.Entidades;

namespace SAMUDataFlow.Infrastructure.Persistencia.Configuracoes;

public sealed class ConfiguracaoSistemaConfiguracao : IEntityTypeConfiguration<ConfiguracaoSistema>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoSistema> b)
    {
        b.ToTable("configuracoes_sistema");
        b.HasKey(x => x.Id);
        b.Property(x => x.NomeServico).IsRequired().HasMaxLength(100);
        b.Property(x => x.Municipio).IsRequired().HasMaxLength(100);
        b.Property(x => x.UnidadeFederativa).IsRequired().HasMaxLength(2);
        b.Property(x => x.DiasPadraoHistorico).IsRequired();
        b.Property(x => x.AtualizadoEmUtc).HasColumnType("timestamp with time zone");
        b.Property(x => x.AtualizadoPor).IsRequired().HasMaxLength(100);
        b.HasData(new
        {
            Id = ConfiguracaoSistema.IdUnico,
            NomeServico = "SAMU DataFlow",
            Municipio = "Recife",
            UnidadeFederativa = "PE",
            DiasPadraoHistorico = ConfiguracaoSistema.PeriodoHistoricoPadrao,
            AtualizadoEmUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            AtualizadoPor = "Sistema"
        });
    }
}
