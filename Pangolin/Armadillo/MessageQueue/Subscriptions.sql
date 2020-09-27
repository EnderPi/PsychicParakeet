CREATE TABLE [MessageQueue].[Subscriptions]
(
	[ApplicationQueue] VARCHAR(100) NOT NULL,
	[EventType] VARCHAR(200) NOT NULL
)

GO

CREATE UNIQUE CLUSTERED INDEX [SubscriptionIndex] ON [MessageQueue].[Subscriptions] ([ApplicationQueue] ASC, [EventType] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
