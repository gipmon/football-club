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
CREATE FUNCTION football.udf_sections_annual(@bi int=null, @n_spot int = null, @row varchar(1) = null, @id_section int = null, @season int = null) 
RETURNS TABLE
WITH SCHEMABINDING, ENCRYPTION
AS
	RETURN SELECT type, section.id_section
		   FROM football.section JOIN football.annual_spot ON section.id_section=annual_spot.id_section
		   WHERE annual_spot.bi = @bi AND annual_spot.n_spot = @n_spot AND annual_spot.row = @row AND annual_spot.id_section = @id_section AND annual_spot.season = @season;

-- DROP FUNCTION football.udf_sections_spots

go
CREATE FUNCTION football.udf_sections_spots(@n_spot int = null, @row varchar(1) = null, @id_section int = null) 
RETURNS TABLE
WITH SCHEMABINDING, ENCRYPTION
AS
	RETURN SELECT type, section.id_section
		   FROM football.section JOIN football.spot ON section.id_section=spot.id_section
		   WHERE spot.n_spot = @n_spot AND spot.row = @row AND spot.id_section = @id_section;
