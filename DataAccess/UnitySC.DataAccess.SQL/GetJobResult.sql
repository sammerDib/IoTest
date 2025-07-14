USE [UnityControlv7]
GO

/****** Object:  StoredProcedure [dbo].[sp_GetJobResults]    Script Date: 08/01/2021 12:05:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		YSI / RTI
-- Create date: 21/06/2019
-- Update date : 05/07/2022
-- Description:	retourtne une liste des jobs 
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetJobResults] 
				@pToolId int = null,
				@pStartDate dateTime2 = null,
				@pEndDate dateTime2 = null,		
				@pProductId int = null ,
				@pLotName nvarchar(max) = null,
				@pRecipeName nvarchar(max) = null, 
				@pActorType int = null,			
				@pResultState int = null,
				@pWaferName nvarchar(max) = null,
				@pTag int = null	
				
AS
BEGIN
	--Déclaration des variables locales 
	DECLARE @vSqlCommand nvarchar (max);
	DECLARE @vlCondition nvarchar (max) = '';
	DECLARE @vDate nvarchar (max) = '';

	-- L'id du tool 		
	If(@pToolId Is Not Null )
	Begin
	SET @vlCondition = CONCAT('J.ToolId = ', @pToolId);
	End
	
	--Recherche par date
	If((@pStartDate Is Not Null) OR (@pEndDate Is Not Null) )
	BEGIN
		-- Cas le ToolId a été renseigné, on ajoute un AND à la condition 
		If(@vlCondition != '')
		Begin
			SET @vlCondition = CONCAT(@vlCondition, ' AND ')
		End 

		If(@pStartDate Is Not Null and @pEndDate Is Not Null) 
		Begin 
			SET @vDate = CONCAT( '''' , @pStartDate, '''', ' AND ', '''',  @pEndDate, '''')
			SET @vlCondition = CONCAT(@vlCondition, ' J.[Date] BETWEEN ', @vDate) 
		End
		Else if(@pStartDate Is Not Null)
		Begin 
			--SET @vDate = ''''+  @pStartDate + ''''
			SET @vlCondition = CONCAT(@vlCondition, ' J.[Date] >= ', '''', @pStartDate, '''') 
		End 
		Else if (@pEndDate Is Not Null)
		Begin
			SET @vlCondition = CONCAT(@vlCondition, ' J.[Date] <= ', '''', @pEndDate, '''') 
		End 
	END

	-- L'id du produit (WaferType)
	if(@pProductId Is Not Null)
	BEGIN
		If(@vlCondition != '')
		Begin
			SET @vlCondition = CONCAT(@vlCondition, ' AND ')
		End 
		SET @vlCondition = CONCAT(@vlCondition, ' W.ProductId = ', @pProductId) 
	END 
	-- Job Id 
	if(@pLotName Is Not NulL)
	BEGIN
		If(@vlCondition != '')
		Begin
			SET @vlCondition = CONCAT(@vlCondition, ' AND ')
		End 	
		SET @vlCondition = CONCAT(@vlCondition,' J.LotName LIKE ' ) + '''' + @pLotName + ''''
	END 
	-- Recipe Id  : TO DO -> recherche par id ou nom de la rectte 
	if(@pRecipeName IS NOT NULL)
	BEGIN
		If(@vlCondition != '')
		Begin
			SET @vlCondition = CONCAT(@vlCondition, ' AND ')
		End 
		SET @vlCondition = CONCAT(@vlCondition,' J.RecipeName LIKE ') +  '''' + @pRecipeName + ''''
	END 
	-- Chamber Id 
	if(@pActorType IS NOT NULL)
	BEGIN
		If(@vlCondition != '')
		Begin
			SET @vlCondition = CONCAT(@vlCondition, ' AND ')
		End 
		SET @vlCondition = CONCAT(@vlCondition, ' ( R.ActorType =',  @pActorType )
		SET @vlCondition = CONCAT(@vlCondition, ' OR Ra.ActorType =',  @pActorType ) + ' ) '
	END 
	-- Wafer Id
	if(@pWaferName Is Not Null)
	Begin
		If(@vlCondition != '')
		Begin
			SET @vlCondition = CONCAT(@vlCondition, ' AND ')
		End 
		SET @vlCondition = CONCAT(@vlCondition ,' W.WaferName = ') + ''''+  @pWaferName + ''''
	End
	-- Wafer result state 
	if(@pResultState Is Not Null)
	Begin
		If(@vlCondition != '')
		Begin
			SET @vlCondition = CONCAT(@vlCondition, ' AND ')
		End 
		SET @vlCondition = CONCAT(@vlCondition, ' ( R.Id in (Select ResultId from ResultItem where State =',  @pResultState ) + ') OR '
		SET @vlCondition = CONCAT(@vlCondition, ' Ra.Id in (Select ResultAcqId from ResultAcqItem where State =',  @pResultState ) + ') )'
	End
	-- Tag  
	if(@pTag IS NOT NULL)
	BEGIN
		If(@vlCondition != '')
		Begin
			SET @vlCondition = CONCAT(@vlCondition, ' AND ')
		End 
		if(@pTag != 0)
		Begin
			SET @vlCondition = CONCAT(@vlCondition, ' ( R.Id in (Select ResultId from ResultItem where Tag =',  @pTag ) + ') OR '
			SET @vlCondition = CONCAT(@vlCondition, ' Ra.Id in (Select ResultAcqId from ResultAcqItem where Tag =',  @pTag ) + ') )'
		End
		ELSE
		Begin
			SET @vlCondition = CONCAT(@vlCondition, ' ( R.Id in (Select ResultId from ResultItem where (Tag is NULL OR Tag =',  @pTag ) + ')) OR '
			SET @vlCondition = CONCAT(@vlCondition, ' Ra.Id in (Select ResultAcqId from ResultAcqItem where (Tag is NULL OR Tag =',  @pTag ) + ')) )'
		End
	END 
	-- Le corps de la requête
	SET @vSqlCommand = 'SELECT distinct( J.Id), JobName, LotName, J.[Date], RecipeName, RunIter, J.ToolId ' +
					   'FROM Job J '+ 
					   'inner join WaferResult W ON W.JobId = J.Id ' + 
					   'left join Result R on W.Id = R.WaferResultId ' + 
					   'left join ResultAcq Ra on W.Id = Ra.WaferResultId '

	-- Si au moins un paramètre a été renseigné, on ajoute la condition WHERE à la requête 				   
	if(@vlCondition != '')
	BEGIN
	SET @vSqlCommand = CONCAT(@vSqlCommand,' WHERE ', @vlCondition);
	End 

	SET @vSqlCommand = CONCAT(@vSqlCommand,' ORDER BY ', ' J.[Date] DESC',' ,RunIter DESC' );
	-- Execution de la requête 
	EXEC (@vSqlCommand) ;

	PRINT(@vSqlCommand);
	--PRINT(CONCAT(@vSqlCommand,' WHERE ', @vlCondition));
END
GO


