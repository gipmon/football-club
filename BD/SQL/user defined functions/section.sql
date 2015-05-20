use p4g5;

-- DROP FUNCTION football.udf_sections

go
CREATE FUNCTION football.udf_sections(@id_section int=null) 
RETURNS @table TABLE ("section name" varchar(75), "section_id" int)
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	IF(@id_section is null)
		BEGIN
			INSERT @table SELECT type, id_section
		   			      FROM football.section;
		END;

	ELSE
		BEGIN
			INSERT @table SELECT type, id_section
		   			      FROM football.section
						  WHERE id_section = @id_section;
		END;
	RETURN;
END;

-- DROP FUNCTION football.udf_sections_annual

go
CREATE FUNCTION football.udf_sections_annual(@bi int=null, @n_seat int = null, @row varchar(1) = null, @id_section int = null, @season int = null) 
RETURNS TABLE
WITH SCHEMABINDING, ENCRYPTION
AS
	RETURN SELECT type, section.id_section
		   FROM football.section JOIN football.annual_seat ON section.id_section=annual_seat.id_section
		   WHERE annual_seat.bi = @bi AND annual_seat.n_seat = @n_seat AND annual_seat.row = @row AND annual_seat.id_section = @id_section AND annual_seat.season = @season;

-- DROP FUNCTION football.udf_sections_seats

go
CREATE FUNCTION football.udf_sections_seats(@n_seat int = null, @row varchar(1) = null, @id_section int = null) 
RETURNS TABLE
WITH SCHEMABINDING, ENCRYPTION
AS
	RETURN SELECT type, section.id_section
		   FROM football.section JOIN football.seat ON section.id_section=seat.id_section
		   WHERE seat.n_seat = @n_seat AND seat.row = @row AND seat.id_section = @id_section;
