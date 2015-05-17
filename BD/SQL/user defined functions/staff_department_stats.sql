use p4g5;
-- DROP FUNCTION football.udf_staff_department_stats
go
CREATE FUNCTION football.udf_staff_department_stats()
RETURNS @table TABLE ("name" varchar(50), "result" varchar(120))
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN

	-- bigger_nacionality
	DECLARE @counting_table TABLE ("key_search" varchar(125), "count_search" int)
	
	INSERT @counting_table SELECT person.nationality as 'key_search', COUNT(person.bi) as 'count_search'
							FROM (football.staff JOIN (football.internal_people JOIN
							football.person ON internal_people.bi = person.bi) 
							ON staff.bi = internal_people.bi)
							GROUP BY person.nationality
							
	DECLARE @max INT;

	SELECT @max = MAX(count_search) FROM @counting_table;

	INSERT @table SELECT 'bigger_nacionality' as 'name', key_search as 'result'
						  FROM @counting_table
						  WHERE count_search = @max
	
	-- total_salary_of_staff
	INSERT @table SELECT 'total_salary_of_staff' as 'name', SUM(salary) as 'count'
						  FROM football.staff JOIN football.internal_people ON staff.bi = internal_people.bi
	-- total_of_staff
	INSERT @table SELECT 'total_of_departments' as 'name', COUNT(department_id) as 'count'
						  FROM football.department

	-- average_age_of_staff
	INSERT @table SELECT 'average_age_of_staff' as 'name', AVG(Age) as 'count'
				         FROM (SELECT person.bi, DATEDIFF(hour,birth_date,GETDATE())/8766 AS Age FROM football.person JOIN football.internal_people ON person.bi = internal_people.bi) AS tmp JOIN
						 football.staff ON tmp.bi = staff.bi
	-- RETURN
	RETURN;
END;

-- DROP FUNCTION football.udf_staff_department_stats_next_birthday

go
CREATE FUNCTION football.udf_staff_department_stats_next_birthday()
RETURNS @table TABLE ("name" varchar(75), "bi" int, "day" int, "month" int)
WITH SCHEMABINDING, ENCRYPTION
AS
	BEGIN
	
		DECLARE @daysToBirthday TABLE ("bi" int, "name" varchar(75), "birth" date, "days" int)
		INSERT @daysToBirthday SELECT person.bi, person.name, person.birth_date, DATEDIFF(day,
		CURRENT_TIMESTAMP,
		DATEADD(year,
		DATEDIFF(year, person.birth_date, CURRENT_TIMESTAMP)
		+ CASE WHEN DATEADD(year, DATEDIFF(year,
		person.birth_date, CURRENT_TIMESTAMP), person.birth_date) < CURRENT_TIMESTAMP THEN 1
		ELSE 0 END,
		person.birth_date))
		FROM (football.person JOIN football.internal_people ON person.bi = internal_people.bi)
		JOIN football.staff oN person.bi = staff.bi

		DECLARE @minDays INT
		SELECT @minDays = MIN(days) from @daysToBirthday

		INSERT @table SELECT name, bi, day(birth) as day, month(birth) as month from @daysToBirthday
		WHERE days = @minDays

	-- RETURN
	RETURN;
END;

go
CREATE FUNCTION football.udf_staff_department_count()
RETURNS @table TABLE ("name" varchar(75), "count" int)
WITH SCHEMABINDING, ENCRYPTION
AS
	BEGIN

		INSERT @table SELECT name, count(bi) from football.department JOIN football.staff ON department.department_id = staff.department_id
					  GROUP BY department.name

	-- RETURN
	RETURN;
END;





