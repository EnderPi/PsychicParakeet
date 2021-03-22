CREATE TABLE [GeneticRng].[SpeciesNames]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] VARCHAR(100) NOT NULL,
	[Counter] INT NOT NULL DEFAULT(0)
)

GO

CREATE UNIQUE INDEX [IX_SpeciesNames_Name] ON [GeneticRng].[SpeciesNames] ([Name])
