CREATE PROCEDURE [GeneticRng].[CreateSpeciesName]
	@Name VARCHAR(100)
AS
	IF NOT EXISTS (SELECT 1 FROM [GeneticRng].[SpeciesNames] WHERE [Name] = @Name)
	BEGIN
		INSERT INTO [GeneticRng].[SpeciesNames]
		([Name]) VALUES (@Name)
	END
RETURN 0
