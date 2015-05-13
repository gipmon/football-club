use p4g5;

go

-- DROP FUNCTION football.udf_members_data_grid

CREATE FUNCTION football.udf_members_data_grid(@bi INT = null)
RETURNS @table TABLE ( "member number" int, "name" varchar(75), "bi" int)
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	IF (@bi is null)
		BEGIN
			INSERT @table SELECT	members.n_member AS 'member number',
									person.name,
									members.bi
							FROM	(football.members JOIN 
									 football.person ON members.bi = person.bi);
									
		END;
	ELSE
		BEGIN
			INSERT @table SELECT	person.name,
									members.n_member AS 'member number',
									person.bi
							FROM	(football.members JOIN 
										(football.internal_people JOIN
										football.person ON internal_people.bi = person.bi)
									ON members.bi = football.internal_people.bi)
							WHERE person.bi = @bi;
		END;
	RETURN;
END;

go
CREATE FUNCTION football.udf_member(@bi int=null) 
RETURNS TABLE
WITH SCHEMABINDING, ENCRYPTION
AS
	RETURN (SELECT					members.n_member,
									person.bi,
									person.name, 
									person.birth_date AS 'birth date',
									person.gender,
									members.shares_in_day,
									members.shares_value,
									person.address,
									person.nif,
									person.nationality
								
			FROM	(football.members JOIN 
					football.person ON members.bi = person.bi)
					
			WHERE person.bi = @bi);
