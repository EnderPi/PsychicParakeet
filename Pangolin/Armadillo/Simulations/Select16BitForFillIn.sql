CREATE PROCEDURE [Simulations].[Select16BitForFillIn]	
AS
	SELECT 
	[Id] ,	
	[Multiplier] ,	
	[Rotate] ,	[Seed], [LevelOneFitness],[LevelTwoFitness],[LevelThreeFitness],[LevelTwelveFitness],[LevelThirteenFitness],
	[LevelFourteenFitness] 
	FROM [Simulations].[MultiplyRotate16]
	WHERE [Gorilla8Fitness] IS NULL
