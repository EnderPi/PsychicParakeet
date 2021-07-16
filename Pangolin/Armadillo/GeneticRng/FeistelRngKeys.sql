CREATE TABLE [GeneticRng].[FeistelRngKeys]
(
	[SpecimenId] INT NOT NULL,
	[Index] INT NOT NULL,
	[Key] BIGINT NOT NULL,
	CONSTRAINT PK_FeistelRngKeys PRIMARY KEY ([SpecimenId], [Index])
)
