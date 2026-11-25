# SAMU DataFlow

Sistema web de visualização, monitoramento e análise de dados operacionais do SAMU
Metropolitano do Recife. Projeto acadêmico da CESAR School (Projetos 2) em colaboração
com a SESAU Recife. Complementa o sistema existente, não o substitui.

> A aplicação inicia sem registros de demonstração. Importe os arquivos CSV autorizados na página **Importar dados**.

## Tecnologias

C# com .NET 10, ASP.NET Core, Blazor Web App (Interactive Server), EF Core com PostgreSQL.

## Estrutura

- `src/SAMUDataFlow.Domain`: entidades, enums e regras de negócio
- `src/SAMUDataFlow.Infrastructure`: EF Core, PostgreSQL e migrations
- `src/SAMUDataFlow.Web`: interface Blazor e API

## Banco de dados

1. Tenha o PostgreSQL instalado e rodando (porta 5432, usuário `postgres`).
2. Guarde a senha fora do código:

       dotnet user-secrets set "Banco:Senha" "SUA_SENHA" --project src/SAMUDataFlow.Web

3. Rode a aplicação. Em desenvolvimento ela cria o banco `samudataflow` e aplica as
   migrations, sem inserir dados automaticamente.
   Registros já existentes em um banco não são apagados na inicialização; use a opção de substituição na importação somente após conferir a base.

## Importação de dados

- A página **Importar dados** aceita pastas ou arquivos CSV (UTF-8), com até 50 MB por arquivo e 200 MB por importação.
- O importador reconhece separadores `;`, `,` e tabulação e permite mapear os cabeçalhos para os campos de ocorrência.
- Campos obrigatórios: código, data do chamado, distrito, bairro, tipo de agravo, gravidade e status. Chegada e coordenadas são opcionais.
- Colunas não mapeadas são ignoradas. Códigos repetidos e linhas inválidas são contabilizados no resumo.
- A opção para substituir a base exige confirmação e só apaga dados se houver pelo menos um registro válido para importar.
- Esta versão ainda não tem autenticação. Restrinja o acesso ao computador/rede antes de usar dados sensíveis.

## Como rodar

    dotnet run --project src/SAMUDataFlow.Web

Abra o endereço exibido no terminal.

## Evolução futura

- ETL / Processamento
- Logs de execução
- Importação de planilhas .xlsx
- Integração com o sistema legado
