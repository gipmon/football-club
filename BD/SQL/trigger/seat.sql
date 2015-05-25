CREATE Trigger seat ON football.seat
INSTEAD OF DELETE
AS
	SET NOCOUNT ON;

	BEGIN
		UPDATE football.seat SET
			   active = 0
		WHERE n_seat = deleted.n_seat AND row = deleted.row AND id_section = deleted.id_section
	END