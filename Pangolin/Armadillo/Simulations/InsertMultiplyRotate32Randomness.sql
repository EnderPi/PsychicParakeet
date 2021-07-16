CREATE PROCEDURE [Simulations].[InsertMultiplyRotate32Randomness]
	@Multiplier BIGINT,
	@Rotate smallint,
	@Seed DECIMAL(20,0),
	@LevelOneFitness INT,
	@LevelTwoFitness INT,
	@LevelThreeFitness INT,
	@LevelTwelveFitness BIGINT,
	@LevelThirteenFitness BIGINT
AS
	INSERT INTO [Simulations].[MultiplyRotate32Randomness]
	([Multiplier], [Rotate], [Seed], [LevelOneFitness], [LevelTwoFitness], [LevelThreeFitness], [LevelTwelveFitness], [LevelThirteenFitness])
	VALUES
	(@Multiplier, @Rotate, @Seed, @LevelOneFitness, @LevelTwoFitness, @LevelThreeFitness, @LevelTwelveFitness, @LevelThirteenFitness)
RETURN 0
