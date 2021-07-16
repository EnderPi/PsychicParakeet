CREATE TABLE [Simulations].[MultiplyRotate32Randomness]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Multiplier] Decimal(20,0) NOT NULL,
	[Rotate] SMALLINT NOT NULL,
	[Seed] Decimal(20,0) NOT NULL,
	[LevelOneFitness] Int NOT NULL,
	[LevelTwoFitness] Int NOT NULL,
	[LevelThreeFitness] Int NOT NULL,
	[LevelTwelveFitness] BIGINT NOT NULL,
	[LevelThirteenFitness] BIGINT NULL,
)
