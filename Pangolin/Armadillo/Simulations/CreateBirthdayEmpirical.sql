CREATE PROCEDURE [Simulations].[CreateBirthdayEmpirical]
	@SimulationId INT,
	@Seed DECIMAL(20,0),
	@NumberOfBirthdays INT
AS
	INSERT INTO [Simulations].[BirthdayEmpirical] 
	([SimulationId], [Seed], [NumberOfBirthdays])
	VALUES
	(@SimulationId, @Seed, @NumberOfBirthdays)
