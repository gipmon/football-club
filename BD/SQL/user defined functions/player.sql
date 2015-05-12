use p4g5;

go

-- DROP FUNCTION football.udf_players_data_grid

CREATE FUNCTION football.udf_players_data_grid(@team_name VARCHAR(50)=null)
RETURNS @table TABLE ("internal id" int, "bi" int, "name" varchar(75), 
					  "salary" money, "gender" varchar(1), "federation_id" int)
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	IF (@team_name is null)
		BEGIN
			INSERT @table SELECT	internal_people.internal_id AS 'internal id',
									person.bi, 
									person.name, 
									internal_people.salary,
									person.gender,
									player.federation_id AS 'federation id'
							FROM	(football.player JOIN 
										(football.internal_people JOIN
										football.person ON internal_people.bi = person.bi)
									ON player.bi = football.internal_people.bi);
		END;
	ELSE
		BEGIN
			INSERT @table SELECT	internal_people.internal_id AS 'internal id',
									person.bi, 
									person.name, 
									internal_people.salary,
									person.gender,
									player.federation_id AS 'federation id'
							FROM	(football.play JOIN	(football.player JOIN 
										(football.internal_people JOIN
										football.person ON internal_people.bi = person.bi)
									ON player.bi = football.internal_people.bi)
									ON play.bi = player.bi)
							WHERE team_name = @team_name;
		END;
	RETURN;
END;

go
CREATE FUNCTION football.udf_player(@player_bi int=null) 
RETURNS TABLE
WITH SCHEMABINDING, ENCRYPTION
AS
	RETURN (SELECT	internal_people.internal_id AS 'internal id',
									person.bi,
									person.name, 
									person.birth_date AS 'birth date',
									person.gender,
									player.federation_id AS 'federation id',
									internal_people.salary,
									person.address,
									person.nif,
									person.nationality,
									player.weight,
									player.height
			FROM	(football.player JOIN 
						(football.internal_people JOIN
						football.person ON internal_people.bi = person.bi)
					ON player.bi = football.internal_people.bi)
			WHERE person.bi = @player_bi);