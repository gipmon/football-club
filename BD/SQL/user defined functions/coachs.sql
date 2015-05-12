use p4g5;

go

-- DROP FUNCTION football.udf_coachs_data_grid

CREATE FUNCTION football.udf_coachs_data_grid()
RETURNS TABLE
WITH SCHEMABINDING, ENCRYPTION
AS
	RETURN (SELECT	internal_people.internal_id AS 'internal id',
									person.bi, 
									person.name, 
									coach.role, 
									internal_people.salary,
									person.gender,
									coach.federation_id AS 'federation id'
			FROM	(football.coach JOIN 
						(football.internal_people JOIN
						football.person ON internal_people.bi = person.bi)
					ON coach.bi = football.internal_people.bi));

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