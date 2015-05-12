use p4g5;

-- DROP FUNCTION football.udf_team_names

go
CREATE FUNCTION football.udf_team_names() 
RETURNS TABLE
WITH SCHEMABINDING, ENCRYPTION
AS
	RETURN SELECT name
		   FROM football.team;

-- DROP FUNCTION football.udf_team_names_player

go
CREATE FUNCTION football.udf_team_names_player(@player_bi int=null) 
RETURNS TABLE
WITH SCHEMABINDING, ENCRYPTION
AS
	RETURN	SELECT	play.team_name
						  FROM	(football.play JOIN football.player
								 ON play.bi = player.bi)
						  WHERE player.bi = @player_bi;

-- DROP FUNCTION football.udf_team_names_coach

go
CREATE FUNCTION football.udf_team_names_coach(@coach_bi int=null) 
RETURNS TABLE
WITH SCHEMABINDING, ENCRYPTION
AS
	RETURN	SELECT	heads.team_name
						  FROM	(football.heads JOIN football.coach
								 ON heads.bi = coach.bi)
						  WHERE heads.bi = @coach_bi;

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