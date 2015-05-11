use p4g5;

go 

-- DROP PROC football.sp_createStaff

CREATE PROCEDURE football.sp_createStaff
  @bi				INT, 
  @name				VARCHAR(75),
  @address			VARCHAR(75), 
  @birth_date		DATE, 
  @nif				INT, 
  @gender			VARCHAR(1), 
  @nationality		VARCHAR(75),
  @salary			MONEY,
  @department_id    INT,
  @role	            VARCHAR(50)

WITH ENCRYPTION
AS 
	IF @bi is null OR @name is null OR @address is null OR @birth_date is null OR @nif is null OR 
		@gender is null OR @nationality is null OR @salary is null OR @department_id is null OR @role is null
	BEGIN
		PRINT 'The bi, name, address, birth_date, nif, nationality, salary, department id and role can not be null!'
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

		INSERT INTO football.internal_people 
					([bi], 
					 [salary]) 
		VALUES      ( @bi, 
					  @salary) 

		INSERT INTO football.staff 
					([bi],
					 [department_id],
					 [role]) 
		VALUES      ( @bi, 
					  @department_id,
					  @role)
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when creating the staff!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;

	go 

-- DROP PROC football.sp_modifyStaff

CREATE PROCEDURE football.sp_modifyStaff
  @bi				INT, 
  @name				VARCHAR(75),
  @address			VARCHAR(75), 
  @birth_date		DATE, 
  @gender			VARCHAR(1), 
  @nationality		VARCHAR(75),
  @salary			MONEY,
  @department_id	INT,
  @role				VARCHAR(50)

WITH ENCRYPTION
AS 
	IF @bi is null OR @name is null OR @address is null OR @birth_date is null OR @gender is null
		OR @nationality is null OR @salary is null OR @department_id is null OR @role is null 

	BEGIN
		PRINT 'The bi, name, address, birth_date, nationality, salary, department_id and role can not be null!'
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

		UPDATE football.internal_people SET
			   salary = @salary
		WHERE bi = @bi;

		UPDATE football.staff SET
			   department_id = @department_id, 
			   role = @role
		WHERE bi = @bi;

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when updating the staff!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;

	go 

-- DROP PROC football.sp_deleteStaff

CREATE PROCEDURE football.sp_deleteStaff
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
		DELETE FROM football.staff WHERE bi = @bi;
		DELETE FROM football.internal_people WHERE bi = @bi;
		DELETE FROM football.person WHERE bi = @bi;
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when try delete the staff!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;