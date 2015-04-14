-- Quantos jogadores jogam por escalão?
SELECT joga.nome_escalao AS nome_escalao, COUNT(joga.bi) AS jogadores
FROM futebol.joga
GROUP BY joga.nome_escalao

-- Quantos dirigentes por escalão?
SELECT dirige.nome_escalao AS nome_escalao, COUNT(dirige.bi) AS dirigentes
FROM futebol.dirige
GROUP BY dirige.nome_escalao

-- Quantos socios têm lugar anual?
SELECT COUNT(DISTINCT lugar_anual.bi) AS socios_com_lugar_anual
FROM futebol.lugar_anual

-- Quantos socios têm quotas em dia e quantos não têm em dia?
SELECT socios_em_dia, socios_em_divida
FROM ((SELECT COUNT(socios.bi) AS socios_em_dia FROM futebol.socios WHERE socios.quotas_em_dia=1) AS tmp1
      UNION
      (SELECT count(socios.bi) AS socios_em_divida FROM futebol.socios WHERE socios.quotas_em_dia=0) AS tmp2
    ) as tmp

-- Nomes dos socios que não têm as quotas em dia
SELECT pessoa.nome
FROM (futebol.socios JOIN futebol.pessoa ON socios.bi=pessoa.bi)
WHERE socios.quotas_em_dia=0

-- Quantos funcionarios por departamento?
SELECT departamento.nome, COUNT(staff_clube.bi) AS funcionarios
FROM (futebol.departamento JOIN futebol.staff_clube
ON departamento.id_departamento=staff_clube.id_departamento)
GROUP BY departamento.nome

-- Quantos treinos por campo?
SELECT campo.local, COUNT(treino.data) AS treinos
FROM (futebol.treino JOIN futebol.campo ON treino.id_campo=campo.id_campo)
GROUP BY campo.local

-- Quantos treinos por dia?
SELECT treino.data, COUNT(treino.hora) AS treinos
FROM futebol.treino
GROUP BY treino.data

-- Salário máximo dos jogadores
SELECT MAX(pessoal_interno.salario) AS salario_max
FROM (futebol.pessoal_interno JOIN futebol.jogador ON pessoal_interno.bi=jogador.bi)

-- Total de salários dos jogadores
SELECT SUM(pessoal_interno.salario) AS total_salarios
FROM (futebol.pessoal_interno JOIN futebol.jogador ON pessoal_interno.bi=jogador.bi)

-- Jogador mais pesado
SELECT pessoa.nome
FROM (pessoa JOIN (futebol.jogador JOIN
  (SELECT MAX(jogador.peso) AS max_peso
   FROM futebol.jogador)
  as tmp ON jogador.peso = tmp.max_peso) ON pessoa.bi = jogador.bi)

-- Jogador mais alto
SELECT pessoa.nome
FROM (pessoa JOIN (futebol.jogador JOIN
  (SELECT MAX(jogador.altura) AS max_altura
   FROM futebol.jogador)
  as tmp ON jogador.peso = tmp.max_altura) ON pessoa.bi = jogador.bi)

-- Salário médio por nacionalidade
SELECT pessoa.nacionalidade, AVG(pessoal_interno.salario) AS salario_medio
FROM (futebol.pessoa JOIN futebol.pessoal_interno ON pessoa.bi=pessoal_interno.bi)
GROUP BY pessoa.nacionalidade

-- Do staff quem tem o salário mais alto
SELECT pessoa.nome
FROM (pessoa JOIN (futebol.jogador JOIN
  (SELECT MAX(pessoal_interno.salario) AS max_salario
   FROM (futebol.staff_clube JOIN futebol.pessoal_interno ON staff_clube.bi=pessoal_interno.bi))
  as tmp ON pessoal_interno.salario = tmp.max_salario) ON pessoa.bi = pessoal_interno.bi)

-- Média de salários por departamento
SELECT departamento.nome, AVG(pessoal_interno.salario)
FROM (futebol.pessoal_interno JOIN
        (futebol.staff_clube JOIN futebol.departamento
          ON staff_clube.id_departamento=departamento.id_departamento)
     ON pessoal_interno.bi=staff_clube.bi
     )
GROUP BY departamento.nome

-- Nº lugares por secção
SELECT lugar.id_seccao, COUNT(n_lugar)
FROM lugar
GROUP BY lugar.id_seccao

-- Nº de lugares anuais opr epoca
SELECT lugar_anual.epoca, COUNT(n_lugar)
FROM lugar_anual
GROUP BY lugar_anual.epoca
