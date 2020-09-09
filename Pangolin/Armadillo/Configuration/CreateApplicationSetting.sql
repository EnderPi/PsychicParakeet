CREATE PROCEDURE [Configuration].[CreateApplicationSetting]
	@ApplicationName VARCHAR(100),
	@SettingName VARCHAR(100),
	@SettingValue VARCHAR(MAX)
AS
	INSERT INTO [Configuration].[ApplicationSettings] ([Application], [Name], [Value]) VALUES (@ApplicationName, @SettingName, @SettingValue)
