CREATE PROCEDURE [GeneticRng].[MarkFeistelSpecimenConverged]
	@Id INT
AS
	UPDATE [GeneticRng].[FeistelRngSpecimens]
	SET [Converged] = 1
	WHERE [Id] = @Id
RETURN 0
