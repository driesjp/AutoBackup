AutoBackup (service) is a Windows Service that utilises FileSystemWatcher to monitor filechanges and create an identical backup as files are created/renamed/deleted. Logs are kept in the /Logs folder.

AutoBackup Config (GUI) is an (admittedly, old skool) WinForms application for configuring the service, choosing sources and target folder and running an integrity check to validate the current backups.

Powershell script (testing only) is a PowerShell script for generating a series of text files in several folders to test the service.
