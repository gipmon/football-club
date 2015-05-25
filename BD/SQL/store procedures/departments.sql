use p4g5;

go 

-- DROP PROC football.sp_createDepartment

CREATE PROCEDURE football.sp_createDepartment
  @name				VARCHAR(75),
  @address		    VARCHAR(75)
WITH ENCRYPTION
AS 
	IF @name is null OR @address is null
	BEGIN
		PRINT 'The name and address can not be null!'
		RETURN
	END

	BEGIN TRY
		INSERT INTO football.department 
					([name], 
					 [address]) 
		VALUES      ( @name, 
					  @address)
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when creating the department!', 14, 1)
	END CATCH;

go 

-- DROP PROC football.sp_modifyDepartment

CREATE PROCEDURE football.sp_modifyDepartment
  @name				VARCHAR(75),
  @department_id    INT,
  @address			VARCHAR(75)
WITH ENCRYPTION
AS 
	IF @name is null OR @department_id is null OR @address is null
	BEGIN
		PRINT 'The name, department_id and address can not be null!'
		RETURN
	END
	
	DECLARE @count int

	-- check if the department exists
	SELECT @count = count(department_id) FROM football.department WHERE department_id = @department_id;

	IF @count = 0
	BEGIN
		RAISERROR ('The department that you provided do not exists!', 14, 1)
		RETURN
	END

	BEGIN TRY
		UPDATE  football.department SET
				name = @name,
				address = @address
		WHERE department_id = @department_id;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when updating the department!', 14, 1)
	END CATCH;

go 

-- DROP PROC football.sp_deleteDepartment

CREATE PROCEDURE football.sp_deleteDepartment
  @department_id			INT
WITH ENCRYPTION
AS 
	IF @department_id is null
	BEGIN
		PRINT 'The department_id can not be null!'
		RETURN
	END

	DECLARE @count int

	-- check if the department has staff
	SELECT @count = count(department_id) FROM football.staff WHERE department_id = @department_id;

	IF @count != 0
	BEGIN
		RAISERROR ('The department that you provided has staff!', 14, 1)
		RETURN
	END
	
	BEGIN TRY
		DELETE FROM football.department WHERE department_id = @department_id;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when try delete the department!', 14, 1)
	END CATCH;