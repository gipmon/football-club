use p4g5;

go 

EXEC football.sp_createPlayer @bi = null, @name = null, @address = null, @birth_date = null,
							  @nif = null, @gender = null, @nationality = null, @salary = null,
							  @federation_id = null, @weight = null, @height = null;