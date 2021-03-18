CREATE TABLE [Simulations].[GorillaTest]
(
	[SimulationId] INT NOT NULL PRIMARY KEY,
	[WordSize] INT NOT NULL,
	[PValue] FLOAT NOT NULL,
	[TestResult] INT NOT NULL,
	[DetailedResult] VARCHAR(MAX) NOT NULL
)
