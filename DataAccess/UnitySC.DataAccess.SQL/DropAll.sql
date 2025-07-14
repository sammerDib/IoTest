USE [UnityControlv7Test]
GO

/****** Object:  StoredProcedure [dbo].[DeleteAllData]    Script Date: 08/01/2021 12:04:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Foucauld
-- Create date: 05/07/2022
-- Description:	Remove all data in database
-- =============================================
CREATE PROCEDURE [dbo].[DeleteAllData]

AS

BEGIN

EXEC sp_MSForEachTable 'DISABLE TRIGGER ALL ON ?'
EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'
EXEC sp_MSForEachTable 'SET QUOTED_IDENTIFIER ON; DELETE FROM ?'
EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'
EXEC sp_MSForEachTable 'ENABLE TRIGGER ALL ON ?'

declare @DbVersion varchar(50)
declare @DbDateVersion varchar(50)
declare @InsertVersionQuery nvarchar(500)

set @DbVersion =CONCAT('''', N'7.0.0','''')
set @DbDateVersion =CONCAT('''', N'2022-07-05 14:00:00','''')
set @InsertVersionQuery = 'INSERT INTO [dbo].[DatabaseVersion] (Version, Date) VALUES ( ' + @DbVersion + ' , ' +  @DbDateVersion + ' )'

EXEC sp_executesql @InsertVersionQuery

END

GO


