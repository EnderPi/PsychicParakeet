CREATE TABLE [Configuration].[ApplicationSettings]
(
	[Application] VARCHAR(100) NOT NULL,
	[Name] VARCHAR(100) NOT NULL,
	[Value] VARCHAR(MAX) NOT NULL,
	CONSTRAINT PK_ApplicationSetting PRIMARY KEY ([Application], [Name])
)

