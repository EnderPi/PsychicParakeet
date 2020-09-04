CREATE PROCEDURE [Configuration].[GetApplicationSettings]	
AS
	SELECT [Application], [Name], [Value] FROM [Configuration].[ApplicationSettings]
