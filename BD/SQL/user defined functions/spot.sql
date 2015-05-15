use p4g5;

-- DROP FUNCTION football.udf_spots

go
CREATE FUNCTION football.udf_spots(@n_spot INT=null, @row VARCHAR(1)=null, @id_section INT=null)
RETURNS @table TABLE ("section name" varchar(50), "section id" int, "spot number" int, "row" varchar(1))
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	IF (@n_spot is null OR @row is null OR @id_section is null)
		BEGIN
			INSERT @table SELECT section.type,
								 spot.id_section,
								 n_spot AS 'spot number', 
								 row
								 
								
						  FROM (football.spot JOIN 
						  football.section ON spot.id_section = section.id_section);
		END;
	ELSE
		BEGIN
			INSERT @table SELECT section.type,
								 spot.id_section,
								 n_spot AS 'spot number', 
								 row
								
						  FROM (football.spot JOIN 
						  football.section ON spot.id_section = section.id_section)
						  WHERE n_spot = @n_spot AND row = @row AND spot.id_section = @id_section;
		END;
	RETURN;
END;
