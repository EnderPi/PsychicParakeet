CREATE PROCEDURE [Simulations].[CreateRandomnessSimulation]
	@SimulationId INT,
	@NumbersGenerated BIGINT,
	@TargetNumbersGenerated BIGINT,
	@Result INT,
	@RandomNumberEngine VARCHAR(200),
	@Description VARCHAR(500)
AS
	INSERT INTO [Simulations].[RandomnessSimulation]
	([SimulationId], [NumbersGenerated], [TargetNumbersGenerated], [Result], [RandomNumberEngine], [Description])
	VALUES
	(@SimulationId, @NumbersGenerated, @TargetNumbersGenerated, @Result, @RandomNumberEngine, @Description)
RETURN 0
