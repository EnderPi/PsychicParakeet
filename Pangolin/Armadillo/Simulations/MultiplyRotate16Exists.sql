CREATE PROCEDURE [Simulations].[MultiplyRotate16Exists]
	@Multiplier int,
	@Rotate smallint
AS
	SELECT [Multiplier] FROM [Simulations].[MultiplyRotate16] WHERE [Multiplier] = @Multiplier AND [Rotate] = @Rotate
