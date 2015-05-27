use p4g5;

go

CREATE NONCLUSTERED INDEX indexactiveseat ON football.seat(active)
WITH (FILLFACTOR=75,pad_index=ON);
