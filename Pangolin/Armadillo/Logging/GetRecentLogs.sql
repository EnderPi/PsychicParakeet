CREATE PROCEDURE [Logging].[GetRecentLogs]
	@Count int	
AS
	SELECT TOP (@Count) [Id], [Source], [TimeStamp], [LogLevel], [Message] FROM [Logging].[Log] ORDER BY ID DESC

