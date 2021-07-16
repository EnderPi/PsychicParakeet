CREATE PROCEDURE [Simulations].[MultiplyRotate32RandomnessExists]
	@Multiplier BIGINT,
	@Rotate smallint,
	@Seed DECIMAL(20,0)
AS
	SELECT [Multiplier] FROM [Simulations].[MultiplyRotate32Randomness] WHERE [Multiplier] = @Multiplier AND [Rotate] = @Rotate AND [Seed] = @Seed
