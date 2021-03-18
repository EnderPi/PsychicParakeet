CREATE TABLE [Simulations].[RandomnessSimulation]
(
	[SimulationId] INT NOT NULL PRIMARY KEY,
	[NumbersGenerated] BIGINT NOT NULL,
	[TargetNumbersGenerated] BIGINT NOT NULL,
	--this engine probably needs to be an enum, with child tables for those things that need parameterized.
	[RandomNumberEngine] VARCHAR(200) NOT NULL,
	[Result] INT NOT NULL,
	[Description] VARCHAR(500) NULL,
	[TestLevel] INT NOT NULL	
)
