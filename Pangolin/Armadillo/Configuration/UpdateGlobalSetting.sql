CREATE PROCEDURE [Configuration].[UpdateGlobalSetting]
	@SettingName VARCHAR(100),
	@SettingValue VARCHAR(MAX)
AS
	UPDATE [Configuration].[GlobalSettings] SET [Value] = @SettingValue WHERE [Name] = @SettingName
