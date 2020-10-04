CREATE PROCEDURE [MessageQueue].[CreateSubscription]
	@ApplicationName VARCHAR(100),
	@EventType VARCHAR(200)
AS
	IF NOT EXISTS (SELECT 1 FROM [MessageQueue].[Subscriptions] WHERE [ApplicationQueue] = @ApplicationName AND [EventType] = @EventType)
	BEGIN
		INSERT INTO [MessageQueue].[Subscriptions] ([ApplicationQueue],[EventType]) VALUES (@ApplicationName,@EventType)
	END
