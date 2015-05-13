use p4g5;

go 

-- DROP PROC football.sp_createPractice

CREATE PROCEDURE football.sp_createPractice
	@id_court		INT,
	@date			DATE,
	@hour			TIME(7),
	@team_name		VARCHAR(50)
WITH ENCRYPTION
AS 
	IF @id_court is null OR @date is null OR @hour is null
		OR @team_name is null
	BEGIN
		PRINT 'The court id, date, hour or team name can not be null!'
		RETURN
	END
	
	
	DECLARE @count int

	-- check if the practice exists
	SELECT @count = count(id_court) FROM football.practice WHERE id_court = @id_court
																 AND date = @date
																 AND hour = @hour;

	IF @count != 0
	BEGIN
		RAISERROR ('The practice that you provided already exists!', 14, 1)
		RETURN
	END

	-- check if the team exists
	SELECT @count = count(name) FROM football.team WHERE name = @team_name;

	IF @count = 0
	BEGIN
		RAISERROR ('The team that you provided do not exists!', 14, 1)
		RETURN
	END

	BEGIN TRANSACTION;

	BEGIN TRY
		INSERT INTO football.practice 
					([date],
					 [hour],
					 [id_court],
					 [team_name]) 
		VALUES      ( @date,
					  @hour,
					  @id_court,
					  @team_name) 
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when creating the practice!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;

go 

-- DROP PROC football.sp_modifyPractice

CREATE PROCEDURE football.sp_modifyPractice
	@id_court		INT,
	@date			DATE,
	@hour			TIME(7),
	@team_name		VARCHAR(50)
WITH ENCRYPTION
AS 
	IF @id_court is null OR @date is null OR @hour is null
		OR @team_name is null
	BEGIN
		PRINT 'The court id, date, hour or team name can not be null!'
		RETURN
	END
	
	DECLARE @count int

	-- check if the practice exists
	SELECT @count = count(id_court) FROM football.practice WHERE id_court = @id_court
																 AND date = @date
																 AND hour = @hour;

	IF @count = 0
	BEGIN
		RAISERROR ('The practice that you provided do not exists!', 14, 1)
		RETURN
	END
	
	-- check if the team exists
	SELECT @count = count(name) FROM football.team WHERE name = @team_name;

	IF @count = 0
	BEGIN
		RAISERROR ('The team that you provided do not exists!', 14, 1)
		RETURN
	END

	BEGIN TRANSACTION;

	BEGIN TRY
		UPDATE  football.practice SET
				team_name = @team_name
		WHERE id_court = @id_court
			  AND date = @date
			  AND hour = @hour;

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when updating the practice!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;

go 

-- DROP PROC football.sp_deletePractice

CREATE PROCEDURE football.sp_deletePractice
	@id_court		INT,
	@date			DATE,
	@hour			TIME(7)
WITH ENCRYPTION
AS 
	IF @id_court is null
	BEGIN
		PRINT 'The court id can not be null!'
		RETURN
	END
	
	IF @id_court is null OR @date is null OR @hour is null
	BEGIN
		PRINT 'The court id, date or hour can not be null!'
		RETURN
	END
	
	DECLARE @count int

	-- check if the practice exists
	SELECT @count = count(id_court) FROM football.practice WHERE id_court = @id_court
																 AND date = @date
																 AND hour = @hour;

	IF @count = 0
	BEGIN
		RAISERROR ('The practice that you provided do not exists!', 14, 1)
		RETURN
	END

	BEGIN TRANSACTION;

	BEGIN TRY
		DELETE FROM football.practice WHERE id_court = @id_court
											AND date = @date
											AND hour = @hour;
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when try delete the practice!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;