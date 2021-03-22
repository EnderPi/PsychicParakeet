CREATE PROCEDURE [GeneticRng].[GetNameCount]	
AS
	SELECT COUNT(Id) FROM [GeneticRng].[SpeciesNames]

