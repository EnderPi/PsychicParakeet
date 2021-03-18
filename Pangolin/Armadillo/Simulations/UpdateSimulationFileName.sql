CREATE PROCEDURE [Simulations].[UpdateSimulationFileName]
	@Id INT,
	@FileName VARCHAR(200)
AS
	UPDATE [Simulations].[Simulation] SET SaveFile = @FileName, LastTimeSaved = CURRENT_TIMESTAMP WHERE [Id]=@Id
