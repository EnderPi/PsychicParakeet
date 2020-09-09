CREATE PROCEDURE [Configuration].[DeleteGlobalSetting]
	@SettingName VARCHAR(100)
AS
	DELETE FROM [Configuration].[GlobalSettings] WHERE [Name] = @SettingName
