# SAMU DataFlow 

Projeto em desenvolvimento para a disciplina de Programação Orientada a Objetos (POO) na **CESAR School**, em parceria com a Secretaria Executiva de Vigilância em Saúde (SESAU) / Secretaria de Saúde do Recife[cite: 1].

## Descrição

O SAMU DataFlow tem como objetivo automatizar a extração, consolidação e tratamento de dados das ocorrências do Serviço de Atendimento Móvel de Urgência (SAMU) do Recife[cite: 1]. A solução visa substituir a necessidade de exportações manuais e fragmentadas em planilhas `.xlsx`, transformando dados isolados em uma base de dados histórica unificada, validada e pronta para análises epidemiológicas[cite: 1].

O sistema será desenvolvido aplicando os conceitos de Programação Orientada a Objetos em C#, fornecendo uma plataforma centralizada para os técnicos da saúde e para o Centro de Informações Estratégicas em Vigilância em Saúde (CIE).

## Objetivos do Projeto

* Aplicar os conceitos de Programação Orientada a Objetos (POO) em C#[cite: 1].
* Automatizar a leitura e consolidação de planilhas de ocorrências em formato `.xlsx`[cite: 1].
* Identificar, tratar inconsistências e remover registros duplicados entre bases mensais[cite: 1].
* Registrar logs de execução e auditoria para monitoramento das rotinas de dados[cite: 1].
* Disponibilizar dados tratados para apoiar tomadas de decisão céleres e predições epidemiológicas pela SESAU/CIE[cite: 1].

## Tecnologias Planejadas

 C# (.NET)
 Visual Studio / JetBrains Rider
 Git e GitHub
Arquivos .xlsx / Banco de Dados Relacional

 ## 🏗️ Diagrama de Classes (UML)

```mermaid
classDiagram
    class ExtratorLegado {
        -periodoInicio: Date
        -periodoFim: Date
        -statusExecucao: String
        +executarExtracaoManual(): File
        +agendarExtracaoAutomatica(): void
    }

    class LeitorPlanilha {
        -caminhoArquivo: String
        -formatoValido: Boolean
        +carregarArquivoXlsx(arquivo: File): List~Ocorrencia~
        +validarEstrutura(): Boolean
    }

    class PipelineETL {
        -dataProcessamento: DateTime
        -registrosProcessados: int
        +consolidarBase(): void
        +removerDuplicados(dados: List~Ocorrencia~): List~Ocorrencia~
        +tratarInconsistencias(): void
    }

    class Ocorrencia {
        -idOcorrencia: String
        -dataHoraAtendimento: DateTime
        -bairro: String
        -tipoAgravo: String
        -gravidade: String
        -status: String
        +validarCamposObrigatorios(): Boolean
        +anonimizarDadosSensiveis(): void
    }

    class BaseHistorica {
        -totalRegistros: int
        -ultimaAtualizacao: DateTime
        +salvarOcorrencias(dados: List~Ocorrencia~): void
        +consultarOcorrencias(filtro: String): List~Ocorrencia~
        +exportarDadosEstruturados(formato: String): File
    }

    class GerenciadorLogs {
        -idLog: String
        -dataHora: DateTime
        -tipoEvento: String
        -mensagem: String
        +registrarLog(mensagem: String, tipo: String): void
        +enviarAlertaFalha(erro: String): void
    }

    ExtratorLegado ..> LeitorPlanilha : usa
    LeitorPlanilha -- PipelineETL
    PipelineETL *-- Ocorrencia
    PipelineETL *-- BaseHistorica
    PipelineETL ..> GerenciadorLogs : registra
