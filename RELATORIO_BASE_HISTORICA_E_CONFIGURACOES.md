# Relatório de melhorias — Base Histórica e Configurações

**Projeto:** SAMU DataFlow

**Data:** 26/11/2026

**Escopo:** implementar as áreas Base Histórica e Configurações, conservar dados em substituições de importação, registrar atividade administrativa e revisar compilação, testes e migration.

## 1. Base Histórica

A rota `/historico` deixou de exibir a página em construção e agora oferece duas consultas somente leitura:

- **Ocorrências:** consulta registros atuais e arquivados por intervalo de datas, código, bairro e status. A lista é paginada em grupos de 25; o detalhe mostra os campos operacionais e complementares já existentes, inclusive desfecho, hospital, anotações, acompanhante e observações extras.
- **Atividade registrada:** exibe data/hora, área, operação, código da ocorrência quando aplicável, responsável e resumo da ação. É possível filtrar por período e código, com limite de 200 eventos por consulta.

Os perfis Administrador 1 e Administrador 2 podem consultar o histórico, de acordo com o acesso de leitura existente. A tela não oferece operações de alteração.

### Preservação dos registros

A opção de substituir dados na importação agora marca os registros ativos como arquivados e inclui o novo lote na mesma transação. Os registros anteriores continuam consultáveis. A listagem operacional, o painel, o mapa e os relatórios operacionais usam apenas ocorrências ativas; a Base Histórica mostra ambas as coleções. Um índice único parcial mantém códigos únicos entre ocorrências ativas e permite que o mesmo código reapareça em um novo lote depois que sua versão anterior foi arquivada.

Registros existentes recebem `EstaArquivada = false` na migration, sem perder conteúdo. O histórico de atividades começa com a implantação: não há como reconstruir retroativamente quem fez alterações antes de a trilha existir.

## 2. Trilha de atividade

A entidade `EventoHistorico` persiste os eventos em `eventos_historicos`, com horário UTC, área, id/código relacionado quando aplicável, ação, responsável e resumo. A trilha recebe eventos para:

- criação e edição dos detalhes de ocorrências;
- importação ou substituição da base, com totais de carregados, arquivados e ignorados;
- criação, ativação, alteração de perfil e remoção de contas;
- alteração das configurações do sistema.

Os resumos não incluem anotações clínicas, informações de acompanhante, senhas, hashes ou valores de credenciais. O evento da substituição resume a operação e a quantidade arquivada; os registros em si permanecem na tabela de ocorrências.

## 3. Configurações

A rota `/configuracoes` agora oferece configurações persistentes para:

- nome do serviço;
- município e UF exibidos no rodapé do sistema;
- intervalo inicial da Base Histórica: 30, 90, 180 ou 365 dias.

Os campos são validados no domínio. A alteração grava o novo estado e um evento de auditoria. Configurações são exclusivas do Administrador 2: o menu é ocultado para os demais perfis, a rota exige o papel e a ação de gravação confere novamente o papel atual na tabela de usuários.

A tela também reúne o controle de tema já existente (claro/escuro) e um resumo da matriz de acesso. A preferência de tema continua salva no cookie do navegador, enquanto as configurações de serviço e do período histórico ficam no PostgreSQL.

## 4. Persistência e compatibilidade

A migration `20261126200613_HistoricoConfiguracoesArquivo`:

1. cria `eventos_historicos` e seus índices por data e código;
2. cria `configuracoes_sistema` com a configuração inicial Recife/PE, SAMU DataFlow e 30 dias;
3. adiciona `EstaArquivada` às ocorrências com valor padrão falso;
4. troca o índice de código externo por um índice único parcial que considera apenas registros ativos.

A migration foi aplicada com êxito ao banco PostgreSQL local configurado pelo projeto. O contexto e o snapshot do EF Core não apresentam alterações pendentes. Em novos ambientes, a aplicação em desenvolvimento executa migrations ao iniciar; em produção, execute:

```powershell
dotnet ef database update --project src/SAMUDataFlow.Infrastructure --startup-project src/SAMUDataFlow.Web
```

## 5. Organização e autorização

- O item Configurações agora só aparece para Administrador 2. A proteção também ocorre no acesso à rota e antes de persistir alterações.
- A Base Histórica mantém leitura para os dois perfis; todos os controles apresentados nela são de consulta.
- Operações administrativas relevantes passaram a compor a trilha, com resumos limitados e sem conteúdo clínico.
- O município/UF definido em Configurações é refletido no rodapé do layout.
- A importação não apaga registros históricos ao substituir o conjunto ativo.
- O README foi ampliado com comportamento, permissões, preservação e instruções de migration.

## 6. Arquivos alterados

- Domínio: `ConfiguracaoSistema.cs`, `EventoHistorico.cs`, `Ocorrencia.cs`.
- Persistência: `SamuDbContext.cs`, mapeamentos de configuração e a migration/snapshot do EF Core.
- Interface: páginas `Historico.razor` e `Configuracoes.razor`, layout, ocorrências, importação, usuários, painel, mapa e relatórios.
- Apresentação: `wwwroot/css/app.css`.
- Qualidade/documentação: `OcorrenciaEUsuarioTests.cs`, `README.md` e este relatório.

## 7. Verificações executadas

| Verificação | Resultado |
| --- | --- |
| Build da solução .NET 10 | Sucesso; 0 avisos e 0 erros |
| Testes automatizados existentes e novos | 8 aprovados; 0 falhas |
| EF Core `has-pending-model-changes` | Nenhuma alteração pendente no snapshot |
| Aplicação da migration ao PostgreSQL local | Concluída com êxito |
| `git diff --check` | Sem erros de whitespace; Git informou apenas conversão normal de LF para CRLF no Windows |

Os testes de unidade cobrem preservação de uma ocorrência arquivada, validação de período/configuração e validação de responsável em evento histórico, além dos testes de domínio já existentes. A compilação também verificou as páginas Razor. A trilha administrativa e as telas não foram testadas por navegação autenticada automatizada.
