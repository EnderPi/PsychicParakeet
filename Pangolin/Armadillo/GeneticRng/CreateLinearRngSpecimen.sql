CREATE PROCEDURE [GeneticRng].[CreateLinearRngSpecimen]
	@GeneticSimulationId INT,
	@Generation INT,
	@Fitness INT,
	@Seed Decimal(20,0),
	@Converged BIT,
	@NumberOfLines INT,
	@SeedProgram VARCHAR(MAX),
	@GenerationProgram VARCHAR(MAX)	
AS
	INSERT INTO [GeneticRng].[LinearRngSpecimens]
	([GeneticSimulationId],[Generation],[Fitness],[Seed],[Converged],[NumberOfLines],[SeedProgram],[GenerationProgram])
	VALUES
	(@GeneticSimulationId,@Generation,@Fitness,@Seed,@Converged,@NumberOfLines,@SeedProgram,@GenerationProgram)
SELECT CONVERT(INT,SCOPE_IDENTITY())
