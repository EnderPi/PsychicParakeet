CREATE PROCEDURE [Simulations].[UpdateRomulLevel14]
	@Id int,
	@Fitness bigint
AS
	UPDATE [Simulations].[MultiplyRotate64] SET [LevelFourteenFitness] = @Fitness WHERE [Id] = @Id

