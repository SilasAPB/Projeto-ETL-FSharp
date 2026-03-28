# Projeto de ETL em F#
### Projeto desenvolvido para a disciplina de Programação Funcional, eletiva de Computação no Insper

- Os passos abaixo servem como um roteiro de todo o projeto, documentando o que foi feito ao longo de seu desenvolvimnto


## Definição do Projeto

- O projeto consiste na aplicação do fluxo ETL para captura, transformação e entrega de dados, trabalhando a aplicação real dentro do contexto de aprendizado utilizando a linguagem funcional F#
- O projeto foi separado em três grandes seções, ETL, ETL.Tests e Main:
  > Main: Que é onde o projeto de fato roda e é o contato do programa o mundo exterior e é onde foi projetada as funções impuras, que utilizam de recursos não funcionais para interagir com arquivos, links e banco de dados por meio de integrações e biblioteca

  > ETL: É o cerne do nosso projeto pois é onde toda a lógica operacional funcional ocorre, com funções e tipos para receber e tratar os dados, é onde ocorre a transformção para um produto final, a Main chama os módulos dessa seção quando necessário na aplicação geral.

  > ETL.Tests: A seção de testes do projeto, que testas as funções puras para garantir o funcionamento tanto em uma situação favoravel mas a boa execução de exceptions em caso de erro.

## Fluxo do Projeto

- O começo do projeto foi arquiteturar toda essa separação, criando os subprojetos listados acima para enfim trabalhar neles.
- Logo após, por recomendação do Professor criei os tipos necessários que seriam utilizados pelo menos na lógica básica, sem considerar as considerações adicionais, que futuramente seriam adicionadas

    > Order e Order_Item: era preciso criar os tipos para ler cada linha dos dados e entender como uma coisa só, além disso, para algumas características como 'status' foi necessária criar um tipo só para aquela feature, já que não se tinha um "tipo" pré-estabelecido como a feature preço por exemplo

- Logo após isso foi necessário criar as funções para ler os dados, e para isso foi necessário criar uma função de leitura de CSV, que é o formato dos dados, e depois criar uma função específica para ler os dados de pedidos, que utiliza a função de leitura de CSV e transforma os dados em um array do tipo Order_Item, para isso foi necessário criar uma função de parsing de string para os tipos específicos, como decimal e int, além de criar uma função semelhante de parsing para o tipo OrderStatus.

    > Ou seja era um desenvolvimento paralelo entre as funções impuras de Main e as funções puras no arquvio HelperFunctions.fs que fariam o parsing e encaixariam cada linha lida em dados para serem utilizados de forma funcional

- E então foi desenvolvido no HelperFunctions.fs outro modulo que seria o de transformação, usar os dados já tipados da maneira desejada para transforma-los no produto desejado pelo hipotético cliente

    > Para isso foi necessário criar um tipo para o produto final, que seria o OrderSummary, e depois criar uma função que recebe um array de Order_Item e transforma em um array de OrderSummary, para isso foi necessário utilizar funções de agrupamento e agregação, como groupBy e map, para agrupar os itens por OrderID e depois calcular o total de cada pedido.

- Por fim, foi necessário criar a função de escrita de CSV, para escrever o resultado final em um arquivo, essa parte, por ser de escrita foi desenvolvida na Main, já que se trata de uma função impura. Nesse ponto do projeto, separei as funções de escrita e leitura de arquivos em um módulo específico 'Extract' e 'Load', e então temos cada parte do projeto separada com o modulo 'Transform' dentro da parte pura do projeto.


- O projeto então tinha a sua funcionalidade básica completa e aí foi adicionada as features extras para garantir melhor pontuação, há cada alteração era testado o funcionamento do projeto e enviado um commit por alteração, para garantir um histórico de desenvolvimento.

### As features extras foram:
> Leitura de arquivos de fontes HTTP: onde foi necessário utilizar o pacote System.Net.Http para ler os arquivos de uma URL.

> Documentação utilizando docstrings: onde foi necessário adicionar uma documentação para cada função, explicando o que ela faz, seus parâmetros e seu retorno.

> Testes unitários utilizando o framework Expecto: onde foi necessário criar testes para as funções puras, garantindo o funcionamento correto do projeto e a boa execução de exceptions em caso de erro.

> Organizar o projeto utilizando um projeto .Net: algo realizado desde o início, mas que foi necessário organizar melhor com a adição das features extras, criando pastas específicas para cada tipo de função e módulo.

> Realizar uma saída adicional para um arquivo CSV com um resumo mensal dos pedidos, onde foi necessário criar uma função de transformação para agrupar os pedidos por mês e calcular a média de receita e impostos, além de criar uma função de escrita para escrever esse resumo em um arquivo CSV, para isso também foi necessário desenvolver mais testes e criar um tipo específico para o resumo mensal, o MonthlySummary.

> Utilizar do Inner Join para realizar a transformação dos dados, o que incrementou eficiência do projeto, já que o join é uma operação mais eficiente do que o agrupamento e agregação manual, para isso foi necessário criar uma função de inner join para juntar os dados de Order_Item com os dados de OrderSummary, utilizando o OrderID como chave de junção.




### Nota Final: Sobre o Uso de IA

- Foi utlizado alguns modelos de IA durante o projeto que atuaram principalemnte em me ajudar a arquiteturar tudo, o que fazer antes ou depois, por onde começar e quais features adicionar primeiro. Além disso foi bem utilizada na criação de testes e na correção de erros durante o desenvolvimento de funções.
