# API Conway's Game of Life

Esta é uma implementação de uma API RESTful para simular o "Conway's Game of Life", como parte de um desafio de código.

## Descrição do Problema

A API deve permitir o upload de um estado de "board" (grid) inicial, e subsequentemente consultar os próximos estados da simulação, de acordo com as regras do Jogo da Vida.

### Requisitos Funcionais

1.  **[POST] /api/boards**: Faz upload de um novo estado de board (JSON `bool[][]`) e retorna um ID.
2.  **[GET] /api/boards/{id}/next**: Retorna o próximo estado de simulação para um board.
3.  **[GET] /api/boards/{id}/states/{x}**: Retorna o estado do board após `x` simulações.
4.  **[GET] /api/boards/{id}/final**: Retorna o estado final (estável) do board. Retorna um erro se não estabilizar após `X` tentativas.
5.  **Persistência**: O estado dos boards deve ser mantido mesmo se a API reiniciar.

## Explicação da Solução e Decisões de Arquitetura

A solução foi construída usando .NET 7 e segue os princípios da **Clean Architecture** para garantir modularidade, testabilidade e separação de conceitos (SoC).

* **`GameOfLife.Core`**: Contém a lógica de negócio pura (as regras do Jogo da Vida) e as entidades (`Board`). Não possui dependências de infraestrutura (web ou banco de dados).
* **`GameOfLife.Infrastructure`**: Implementa a persistência usando Entity Framework Core e um banco de dados SQLite (para portabilidade). Ele implementa a interface `IBoardRepository` definida no Core.
* **`GameOfLife.Api`**: Expõe a funcionalidade via Minimal APIs. É responsável pela Injeção de Dependência, tratamento de erros e roteamento.
* **`GameOfLife.Core.Tests`**: Testes unitários para a lógica de negócio, garantindo a corretude das regras de simulação.

## Suposições e Trade-offs 

* **Persistência do Estado**: Para persistir o `bool[][]` no banco de dados, utilizei um `ValueConverter` do EF Core para serializar o array para uma string JSON. Isso é simples e eficaz, mas menos "queryable" do que normalizar em tabelas `(Cell, X, Y)`.
* **Banco de Dados**: Usei **SQLite** para que o projeto rode "fora da caixa" sem precisar de um servidor de banco de dados. A troca para SQL Server é trivial (mudar o pacote NuGet e a connection string).
* **Performance**: O algoritmo de simulação é $O(r * c)$ para cada geração. Para boards gigantescos, otimizações (como "sparse matrix" ou "hashlife") seriam necessárias, mas estão fora do escopo deste desafio.

## Como Executar Localmente 

### Pré-requisitos
* .NET 7.0 SDK

### Passos

1.  Clone o repositório.
2.  Abra um terminal na pasta raiz da solução.
3.  **Execute as migrações do banco de dados (EF Core):**
    ```bash
    # Instale a ferramenta global do EF Core (se ainda não tiver)
    dotnet tool install --global dotnet-ef
    
    # Navegue até o projeto de infraestrutura
    cd src/GameOfLife.Infrastructure
    
    # Crie a migração inicial
    dotnet ef migrations add InitialCreate --startup-project ../GameOfLife.Api
    
    # Volte para a raiz
    cd ../..
    ```
4.  **Execute a API:**
    ```bash
    dotnet run --project src/GameOfLife.Api
    ```
5.  A API estará disponível em `http://localhost:<porta>`.
6.  Acesse `http://localhost:<porta>/swagger` para ver a documentação da API e testar os endpoints.

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