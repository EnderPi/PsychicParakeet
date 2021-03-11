CREATE TABLE [Simulations].[Simulation]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [SaveFile] VARCHAR(200) NULL, 
    [Description] VARCHAR(200) NULL, 
    [LastTimeSaved] DATETIME NULL, 
    [SimulationObject] VARCHAR(MAX) NULL, 
    [IsRunning] BIT NOT NULL, 
    [IsFinished] BIT NOT NULL, 
    [PercentComplete] FLOAT NOT NULL, 
    [Priority] INT NOT NULL DEFAULT 1, 
    [IsCancelled] BIT NOT NULL, 
    [TimeStarted] DATETIME NULL, 
    [PercentCompleteWhenStarted] FLOAT NOT NULL, 
    [EstimatedFinishTime] DATETIME NULL, 
    [TimeCompleted] DATETIME NULL
)
