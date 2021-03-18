CREATE TABLE [Simulations].[GcdTest]
(
	[SimulationId] INT NOT NULL PRIMARY KEY,
	[NumberOfGcds] BIGINT NULL,
	[GcdCap] INT NULL,
	[PValueGcd] FLOAT NULL,
	[TestResultGcd] INT NULL,
	[PValueSteps] FLOAT NULL,
	[TestResultSteps] INT NULL,
	[DetailedResult] VARCHAR(MAX) NULL
)
