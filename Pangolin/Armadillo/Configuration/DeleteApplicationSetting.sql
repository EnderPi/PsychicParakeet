CREATE PROCEDURE [Configuration].[DeleteApplicationSetting]
	@ApplicationName VARCHAR(100),
	@SettingName VARCHAR(100)
AS
	DELETE FROM [Configuration].[ApplicationSettings] WHERE [Application] = @ApplicationName AND [Name] = @SettingName

