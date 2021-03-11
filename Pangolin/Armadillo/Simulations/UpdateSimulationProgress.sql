CREATE PROCEDURE [Simulations].[UpdateSimulationProgress]
	@Id INT,
	@PercentComplete FLOAT,
	@EstimatedFinishTime DATETIME NULL
AS
	UPDATE [Simulations].[Simulation] SET [PercentComplete] = @PercentComplete, [EstimatedFinishTime] = @EstimatedFinishTime WHERE [Id] = @Id
