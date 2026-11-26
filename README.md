# SAMU DataFlow

Sistema web de visualização, monitoramento e análise de dados operacionais do SAMU Metropolitano do Recife. Projeto acadêmico da CESAR School em colaboração com a SESAU Recife.

## Tecnologias e estrutura

- C#/.NET 10, ASP.NET Core, Blazor Web App e EF Core com PostgreSQL.
- `src/SAMUDataFlow.Domain`: entidades, enums e regras de negócio.
- `src/SAMUDataFlow.Infrastructure`: persistência e migrations.
- `src/SAMUDataFlow.Web`: interface, autenticação e operações.

## Banco de dados

1. Tenha o PostgreSQL disponível (porta 5432, usuário `postgres`).
2. Guarde a senha do banco fora do código:

       dotnet user-secrets set "Banco:Senha" "<senha do banco>" --project src/SAMUDataFlow.Web

3. Em desenvolvimento, a aplicação cria o banco `samudataflow` e aplica migrations.
   Nenhuma ocorrência de demonstração é inserida. Registros existentes são mantidos.
4. Em produção, aplique as migrations antes de iniciar a aplicação.

## Acesso e perfis

A aplicação exige autenticação. Não há senha padrão nem conta de demonstração. Para
criar a primeira conta **Administrador 2**, configure um login e uma senha forte fora
do código antes do primeiro início:

    dotnet user-secrets set "SamuAdmin:Login" "<login inicial>" --project src/SAMUDataFlow.Web
    dotnet user-secrets set "SamuAdmin:Senha" "<senha forte>" --project src/SAMUDataFlow.Web
    dotnet user-secrets set "SamuAdmin:Nome" "<nome>" --project src/SAMUDataFlow.Web
    dotnet user-secrets set "SamuAdmin:Setor" "<setor>" --project src/SAMUDataFlow.Web

Em produção, forneça `SamuAdmin__Login`, `SamuAdmin__Senha`, `SamuAdmin__Nome` e
`SamuAdmin__Setor` pelo gerenciador de segredos do ambiente. A conta é criada apenas
se o login configurado ainda não existir; remova os segredos de bootstrap após o
primeiro início. Contas antigas recebem **Administrador 1** na migration e não são
promovidas. Como o sistema anterior não autenticava essas contas, um administrador
deve criar novas credenciais para elas pela tela **Usuários**, se necessário.

| Perfil | Permissões |
| --- | --- |
| Administrador 1 | Consulta as telas e ocorrências. Não cria nem edita registros, não importa dados e não administra contas. |
| Administrador 2 | Acesso completo: cria/edita ocorrências, importa ou substitui dados e administra contas e perfis. |

O Administrador 2 cria usuários e atribui um dos dois papéis em **Usuários**. Senhas
iniciais precisam ter pelo menos 12 caracteres e são armazenadas com hash. A sessão
usa cookie `HttpOnly`, `Secure` e `SameSite=Strict`, com validade de até 8 horas.

## Detalhes da ocorrência

Os campos opcionais de anotações dos paramédicos, desfecho, hospital de destino,
observações extras e acompanhante aparecem no cadastro, edição e detalhe. Os desfechos
são: encaminhado para hospital, atendido no local, recusa, óbito, não localizado,
cancelada e outro. Registros existentes permanecem válidos com esses campos vazios.

## Importação de dados

- A página **Importar dados** aceita o arquivo SQL PostgreSQL `samu_mock_postgres.sql` (até 50 MB).
- O importador lê somente os `INSERT`s reconhecidos da tabela `samu_ocorrencia_bronze`; não executa o script SQL.
- Dados pessoais e clínicos descartados pelo importador não são carregados.
- Códigos duplicados e linhas inválidas são ignorados e contabilizados.
- Substituir a base exige confirmação e só ocorre quando há ao menos um registro válido.
- A consulta da página está disponível ao Administrador 1; selecionar e importar arquivos é restrito ao Administrador 2.

## Como rodar

    dotnet run --project src/SAMUDataFlow.Web

Abra o endereço exibido no terminal. Configure a conta inicial antes de acessar o sistema.
