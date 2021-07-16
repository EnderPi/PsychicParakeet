CREATE TABLE [GeneticRng].[FeistelRngSpecimens]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[GeneticSimulationId] INT NOT NULL,
	[Generation] INT NOT NULL,
	[Fitness] INT NULL,
	[Seed] Decimal(20,0),
	[Converged] BIT NULL,
	[NumberOfNodes] INT NOT NULL,
	[Cost] FLOAT NOT NULL,
	[Rounds] INT NOT NULL,
	[Avalanche] FLOAT NOT NULL,
	[AvalancheRange] FLOAT NOT NULL,
	[OutputExpression] VARCHAR(MAX) NULL,
	[OutputExpressionPretty] VARCHAR(MAX) NULL,
	
)
