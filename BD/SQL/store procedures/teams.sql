use p4g5;

go 

-- DROP PROC football.sp_createTeam

CREATE PROCEDURE football.sp_createTeam
  @name				VARCHAR(50),
  @max_age			INT
WITH ENCRYPTION
AS 
	IF @name is null OR @max_age is null
	BEGIN
		PRINT 'The name and max_age can not be null!'
		RETURN
	END
	
	DECLARE @count int

	-- check if the name is already in use
	SELECT @count = count(name) FROM football.team WHERE name = @name;

	IF @count != 0
	BEGIN
		RAISERROR ('The name is already in use!', 14, 1)
		RETURN
	END

	BEGIN TRY
		INSERT INTO football.team 
					([name], 
					 [max_age]) 
		VALUES      ( @name, 
					  @max_age) 
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when creating the team!', 14, 1)
	END CATCH;

go 

-- DROP PROC football.sp_modifyTeam

CREATE PROCEDURE football.sp_modifyTeam
  @name				VARCHAR(50),
  @max_age			INT
WITH ENCRYPTION
AS 
	IF @name is null OR @max_age is null
	BEGIN
		PRINT 'The name and max_age can not be null!'
		RETURN
	END
	
	DECLARE @count int

	-- check if the team exists
	SELECT @count = count(name) FROM football.team WHERE name = @name;

	IF @count = 0
	BEGIN
		RAISERROR ('The team that you provided do not exists!', 14, 1)
		RETURN
	END

	BEGIN TRY
		UPDATE  football.team SET
				max_age = @max_age
		WHERE name = @name;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when updating the team!', 14, 1)
	END CATCH;

go 

-- DROP PROC football.sp_deleteTeam

CREATE PROCEDURE football.sp_deleteTeam
  @name			VARCHAR(50)
WITH ENCRYPTION
AS 
	IF @name is null
	BEGIN
		PRINT 'The name can not be null!'
		RETURN
	END
	
	BEGIN TRANSACTION;

	BEGIN TRY
		DELETE FROM football.heads WHERE team_name = @name;
		DELETE FROM football.play WHERE team_name = @name;
		DELETE FROM football.practice WHERE team_name = @name;
		DELETE FROM football.team WHERE name = @name;
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when try delete the team!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;