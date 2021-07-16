CREATE PROCEDURE [Simulations].[MultiplyRotate32Exists]
	@Multiplier bigint,
	@Rotate smallint
AS
	SELECT [Multiplier] FROM [Simulations].[MultiplyRotateLeft32] WHERE [Multiplier] = @Multiplier AND [Rotate] = @Rotate

