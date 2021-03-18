CREATE PROCEDURE [Simulations].[CreateGcdTestChiSquared]
	@SimulationId INT,
	@Gcd INT,
	@Count BIGINT,
	@ExpectedCount FLOAT,
	@FractionOfChiSquared FLOAT
AS
	INSERT INTO [Simulations].[GcdTestChiSquared]
	([SimulationId], [Gcd], [Count], [ExpectedCount], [FractionOfChiSquared])
	VALUES
	(@SimulationId, @Gcd, @Count, @ExpectedCount, @FractionOfChiSquared)

