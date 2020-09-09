CREATE PROCEDURE [Configuration].[GetApplicationSettingsByApplication]
	@ApplicationName VARCHAR(100)
AS
	SELECT [Application], [Name], [Value] FROM [Configuration].[ApplicationSettings] WHERE [Application] = @ApplicationName
