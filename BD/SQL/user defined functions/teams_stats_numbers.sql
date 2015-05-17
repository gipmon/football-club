use p4g5;
go
DROP FUNCTION football.udf_number_players_per_team;

go
CREATE FUNCTION football.udf_number_players_per_team()
RETURNS @table TABLE ("team name" varchar(50), "result" int)
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	-- number_players_per_team
	INSERT @table SELECT team.name as 'team name', COUNT(play.bi) as 'result'
					FROM football.play FULL OUTER JOIN football.team
					ON team.name = play.team_name
					GROUP BY team.name;
	RETURN;
END;

go
SELECT * FROM football.udf_number_players_per_team();

go
DROP FUNCTION football.udf_number_coachs_per_team;

go
CREATE FUNCTION football.udf_number_coachs_per_team()
RETURNS @table TABLE ("team name" varchar(50), "result" int)
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	-- number_coachs_per_team
	INSERT @table SELECT team.name as 'team name', COUNT(heads.bi) as 'result'
					FROM football.heads FULL OUTER JOIN football.team
					ON team.name = heads.team_name
					GROUP BY team.name;
	RETURN;
END;

go
SELECT * FROM football.udf_number_coachs_per_team();