#Introduction
TODO: Give a short introduction of your project. Let this section explain the objectives or the motivation behind this project. 

#Getting Started
TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
1.	Installation process
2.	Software dependencies
3.	Latest releases
4.	API references

#Build and Test
TODO: Describe and show how to build your code and run the tests. 

#C++ guidelines
1. const / const& nu are sens pentru tipuri de baza - so const int&; cont int e pointless
2. cand dati parametri (de input) liste, obiecte, etc e important sa avem const&, pentru ca altfel se apeleaza copy contructorul si e fie unsafe, fie slow
3. Metodele care nu modifica starea sa fie const e.g. getter-ele
4. Metodele care nu au legatura cu clasa sa le faceti statice e.g. ChessBoardImpl::IsValid
5. Unless imperios necesar, fara pointeri! stiva(pe oriunde se poate) sau shared_ptr

