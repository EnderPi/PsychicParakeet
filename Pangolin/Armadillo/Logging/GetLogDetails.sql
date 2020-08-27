CREATE PROCEDURE [Logging].[GetLogDetails]
	@Id BIGINT	
AS
	SELECT [Key], [Value] FROM [Logging].[LogDetail] WHERE [Id] = @Id

