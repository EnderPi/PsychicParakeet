CREATE PROCEDURE [Simulations].[CreateBirthdayTestDetail]
	@SimulationId INT,
	@NumberOfDuplicates INT,
	@ExpectedNumberOfDuplicates FLOAT,
	@ActualNumberOfDuplicates INT
AS
	INSERT INTO [Simulations].[BirthdayTestDetail]
	([SimulationId], [NumberOfDuplicates], [ExpectedNumberOfDuplicates], [ActualNumberOfDuplicates])
	VALUES
	(@SimulationId, @NumberOfDuplicates, @ExpectedNumberOfDuplicates, @ActualNumberOfDuplicates)
RETURN 0
