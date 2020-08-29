CREATE PROCEDURE [Logging].[SearchLogs]
	@Source VARCHAR(100),
	@BeginTime DateTime,
	@EndTime DateTime
AS
	SELECT TOP (25) [Id], [Source], [TimeStamp], [LogLevel], [Message] 
	FROM [Logging].[Log] 
	WHERE [Source] = @Source AND [TimeStamp] BETWEEN @BeginTime AND @EndTime ORDER BY ID DESC

