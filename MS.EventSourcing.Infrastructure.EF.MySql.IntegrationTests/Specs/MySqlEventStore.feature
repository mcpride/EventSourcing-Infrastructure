#language: de
Funktionalität: Schreiben in und Lesen aus dem MySql-EventStore

Szenario: Schreiben und wieder Auslesen von Domänenereignissen
	Gegeben sei ein MySqlEventStore
	Und eine definierte Aggregat-ID
	Und eine Liste von Domänen-Ereignissen zu dieser Aggregat-ID
	Wenn ich diese Liste von Domänen-Ereignissen in den MySqlEventStore speichere
	Dann sollte ich diese Domänen-Ereignisse auch wieder auslesen können