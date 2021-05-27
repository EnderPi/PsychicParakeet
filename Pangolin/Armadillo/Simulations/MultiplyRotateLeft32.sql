CREATE TABLE [Simulations].[MultiplyRotateLeft32]
(
	[Multiplier] BIGINT NOT NULL,
	[Rotate] SMALLINT NOT NULL,
	[Period] BIGINT NOT NULL,
	[LinearityBit0] INT NOT NULL,
	[LinearityBit1] INT NOT NULL,
	[LinearityBit2] INT NOT NULL,
	[LinearityBit3] INT NOT NULL,
	[LinearityBit4] INT NOT NULL,
	[LinearityBit5] INT NOT NULL,
	[LinearityBit6] INT NOT NULL,
	[LinearityBit7] INT NOT NULL,
	[LinearityBit8] INT NOT NULL,
	[LinearityBit9] INT NOT NULL,
	[LinearityBit10] INT NOT NULL,
	[LinearityBit11] INT NOT NULL,
	[LinearityBit12] INT NOT NULL,
	[LinearityBit13] INT NOT NULL,
	[LinearityBit14] INT NOT NULL,
	[LinearityBit15] INT NOT NULL,
	[LinearityBit16] INT NOT NULL,
	[LinearityBit17] INT NOT NULL,
	[LinearityBit18] INT NOT NULL,
	[LinearityBit19] INT NOT NULL,
	[LinearityBit20] INT NOT NULL,
	[LinearityBit21] INT NOT NULL,
	[LinearityBit22] INT NOT NULL,
	[LinearityBit23] INT NOT NULL,
	[LinearityBit24] INT NOT NULL,
	[LinearityBit25] INT NOT NULL,
	[LinearityBit26] INT NOT NULL,
	[LinearityBit27] INT NOT NULL,
	[LinearityBit28] INT NOT NULL,
	[LinearityBit29] INT NOT NULL,
	[LinearityBit30] INT NOT NULL,
	[LinearityBit31] INT NOT NULL,
	CONSTRAINT PK_MultiplyRotate32 PRIMARY KEY ([Multiplier], [Rotate])
)

GO

CREATE INDEX [IX_MultiplyRotateLeft32_Period] ON [Simulations].[MultiplyRotateLeft32] ([Period] DESC)
