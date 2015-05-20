use p4g5;

-- DROP FUNCTION football.udf_annualSeats

go
CREATE FUNCTION football.udf_annualSeats(@n_seat INT=null, @row VARCHAR(1)=null, @id_section INT=null, @bi INT=null, @season INT=null)
RETURNS @table TABLE ("member number" int, "name" varchar(75), "bi" int, "section name" varchar(50), "section id" int, "row" varchar(1), "seat number" int, "season" int)
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	IF (@n_seat is null OR @row is null OR @id_section is null OR @bi is null OR @season is null)
		BEGIN
			INSERT @table SELECT n_member AS 'member number',
								 name,
								 annual_seat.bi,
								 type AS 'section name',
								 annual_seat.id_section,
								 row, 
								 n_seat AS 'seat number', 
								 season
						  FROM (football.annual_seat JOIN 
						  (football.members JOIN football.person ON members.bi = person.bi) ON annual_seat.bi = members.bi) JOIN
						  football.section ON annual_seat.id_section = section.id_section;
		END;
	ELSE
		BEGIN
			INSERT @table SELECT n_member AS 'member number',
								 name, 
								 annual_seat.bi, 
								 type AS 'section name', 
								 annual_seat.id_section,
								 row, 
								 n_seat AS 'seat number', 
								 season
						  FROM (football.annual_seat JOIN 
						  (football.members JOIN football.person ON members.bi = person.bi) ON annual_seat.bi = members.bi) JOIN
						  football.section ON annual_seat.id_section = section.id_section
						  WHERE n_seat = @n_seat AND row = @row AND annual_seat.id_section = @id_section AND annual_seat.bi = @bi AND season = @season;
		END;
	RETURN;
END;

-- DROP FUNCTION football.udf_annualSeats

go
CREATE FUNCTION football.udf_annualSeats_full(@n_seat INT=null, @row VARCHAR(1)=null, @id_section INT=null, @bi INT=null, @season INT=null)
RETURNS @table TABLE ("member number" int, "name" varchar(75), "bi" int, "section name" varchar(50), "section id" int, "row" varchar(1), "seat number" int, "season" int, duration int, value int, start_date date)
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	IF (@n_seat is null OR @row is null OR @id_section is null OR @bi is null OR @season is null)
		BEGIN
			INSERT @table SELECT n_member AS 'member number',
								 name,
								 annual_seat.bi,
								 type AS 'section name',
								 annual_seat.id_section,
								 row, 
								 n_seat AS 'seat number', 
								 season,
								 duration,
								 value,
								 start_date
						  FROM (football.annual_seat JOIN 
						  (football.members JOIN football.person ON members.bi = person.bi) ON annual_seat.bi = members.bi) JOIN
						  football.section ON annual_seat.id_section = section.id_section;
		END;
	ELSE
		BEGIN
			INSERT @table SELECT n_member AS 'member number',
								 name, 
								 annual_seat.bi, 
								 type AS 'section name', 
								 annual_seat.id_section,
								 row, 
								 n_seat AS 'seat number', 
								 season,
								 duration,
								 value,
								 start_date
						  FROM (football.annual_seat JOIN 
						  (football.members JOIN football.person ON members.bi = person.bi) ON annual_seat.bi = members.bi) JOIN
						  football.section ON annual_seat.id_section = section.id_section
						  WHERE n_seat = @n_seat AND row = @row AND annual_seat.id_section = @id_section AND annual_seat.bi = @bi AND season = @season;
		END;
	RETURN;
END;