use p4g5;
-- DROP FUNCTION football.udf_stadium_stats
go
CREATE FUNCTION football.udf_stadium_stats()
RETURNS @table TABLE ("name" varchar(50), "result" varchar(120))
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	
	-- total_sections
	INSERT @table SELECT 'total_Sections' as 'name', COUNT(id_section) as 'count'
						  FROM football.section
	-- total_seats
	INSERT @table SELECT 'total_of_seats' as 'name', COUNT(n_seat) as 'count'
						  FROM football.seat

	-- RETURN
	RETURN;
END;

go 
SELECT * from football.udf_stadium_stats()

--DROP FUNCTION football.udf_seats_per_section_count

go
CREATE FUNCTION football.udf_seats_per_section_count()
RETURNS @table TABLE ("section" varchar(75), "section id" int, "seats" int)
WITH SCHEMABINDING, ENCRYPTION
AS
	BEGIN

		INSERT @table SELECT type AS 'section', section.id_section AS 'section id', count(n_seat) AS 'seats' 
					  FROM football.section JOIN football.seat ON section.id_section = seat.id_section
					  GROUP BY type, section.id_section

	-- RETURN
	RETURN;
END;

--DROP FUNCTION football.udf_annual_seats_per_section_count

go
CREATE FUNCTION football.udf_annual_seats_per_section_count()
RETURNS @table TABLE ("section" varchar(75), "section id" int, "annual seats" int)
WITH SCHEMABINDING, ENCRYPTION
AS
	BEGIN

		INSERT @table SELECT type AS 'section', section.id_section AS 'section id', count(n_seat) AS 'annual seats' 
					  FROM football.section JOIN football.annual_seat ON section.id_section = annual_seat.id_section
					  GROUP BY type, section.id_section

	-- RETURN
	RETURN;
END;