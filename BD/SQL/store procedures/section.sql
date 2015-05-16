use p4g5;

go 

-- DROP PROC football.sp_createSection

CREATE PROCEDURE football.sp_createSection
  @type				VARCHAR(75)
WITH ENCRYPTION
AS 
	IF @type is null
	BEGIN
		PRINT 'The type can not be null!'
		RETURN
	END

	BEGIN TRANSACTION;

	BEGIN TRY
		INSERT INTO football.section 
					([type]) 
		VALUES      ( @type) 
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when creating the section!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;

go 

-- DROP PROC football.sp_modifySection

CREATE PROCEDURE football.sp_modifySection
  @type  		VARCHAR(75),
  @id_section   INT
WITH ENCRYPTION
AS 
	IF @type is null
	BEGIN
		PRINT 'The type can not be null!'
		RETURN
	END
	
	DECLARE @count int

	-- check if the section exists
	SELECT @count = count(id_section) FROM football.section WHERE id_section = @id_section;

	IF @count = 0
	BEGIN
		RAISERROR ('The section that you provided do not exists!', 14, 1)
		RETURN
	END

	BEGIN TRANSACTION;

	BEGIN TRY
		UPDATE  football.section SET
				type = @type
		WHERE id_section = @id_section;

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when updating the section!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;

go 

-- DROP PROC football.sp_deleteSection

CREATE PROCEDURE football.sp_deleteSection
  @id_section			INT
WITH ENCRYPTION
AS 
	IF @id_section is null
	BEGIN
		PRINT 'The section ID can not be null!'
		RETURN
	END

	DECLARE @count int

	-- check if the section exists
	SELECT @count = count(id_section) FROM football.section WHERE id_section = @id_section;

	IF @count = 0
	BEGIN
		RAISERROR ('The section that you provided do not exists!', 14, 1)
		RETURN
	END
	
	BEGIN TRANSACTION;

	BEGIN TRY
		DELETE FROM football.annual_spot WHERE id_section = @id_section;
		DELETE FROM football.spot WHERE id_section = @id_section;
		DELETE FROM football.section WHERE id_section = @id_section;
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		RAISERROR ('An error occurred when try delete the section!', 14, 1)
		ROLLBACK TRANSACTION;
	END CATCH;