use p4g5;

-- DROP FUNCTION football.udf_department_names

go
CREATE FUNCTION football.udf_department_names(@staff_bi int=null) 
RETURNS @table TABLE ("department_name" varchar(50), "department_id" int)
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	IF (@staff_bi is null)
		BEGIN
			INSERT @table SELECT name, department_id
						  FROM football.department;
		END;
	ELSE
		BEGIN
			INSERT @table SELECT department.name, staff.department_id
						  FROM (football.department JOIN football.staff
						  ON department.department_id = staff.department_id)
						  WHERE staff.bi = @staff_bi;
		END;
	RETURN;
END;

go
SELECT * FROM football.udf_department_names(DEFAULT)