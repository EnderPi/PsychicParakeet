/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
DECLARE @VersionGuid UNIQUEIDENTIFIER

SET @VersionGuid = 'E91B892D-A1D0-4D2A-80B3-2F61990D11DB'
IF NOT EXISTS (SELECT 1 FROM [Configuration].[DatabaseScriptHistory] WHERE [Id] = @VersionGuid)
BEGIN

    INSERT INTO [Configuration].[DatabaseScriptHistory] ([Id], [Description], [TimeStamp])
        VALUES (@VersionGuid, 'Junk Logging Data', CURRENT_TIMESTAMP)
END

SET @VersionGuid = 'CFACE5B8-7B52-485B-BEC9-E5B714736EC2'
IF NOT EXISTS (SELECT 1 FROM [Configuration].[DatabaseScriptHistory] WHERE [Id] = @VersionGuid)
BEGIN
    INSERT INTO [Configuration].[ApplicationSettings] ([Application], [Name], [Value]) VALUES ('MessageQueue','MaximumQueueCount','128')
    INSERT INTO [Configuration].[DatabaseScriptHistory] ([Id], [Description], [TimeStamp])
        VALUES (@VersionGuid, 'Default Message Queue Configuration', CURRENT_TIMESTAMP)
END

