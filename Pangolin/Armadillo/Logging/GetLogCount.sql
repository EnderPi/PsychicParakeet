CREATE PROCEDURE [Logging].[GetLogCount]	
AS
	SELECT COUNT(Id) FROM [Logging].[Log]

