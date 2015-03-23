#language: de
Funktionalität: Schreiben in und Lesen aus dem MySqlSnapshotStore

Szenario: Schreiben und wieder Auslesen von Snapshots
	Gegeben sei ein MySqlSnapshotStore
	Und eine Snapshotentität
	Wenn ich die Snapshotentität in den MySqlSnapshotStore speichere
	Dann sollte ich diese Snapshotentität auch wieder auslesen können