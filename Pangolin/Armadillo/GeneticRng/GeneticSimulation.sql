﻿CREATE TABLE [GeneticRng].[GeneticSimulation]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[TestLevel] INT NOT NULL,
    [ModeStateOne] INT NOT NULL,
    [ModeStateTwo] INT NOT NULL,
    [CostMode] INT NOT NULL,
    [UseStateTwo] BIT NOT NULL,
    [AllowAdditionNodes] BIT NOT NULL,
    [AllowSubtractionNodes] BIT NOT NULL,
    [AllowMultiplicationNodes] BIT NOT NULL,
    [AllowDivisionNodes] BIT NOT NULL,
    [AllowRemainderNodes] BIT NOT NULL,
    [AllowRightShiftNodes] BIT NOT NULL,
    [AllowLeftShiftNodes] BIT NOT NULL,
    [AllowRotateLeftNodes] BIT NOT NULL,
    [AllowRotateRightNodes] BIT NOT NULL,
    [AllowAndNodes] BIT NOT NULL,
    [AllowOrNodes] BIT NOT NULL,
    [AllowNotNodes] BIT NOT NULL,
    [AllowXorNodes] BIT NOT NULL,
    [Iterations] INT NULL,
    [TimeCreated] DATETIME NULL
)
