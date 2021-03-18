CREATE TABLE [Simulations].[BirthdayTestDetail]
(
	[SimulationId] INT NOT NULL,
	[NumberOfDuplicates] INT NOT NULL,
	[ExpectedNumberOfDuplicates] FLOAT NOT NULL,
	[ActualNumberOfDuplicates] INT NOT NULL,
	PRIMARY KEY ([SimulationId], [NumberOfDuplicates])
)
