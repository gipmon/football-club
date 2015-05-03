CREATE SCHEMA football;

-- person
CREATE TABLE football.person(
    bi INT PRIMARY KEY CHECK(bi>0),
    name VARCHAR(75) NOT NULL,
    address VARCHAR(75) NOT NULL,
    birth_date DATE NOT NULL,
    nif INT UNIQUE NOT NULL,
	gender VARCHAR(1) NOT NULL CHECK (gender IN('M','F')),
	nationality VARCHAR(75) NOT NULL
);

-- internal people
CREATE TABLE football.internal_people(
    bi INT PRIMARY KEY,
    salary MONEY NOT NULL CHECK (salary >= 0),
    internal_id INT UNIQUE NOT NULL IDENTITY
);

-- ALTERs internal people
ALTER TABLE football.internal_people ADD CONSTRAINT FORPIBIPBI
FOREIGN KEY (bi) REFERENCES football.person(bi)
ON UPDATE CASCADE;

-- members
CREATE TABLE football.members(
    bi INT PRIMARY KEY,
    n_member INT NOT NULL UNIQUE IDENTITY,
    shares_in_day BIT NOT NULL,
    shares_value MONEY NOT NULL CHECK (shares_value >= 0),
);

-- ALTERs members
ALTER TABLE football.members ADD CONSTRAINT FORSBIPBI
FOREIGN KEY (bi) REFERENCES football.person(bi)
ON UPDATE CASCADE;

-- player
CREATE TABLE football.player(
    bi INT PRIMARY KEY,
    federation_id INT UNIQUE NOT NULL CHECK(federation_id >= 0),
    weight int NOT NULL CHECK(weight > 0),
    height int NOT NULL CHECK(height > 0)
);

-- player ALTERs
ALTER TABLE football.player ADD CONSTRAINT FORJBIPIBI
FOREIGN KEY (bi) REFERENCES football.internal_people(bi)
ON UPDATE CASCADE;

-- coach
CREATE TABLE football.coach(
    bi INT PRIMARY KEY,
    federation_id INT NOT NULL CHECK(federation_id >= 0),
    role VARCHAR(50) NOT NULL
);

-- coach ALTERs
ALTER TABLE football.coach ADD CONSTRAINT FORTBIPIBI
FOREIGN KEY (bi) REFERENCES football.internal_people(bi)
ON UPDATE CASCADE;

-- STAFF CLUBE
CREATE TABLE football.staff(
    bi INT PRIMARY KEY,
    role VARCHAR(50) NOT NULL,
    department_id INT NOT NULL
);

-- STAFF ALTERs
ALTER TABLE football.staff ADD CONSTRAINT FORSCBIPIBI
FOREIGN KEY (bi) REFERENCES football.internal_people(bi)
ON UPDATE CASCADE;

-- department
CREATE TABLE football.department(
    department_id INT PRIMARY KEY,
    address VARCHAR(75) NOT NULL,
    name VARCHAR(75) NOT NULL
);

-- STAFF ALTERs
ALTER TABLE football.staff ADD CONSTRAINT FORSCIDDID
FOREIGN KEY (department_id) REFERENCES football.department(department_id)
ON UPDATE CASCADE;

-- ESCALÃO
CREATE TABLE football.team(
    name VARCHAR(50) PRIMARY KEY,
    max_age INT NOT NULL CHECK(max_age > 0)
);

-- play
CREATE TABLE football.play(
    bi INT,
    team_name VARCHAR(50),
	PRIMARY KEY(bi, team_name)
);

-- ALTER play
ALTER TABLE football.play ADD CONSTRAINT FORJBIJBI
FOREIGN KEY (bi) REFERENCES football.player(bi)
ON UPDATE CASCADE;

ALTER TABLE football.play ADD CONSTRAINT FORJEEE
FOREIGN KEY (team_name) REFERENCES football.team(name)
ON UPDATE CASCADE;

-- heads
CREATE TABLE football.heads(
    bi int,
    team_name VARCHAR(50),
	PRIMARY KEY(bi, team_name)
);

-- ALTER heads
ALTER TABLE football.heads ADD CONSTRAINT FORDBITBI
FOREIGN KEY (bi) REFERENCES football.coach(bi)
ON UPDATE CASCADE;

ALTER TABLE football.heads ADD CONSTRAINT FORDEEE
FOREIGN KEY (team_name) REFERENCES football.team(name)
ON UPDATE CASCADE;

-- court
CREATE TABLE football.court(
    id_court INT PRIMARY KEY IDENTITY,
    address VARCHAR(150) NOT NULL
);

-- practice
CREATE TABLE football.practice(
    date DATE NOT NULL,
    hour TIME NOT NULL,
    id_court INT NOT NULL,
    team_name VARCHAR(50) NOT NULL,
    PRIMARY KEY(date, hour, id_court)
);

-- ALTERs practice
ALTER TABLE football.practice ADD CONSTRAINT FORTICCIC
FOREIGN KEY (id_court) REFERENCES football.court(id_court)
ON UPDATE CASCADE;

ALTER TABLE football.practice ADD CONSTRAINT FORTNEEN
FOREIGN KEY (team_name) REFERENCES football.team(name)
ON UPDATE CASCADE;

-- section
CREATE TABLE football.section(
    id_section INT PRIMARY KEY,
    type VARCHAR(50)
);

-- spot
CREATE TABLE football.spot(
    n_spot INT NOT NULL,
    row VARCHAR(1) NOT NULL,
    id_section INT NOT NULL,
    PRIMARY KEY(n_spot, row, id_section)
);

-- ALTER spot
ALTER TABLE football.spot ADD CONSTRAINT FORLISSIS
FOREIGN KEY (id_section)  REFERENCES football.section(id_section)
ON UPDATE CASCADE;

-- annual_spot
CREATE TABLE football.annual_spot(
  n_spot INT NOT NULL,
  row VARCHAR(1) NOT NULL,
  id_section INT NOT NULL,
  start_date DATE NOT NULL,
  duration INT NOT NULL,
  value INT NOT NULL,
  bi INT NOT NULL,
  season INT NOT NULL,
  PRIMARY KEY(n_spot, bi, row, id_section, season)
);



-- annual_spot ALTER's
ALTER TABLE football.annual_spot ADD CONSTRAINT FORLAL
FOREIGN KEY (n_spot, row, id_section) REFERENCES football.spot(n_spot, row, id_section)
ON UPDATE CASCADE;

ALTER TABLE football.annual_spot ADD CONSTRAINT FORLABISBI
FOREIGN KEY (bi) REFERENCES football.members(bi)
ON UPDATE CASCADE;
