use p4g5;

-- DROP FUNCTION football.udf_departments_names

go
CREATE FUNCTION football.udf_department_names(@staff_bi int=null) 
RETURNS @table TABLE ("department_name" varchar(75), "department_id" int)
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
CREATE FUNCTION football.udf_departments(@department_id int=null)
RETURNS @table TABLE ("department_name" varchar(75), "department_id" int, "address" varchar(75))
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	IF (@department_id is null)
		BEGIN
			INSERT @table SELECT name, department_id, address
						  FROM football.department
		END;
	ELSE
		BEGIN
			INSERT @table SELECT name, department_id, address
						  FROM football.department
						  WHERE department.department_id = @department_id
		END;
	RETURN;
END;