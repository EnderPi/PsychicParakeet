CREATE PROCEDURE [Logging].[CreateLog]
    @TimeStamp DATETIME,
	@Source VARCHAR(100),
	@LogLevel TINYINT,
	@Message VARCHAR(max)
AS
	INSERT INTO [Logging].[Log] ([TimeStamp], [Source], [LogLevel],	[Message])
	VALUES (@TimeStamp, @Source, @LogLevel, @Message)
RETURN SCOPE_IDENTITY()
