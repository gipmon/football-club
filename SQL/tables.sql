CREATE SCHEMA futebol;

-- PESSOA
CREATE TABLE futebol.pessoa(
    bi INT PRIMARY KEY CHECK(bi>0),
    nome VARCHAR(75) NOT NULL,
    endereco VARCHAR(75) NOT NULL,
    data_nasc DATE NOT NULL,
    nif INT UNIQUE NOT NULL,
	nacionalidade VARCHAR(75) NOT NULL
);

-- PESSOAL INTERNO
CREATE TABLE futebol.pessoal_interno(
    bi INT PRIMARY KEY,
    salario MONEY NOT NULL CHECK (salario >= 0),
    id_interno INT UNIQUE NOT NULL
);

-- ALTERs PESSOAL INTERNO
ALTER TABLE futebol.pessoal_interno ADD CONSTRAINT FORPIBIPBI
FOREIGN KEY (bi) REFERENCES futebol.pessoa(bi)
ON UPDATE CASCADE;

-- SOCIOS
CREATE TABLE futebol.socios(
    bi INT PRIMARY KEY,
    n_socio INT NOT NULL UNIQUE CHECK(n_socio >= 0),
    quotas_em_dia BIT NOT NULL,
    valor_das_quotas MONEY NOT NULL CHECK (valor_das_quotas >= 0),
);

-- ALTERs SOCIOS
ALTER TABLE futebol.socios ADD CONSTRAINT FORSBIPBI
FOREIGN KEY (bi) REFERENCES futebol.pessoa(bi)
ON UPDATE CASCADE;

-- JOGADOR
CREATE TABLE futebol.jogador(
    bi INT PRIMARY KEY,
    id_federacao INT UNIQUE NOT NULL CHECK(id_federacao >= 0),
    peso int NOT NULL CHECK(peso > 0),
    altura int NOT NULL CHECK(altura > 0)
);

-- JOGADOR ALTERs
ALTER TABLE futebol.jogador ADD CONSTRAINT FORJBIPIBI
FOREIGN KEY (bi) REFERENCES futebol.pessoal_interno(bi)
ON UPDATE CASCADE;

-- TECNICO
CREATE TABLE futebol.tecnico(
    bi INT PRIMARY KEY,
    id_federacao INT NOT NULL CHECK(id_federacao >= 0),
    funcao VARCHAR(50) NOT NULL
);

-- TECNICO ALTERs
ALTER TABLE futebol.tecnico ADD CONSTRAINT FORTBIPIBI
FOREIGN KEY (bi) REFERENCES futebol.pessoal_interno(bi)
ON UPDATE CASCADE;

-- STAFF CLUBE
CREATE TABLE futebol.staff_clube(
    bi INT PRIMARY KEY,
    cargo VARCHAR(50) NOT NULL,
    id_departamento INT NOT NULL
);

-- STAFF ALTERs
ALTER TABLE futebol.staff_clube ADD CONSTRAINT FORSCBIPIBI
FOREIGN KEY (bi) REFERENCES futebol.pessoal_interno(bi)
ON UPDATE CASCADE;

-- DEPARTAMENTO
CREATE TABLE futebol.departamento(
    id_departamento INT PRIMARY KEY,
    endereco VARCHAR(75) NOT NULL,
    nome VARCHAR(75) NOT NULL
);

-- STAFF ALTERs
ALTER TABLE futebol.staff_clube ADD CONSTRAINT FORSCIDDID
FOREIGN KEY (id_departamento) REFERENCES futebol.departamento(id_departamento)
ON UPDATE CASCADE;

-- ESCALÃO
CREATE TABLE futebol.escalao(
    nome VARCHAR(50) PRIMARY KEY,
    idade_max INT NOT NULL CHECK(idade_max > 0)
);

-- JOGA
CREATE TABLE futebol.joga(
    bi INT,
    nome_escalao VARCHAR(50),
	PRIMARY KEY(bi, nome_escalao)
);

-- ALTER JOGA
ALTER TABLE futebol.joga ADD CONSTRAINT FORJBIJBI
FOREIGN KEY (bi) REFERENCES futebol.jogador(bi)
ON UPDATE CASCADE;

ALTER TABLE futebol.joga ADD CONSTRAINT FORJEEE
FOREIGN KEY (nome_escalao) REFERENCES futebol.escalao(nome)
ON UPDATE CASCADE;

-- DIRIGE
CREATE TABLE futebol.dirige(
    bi int,
    nome_escalao VARCHAR(50),
	PRIMARY KEY(bi, nome_escalao)
);

-- ALTER DIRIGE
ALTER TABLE futebol.dirige ADD CONSTRAINT FORDBITBI
FOREIGN KEY (bi) REFERENCES futebol.tecnico(bi)
ON UPDATE CASCADE;

ALTER TABLE futebol.dirige ADD CONSTRAINT FORDEEE
FOREIGN KEY (nome_escalao) REFERENCES futebol.escalao(nome)
ON UPDATE CASCADE;

-- CAMPO
CREATE TABLE futebol.campo(
    id_campo INT PRIMARY KEY,
    local VARCHAR(150) NOT NULL
);

-- TREINO
CREATE TABLE futebol.treino(
    data DATE NOT NULL,
    hora TIME NOT NULL,
    id_campo INT NOT NULL,
    nome_escalao VARCHAR(50) NOT NULL,
    PRIMARY KEY(data, hora, id_campo)
);

-- ALTERs TREINO
ALTER TABLE futebol.treino ADD CONSTRAINT FORTICCIC
FOREIGN KEY (id_campo) REFERENCES futebol.campo(id_campo)
ON UPDATE CASCADE;

ALTER TABLE futebol.treino ADD CONSTRAINT FORTNEEN
FOREIGN KEY (nome_escalao) REFERENCES futebol.escalao(nome)
ON UPDATE CASCADE;

-- SECCAO
CREATE TABLE futebol.seccao(
    id_seccao INT PRIMARY KEY,
    tipo VARCHAR(50)
);

-- LUGAR
CREATE TABLE futebol.lugar(
    n_lugar INT NOT NULL,
    fila INT NOT NULL,
    id_seccao INT NOT NULL,
    PRIMARY KEY(n_lugar, fila, id_seccao)
);

-- ALTER LUGAR
ALTER TABLE futebol.lugar ADD CONSTRAINT FORLISSIS
FOREIGN KEY (id_seccao)  REFERENCES futebol.seccao(id_seccao)
ON UPDATE CASCADE;

-- LUGAR ANUAL
CREATE TABLE futebol.lugar_anual(
  n_lugar INT NOT NULL,
  fila INT NOT NULL,
  id_seccao INT NOT NULL,
  data_inscricao DATE NOT NULL,
  duracao INT NOT NULL,
  valor INT NOT NULL,
  bi INT NOT NULL,
  PRIMARY KEY(n_lugar, bi, fila, id_seccao)
);

-- LUGAR ANUAL ALTER's
ALTER TABLE futebol.lugar_anual ADD CONSTRAINT FORLAL
FOREIGN KEY (n_lugar, fila, id_seccao) REFERENCES futebol.lugar(n_lugar, fila, id_seccao)
ON UPDATE CASCADE;

ALTER TABLE futebol.lugar_anual ADD CONSTRAINT FORLABISBI
FOREIGN KEY (bi) REFERENCES futebol.socios(bi)
ON UPDATE CASCADE;

