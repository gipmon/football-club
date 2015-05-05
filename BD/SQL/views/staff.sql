use p4g5;

go 

CREATE VIEW football.staffView 
WITH schemabinding, encryption 
AS 
	SELECT	internal_people.internal_id AS 'internal id',
			person.bi, 
			person.name, 
			staff.role,
			department.name AS 'department name',
			person.birth_date AS 'birth date',
			person.gender
			
	FROM	((football.staff JOIN 
				(football.internal_people JOIN
				football.person ON internal_people.bi = person.bi)
			ON staff.bi = football.internal_people.bi)) JOIN football.department
			ON football.staff.department_id = department.department_id;

go 

CREATE VIEW football.individualStaffView 
WITH schemabinding, encryption 
AS 
	SELECT	internal_people.internal_id AS 'internal id',
			person.bi,
			person.name, 
			person.birth_date AS 'birth date',
			person.gender,
			department.name AS 'department name',
			internal_people.salary,
			person.address,
			person.nif,
			person.nationality,
			staff.role
		
	FROM	((football.staff JOIN 
				(football.internal_people JOIN
				football.person ON internal_people.bi = person.bi)
			ON staff.bi = football.internal_people.bi)) JOIN football.department
			ON football.staff.department_id = department.department_id;