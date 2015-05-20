use p4g5;

go 

-- DROP PROC football.sp_createSeat

CREATE PROCEDURE football.sp_createSeat
  @n_seat			INT, 
  @row  			VARCHAR(1), 
  @id_section       INT

WITH ENCRYPTION
AS 
	IF @n_seat is null OR @row is null OR @id_section is null
	BEGIN
		PRINT 'The n_seat, row and id_section can not be null!'
		RETURN
	END
	
	DECLARE @count int

	-- check if the Annual seat is already in use
	SELECT @count = count(n_seat) FROM football.annual_seat WHERE n_seat = @n_seat AND row = @row AND id_section = @id_section;

	IF @count != 0
	BEGIN
		RAISERROR ('The seat already exists!', 14, 1)
		RETURN
	END

	BEGIN TRANSACTION;

	BEGIN TRY
		INSERT INTO football.seat
					([n_seat], 
					 [row], 
					 [id_section]) 
		VALUES      ( @n_seat, 
					  @row, 
					  @id_section) 
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when creating the seat!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;


go 

-- DROP PROC football.sp_deleteSeat

CREATE PROCEDURE football.sp_deleteSeat
  @n_seat				INT,
  @row					VARCHAR(1),
  @id_section			INT
WITH ENCRYPTION
AS 
	IF @n_seat is null OR @id_section is null OR @row is null
	BEGIN
		PRINT 'The n_seat, id_section and row can not be null!'
		RETURN
	END

	DECLARE @count int

		-- check if the seat is Annual seat
	SELECT @count = count(n_seat) FROM football.annual_seat WHERE n_seat = @n_seat AND row = @row AND id_section = @id_section;

	IF @count != 0
	BEGIN
		RAISERROR ('The seat is Annual seat!', 14, 1)
		RETURN
	END

	-- check if the seat exists
	SELECT @count = count(n_seat) FROM football.seat WHERE n_seat = @n_seat AND row = @row AND id_section = @id_section;

	IF @count = 0
	BEGIN
		RAISERROR ('The seat doesnt exists!', 14, 1)
		RETURN
	END
	
	BEGIN TRANSACTION;

	BEGIN TRY
		DELETE FROM football.seat WHERE n_seat = @n_seat AND id_section = @id_section AND row = @row;
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when try delete the seat!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;
