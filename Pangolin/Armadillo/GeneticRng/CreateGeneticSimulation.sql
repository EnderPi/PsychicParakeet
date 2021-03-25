CREATE PROCEDURE [GeneticRng].[CreateGeneticSimulation]
	@TestLevel INT,
    @ModeStateOne INT,
    @ModeStateTwo INT,
    @CostMode INT,
    @UseStateTwo BIT,
    @AllowAdditionNodes BIT,
    @AllowSubtractionNodes BIT,
    @AllowMultiplicationNodes BIT,
    @AllowDivisionNodes BIT,
    @AllowRemainderNodes BIT,
    @AllowRightShiftNodes BIT,
    @AllowLeftShiftNodes BIT,
    @AllowRotateLeftNodes BIT,
    @AllowRotateRightNodes BIT,
    @AllowAndNodes BIT,
    @AllowOrNodes BIT,
    @AllowNotNodes BIT,
    @AllowXorNodes BIT	
AS
	INSERT INTO [GeneticRng].[GeneticSimulation]
    ([TestLevel],[ModeStateOne],[ModeStateTwo],[CostMode],[UseStateTwo],[AllowAdditionNodes],[AllowSubtractionNodes],[AllowMultiplicationNodes],
    [AllowDivisionNodes],[AllowRemainderNodes],[AllowRightShiftNodes],[AllowLeftShiftNodes],[AllowRotateLeftNodes],[AllowRotateRightNodes],
    [AllowAndNodes],[AllowOrNodes],[AllowNotNodes],[AllowXorNodes])
    VALUES
    (@TestLevel,@ModeStateOne,@ModeStateTwo,@CostMode,@UseStateTwo,@AllowAdditionNodes,@AllowSubtractionNodes,@AllowMultiplicationNodes,
    @AllowDivisionNodes,@AllowRemainderNodes,@AllowRightShiftNodes,@AllowLeftShiftNodes,@AllowRotateLeftNodes,@AllowRotateRightNodes,
    @AllowAndNodes,@AllowOrNodes,@AllowNotNodes,@AllowXorNodes)
SELECT CONVERT(INT,SCOPE_IDENTITY())
