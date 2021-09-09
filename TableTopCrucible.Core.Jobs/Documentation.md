1.- Es wird nur der Fortschritt getrackt, das Thread/Taskmanagement muss extern gehandelt werden
2.- Ein Job besteht aus mehreren ProgressHandler, diese können gewichtet werden und werden für die Gesamtfortschrittsberechnung verwendet
3.- (TODO)Job und Progression implementieren IDisposable. Wird dispose aufgerufen, witd das Objekt jedoch nicht zerstört sondern der Progress als abgeschlossen betrachtet
4.- Ein Job wird nach dem Erstellen um ProgressionManager erweitert, entweder sofort oder im Laufe der Ausführung