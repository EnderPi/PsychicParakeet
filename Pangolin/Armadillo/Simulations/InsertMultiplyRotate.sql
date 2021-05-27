﻿CREATE PROCEDURE [Simulations].[InsertMultiplyRotate]
	@Multiplier BIGINT,
	@Rotate SMALLINT,
	@Period BIGINT,
	@LinearityBit0 INT,
	@LinearityBit1 INT,
	@LinearityBit2 INT,
	@LinearityBit3 INT,
	@LinearityBit4 INT,
	@LinearityBit5 INT,
	@LinearityBit6 INT,
	@LinearityBit7 INT,
	@LinearityBit8 INT,
	@LinearityBit9 INT,
	@LinearityBit10 INT,
	@LinearityBit11 INT,
	@LinearityBit12 INT,
	@LinearityBit13 INT,
	@LinearityBit14 INT,
	@LinearityBit15 INT,
	@LinearityBit16 INT,
	@LinearityBit17 INT,
	@LinearityBit18 INT,
	@LinearityBit19 INT,
	@LinearityBit20 INT,
	@LinearityBit21 INT,
	@LinearityBit22 INT,
	@LinearityBit23 INT,
	@LinearityBit24 INT,
	@LinearityBit25 INT,
	@LinearityBit26 INT,
	@LinearityBit27 INT,
	@LinearityBit28 INT,
	@LinearityBit29 INT,
	@LinearityBit30 INT,
	@LinearityBit31 INT
AS
	INSERT INTO [Simulations].[MultiplyRotateLeft32]
	([Multiplier],
	[Rotate],
	[Period],
	[LinearityBit0],
	[LinearityBit1],
	[LinearityBit2],
	[LinearityBit3],
	[LinearityBit4],
	[LinearityBit5],
	[LinearityBit6],
	[LinearityBit7],
	[LinearityBit8],
	[LinearityBit9],
	[LinearityBit10],
	[LinearityBit11],
	[LinearityBit12],
	[LinearityBit13],
	[LinearityBit14],
	[LinearityBit15],
	[LinearityBit16],
	[LinearityBit17],
	[LinearityBit18],
	[LinearityBit19],
	[LinearityBit20],
	[LinearityBit21],
	[LinearityBit22],
	[LinearityBit23],
	[LinearityBit24],
	[LinearityBit25],
	[LinearityBit26],
	[LinearityBit27],
	[LinearityBit28],
	[LinearityBit29],
	[LinearityBit30],
	[LinearityBit31])
	VALUES
	(@Multiplier,
	@Rotate,
	@Period,
	@LinearityBit0,
	@LinearityBit1,
	@LinearityBit2,
	@LinearityBit3,
	@LinearityBit4,
	@LinearityBit5,
	@LinearityBit6,
	@LinearityBit7,
	@LinearityBit8,
	@LinearityBit9,
	@LinearityBit10,
	@LinearityBit11,
	@LinearityBit12,
	@LinearityBit13,
	@LinearityBit14,
	@LinearityBit15,
	@LinearityBit16,
	@LinearityBit17,
	@LinearityBit18,
	@LinearityBit19,
	@LinearityBit20,
	@LinearityBit21,
	@LinearityBit22,
	@LinearityBit23,
	@LinearityBit24,
	@LinearityBit25,
	@LinearityBit26,
	@LinearityBit27,
	@LinearityBit28,
	@LinearityBit29,
	@LinearityBit30,
	@LinearityBit31)
RETURN 0
