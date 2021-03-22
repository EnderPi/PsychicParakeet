CREATE PROCEDURE [GeneticRng].[GetSpeciesName]
	@Id INT	
AS
	UPDATE [GeneticRng].[SpeciesNames] 
	SET [Counter] = [Counter] + 1 
	OUTPUT INSERTED.[Id], INSERTED.[Name], INSERTED.[Counter]
	WHERE [Id] = @Id

