use p4g5;

-- DROP FUNCTION football.udf_seats

go
CREATE FUNCTION football.udf_seats(@n_seat INT=null, @row VARCHAR(1)=null, @id_section INT=null)
RETURNS @table TABLE ("section name" varchar(50), "section id" int, "seat number" int, "row" varchar(1))
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	IF (@n_seat is null OR @row is null OR @id_section is null)
		BEGIN
			INSERT @table SELECT section.type,
								 seat.id_section,
								 n_seat AS 'seat number', 
								 row
								 
								
						  FROM (football.seat JOIN 
						  football.section ON seat.id_section = section.id_section);
		END;
	ELSE
		BEGIN
			INSERT @table SELECT section.type,
								 seat.id_section,
								 n_seat AS 'seat number', 
								 row
								
						  FROM (football.seat JOIN 
						  football.section ON seat.id_section = section.id_section)
						  WHERE n_seat = @n_seat AND row = @row AND seat.id_section = @id_section;
		END;
	RETURN;
END;
