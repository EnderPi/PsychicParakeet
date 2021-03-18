CREATE TABLE [Simulations].[GcdTestChiSquared]
(
	[SimulationId] INT NOT NULL,
	[Gcd] int NOT NULL,
	[Count] BIGINT NOT NULL,
	[ExpectedCount] FLOAT NOT NULL,
	[FractionOfChiSquared] FLOAT NOT NULL,
	PRIMARY KEY ([SimulationId], [Gcd])
)
