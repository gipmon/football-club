use p4g5;

go 

CREATE VIEW football.playersView 
WITH schemabinding, encryption 
AS 
	SELECT	internal_people.internal_id AS 'internal id',
			person.bi, 
			person.name, 
			person.birth_date AS 'birth date',
			person.gender,
			player.federation_id AS 'federation id'
	FROM	(football.player JOIN 
				(football.internal_people JOIN
				football.person ON internal_people.bi = person.bi)
			ON player.bi = football.internal_people.bi);

go 

CREATE VIEW football.playerView 
WITH schemabinding, encryption 
AS 
	SELECT	internal_people.internal_id AS 'internal id',
			person.bi,
			person.name, 
			person.birth_date AS 'birth date',
			person.gender,
			player.federation_id AS 'federation id',
			internal_people.salary,
			person.address,
			person.nif,
			person.nationality,
			player.weight,
			player.height
	FROM	(football.player JOIN 
				(football.internal_people JOIN
				football.person ON internal_people.bi = person.bi)
			ON player.bi = football.internal_people.bi);