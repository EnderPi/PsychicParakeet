CREATE TABLE [Simulations].[MultiplyRotate16]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Multiplier] INT NOT NULL,
	[Rotate] SMALLINT NOT NULL,
	[Seed] Decimal(20,0) NOT NULL,
	[LevelOneFitness] Int NOT NULL,
	[LevelTwoFitness] Int NOT NULL,
	[LevelThreeFitness] Int NOT NULL,
	[LevelTwelveFitness] BIGINT NULL,
	[LevelThirteenFitness] BIGINT NULL,
	[LevelFourteenFitness] BIGINT NULL,
	[Gorilla8Fitness] INT NULL,
	[Gorilla16Fitness] INT NULL,
	[GcdFitness] INT NULL,
	[BirthdayFitness] INT NULL,
	[Maurer8Fitness] INT NULL,
	[Maurer16Fitness] INT NULL
)
