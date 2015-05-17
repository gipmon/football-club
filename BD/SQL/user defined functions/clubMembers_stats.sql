use p4g5;
-- DROP FUNCTION football.udf_members_stats
go
CREATE FUNCTION football.udf_members_stats()
RETURNS @table TABLE ("name" varchar(50), "result" varchar(120))
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	
	-- average_shares
	INSERT @table SELECT 'average_shares' as 'name', AVG(shares_value) as 'count'
						  FROM football.members
	-- total_of_members
	INSERT @table SELECT 'total_of_members' as 'name', COUNT(bi) as 'count'
						  FROM football.members

	-- total_of_annual_spots
	INSERT @table SELECT 'total_of_annual_spots' as 'name', COUNT(n_spot) as 'count'
						  FROM football.annual_spot

	-- total_members_shares_in_day_false
	INSERT @table SELECT 'shares_in_day_false' as 'name', COUNT(bi) as 'count'
				         FROM football.members WHERE shares_in_day = 0;
	-- RETURN
	RETURN;
END;

-- DROP FUNCTION football.udf_staff_department_stats_next_birthday

go
CREATE FUNCTION football.udf_members_stats_next_birthday()
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
		FROM football.person JOIN football.members ON person.bi = members.bi

		DECLARE @minDays INT
		SELECT @minDays = MIN(days) from @daysToBirthday

		INSERT @table SELECT name, bi, day(birth) as day, month(birth) as month from @daysToBirthday
		WHERE days = @minDays

	-- RETURN
	RETURN;
END;

--DROP FUNCTION football.udf_annual_spots_per_season_count

go
CREATE FUNCTION football.udf_annual_spots_per_season_count()
RETURNS @table TABLE ("season" varchar(75), "annual seats" int)
WITH SCHEMABINDING, ENCRYPTION
AS
	BEGIN

		INSERT @table SELECT season as 'season', count(n_spot) as 'number of annual seats' from football.annual_spot
					  GROUP BY annual_spot.season

	-- RETURN
	RETURN;
END;





