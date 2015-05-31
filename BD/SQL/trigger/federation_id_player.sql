CREATE Trigger federation_player_insert ON football.player
AFTER INSERT
AS
	SET NOCOUNT ON;

	DECLARE @count int
	-- check if the federation id is already in use
	SELECT @count = count(player.federation_id) FROM football.player JOIN inserted ON player.federation_id = inserted.federation_id;

	IF @count > 1
	BEGIN
		RAISERROR ('The federation id is already in use!', 16, 1)
		ROLLBACK TRANSACTION
	END

	-- check if the federation id is already in use
	SELECT @count = count(coach.federation_id) FROM football.coach JOIN inserted ON coach.federation_id = inserted.federation_id;

	IF @count > 1
	BEGIN
		RAISERROR ('The federation id is already in use by one coach!', 16, 1)
		ROLLBACK TRANSACTION
	END

go

CREATE Trigger federation_player_update ON football.player
AFTER UPDATE
AS
	SET NOCOUNT ON;

	DECLARE @count int
	-- check if the federation id is already in use
	SELECT @count = count(player.federation_id) FROM football.player JOIN inserted ON player.federation_id = inserted.federation_id AND player.bi != inserted.bi;

	IF @count > 1
	BEGIN
		RAISERROR ('The federation id is already in use!', 14, 1)
		ROLLBACK TRANSACTION
	END

	-- check if the federation id is already in use
	SELECT @count = count(coach.federation_id) FROM football.coach JOIN inserted ON coach.federation_id = inserted.federation_id AND coach.bi != inserted.bi;

	IF @count > 1
	BEGIN
		RAISERROR ('The federation id is already in use by one coach!', 14, 1)
		ROLLBACK TRANSACTION
	END