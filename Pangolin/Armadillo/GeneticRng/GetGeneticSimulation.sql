CREATE PROCEDURE [GeneticRng].[GetGeneticSimulation]
	@Id int	
AS
	SELECT [Id],[TestLevel],[ModeStateOne],[ModeStateTwo],[CostMode],[UseStateTwo],[AllowAdditionNodes],[AllowSubtractionNodes],[AllowMultiplicationNodes],
    [AllowDivisionNodes],[AllowRemainderNodes],[AllowRightShiftNodes],[AllowLeftShiftNodes],[AllowRotateLeftNodes],[AllowRotateRightNodes],
    [AllowAndNodes],[AllowOrNodes],[AllowNotNodes],[AllowXorNodes]
	FROM [GeneticRng].[GeneticSimulation]
	WHERE [Id] = @Id
RETURN 0
