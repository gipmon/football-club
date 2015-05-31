use p4g5;

go 

-- DROP PROC football.sp_createMember

CREATE PROCEDURE football.sp_createMember
  @bi				INT, 
  @name				VARCHAR(75),
  @address			VARCHAR(75), 
  @birth_date		DATE, 
  @nif				INT, 
  @gender			VARCHAR(1), 
  @nationality		VARCHAR(75),
  @shares_value		MONEY,
  @shares_in_day	INT
 
WITH ENCRYPTION
AS 
	IF @bi is null OR @name is null OR @address is null OR @birth_date is null OR @nif is null OR 
		@gender is null OR @nationality is null OR @shares_value is null OR
		@shares_in_day is null
	BEGIN
		PRINT 'The bi, name, address, birth_date, nif, nationality, shares value and shares in day can not be null!'
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

		INSERT INTO football.members 
					([bi], 
					 [shares_value], 
					 [shares_in_day]) 
		VALUES      ( @bi, 
					  @shares_value, 
					  @shares_in_day)
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when creating the member!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;

go

-- DROP PROC football.sp_modifyMember

CREATE PROCEDURE football.sp_modifyMember
  @bi				INT, 
  @name				VARCHAR(75),
  @address			VARCHAR(75), 
  @birth_date		DATE, 
  @gender			VARCHAR(1), 
  @nationality		VARCHAR(75),
  @shares_value		MONEY,
  @shares_in_day	INT
WITH ENCRYPTION
AS 
	IF @bi is null OR @name is null OR @address is null OR @birth_date is null OR 
		@gender is null OR @nationality is null OR @shares_value is null OR
		@shares_in_day is null
	BEGIN
		PRINT 'The bi, name, address, birth_date, nationality, shares value and shares in day can not be null!'
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

	BEGIN TRANSACTION;

	BEGIN TRY
		UPDATE  football.person SET
				name = @name, 
				address = @address, 
				birth_date = @birth_date,
				gender = @gender,
				nationality = @nationality
		WHERE bi = @bi;

		UPDATE football.members SET
			   shares_value = @shares_value, 
			   shares_in_day = @shares_in_day	   
		WHERE bi = @bi;

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when updating the member!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;

go

CREATE PROCEDURE football.sp_deleteMember
  @bi				INT
WITH ENCRYPTION
AS 
	IF @bi is null
	BEGIN
		PRINT 'The bi can not be null!'
		RETURN
	END
	
	BEGIN TRANSACTION;

	BEGIN TRY
		DELETE FROM football.annual_seat WHERE bi = @bi;
		DELETE FROM football.members WHERE bi = @bi;
		DELETE FROM football.person WHERE bi = @bi;
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when try delete the member!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;