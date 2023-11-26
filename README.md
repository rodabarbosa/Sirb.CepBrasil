# Sirb.CepBrasil

O projeto tem como objetivo viabilizar uma interface para busca de logradouro por CEP.

Os serviços utilizados pelo projeto ***não*** é de responsabilidade e/ou mantido pelo mesmo.

## Execução do serviço

Os serviços externos utilizados são disponibilizados pelos Correios e ViaCep. Se a busca em ambos os serviços não tenha
resulta o serviço irá definir a busca como ***não sucesso***.A implementação desta interface funciona da seguinte forma:

### Fluxo Principal

1. O usuário instancia ICepService. O componente pode receber um objeto do tipo HttpClient ou o mesmo instancia e
   gerencia a utilização do mesmo;
1. O usuário informa o CEP para busca;
1. É efetuado uma validação de parâmetro minimo. (CEP deve conter 8 caracteres numericos);
1. A interface efetua a busca no serviço dos Correios;
    * Fluxo Alternativo - Logradouro não encontrado
    * Fluxo Alternativo - Falha na busca
1. A interface retorna retorna o resultado no objeto tipo CepResult.

### Fluxo Alternativo

#### Logradouro não encontrado

1. Busca no serviço dos Correios não houver um retorno;
1. A interface efetua a busca no serviço de ViaCep;
    * Fluxo Alternativo - Falha na busca
1. A interface retorna retorna o resultado no objeto tipo CepResult.

#### Falha na busca

1. Toda falha na busca é preenchido a propriedade CepResult.Message com a mensagem da falha;
1. Caso exista uma exceção na execução esta será inclusa na lista de exceções, CepResult.Exceptions;
1. A interface retorna retorna o resultado no objeto tipo CepResult.

## Composição

### CepResult

Success: bool
CepContainer: CepContainer
Message: string
Exceptions: List<`Exception`>

### CepContainer

Uf: string
Cidade: string
Bairro: string
Complemento: string
Cep: string

## Informações Adicionais

Para informações sobre o serviço dos Correios, visite <https://www.correios.com.br/enviar-e-receber/precisa-de-ajuda/>
ou <https://www.correios.com.br/enviar-e-receber/precisa-de-ajuda/Manual_de_Implementacao_do_Web_Service_SIGEP_WEB.pdf>

Para informações sobre ViaCEP, visite <https://viacep.com.br/>;

## Nota

Os demais métodos disponibilizados pelo serviço dos Correios no qual é necessário cadastro de usuário e todo o processo
que envolva ou relacione a cadastro de usuário não será disponível nesta interface.

## Versões 1.3

- Inclusão de compatibilidade com .NET 8.
- Remoção do serviço dos correios. Em estudo para entrega de dados requeridos para o serviço de correios, enquanto não houver 100% para a solução o serviço ficará indisponível através este projeto.
- Inclusão de chamada asincrona com utilização do token de cancelamento.
- Ajuste para performance

## Versões 1.2

Bug fix e melhora da performance

## Versões 1.1

Bug fix

## Versão  1.0.3

Inclusão de compatibilidade com .NET 6 e 7.

## Versão 1.0.2

Limpeza do retorno para remoção de caracteres indesejados.

## Versão 1.0.1

Inclusão de compatibilidade com dotnet core 3 e 3.1.

### Versão 1.0.0

Disponibilização da interface para a versão dotnet 5.
