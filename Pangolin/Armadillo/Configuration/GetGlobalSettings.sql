CREATE PROCEDURE [Configuration].[GetGlobalSettings]
AS
	SELECT [Name], [Value] FROM [Configuration].[GlobalSettings]

