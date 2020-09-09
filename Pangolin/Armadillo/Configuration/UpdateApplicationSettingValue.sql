CREATE PROCEDURE [Configuration].[UpdateApplicationSettingValue]
	@ApplicationName VARCHAR(100),
	@SettingName VARCHAR(100),
	@SettingValue VARCHAR(MAX)
AS
	UPDATE [Configuration].[ApplicationSettings] SET [Value] = @SettingValue WHERE [Application] = @ApplicationName AND [Name] = @SettingName

