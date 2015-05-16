use p4g5;
-- DROP FUNCTION football.udf_general_stats
go
CREATE FUNCTION football.udf_general_stats()
RETURNS @table TABLE ("name" varchar(50), "count" int)
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	
	-- total_of_players
	INSERT @table SELECT 'total_of_players' as 'name', COUNT(bi) as 'count'
						  FROM football.player
	-- total_of_staff
	INSERT @table SELECT 'total_of_staff' as 'name', COUNT(bi) as 'count'
						  FROM football.staff
	-- total_internal_people
	INSERT @table SELECT 'total_internal_people' as 'name', COUNT(bi) as 'count'
						  FROM football.internal_people
	-- total_coachs
	INSERT @table SELECT 'total_coachs' as 'name', COUNT(bi) as 'count'
						  FROM football.coach
	-- total_club_members
	INSERT @table SELECT 'total_club_members' as 'name', COUNT(bi) as 'count'
						  FROM football.members
	-- total_salaries_per_month
	INSERT @table SELECT 'total_salaries_per_month' as 'name', SUM(salary) as 'count'
						  FROM football.internal_people
	-- total_of_seats
	INSERT @table SELECT 'total_of_seats' as 'name', COUNT(n_spot) as 'count'
						  FROM football.spot
	-- total_of_teams
	INSERT @table SELECT 'total_of_teams' as 'name', COUNT(name) as 'count'
						  FROM football.team

	-- RETURN
	RETURN;
END;