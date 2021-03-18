CREATE TABLE [Simulations].[BirthdayTest]
(
	[SimulationId] INT NOT NULL PRIMARY KEY,
	[NumberOfIterations] INT NOT NULL,
	[Lambda] INT NOT NULL,
	[PValue] FLOAT NOT NULL,
	[TestResult] INT NOT NULL,
	[DetailedResult] VARCHAR(MAX) NOT NULL
)
