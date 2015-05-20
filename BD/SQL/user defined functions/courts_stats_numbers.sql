use p4g5;

--# # #    udf_number_practices_per_court    # # #

go
--DROP FUNCTION football.udf_number_practices_per_court;

go
CREATE FUNCTION football.udf_number_practices_per_court()
RETURNS @table TABLE ("court id" int, "court address" varchar(150),  "practices" int)
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	INSERT @table SELECT court.id_court as 'court id', court.address as "court address", 
					COUNT(practice.team_name) as "practices"
					FROM football.court FULL OUTER JOIN football.practice
					ON court.id_court = practice.id_court
					GROUP BY court.id_court, court.address;
	RETURN;
END;

go
SELECT * FROM football.udf_number_practices_per_court();

--# # #    udf_average_hour_of_training_by_court    # # #

go
--DROP FUNCTION football.udf_average_hour_of_training_by_court;

go
CREATE FUNCTION football.udf_average_hour_of_training_by_court()
RETURNS @table TABLE ("court id" int, "court address" varchar(150),  "hour" time(7))
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	INSERT @table SELECT court.id_court as 'court id', court.address as "court address",
					CAST(DATEADD(ms, AVG(DATEDIFF(ms, '00:00:00', practice.hour)), '00:00:00') AS time(7)) as "hour"
					FROM football.court JOIN football.practice
					ON court.id_court = practice.id_court
					GROUP BY court.id_court, court.address;
	RETURN;
END;

go
SELECT * FROM football.udf_average_hour_of_training_by_court();

--# # #    udf_latest_team_to_train_in_each_court    # # #

go
--DROP FUNCTION football.udf_latest_team_to_train_in_each_court;

go
CREATE FUNCTION football.udf_latest_team_to_train_in_each_court()
RETURNS @table TABLE ("court id" int, "court address" varchar(150),  "team name" varchar(50))
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	-- table com id_court, date_int e team_name
	DECLARE @counting_table_time TABLE ("id_court" int, "address" varchar(150), "date_int" int, "team_name" varchar(50))
	
	INSERT @counting_table_time SELECT court.id_court, court.address, DATEDIFF(ms, '00:00:00', practice.hour) +
								DATEDIFF(ms, GETDATE(), practice.date) as "date_int", 
								practice.team_name
								FROM football.court JOIN football.practice
								ON court.id_court = practice.id_court;

	-- get max for each court
	DECLARE @counting_table TABLE ("id_court" int, "date_int" int)

	INSERT @counting_table SELECT id_court, MAX(date_int)
						   FROM @counting_table_time
						   GROUP BY id_court;

	-- result
	INSERT @table SELECT tmp1.id_court as 'court id', tmp1.address as "court address",
					tmp1.team_name as 'team name'
					FROM @counting_table_time as "tmp1" JOIN @counting_table as "tmp2"
					ON tmp1.id_court = tmp2.id_court AND tmp1.date_int = tmp2.date_int;
	RETURN;
END;

go
SELECT * FROM football.udf_latest_team_to_train_in_each_court();


--# # #    udf_team_that_trained_more_by_court    # # #

go
--DROP FUNCTION football.udf_team_that_trained_more_by_court;

go
CREATE FUNCTION football.udf_team_that_trained_more_by_court()
RETURNS @table TABLE ("court id" int, "court address" varchar(150),  "team name" varchar(50))
WITH SCHEMABINDING, ENCRYPTION
AS
BEGIN
	-- table com id_court, practice_count e team_name
	DECLARE @counting_table_practices TABLE ("id_court" int, "address" varchar(150), "practice_count" int, "team_name" varchar(50))
	
	INSERT @counting_table_practices SELECT court.id_court, court.address,
										COUNT(practice.date) + COUNT(practice.hour) as "practice_count",
										practice.team_name
										FROM football.court JOIN football.practice
										ON court.id_court = practice.id_court
										GROUP BY court.id_court, court.address, practice.team_name;

	-- get max for each court
	DECLARE @counting_table TABLE ("id_court" int, "practice_count" int)

	INSERT @counting_table SELECT id_court, MAX(practice_count)
						   FROM @counting_table_practices
						   GROUP BY id_court;

	-- result
	INSERT @table SELECT tmp1.id_court as 'court id', tmp1.address as "court address",
					tmp1.team_name as 'team name'
					FROM @counting_table_practices as "tmp1" JOIN @counting_table as "tmp2"
					ON tmp1.id_court = tmp2.id_court AND tmp1.practice_count = tmp2.practice_count;
	RETURN;
END;

go
SELECT * FROM football.udf_team_that_trained_more_by_court();
