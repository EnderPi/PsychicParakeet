CREATE PROCEDURE [MessageQueue].[DeleteSubscription]
	@ApplicationName VARCHAR(100),
	@EventType VARCHAR(200)
AS
	DELETE FROM [MessageQueue].[Subscriptions] WHERE [ApplicationQueue] = @ApplicationName AND EventType =  @EventType
