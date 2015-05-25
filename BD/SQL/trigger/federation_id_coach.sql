CREATE Trigger federation_coach ON football.coach
AFTER INSERT
AS
	SET NOCOUNT ON;

	DECLARE @count int
	-- check if the federation id is already in use
	SELECT @count = count(coach.federation_id) FROM football.coach JOIN inserted ON coach.federation_id = inserted.federation_id;

	IF @count != 0
	BEGIN
		RAISERROR ('The federation id is already in use!', 14, 1)
		RETURN
	END

	-- check if the federation id is already in use
	SELECT @count = count(player.federation_id) FROM football.player JOIN inserted ON player.federation_id = inserted.federation_id;

	IF @count != 0
	BEGIN
		RAISERROR ('The federation id is already in use by one player!', 14, 1)
		RETURN
	END