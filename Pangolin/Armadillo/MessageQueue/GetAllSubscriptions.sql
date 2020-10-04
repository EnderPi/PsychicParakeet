CREATE PROCEDURE [MessageQueue].[GetAllSubscriptions]	
AS
	SELECT [ApplicationQueue], [EventType] FROM [MessageQueue].[Subscriptions]

