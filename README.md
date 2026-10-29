## Descrição

O SAMU DataFlow tem como objetivo automatizar a extração, consolidação e tratamento de dados das ocorrências do Serviço de Atendimento Móvel de Urgência (SAMU) do Recife. A solução visa substituir a necessidade de exportações manuais e fragmentadas em planilhas .xlsx, transformando dados isolados em uma base de dados histórica unificada, validada e pronta para análises epidemiológicas.

O sistema será desenvolvido aplicando os conceitos de Programação Orientada a Objetos em C#, fornecendo uma plataforma centralizada para os técnicos da saúde e para o Centro de Informações Estratégicas em Vigilância em Saúde (CIE).

## Objetivos do Projeto

* Aplicar os conceitos de Programação Orientada a Objetos (POO) em C#.
* Automatizar a leitura e consolidação de planilhas de ocorrências em formato .xlsx
* Identificar, tratar inconsistências e remover registros duplicados entre bases mensais.
* Registrar logs de execução e auditoria para monitoramento das rotinas de dados.
* Disponibilizar dados tratados para apoiar tomadas de decisão céleres e predições epidemiológicas pela SESAU/CIE.

## Tecnologias

- C# / .NET (ASP.NET Core Web API, Worker Service)
- Playwright para .NET (robô de extração)
- Entity Framework Core + PostgreSQL
- Quartz.NET (agendamento) e Serilog (logs)
- xUnit (testes)

 ## 🏗️ Diagrama de Classes (UML) Atualizado

```mermaid
classDiagram
    class AgendadorExtracao {
        -expressaoCron: String
        -tamanhoLote: int
        -tentativasMaximas: int
        +atualizarConfiguracao() void
        +dispararExecucao() void
    }
    class ExtratorLegado {
        -periodoInicio: Date
        -periodoFim: Date
        -statusExecucao: String
        +executarExtracaoManual() File
        +agendarExtracaoAutomatica() void
        +preencherPeriodo() void
        +lerTotalDoLegado() int
    }
    class ServicoCriptografia {
        +criptografarArquivo(caminho: String) File
        +descriptografarArquivo(caminho: String) File
    }
    class LeitorPlanilha {
        -caminhoArquivo: String
        -formatoValido: Boolean
        +carregarArquivoXlsx(arquivo: File) List~Ocorrencia~
        +validarEstrutura() Boolean
    }
    class PipelineETL {
        -dataProcessamento: DateTime
        -registrosProcessados: int
        +consolidarBase() void
        +removerDuplicados(dados: List~Ocorrencia~) List~Ocorrencia~
        +tratarInconsistencias() void
    }
    class Pseudonimizador {
        +pseudonimizar(valor: String) String
    }
    class Ocorrencia {
        -idOcorrencia: String
        -codigoChamado: String
        -numeroRegistroICard: String
        -dataHoraAtendimento: DateTime
        -bairro: String
        -endereco: String
        -observacoes: String
        -tipoAgravo: String
        -gravidade: String
        -status: String
        +validarCamposObrigatorios() Boolean
        +anonimizarDadosSensiveis() void
    }
    class BaseHistorica {
        -totalRegistros: int
        -ultimaAtualizacao: DateTime
        +salvarOcorrencias(dados: List~Ocorrencia~) void
        +consultarOcorrencias(filtro: String) List~Ocorrencia~
        +exportarDadosEstruturados(formato: String) File
    }
    class ConferenciaContagem {
        -totalLegado: int
        -totalImportado: int
        +conferir() Boolean
        +gerarAlerta() void
    }
    class ServicoBusca {
        +buscar(termo: String, filtros: String) List~Ocorrencia~
        +sugerir(prefixo: String) List~String~
    }
    class Usuario {
        -email: String
        -papel: String
        +autenticar(senha: String) Boolean
    }
    class GerenciadorLogs {
        -idLog: String
        -dataHora: DateTime
        -tipoEvento: String
        -mensagem: String
        +registrarLog(mensagem: String, tipo: String) void
        +registrarAuditoria(usuario: Usuario, acao: String) void
    }

    AgendadorExtracao --> ExtratorLegado : aciona
    ExtratorLegado --> ServicoCriptografia : cifra arquivos
    ExtratorLegado --> LeitorPlanilha : usa
    LeitorPlanilha --> ServicoCriptografia : decifra
    LeitorPlanilha --> PipelineETL : entrega ocorrencias
    PipelineETL --> Pseudonimizador : usa
    PipelineETL --> BaseHistorica : grava
    BaseHistorica "1" *-- "*" Ocorrencia : contem
    ConferenciaContagem --> ExtratorLegado : total do legado
    ConferenciaContagem --> BaseHistorica : total importado
    ServicoBusca --> BaseHistorica : consulta
    Usuario --> ServicoBusca : usa
    PipelineETL ..> GerenciadorLogs : registra
    ConferenciaContagem ..> GerenciadorLogs : registra
    ServicoBusca ..> GerenciadorLogs : audita
```
