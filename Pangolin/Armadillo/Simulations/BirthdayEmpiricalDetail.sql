CREATE TABLE [Simulations].[BirthdayEmpiricalDetail]
(
	[SimulationId] INT NOT NULL,
	[DetailId] INT NOT NULL,
	[NumberOfDuplicates] INT NOT NULL, 
    PRIMARY KEY ([SimulationId], [DetailId])
)
