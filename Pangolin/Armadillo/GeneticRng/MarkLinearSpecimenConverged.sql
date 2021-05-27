CREATE PROCEDURE [GeneticRng].[MarkLinearSpecimenConverged]
	@Id INT
AS
	UPDATE [GeneticRng].[LinearRngSpecimens]
	SET [Converged] = 1
	WHERE [Id] = @Id
RETURN 0
