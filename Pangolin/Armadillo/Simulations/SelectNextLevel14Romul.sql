CREATE PROCEDURE [Simulations].[SelectNextLevel14Romul]	
AS
	SELECT 
	[Id] ,	
	[Multiplier] ,	
	[Rotate] ,	[Seed], [LevelOneFitness],[LevelTwoFitness],[LevelThreeFitness],[LevelTwelveFitness],[LevelThirteenFitness],
	[LevelFourteenFitness] 
	FROM [Simulations].[MultiplyRotate64]
	WHERE [LevelFourteenFitness] IS NULL

