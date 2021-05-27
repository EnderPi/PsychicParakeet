CREATE TABLE [GeneticRng].[LinearRngSpecimens]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[GeneticSimulationId] INT NOT NULL,
	[Generation] INT NOT NULL,
	[Fitness] INT NULL,
	[Seed] Decimal(20,0),
	[Converged] BIT NULL,
	[NumberOfLines] INT NOT NULL,
	[SeedProgram] VARCHAR(MAX) NULL,
	[GenerationProgram] VARCHAR(MAX) NULL,
)

GO

CREATE INDEX [IX_LinearRngSpecimens_Converged] ON [GeneticRng].[LinearRngSpecimens] ([GeneticSimulationId] ASC) WHERE ([Converged]=(1))
