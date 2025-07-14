USE [UnityControlv8]
GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [Recipe_Unique_GuidKey_Step_ActorType_Name_Version]    Script Date: 05/09/2023 17:36:29 ******/
ALTER TABLE [dbo].[Recipe] ADD  CONSTRAINT [Recipe_Unique_GuidKey_Step_ActorType_Name_Version] UNIQUE NONCLUSTERED 
(
	[KeyForAllVersion] ASC,
	[StepId] ASC,
	[ActorType] ASC,
	[Name] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [Recipe_Unique_Step_ActorType_Name_Version]    Script Date: 05/09/2023 17:35:42 ******/
ALTER TABLE [dbo].[Recipe] DROP CONSTRAINT [Recipe_Unique_Step_ActorType_Name_Version]
GO



