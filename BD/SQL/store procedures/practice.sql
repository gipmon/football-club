use p4g5;

-- DROP FUNCTION football.udf_practices

go
CREATE FUNCTION football.udf_practices(@date date=null, @hour time(7)=null, @id_court int=null)
RETURNS @table TABLE ("date" date, "hour" time(7), "id_court" int, "team_name" varchar(50))
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	IF (@date is null OR @hour is null OR @id_court is null)
		BEGIN
			INSERT @table SELECT date, hour, id_court AS 'id court', team_name
						  FROM football.practice;
		END;
	ELSE
		BEGIN
			INSERT @table SELECT date, hour, id_court AS 'id court', team_name
						  FROM football.practice
						  WHERE date=@date AND hour=@hour AND id_court=@id_court;
		END;
	RETURN;
END;