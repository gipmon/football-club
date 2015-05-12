use p4g5;

go

-- DROP FUNCTION football.udf_coachs_data_grid

CREATE FUNCTION football.udf_coachs_data_grid(@team_name VARCHAR(50)=null)
RETURNS @table TABLE ("internal id" int, "bi" int, "name" varchar(75), 
					  "salary" money, "gender" varchar(1), "federation_id" int,
					  "role" varchar(50))
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
									coach.federation_id AS 'federation id',
									coach.role
							FROM	(football.coach JOIN 
										(football.internal_people JOIN
										football.person ON internal_people.bi = person.bi)
									ON coach.bi = football.internal_people.bi);
		END;
	ELSE
		BEGIN
			INSERT @table SELECT	internal_people.internal_id AS 'internal id',
									person.bi, 
									person.name, 
									internal_people.salary,
									person.gender,
									coach.federation_id AS 'federation id',
									coach.role
							FROM	(football.heads JOIN (football.coach JOIN 
										(football.internal_people JOIN
										football.person ON internal_people.bi = person.bi)
									ON coach.bi = football.internal_people.bi)
									ON heads.bi = coach.bi);
		END;
	RETURN;
END;

go
CREATE FUNCTION football.udf_coach(@coach_bi int=null) 
RETURNS TABLE
WITH SCHEMABINDING, ENCRYPTION
AS
	RETURN (SELECT	internal_people.internal_id AS 'internal id',
									person.bi,
									person.name, 
									person.birth_date AS 'birth date',
									person.gender,
									coach.federation_id AS 'federation id',
									internal_people.salary,
									person.address,
									person.nif,
									person.nationality,
									coach.role
			FROM	(football.coach JOIN 
						(football.internal_people JOIN
						football.person ON internal_people.bi = person.bi)
					ON coach.bi = internal_people.bi)
			WHERE person.bi = @coach_bi);