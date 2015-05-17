use p4g5;
go
DROP FUNCTION football.udf_teams_stats;

go
CREATE FUNCTION football.udf_teams_stats()
RETURNS @table TABLE ("name" varchar(50), "result" varchar(120))
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	-- bigger_nacionality
	DECLARE @counting_table TABLE ("key_search" varchar(125), "count_search" int)
	
	INSERT @counting_table SELECT person.nationality as 'key_search', COUNT(person.bi) as 'count_search'
							FROM (football.player JOIN (football.internal_people JOIN
							football.person ON internal_people.bi = person.bi) 
							ON player.bi = internal_people.bi)
							GROUP BY person.nationality
							
	DECLARE @max INT;

	SELECT @max = MAX(count_search) FROM @counting_table;

	INSERT @table SELECT 'bigger_nacionality' as 'name', key_search as 'result'
						  FROM @counting_table
						  WHERE count_search = @max

	-- salaries_by_players
	INSERT @table SELECT 'salaries_by_players' as 'name', AVG(salary)
					FROM (football.player JOIN (football.internal_people JOIN
						football.person ON internal_people.bi = person.bi)
						 ON player.bi = person.bi)

	-- salaries_by_coachs
	INSERT @table SELECT 'salaries_by_coachs' as 'name', AVG(salary)
					FROM (football.coach JOIN (football.internal_people JOIN
						football.person ON internal_people.bi = person.bi)
						 ON coach.bi = person.bi)

	-- player_with_higher_salary
	SELECT @max = MAX(salary) FROM (football.player JOIN (football.internal_people JOIN
							football.person ON internal_people.bi = person.bi) 
							ON player.bi = internal_people.bi)

	INSERT @table SELECT TOP 1 'player_with_higher_salary' as 'name', person.bi as 'result'
						  FROM (football.player JOIN (football.internal_people JOIN
							football.person ON internal_people.bi = person.bi) 
							ON player.bi = internal_people.bi)
						  WHERE internal_people.salary = @max

	-- tallest_player
	SELECT @max = MAX(height) FROM (football.player JOIN (football.internal_people JOIN
							football.person ON internal_people.bi = person.bi) 
							ON player.bi = internal_people.bi)

	INSERT @table SELECT TOP 1 'tallest_player' as 'name', person.bi as 'result'
						  FROM (football.player JOIN (football.internal_people JOIN
							football.person ON internal_people.bi = person.bi) 
							ON player.bi = internal_people.bi)
						  WHERE player.height = @max

	-- heavier_player
	SELECT @max = MAX(weight) FROM (football.player JOIN (football.internal_people JOIN
							football.person ON internal_people.bi = person.bi) 
							ON player.bi = internal_people.bi)

	INSERT @table SELECT TOP 1 'heavier_player' as 'name', person.bi as 'result'
						  FROM (football.player JOIN (football.internal_people JOIN
							football.person ON internal_people.bi = person.bi) 
							ON player.bi = internal_people.bi)
						  WHERE player.weight = @max
	RETURN;
END;

go
SELECT * FROM football.udf_teams_stats();