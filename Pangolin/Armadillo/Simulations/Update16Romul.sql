CREATE PROCEDURE [Simulations].[Update16Romul]
	@Id Int,
    @Gorilla8Fitness Int,
    @Gorilla16Fitness Int,
    @GcdFitness Int,
    @BirthdayFitness Int,
    @Maurer16Fitness Int,
    @Maurer8Fitness Int
AS
	UPDATE [Simulations].[MultiplyRotate16] SET 
    [Gorilla8Fitness] = @Gorilla8Fitness,
    [Gorilla16Fitness] = @Gorilla16Fitness,
    [GcdFitness] = @GcdFitness,
    [BirthdayFitness] = @BirthdayFitness,
    [Maurer16Fitness] = @Maurer16Fitness,
    [Maurer8Fitness] = @Maurer8Fitness
    WHERE [Id] = @Id
RETURN 0
