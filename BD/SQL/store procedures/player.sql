use p4g5;

go 

-- DROP PROC football.sp_createPlayer

CREATE PROCEDURE football.sp_createPlayer
  @bi				INT, 
  @name				VARCHAR(75),
  @address			VARCHAR(75), 
  @birth_date		DATE, 
  @nif				INT, 
  @gender			VARCHAR(1), 
  @nationality		VARCHAR(75),
  @salary			MONEY,
  @federation_id	INT,
  @weight			INT,
  @height			INT
WITH ENCRYPTION
AS 
	IF @bi is null OR @name is null OR @address is null OR @birth_date is null OR @nif is null OR 
		@gender is null OR @nationality is null OR @salary is null OR @federation_id is null OR
		@weight is null OR @height is null
	BEGIN
		PRINT 'The bi, name, address, birth_date, nif, nationality, salary, federation_id, weight and height can not be null!'
		RETURN
	END
	
	DECLARE @count int

	-- check if the BI is already in use
	SELECT @count = count(bi) FROM football.person WHERE bi = @bi;

	IF @count != 0
	BEGIN
		RAISERROR ('The BI id is already in use!', 14, 1)
		RETURN
	END

	-- check if the federation id is already in use
	SELECT @count = count(federation_id) FROM football.player WHERE federation_id = @federation_id;

	IF @count != 0
	BEGIN
		RAISERROR ('The federation id is already in use!', 14, 1)
		RETURN
	END

	-- check if the NIF is already in use
	SELECT @count = count(nif) FROM football.person WHERE nif = @nif;

	IF @count != 0
	BEGIN
		RAISERROR ('The NIF id is already in use!', 14, 1)
		RETURN
	END

	BEGIN TRANSACTION;

	BEGIN TRY
		INSERT INTO football.person 
					([bi], 
					 [name], 
					 [address], 
					 [birth_date], 
					 [nif], 
					 [gender],
					 [nationality]) 
		VALUES      ( @bi, 
					  @name, 
					  @address, 
					  @birth_date, 
					  @nif, 
					  @gender,
					  @nationality) 

		INSERT INTO football.internal_people 
					([bi], 
					 [salary]) 
		VALUES      ( @bi, 
					  @salary) 

		INSERT INTO football.player 
					([bi], 
					 [federation_id], 
					 [weight],
					 [height]) 
		VALUES      ( @bi, 
					  @federation_id, 
					  @weight,
					  @height)
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when creating the player!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;