CREATE Trigger seatTrigger ON football.seat
INSTEAD OF DELETE
AS
	SET NOCOUNT ON;
	DECLARE @n_seat INT;
	DECLARE @row VARCHAR(1);
	DECLARE @id_Section INT;

	SELECT @n_seat = deleted.n_seat FROM deleted;
	SELECT @row = deleted.row FROM deleted;
	SELECT @id_section = deleted.id_section FROM deleted;

	BEGIN
		UPDATE football.seat SET
			   active = 0
			   WHERE seat.n_seat = @n_seat AND seat.row = @row AND seat.id_section = @id_section;
	END