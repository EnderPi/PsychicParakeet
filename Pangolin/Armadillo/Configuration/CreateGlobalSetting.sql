CREATE PROCEDURE [Configuration].[CreateGlobalSetting]
	@SettingName VARCHAR(100),
	@SettingValue VARCHAR(MAX)
AS
	INSERT INTO [Configuration].[GlobalSettings] ([Name], [Value]) VALUES (@SettingName, @SettingValue)
