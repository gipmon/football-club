use p4g5;

go 

-- DROP PROC football.sp_createAnnualSeat

CREATE PROCEDURE football.sp_createAnnualSeat
  @bi				INT,  
  @start_date		DATE, 
  @n_seat			INT, 
  @row  			VARCHAR(1), 
  @value			INT,
  @id_section       INT,
  @season	        INT,
  @duration			INT

WITH ENCRYPTION
AS 
	IF @bi is null OR @start_date is null OR @n_seat is null OR @row is null OR @value is null OR 
		@id_section is null OR @season is null OR @duration is null
	BEGIN
		PRINT 'The bi, start_date, n_seat, row, value, id_section, season and duration can not be null!'
		RETURN
	END
	
	DECLARE @count int

	-- check if the BI exists
	SELECT @count = count(bi) FROM football.person WHERE bi = @bi;

	IF @count = 0
	BEGIN
		RAISERROR ('The BI that you provided do not exists!', 14, 1)
		RETURN
	END

	-- check if the Annual seat is already in use
	SELECT @count = count(bi) FROM football.annual_seat WHERE n_seat = @n_seat AND row = @row AND bi = @bi AND id_section = @id_section AND season = @season;

	IF @count != 0
	BEGIN
		RAISERROR ('The Annual seat is already in use!', 14, 1)
		RETURN
	END

	-- check if the Annual seat is already in use
	SELECT @count = count(n_seat) FROM football.annual_seat WHERE n_seat = @n_seat AND row = @row AND id_section = @id_section AND season = @season;

	IF @count != 0
	BEGIN
		RAISERROR ('The Annual seat is already in use!', 14, 1)
		RETURN
	END


	-- check if the seat exists
	SELECT @count = count(n_seat) FROM football.seat WHERE n_seat = @n_seat AND row = @row AND id_section = @id_section;

	IF @count = 0
	BEGIN
		RAISERROR ('The seat doesnt exist!', 14, 1)
		RETURN
	END

	BEGIN TRANSACTION;

	BEGIN TRY
		INSERT INTO football.annual_seat 
					([n_seat], 
					 [row], 
					 [id_section], 
					 [start_date], 
					 [duration], 
					 [value],
					 [bi],
					 [season]) 
		VALUES      ( @n_seat, 
					  @row, 
					  @id_section, 
					  @start_date, 
					  @duration, 
					  @value,
					  @bi,
					  @season) 
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when creating the annual seat!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;


go 

-- DROP PROC football.sp_modifyAnnualSeat

CREATE PROCEDURE football.sp_modifyAnnualSeat
  @bi				INT,  
  @start_date		DATE, 
  @n_seat			INT, 
  @row  			VARCHAR(1), 
  @value			INT,
  @id_section       INT,
  @season	        INT,
  @duration			INT

WITH ENCRYPTION
AS 
	IF @bi is null OR @start_date is null OR @n_seat is null OR @row is null OR @value is null OR 
		@id_section is null OR @season is null OR @duration is null
	BEGIN
		PRINT 'The bi, start_date, n_seat, row, value, id_section, season and duration can not be null!'
		RETURN
	END
	
	DECLARE @count int

	-- check if the BI exists
	SELECT @count = count(bi) FROM football.person WHERE bi = @bi;

	IF @count = 0
	BEGIN
		RAISERROR ('The BI that you provided do not exists!', 14, 1)
		RETURN
	END

	-- check if the seat exists
	SELECT @count = count(n_seat) FROM football.seat WHERE n_seat = @n_seat AND row = @row AND id_section = @id_section;

	IF @count = 0
	BEGIN
		RAISERROR ('The seat doesnt exist!', 14, 1)
		RETURN
	END

	BEGIN TRANSACTION;

	BEGIN TRY
		UPDATE  football.annual_seat SET
				start_date = @start_date,
				id_section = @id_section,
				duration = @duration,
				value = @value
		WHERE n_seat = @n_seat AND row = @row AND id_section = @id_section AND bi = @bi AND season = @season;

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when updating the annual seat!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;

		go 

-- DROP PROC football.sp_deleteAnnualSeat

CREATE PROCEDURE football.sp_deleteAnnualSeat
  @n_seat				INT,
  @row					VARCHAR(1),
  @id_section			INT,
  @bi					INT,
  @season				INT
WITH ENCRYPTION
AS 
	IF @bi is null OR @n_seat is null OR @id_section is null OR @row is null OR @season is null
	BEGIN
		PRINT 'The bi, n_seat, id_section, row and season can not be null!'
		RETURN
	END
	
	BEGIN TRANSACTION;

	BEGIN TRY
		DELETE FROM football.annual_seat WHERE bi = @bi AND n_seat = @n_seat AND id_section = @id_section AND row = @row AND season = @season;
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when try delete the annual seat!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;