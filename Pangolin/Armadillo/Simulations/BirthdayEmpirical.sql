CREATE TABLE [Simulations].[BirthdayEmpirical]
(
	[SimulationId] INT NOT NULL,	
	[Seed] DECIMAL(20,0) NOT NULL,
	[NumberOfBirthdays] INT NOT NULL,	
    PRIMARY KEY ([SimulationId])
)
