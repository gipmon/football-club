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
SELECT * FROM football.udf_team_names(DEFAULT)