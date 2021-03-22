CREATE PROCEDURE [GeneticRng].[GetAllSpeciesNames]	
AS
	SELECT [Id], [Name], [Counter]
	FROM [GeneticRng].[SpeciesNames]
