use p4g5;

-- DROP FUNCTION football.udf_team_names

go
CREATE FUNCTION football.udf_team_names(@player_bi int=null) 
RETURNS @table TABLE ("team_name" varchar(50))
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	IF (@player_bi is null)
		BEGIN
			INSERT @table SELECT name
						  FROM football.team;
		END;
	ELSE
		BEGIN
			INSERT @table SELECT	play.team_name
						  FROM	(football.play JOIN football.player
								 ON play.bi = player.bi)
						  WHERE player.bi = @player_bi;
		END;
	RETURN;
END;

go
CREATE FUNCTION football.udf_teams(@team_name varchar(50)=null)
RETURNS @table TABLE ("name" varchar(50), "max_age" int)
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	IF (@team_name is null)
		BEGIN
			INSERT @table SELECT name, max_age
						  FROM football.team
		END;
	ELSE
		BEGIN
			INSERT @table SELECT name, max_age
						  FROM football.team
						  WHERE name = @team_name
		END;
	RETURN;
END;