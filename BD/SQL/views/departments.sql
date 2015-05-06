use p4g5;

go 

CREATE VIEW football.departmentsView 
WITH schemabinding, encryption 
AS 
	SELECT	department.name
	FROM	football.department;

go 

CREATE VIEW football.staffDepartmentView 
WITH schemabinding, encryption 
AS 
	SELECT	staff.department_id, department.name, staff.bi
	FROM	(football.department JOIN football.staff
			ON department.department_id = staff.department_id)