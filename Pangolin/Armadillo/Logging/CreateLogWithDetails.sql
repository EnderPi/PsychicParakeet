CREATE PROCEDURE [Logging].[CreateLogWithDetails]
	@TimeStamp DATETIME,
	@Source VARCHAR(100),
	@LogLevel TINYINT,
	@Message VARCHAR(max),
	@LogDetails LogDetails READONLY 
AS
	INSERT INTO [Logging].[Log] ([TimeStamp], [Source], [LogLevel],	[Message])
	VALUES (@TimeStamp, @Source, @LogLevel, @Message)
    DECLARE @LogId BIGINT = SCOPE_IDENTITY()
	INSERT INTO [Logging].[LogDetail] (Id, [DetailId], [Key], [Value])
	SELECT @LogId, ROW_NUMBER() OVER(ORDER BY [Key] ASC), [Key], [Value] FROM @LogDetails
