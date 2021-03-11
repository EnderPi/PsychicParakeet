CREATE PROCEDURE [Simulations].[CreateSimulation]
	@SaveFile VARCHAR(200) NULL,
	@Description VARCHAR(200) NULL, 
    @LastTimeSaved DATETIME NULL, 
    @SimulationObject VARCHAR(MAX), 
    @IsRunning BIT, 
    @IsFinished BIT, 
    @PercentComplete FLOAT, 
    @Priority INT, 
    @IsCancelled BIT,
    @TimeStarted DATETIME NULL,
    @PercentCompleteWhenStarted FLOAT,
    @EstimatedFinishTime DATETIME NULL,
    @TimeCompleted DATETIME NULL
AS
	INSERT INTO [Simulations].[Simulation]
    ([SaveFile], [Description], [LastTimeSaved], [SimulationObject], [IsRunning], [IsFinished], [PercentComplete], [Priority], [IsCancelled], [TimeStarted], [PercentCompleteWhenStarted], [EstimatedFinishTime], [TimeCompleted])
    VALUES
    (@SaveFile, @Description, @LastTimeSaved, @SimulationObject, @IsRunning, @IsFinished, @PercentComplete, @Priority, @IsCancelled, @TimeStarted, @PercentCompleteWhenStarted, @EstimatedFinishTime, @TimeCompleted)

