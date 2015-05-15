use p4g5;

go 

-- DROP PROC football.sp_createSpot

CREATE PROCEDURE football.sp_createSpot
  @n_spot			INT, 
  @row  			VARCHAR(1), 
  @id_section       INT

WITH ENCRYPTION
AS 
	IF @n_spot is null OR @row is null OR @id_section is null
	BEGIN
		PRINT 'The n_spot, row and id_section can not be null!'
		RETURN
	END
	
	DECLARE @count int

	-- check if the Annual Spot is already in use
	SELECT @count = count(n_spot) FROM football.annual_spot WHERE n_spot = @n_spot AND row = @row AND id_section = @id_section;

	IF @count != 0
	BEGIN
		RAISERROR ('The Spot already exists!', 14, 1)
		RETURN
	END

	BEGIN TRANSACTION;

	BEGIN TRY
		INSERT INTO football.spot
					([n_spot], 
					 [row], 
					 [id_section]) 
		VALUES      ( @n_spot, 
					  @row, 
					  @id_section) 
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when creating the spot!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;


go 

-- DROP PROC football.sp_deleteSpot

CREATE PROCEDURE football.sp_deleteSpot
  @n_spot				INT,
  @row					VARCHAR(1),
  @id_section			INT
WITH ENCRYPTION
AS 
	IF @n_spot is null OR @id_section is null OR @row is null
	BEGIN
		PRINT 'The n_spot, id_section and row can not be null!'
		RETURN
	END

	DECLARE @count int

		-- check if the Spot is Annual Spot
	SELECT @count = count(n_spot) FROM football.annual_spot WHERE n_spot = @n_spot AND row = @row AND id_section = @id_section;

	IF @count != 0
	BEGIN
		RAISERROR ('The Spot is Annual Spot!', 14, 1)
		RETURN
	END

	-- check if the spot exists
	SELECT @count = count(n_spot) FROM football.spot WHERE n_spot = @n_spot AND row = @row AND id_section = @id_section;

	IF @count = 0
	BEGIN
		RAISERROR ('The spot doesnt exists!', 14, 1)
		RETURN
	END
	
	BEGIN TRANSACTION;

	BEGIN TRY
		DELETE FROM football.spot WHERE n_spot = @n_spot AND id_section = @id_section AND row = @row;
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when try delete the spot!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;
