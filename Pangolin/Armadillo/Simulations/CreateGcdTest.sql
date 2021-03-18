CREATE PROCEDURE [Simulations].[CreateGcdTest]
	@SimulationId INT,
	@NumberOfGcds BIGINT NULL,
	@PValueGcd FLOAT NULL,
	@TestResultGcd INT NULL,
	@PValueSteps FLOAT NULL,
	@TestResultSteps INT NULL,
	@DetailedResult VARCHAR(MAX) NULL
AS
	INSERT INTO [Simulations].[GcdTest]
	([SimulationId], [NumberOfGcds], [PValueGcd], [TestResultGcd], [PValueSteps], [TestResultSteps], [DetailedResult])
	VALUES
	(@SimulationId, @NumberOfGcds, @PValueGcd, @TestResultGcd, @PValueSteps, @TestResultSteps, @DetailedResult)
