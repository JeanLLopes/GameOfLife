# API Conway's Game of Life

Esta � uma implementa��o de uma API RESTful para simular o "Conway's Game of Life", como parte de um desafio de c�digo.

## Descri��o do Problema

A API deve permitir o upload de um estado de "board" (grid) inicial, e subsequentemente consultar os pr�ximos estados da simula��o, de acordo com as regras do Jogo da Vida.

### Requisitos Funcionais

1.  **[POST] /api/boards**: Faz upload de um novo estado de board (JSON `bool[][]`) e retorna um ID.
2.  **[GET] /api/boards/{id}/next**: Retorna o pr�ximo estado de simula��o para um board.
3.  **[GET] /api/boards/{id}/states/{x}**: Retorna o estado do board ap�s `x` simula��es.
4.  **[GET] /api/boards/{id}/final**: Retorna o estado final (est�vel) do board. Retorna um erro se n�o estabilizar ap�s `X` tentativas.
5.  **Persist�ncia**: O estado dos boards deve ser mantido mesmo se a API reiniciar.

## Explica��o da Solu��o e Decis�es de Arquitetura

A solu��o foi constru�da usando .NET 7 e segue os princ�pios da **Clean Architecture** para garantir modularidade, testabilidade e separa��o de conceitos (SoC).

* **`GameOfLife.Core`**: Cont�m a l�gica de neg�cio pura (as regras do Jogo da Vida) e as entidades (`Board`). N�o possui depend�ncias de infraestrutura (web ou banco de dados).
* **`GameOfLife.Infrastructure`**: Implementa a persist�ncia usando Entity Framework Core e um banco de dados SQLite (para portabilidade). Ele implementa a interface `IBoardRepository` definida no Core.
* **`GameOfLife.Api`**: Exp�e a funcionalidade via Minimal APIs. � respons�vel pela Inje��o de Depend�ncia, tratamento de erros e roteamento.
* **`GameOfLife.Core.Tests`**: Testes unit�rios para a l�gica de neg�cio, garantindo a corretude das regras de simula��o.

## Suposi��es e Trade-offs 

* **Persist�ncia do Estado**: Para persistir o `bool[][]` no banco de dados, utilizei um `ValueConverter` do EF Core para serializar o array para uma string JSON. Isso � simples e eficaz, mas menos "queryable" do que normalizar em tabelas `(Cell, X, Y)`.
* **Banco de Dados**: Usei **SQLite** para que o projeto rode "fora da caixa" sem precisar de um servidor de banco de dados. A troca para SQL Server � trivial (mudar o pacote NuGet e a connection string).
* **Performance**: O algoritmo de simula��o � $O(r * c)$ para cada gera��o. Para boards gigantescos, otimiza��es (como "sparse matrix" ou "hashlife") seriam necess�rias, mas est�o fora do escopo deste desafio.

## Como Executar Localmente 

### Pr�-requisitos
* .NET 7.0 SDK

### Passos

1.  Clone o reposit�rio.
2.  Abra um terminal na pasta raiz da solu��o.
3.  **Execute as migra��es do banco de dados (EF Core):**
    ```bash
    # Instale a ferramenta global do EF Core (se ainda n�o tiver)
    dotnet tool install --global dotnet-ef
    
    # Navegue at� o projeto de infraestrutura
    cd src/GameOfLife.Infrastructure
    
    # Crie a migra��o inicial
    dotnet ef migrations add InitialCreate --startup-project ../GameOfLife.Api
    
    # Volte para a raiz
    cd ../..
    ```
4.  **Execute a API:**
    ```bash
    dotnet run --project src/GameOfLife.Api
    ```
5.  A API estar� dispon�vel em `http://localhost:<porta>`.
6.  Acesse `http://localhost:<porta>/swagger` para ver a documenta��o da API e testar os endpoints.

---
### (Opcional) Executar com Docker

1.  Build a imagem:
    ```bash
    docker build -t gameoflife-api .
    ```
2.  Execute o container:
    ```bash
    docker run -p 8080:80 -d gameoflife-api
    ```
3.  Acesse `http://localhost:8080/swagger`.