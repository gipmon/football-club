-- Quantos playeres playm por escalão?
SELECT play.team_name AS team_name, COUNT(play.bi) AS playeres
FROM football.play
GROUP BY play.team_name

-- Quantos headsntes por escalão?
SELECT heads.team_name AS team_name, COUNT(heads.bi) AS headsntes
FROM football.heads
GROUP BY heads.team_name

-- Quantos members têm spot anual?
SELECT COUNT(DISTINCT annual_spot.bi) AS members_com_annual_spot
FROM football.annual_spot

-- Quantos members têm quotas em dia e quantos não têm em dia?
SELECT members_em_dia, members_em_divida
FROM (SELECT COUNT(members.bi) AS members_em_dia FROM football.members WHERE members.shares_in_day=1) AS tmp1,
      (SELECT count(members.bi) AS members_em_divida FROM football.members WHERE members.shares_in_day=0) AS tmp2

-- names dos members que não têm as quotas em dia
SELECT person.name
FROM (football.members JOIN football.person ON members.bi=person.bi)
WHERE members.shares_in_day=0

-- Quantos funcionarios por department?
SELECT department.name, COUNT(staff.bi) AS funcionarios
FROM (football.department JOIN football.staff
ON department.department_id=staff.department_id)
GROUP BY department.name

-- Quantos practices por court?
SELECT court.address, COUNT(practice.date) AS practices
FROM (football.practice JOIN football.court ON practice.id_court=court.id_court)
GROUP BY court.address

-- Quantos practices por dia?
SELECT practice.date, COUNT(practice.hour) AS practices
FROM football.practice
GROUP BY practice.date

-- Salário máximo dos playeres
SELECT MAX(internal_people.salary) AS salary_max
FROM (football.internal_people JOIN football.player ON internal_people.bi=player.bi)

-- Total de salários dos playeres
SELECT SUM(internal_people.salary) AS total_salarys
FROM (football.internal_people JOIN football.player ON internal_people.bi=player.bi)

-- player mais pesado
SELECT person.name
FROM (football.person JOIN (football.player JOIN
  (SELECT MAX(player.weight) AS max_weight
   FROM football.player)
  as tmp ON player.weight = tmp.max_weight) ON person.bi = player.bi)

-- player mais alto
SELECT person.name
FROM (football.person JOIN (football.player JOIN
  (SELECT MAX(player.height) AS max_height
   FROM football.player)
  as tmp ON player.height = tmp.max_height) ON person.bi = player.bi)

-- Salário médio por nationality
SELECT person.nationality, AVG(internal_people.salary) AS salary_medio
FROM (football.person JOIN football.internal_people ON person.bi=internal_people.bi)
GROUP BY person.nationality

-- Do staff quem tem o salário mais alto
SELECT person.name
FROM (football.person JOIN (football.internal_people JOIN
  (SELECT MAX(internal_people.salary) AS max_salary
   FROM (football.staff JOIN football.internal_people ON staff.bi=internal_people.bi))
  as tmp ON internal_people.salary = tmp.max_salary) ON person.bi = internal_people.bi)

-- Média de salários por department
SELECT department.name, AVG(internal_people.salary)
FROM (football.internal_people JOIN
        (football.staff JOIN football.department
          ON staff.department_id=department.department_id)
     ON internal_people.bi=staff.bi
     )
GROUP BY department.name

-- Nº spotes por secção
SELECT spot.id_section, COUNT(n_spot)
FROM football.spot
GROUP BY spot.id_section

-- Nº de spotes anuais opr season
SELECT annual_spot.season, COUNT(n_spot)
FROM football.annual_spot
GROUP BY annual_spot.season
