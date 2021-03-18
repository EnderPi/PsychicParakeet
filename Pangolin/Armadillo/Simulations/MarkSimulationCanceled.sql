CREATE PROCEDURE [Simulations].[MarkSimulationCanceled]
	@Id int
AS
	UPDATE [Simulations].[Simulation] SET IsCancelled = 1 WHERE [Id] = @Id

