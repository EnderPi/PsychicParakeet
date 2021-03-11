CREATE PROCEDURE [Simulations].[CreateBirthdayTest]
	@SimulationId INT,
	@NumberOfIterations INT,
	@Lambda INT,
	@PValue FLOAT,
	@TestResult INT,
	@DetailedResult VARCHAR(MAX) NULL	
AS
	INSERT INTO 
	[Simulations].[BirthdayTest]
	([SimulationId], [NumberOfIterations], [Lambda], [PValue], [TestResult], [DetailedResult])
	VALUES
	(@SimulationId, @NumberOfIterations, @Lambda, @PValue, @TestResult, @DetailedResult)
RETURN 0
