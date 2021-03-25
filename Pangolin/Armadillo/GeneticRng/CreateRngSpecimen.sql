CREATE PROCEDURE [GeneticRng].[CreateRngSpecimen]
	@GeneticSimulationId INT,
	@Generation INT,
	@Fitness INT,
	@Seed Decimal(20,0),
	@Converged BIT,
	@NumberOfNodes INT,
	@Cost FLOAT,
	@StateOneExpression VARCHAR(MAX),
	@StateTwoExpression VARCHAR(MAX),
	@OutputExpression VARCHAR(MAX),
	@SeedOneExpression VARCHAR(MAX),
	@SeedTwoExpression VARCHAR(MAX),
	@StateOneExpressionPretty VARCHAR(MAX),
	@StateTwoExpressionPretty VARCHAR(MAX),
	@OutputExpressionPretty VARCHAR(MAX),
	@SeedOneExpressionPretty VARCHAR(MAX),
	@SeedTwoExpressionPretty VARCHAR(MAX)
AS
	INSERT INTO [GeneticRng].[RngSpecimens]
	([GeneticSimulationId],[Generation],[Fitness],[Seed],[Converged],[NumberOfNodes],[Cost],[StateOneExpression],
	[StateTwoExpression],[OutputExpression],[SeedOneExpression],[SeedTwoExpression],[StateOneExpressionPretty],
	[StateTwoExpressionPretty],[OutputExpressionPretty],[SeedOneExpressionPretty],[SeedTwoExpressionPretty])
	VALUES
	(@GeneticSimulationId,@Generation,@Fitness,@Seed,@Converged,@NumberOfNodes,@Cost,@StateOneExpression,
	@StateTwoExpression,@OutputExpression,@SeedOneExpression,@SeedTwoExpression,@StateOneExpressionPretty,
	@StateTwoExpressionPretty,@OutputExpressionPretty,@SeedOneExpressionPretty,@SeedTwoExpressionPretty)
SELECT CONVERT(INT,SCOPE_IDENTITY())
