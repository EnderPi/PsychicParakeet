CREATE TABLE [GeneticRng].[RngSpecimens]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[GeneticSimulationId] INT NOT NULL,
	[Generation] INT NOT NULL,
	[Fitness] INT NULL,
	[Seed] Decimal(20,0),
	[Converged] BIT NULL,
	[NumberOfNodes] INT NOT NULL,
	[Cost] FLOAT NOT NULL,
	[StateOneExpression] VARCHAR(MAX) NULL,
	[StateTwoExpression] VARCHAR(MAX) NULL,
	[OutputExpression] VARCHAR(MAX) NULL,
	[SeedOneExpression] VARCHAR(MAX) NULL,
	[SeedTwoExpression] VARCHAR(MAX) NULL,
	[StateOneExpressionPretty] VARCHAR(MAX) NULL,
	[StateTwoExpressionPretty] VARCHAR(MAX) NULL,
	[OutputExpressionPretty] VARCHAR(MAX) NULL,
	[SeedOneExpressionPretty] VARCHAR(MAX) NULL,
	[SeedTwoExpressionPretty] VARCHAR(MAX) NULL
)
