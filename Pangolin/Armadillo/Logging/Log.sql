CREATE TABLE [Logging].[Log]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[TimeStamp] DATETIME NOT NULL,
	[Source] VARCHAR(100) NOT NULL,
	[LogLevel] TINYINT NOT NULL,
	[Message] VARCHAR(MAX) NOT NULL
)

GO

CREATE INDEX [IX_Log_Source_TimeStamp] ON [Logging].[Log] ([Source], [TimeStamp])
