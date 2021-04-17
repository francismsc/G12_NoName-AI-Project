# Projeto 1 de IA 2020/21
## Grupo 12 - NoName
### Autores
- Carolina Bastos nº: 22001952
  - A Carolina realizou o relatório.
  - Pensou nas melhores heurísticas para aplicar na IA.
  - Ajudou no processo de escolha do algoritmo.
- Daniela Gameiro nº: 21901681
  - A Daniela realizou o relatório.
  - Pensou nas melhores heurísticas para aplicar na IA.
  - Ajudou no processo de escolha do algoritmo.
- Francisco Costa nº: 21903228
  - O Francisco programou a IA.
  - Pensou no melhor algoritmo e heurística para aplicar na IA.
-----------
### Algoritmo
- Optamos por escolher o algoritmo Negamax com cortes Alfa-Beta.
- Este acabou por se revelar o melhor algoritmo tendo em conta a implementação no código e a sua eficácia em relação às outras opções (Minimax e MCTS).
- A função avalia da perspetiva de quem é a vez de jogar, ou seja, o sinal das pontuações do nível abaixo é invertido, podemos maximizar sempre.
- Os cortes Alfa-Beta consistem em dois parâmetros α = −∞ e β = +∞ (inicialização). Para todos os nós filhos, é necessário obter uma pontuação de modo recursivo, ou seja, trocando e invertendo os parâmetros atuais. Se o α ≥ β, existe um corte β.
- Foi apropriado o código do jogo do galo com o algoritmo Negamax com cortes Alfa-Beta fornecido pelo docente Nuno Fachada.
------------

### Heurísticas
#### Primeira Heurística
- 1 / (√(((X - Nl/2)^2) + (Y - Nc/2)^2)) + 0.00001) * 40
  - X = Linha do ponto observado no tabuleiro.
  - Y = Coluna do ponto observado no tabuleiro
  - Nl = Número de linhas do tabuleiro.
  - Nc = Número de colunas do tabuleiro.

- Esta fórmula atribui mais pontuação às peças quanto mais próximas do centro do tabuleiro estiverem.
- O "+ 0.00001" é para evitar que o divisor seja igual a zero.
- Retira pontuação as peças inimigas, isto é, a heurística acrescenta pontuação se for uma peça da perspetiva de quem está a jogar e retira se for uma peça inimiga.

#### Segunda Heurística
- Esta heurística vê a quantidade de peças da mesma cor e a quantidade de peças da mesma cor mais espaços em branco seguidos. Se a quantidade de peças mais espaços for maior ou igual à quantidade de peças necessárias para a vitória, atribui pontuação de duas formas diferentes.
- Numa das formas, vai ser adicionado a quantidade de peças da mesma cor mais os espaços em branco seguidos à pontuação total.
- Na outra forma, o total de peças da mesma cor seguidas é elevado ao quadrado e adicionado a pontuação final.
- Esta heurística é repetida para as formas. No entanto, as formas valem um pouco mais, visto que as formas têm prioridade. Isto é útil quando existe um empate entre formas e cores, pois as formas ganham nesse caso.
- A heurística é feita para todas a direções (vertical, horizontal, , diagonais, cima e baixo).
- A heurística repete para as peças inimigas, mas retira pontuação.

#### Terceira Heurística
- Guarda as duas últimas peças da mesma forma para jogadas decisivas (vitória ou derrota).
- A IA guarda um certo número de peças, para jogadas decisivas, consoante o tamanho do tabuleiro.
- Número de peças a guardar = Nl * Nc / 18
- Sempre que é jogada uma das peças guardadas, são retirados pontos. A quantidade de pontos retirados vai aumentado consoante a diferença entre o número de peças a guardar e o número de peças total no momento.
------------

### Referências
- PowerPoints dados em aula.
- TickTackToe por professor Nuno Fachada. 

