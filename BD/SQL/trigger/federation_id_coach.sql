CREATE Trigger federation_coach_insert ON football.coach
AFTER INSERT
AS
	SET NOCOUNT ON;

	DECLARE @count int

	-- check if the federation id is already in use
	SELECT @count = count(coach.federation_id) FROM football.coach JOIN inserted ON coach.federation_id = inserted.federation_id ;

	IF @count > 1
	BEGIN
		RAISERROR ('The federation id is already in use!', 16, 1)
		ROLLBACK TRANSACTION
	END

	-- check if the federation id is already in use
	SELECT @count = count(player.federation_id) FROM football.player JOIN inserted ON player.federation_id = inserted.federation_id;

	IF @count > 1
	BEGIN
		RAISERROR ('The federation id is already in use by one player!', 16, 1)
		ROLLBACK TRANSACTION
	END
go

CREATE Trigger federation_coach_update ON football.coach
AFTER UPDATE
AS
	SET NOCOUNT ON;

	DECLARE @count int

	-- check if the federation id is already in use
	SELECT @count = count(coach.federation_id) FROM football.coach JOIN inserted ON coach.federation_id = inserted.federation_id AND coach.bi != inserted.bi;

	IF @count > 1
	BEGIN
		RAISERROR ('The federation id is already in use!', 16, 1)
		ROLLBACK TRANSACTION
	END

	-- check if the federation id is already in use player
	SELECT @count = count(player.federation_id) FROM football.player JOIN inserted ON player.federation_id = inserted.federation_id AND player.bi != inserted.bi;

	IF @count > 1
	BEGIN
		RAISERROR ('The federation id is already in use by one player!', 16, 1)
		ROLLBACK TRANSACTION
	END