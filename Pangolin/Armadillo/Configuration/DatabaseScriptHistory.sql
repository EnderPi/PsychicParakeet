CREATE TABLE [Configuration].[DatabaseScriptHistory]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[Description] VARCHAR(200) NOT NULL,
	[TimeStamp] DATETIME NOT NULL
)
