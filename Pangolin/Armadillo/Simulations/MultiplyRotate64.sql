CREATE TABLE [Simulations].[MultiplyRotate64]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Multiplier] Decimal(20,0) NOT NULL,
	[Rotate] SMALLINT NOT NULL,
	[Seed] Decimal(20,0) NOT NULL,
	[LevelOneFitness] Int NOT NULL,
	[LevelTwoFitness] Int NOT NULL,
	[LevelThreeFitness] Int NOT NULL,
	[LevelTwelveFitness] BIGINT NULL,
	[LevelThirteenFitness] BIGINT NULL,
	[LevelFourteenFitness] BIGINT NULL
)
