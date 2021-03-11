CREATE PROCEDURE [Simulations].[GetNextReadyTaskNotInFlight]
	@CurrentlyRunningSimulations SimulationIds READONLY
AS
	WITH T AS 
	(SELECT TOP 1 [Id], 
		[SaveFile], 
		[Description], 
		[LastTimeSaved], 
		[SimulationObject], 
		[IsRunning], 
		[IsFinished], 
		[PercentComplete], 
		[Priority], 
		[IsCancelled], 
		[TimeStarted], 
		[PercentCompleteWhenStarted], 
		[EstimatedFinishTime],
		[TimeCompleted]
    FROM [Simulations].[Simulation]
		WHERE 
		[IsFinished] != 1
		AND [IsCancelled] != 1
		AND [Id] NOT IN (SELECT [Id] FROM @CurrentlyRunningSimulations)
		ORDER BY [Priority] DESC, [Id] ASC
		) 
	UPDATE T SET [IsRunning] = 1, [TimeStarted] = CURRENT_TIMESTAMP, [PercentCompleteWhenStarted] = [PercentComplete] OUTPUT INSERTED.*
