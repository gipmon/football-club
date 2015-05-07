use p4g5;

go 

CREATE VIEW football.teamNamesView 
WITH schemabinding, encryption 
AS 
	SELECT	team.name
	FROM	football.team;

go 

CREATE VIEW football.playersTeamsView 
WITH schemabinding, encryption 
AS 
	SELECT	play.team_name, player.bi
	FROM	(football.play JOIN football.player
			ON play.bi = player.bi)

go 

CREATE VIEW football.teamsView 
WITH schemabinding, encryption 
AS 
	SELECT	team.name, team.max_age
	FROM	football.team;