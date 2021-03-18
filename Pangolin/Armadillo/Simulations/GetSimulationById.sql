CREATE PROCEDURE [Simulations].[GetSimulationById]
	@Id int
AS
	SELECT TOP 1 [Id], 
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
		WHERE [Id] = @Id

