use p4g5;

go 

-- DROP PROC football.sp_createCourt

CREATE PROCEDURE football.sp_createCourt
  @address			VARCHAR(150)
WITH ENCRYPTION
AS 
	IF @address is null
	BEGIN
		PRINT 'The address can not be null!'
		RETURN
	END
	
	BEGIN TRY
		INSERT INTO football.court 
					([address]) 
		VALUES      ( @address) 
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when creating the court!', 14, 1)
	END CATCH;

go 

-- DROP PROC football.sp_modifyCourt

CREATE PROCEDURE football.sp_modifyCourt
  @id_court		INT,
  @address		VARCHAR(50)
WITH ENCRYPTION
AS 
	IF @id_court is null OR @address is null
	BEGIN
		PRINT 'The court id and address can not be null!'
		RETURN
	END
	
	DECLARE @count int

	-- check if the court exists
	SELECT @count = count(id_court) FROM football.court WHERE id_court = @id_court;

	IF @count = 0
	BEGIN
		RAISERROR ('The court that you provided do not exists!', 14, 1)
		RETURN
	END

	BEGIN TRY
		UPDATE  football.court SET
				address = @address
		WHERE id_court = @id_court;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when updating the court!', 14, 1)
	END CATCH;

go 

-- DROP PROC football.sp_deleteCourt

CREATE PROCEDURE football.sp_deleteCourt
  @id_court			INT
WITH ENCRYPTION
AS 
	IF @id_court is null
	BEGIN
		PRINT 'The court id can not be null!'
		RETURN
	END
	
	DECLARE @count int;

	-- check if the court exists in practices
	SELECT @count = count(id_court) FROM football.practice WHERE id_court = @id_court;

	IF @count != 0
	BEGIN
		RAISERROR ('Please delete the practices in this court first!', 14, 1)
		RETURN
	END

	BEGIN TRY
		DELETE FROM football.court WHERE id_court = @id_court;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when try delete the court!', 14, 1)
	END CATCH;