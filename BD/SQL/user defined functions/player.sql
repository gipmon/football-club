use p4g5;

go
CREATE FUNCTION football.udf_players_data_grid()
RETURNS TABLE
WITH SCHEMABINDING, ENCRYPTION
AS
	RETURN (SELECT	internal_people.internal_id AS 'internal id',
									person.bi, 
									person.name, 
									person.birth_date AS 'birth date',
									person.gender,
									player.federation_id AS 'federation id'
			FROM	(football.player JOIN 
						(football.internal_people JOIN
						football.person ON internal_people.bi = person.bi)
					ON player.bi = football.internal_people.bi));

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