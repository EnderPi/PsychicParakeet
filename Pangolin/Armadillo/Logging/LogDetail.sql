CREATE TABLE [Logging].[LogDetail]
(
	[Id] BIGINT NOT NULL, 
	[DetailId] INT NOT NULL,
	[Key] VARCHAR(100) NOT NULL,
	[Value] VARCHAR(MAX) NOT NULL,
	CONSTRAINT [PK_LogDetail] PRIMARY KEY ([Id], [DetailId]),
    CONSTRAINT [FK_LogDetail_ToTable] FOREIGN KEY ([Id]) REFERENCES [Logging].[Log]([Id])
)
