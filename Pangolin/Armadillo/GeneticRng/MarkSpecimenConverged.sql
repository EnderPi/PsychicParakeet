CREATE PROCEDURE [GeneticRng].[MarkSpecimenConverged]
	@Id INT
AS
	UPDATE [GeneticRng].[RngSpecimens]
	SET [Converged] = 1
	WHERE [Id] = @Id
RETURN 0
