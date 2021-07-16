CREATE PROCEDURE [Simulations].[InsertMultiplyRotate64]
	@Multiplier DECIMAL(20,0),
	@Rotate smallint,
	@Seed DECIMAL(20,0),
	@LevelOneFitness INT,
	@LevelTwoFitness INT,
	@LevelThreeFitness INT,
	@LevelTwelveFitness BIGINT,
	@LevelThirteenFitness BIGINT
AS
	INSERT INTO [Simulations].[MultiplyRotate64]
	([Multiplier], [Rotate], [Seed], [LevelOneFitness], [LevelTwoFitness], [LevelThreeFitness], [LevelTwelveFitness], [LevelThirteenFitness])
	VALUES
	(@Multiplier, @Rotate, @Seed, @LevelOneFitness, @LevelTwoFitness, @LevelThreeFitness, @LevelTwelveFitness, @LevelThirteenFitness)
RETURN 0
