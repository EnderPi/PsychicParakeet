CREATE PROCEDURE [Configuration].[GetGlobalSetting]
	@SettingName VARCHAR(100)
AS
	SELECT [Name], [Value] FROM [Configuration].[GlobalSettings] WHERE [Name] = @SettingName
