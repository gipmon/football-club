use p4g5;

-- DROP FUNCTION football.udf_annualSpots

go
CREATE FUNCTION football.udf_annualSpots(@n_spot INT=null, @row VARCHAR(1)=null, @id_section INT=null, @bi INT=null, @season INT=null)
RETURNS @table TABLE ("member number" int, "name" varchar(75), "bi" int, "section name" varchar(50), "section id" int, "row" varchar(1), "spot number" int, "season" int)
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	IF (@n_spot is null OR @row is null OR @id_section is null OR @bi is null OR @season is null)
		BEGIN
			INSERT @table SELECT n_member AS 'member number',
								 name,
								 annual_spot.bi,
								 type AS 'section name',
								 annual_spot.id_section,
								 row, 
								 n_spot AS 'spot number', 
								 season
						  FROM (football.annual_spot JOIN 
						  (football.members JOIN football.person ON members.bi = person.bi) ON annual_spot.bi = members.bi) JOIN
						  football.section ON annual_spot.id_section = section.id_section;
		END;
	ELSE
		BEGIN
			INSERT @table SELECT n_member AS 'member number',
								 name, 
								 annual_spot.bi, 
								 type AS 'section name', 
								 annual_spot.id_section,
								 row, 
								 n_spot AS 'spot number', 
								 season
						  FROM (football.annual_spot JOIN 
						  (football.members JOIN football.person ON members.bi = person.bi) ON annual_spot.bi = members.bi) JOIN
						  football.section ON annual_spot.id_section = section.id_section
						  WHERE n_spot = @n_spot AND row = @row AND annual_spot.id_section = @id_section AND annual_spot.bi = @bi AND season = @season;
		END;
	RETURN;
END;

-- DROP FUNCTION football.udf_annualSpots

go
CREATE FUNCTION football.udf_annualSpots_full(@n_spot INT=null, @row VARCHAR(1)=null, @id_section INT=null, @bi INT=null, @season INT=null)
RETURNS @table TABLE ("member number" int, "name" varchar(75), "bi" int, "section name" varchar(50), "section id" int, "row" varchar(1), "spot number" int, "season" int, duration int, value int, start_date date)
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	IF (@n_spot is null OR @row is null OR @id_section is null OR @bi is null OR @season is null)
		BEGIN
			INSERT @table SELECT n_member AS 'member number',
								 name,
								 annual_spot.bi,
								 type AS 'section name',
								 annual_spot.id_section,
								 row, 
								 n_spot AS 'spot number', 
								 season,
								 duration,
								 value,
								 start_date
						  FROM (football.annual_spot JOIN 
						  (football.members JOIN football.person ON members.bi = person.bi) ON annual_spot.bi = members.bi) JOIN
						  football.section ON annual_spot.id_section = section.id_section;
		END;
	ELSE
		BEGIN
			INSERT @table SELECT n_member AS 'member number',
								 name, 
								 annual_spot.bi, 
								 type AS 'section name', 
								 annual_spot.id_section,
								 row, 
								 n_spot AS 'spot number', 
								 season,
								 duration,
								 value,
								 start_date
						  FROM (football.annual_spot JOIN 
						  (football.members JOIN football.person ON members.bi = person.bi) ON annual_spot.bi = members.bi) JOIN
						  football.section ON annual_spot.id_section = section.id_section
						  WHERE n_spot = @n_spot AND row = @row AND annual_spot.id_section = @id_section AND annual_spot.bi = @bi AND season = @season;
		END;
	RETURN;
END;