CREATE PROCEDURE [GeneticRng].[CreateFeistelSpecimen]
	@GeneticSimulationId INT,
	@Generation INT,
	@Fitness INT,
	@Seed Decimal(20,0),
	@Converged BIT,
	@NumberOfNodes INT,
	@Cost FLOAT,
	@Rounds INT,
	@Avalanche FLOAT,
	@AvalancheRange FLOAT,
	@OutputExpression VARCHAR(MAX),
	@OutputExpressionPretty VARCHAR(MAX),
	@FeistelKey Feistel32Key READONLY 
AS
	INSERT INTO [GeneticRng].[FeistelRngSpecimens]
	([GeneticSimulationId],[Generation],[Fitness],[Seed],[Converged],[NumberOfNodes],[Cost],[Rounds],[Avalanche],[AvalancheRange],[OutputExpression],
	[OutputExpressionPretty])
	VALUES
	(@GeneticSimulationId,@Generation,@Fitness,@Seed,@Converged,@NumberOfNodes,@Cost,@Rounds,@Avalanche,@AvalancheRange,@OutputExpression,@OutputExpressionPretty)
	DECLARE @SpecimenId INT = CONVERT(INT,SCOPE_IDENTITY())
	INSERT INTO [GeneticRng].[FeistelRngKeys] ([SpecimenId], [Index], [Key])
	SELECT @SpecimenId, [Index], [Key] FROM @FeistelKey
SELECT @SpecimenId