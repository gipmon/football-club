use p4g5;

go

CREATE NONCLUSTERED INDEX indexsalaryinternalpeople ON football.internal_people(salary)
WITH (FILLFACTOR=75,pad_index=ON);

go

CREATE NONCLUSTERED INDEX indexheightplayer ON football.player(height)
WITH (FILLFACTOR=75,pad_index=ON);

go

CREATE NONCLUSTERED INDEX indexweigthplayer ON football.player(weight)
WITH (FILLFACTOR=75,pad_index=ON);