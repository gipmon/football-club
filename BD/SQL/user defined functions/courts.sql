use p4g5;

-- DROP FUNCTION football.udf_courts

go
CREATE FUNCTION football.udf_courts(@id_court int=null)
RETURNS @table TABLE ("id" int, "address" varchar(150))
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	IF (@id_court is null)
		BEGIN
			INSERT @table SELECT id_court AS 'id', address
						  FROM football.court;
		END;
	ELSE
		BEGIN
			INSERT @table SELECT id_court AS 'id', address
						  FROM football.court
						  WHERE id_court = @id_court;
		END;
	RETURN;
END;