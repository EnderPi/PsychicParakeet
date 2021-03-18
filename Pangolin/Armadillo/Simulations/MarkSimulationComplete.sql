CREATE PROCEDURE [Simulations].[MarkSimulationComplete]
	@Id int
AS
	UPDATE [Simulations].[Simulation]
	SET IsFinished = 1, IsRunning = 0, SaveFile = NULL, [TimeCompleted] = CURRENT_TIMESTAMP WHERE Id = @Id

