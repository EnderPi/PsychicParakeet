CREATE PROCEDURE [dbo].[CreateBirthdayEmpiricalDetail]
	@SimulationId INT,
	@DetailId INT,
	@NumberOfDuplicates INT
AS
	INSERT INTO [Simulations].[BirthdayEmpiricalDetail]
	([SimulationId], [DetailId], [NumberOfDuplicates])
	VALUES
	(@SimulationId, @DetailId, @NumberOfDuplicates)
