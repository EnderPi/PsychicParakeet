CREATE PROCEDURE [Simulations].[MultiplyRotate64Exists]
	@Multiplier DECIMAL(20,0),
	@Rotate smallint,
	@Seed DECIMAL(20,0)
AS
	SELECT [Multiplier] FROM [Simulations].[MultiplyRotate64] WHERE [Multiplier] = @Multiplier AND [Rotate] = @Rotate AND [Seed] = @Seed
