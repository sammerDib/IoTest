USE [master]
GO
/****** create PswdSQL Login if missing ******/
IF NOT EXISTS 
    (SELECT name  
     FROM master.sys.server_principals
     WHERE name = 'PswdSQL')
BEGIN
    CREATE LOGIN [PswdSQL] WITH PASSWORD = N'test',
	CHECK_POLICY = OFF,
	CHECK_EXPIRATION = OFF ;
	EXEC sp_addsrvrolemember  N'PswdSQL', N'sysadmin'
END
GO
/****** create admin Login if missing ******/
IF NOT EXISTS 
    (SELECT name  
     FROM master.sys.server_principals
     WHERE name = 'admin')
BEGIN
    CREATE LOGIN [admin] WITH PASSWORD = N'inspection',
	CHECK_POLICY = OFF,
	CHECK_EXPIRATION = OFF ;
	EXEC sp_addsrvrolemember  N'admin', N'sysadmin'
END
GO

/****** Object:  Database [UnityControlv8]    Script Date: 05/07/2022 16:00:30 ******/
CREATE DATABASE [UnityControlv8]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'UnityControlv8', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\UnityControlv8.mdf' , SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'UnityControlv8_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\UnityControlv8_log.ldf' , SIZE = 3456KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [UnityControlv8] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [UnityControlv8].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [UnityControlv8] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [UnityControlv8] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [UnityControlv8] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [UnityControlv8] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [UnityControlv8] SET ARITHABORT OFF 
GO
ALTER DATABASE [UnityControlv8] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [UnityControlv8] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [UnityControlv8] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [UnityControlv8] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [UnityControlv8] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [UnityControlv8] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [UnityControlv8] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [UnityControlv8] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [UnityControlv8] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [UnityControlv8] SET  DISABLE_BROKER 
GO
ALTER DATABASE [UnityControlv8] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [UnityControlv8] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [UnityControlv8] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [UnityControlv8] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [UnityControlv8] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [UnityControlv8] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [UnityControlv8] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [UnityControlv8] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [UnityControlv8] SET  MULTI_USER 
GO
ALTER DATABASE [UnityControlv8] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [UnityControlv8] SET DB_CHAINING OFF 
GO
ALTER DATABASE [UnityControlv8] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [UnityControlv8] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [UnityControlv8] SET DELAYED_DURABILITY = DISABLED 
GO
USE [UnityControlv8]
GO
/****** Object:  User [PswdSQL]    Script Date: 05/07/2022 16:00:30 ******/
CREATE USER [PswdSQL] FOR LOGIN [PswdSQL] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [admin]    Script Date: 05/07/2022 16:00:30 ******/
CREATE USER [admin] FOR LOGIN [admin] WITH DEFAULT_SCHEMA=[db_accessadmin]
GO
ALTER ROLE [db_owner] ADD MEMBER [PswdSQL]
GO
ALTER ROLE [db_accessadmin] ADD MEMBER [PswdSQL]
GO
ALTER ROLE [db_securityadmin] ADD MEMBER [PswdSQL]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [PswdSQL]
GO
ALTER ROLE [db_datareader] ADD MEMBER [PswdSQL]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [PswdSQL]
GO
ALTER ROLE [db_owner] ADD MEMBER [admin]
GO
ALTER ROLE [db_accessadmin] ADD MEMBER [admin]
GO
ALTER ROLE [db_securityadmin] ADD MEMBER [admin]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [admin]
GO
ALTER ROLE [db_datareader] ADD MEMBER [admin]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [admin]
GO
/****** Object:  Table [dbo].[Chamber]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Chamber](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ToolId] [int] NOT NULL,
	[PhysicalConfigurationId] [int] NULL,
	[IsArchived] [bit] NOT NULL,
	[ActorType] [int] NOT NULL,
	[ChamberKey] [int] NOT NULL,
 CONSTRAINT [PK_Chamber] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConfigurationData]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfigurationData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Contents] [nvarchar](max) NULL,
	[IsExternal] [bit] NULL,
	[Type] [int] NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[Modified] [datetime2](7) NOT NULL,
	[IsArchived] [bit] NULL,
	[CreatorUserId] [int] NOT NULL,
	[MD5] [binary](15) NULL,
	[Version] [int] NOT NULL,
	[ModifiedByUserId] [int] NOT NULL,
 CONSTRAINT [PK_Configuration] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConfigurationHistory]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfigurationHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConfigurationId] [int] NULL,
	[UserId] [int] NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[Contents] [nvarchar](max) NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_ConfigurationHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DatabaseVersion]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DatabaseVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Version] [varchar](50) NULL,
	[Date] [datetime2](7) NULL,
 CONSTRAINT [PK_DatabaseVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GlobalResultSettings]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GlobalResultSettings](
	[Id] [int] NOT NULL,
	[ResultFormat] [int] NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[DataSetting] [nvarchar](max) NOT NULL,
	[XmlSetting] [xml] NULL,
 CONSTRAINT [PK_GlobalResultSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Input]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Input](
	[RecipeId] [int] NOT NULL,
	[ResultType] [int] NOT NULL,
 CONSTRAINT [PK_Input] PRIMARY KEY CLUSTERED 
(
	[RecipeId] ASC,
	[ResultType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Job]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Job](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JobName] [nvarchar](255) NOT NULL,
	[LotName] [nvarchar](255) NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[RecipeName] [nvarchar](255) NOT NULL,
	[RunIter] [int] NOT NULL,
	[ToolId] [int] NOT NULL,
 CONSTRAINT [PK_Job] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Job] UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KlarfBinSettings]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KlarfBinSettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AreaIntervalMax] [bigint] NOT NULL,
	[SquareWidth] [int] NOT NULL,
 CONSTRAINT [PK__KlarfBinSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KlarfRoughSettings]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KlarfRoughSettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoughBin] [int] NOT NULL,
	[Label] [varchar](255) NOT NULL,
	[Color] [int] NOT NULL,
 CONSTRAINT [PK__KlarfRoughSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Layer]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Layer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[MaterialId] [int] NULL,
	[Thickness] [real] NOT NULL,
	[RefractiveIndex] [real] NULL,
	[StepId] [int] NOT NULL,
 CONSTRAINT [PK_Layer_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Log]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ActionType] [int] NULL,
	[Detail] [nvarchar](max) NULL,
	[Date] [datetime2](7) NOT NULL,
	[UserId] [int] NOT NULL,
	[TableType] [int] NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Material]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Material](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[IsArchived] [bit] NOT NULL,
	[XmlContent] [varchar](max) NOT NULL,
 CONSTRAINT [PK_Material] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Output]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Output](
	[RecipeId] [int] NOT NULL,
	[ResultType] [int] NOT NULL,
 CONSTRAINT [PK_Output] PRIMARY KEY CLUSTERED 
(
	[RecipeId] ASC,
	[ResultType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[WaferCategoryId] [int] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[IsArchived] [bit] NOT NULL,
 CONSTRAINT [PK_WaferType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductConfiguration]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductConfiguration](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[ChamberId] [int] NOT NULL,
	[ConfigurationId] [int] NOT NULL,
	[IsArchived] [bit] NOT NULL,
 CONSTRAINT [PK_ProductConfiguration] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Recipe]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Recipe](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[KeyForAllVersion] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[StepId] [int] NULL,
	[Comment] [nvarchar](max) NULL,
	[ActorType] [int] NOT NULL CONSTRAINT [DF_Recipe_RecipeType]  DEFAULT ((9)),
	[Created] [datetime2](7) NOT NULL,
	[CreatorUserId] [int] NOT NULL,
	[CreatorChamberId] [int] NOT NULL,
	[XmlContent] [nvarchar](max) NULL,	
	[Version] [int] NOT NULL,
	[IsArchived] [bit] NOT NULL,
	[IsShared] [bit] NOT NULL,
	[IsTemplate] [bit] NOT NULL,
    [IsValidated] [bit] NOT NULL,
 CONSTRAINT [PK_Recipe] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [Recipe_Unique_Key_Version] UNIQUE NONCLUSTERED 
(
	[KeyForAllVersion] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [Recipe_Unique_GuidKey_Step_ActorType_Name_Version] UNIQUE NONCLUSTERED 
(
    [KeyForAllVersion] ASC,
	[StepId] ASC,
	[ActorType] ASC,
	[Name] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecipeFile]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RecipeFile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](max) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[CreatorUserId] [int] NOT NULL,
	[RecipeID] [int] NULL,
	[Version] [int] NULL,
	[IsArchived] [bit] NOT NULL,
	[MD5] [varchar](max) NULL,
	[FileType] [nvarchar](max) NULL,
 CONSTRAINT [PK_RecipeFile] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RecipeResultType]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecipeResultType](
	[Id] [int] NOT NULL,
 CONSTRAINT [PK_RecipeResultType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RecipeDataflowMap]    Script Date: 05/07/2022 16:00:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RecipeDataflowMap](
	[RecipeKey] [uniqueidentifier] NOT NULL,
	[DataflowKey] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_RecipeDataflowMap] PRIMARY KEY CLUSTERED 
(
	[RecipeKey] ASC,
	[DataflowKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Result]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Result](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[WaferResultId] [bigint] NOT NULL,
	[ChamberId] [int] NOT NULL,
	[RecipeId] [int] NOT NULL,
	[ActorType] [int] NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[State] [int] NOT NULL,
 CONSTRAINT [PK_Result] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [Uq_Result] UNIQUE NONCLUSTERED 
(
	[WaferResultId] ASC,
	[ChamberId] ASC,
	[ActorType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ResultAcq]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResultAcq](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[WaferResultId] [bigint] NOT NULL,
	[ChamberId] [int] NOT NULL,
	[RecipeId] [int] NOT NULL,
	[ActorType] [int] NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[PathName] [nvarchar](max) NOT NULL,
	[State] [int] NOT NULL,
 CONSTRAINT [PK_ResultAcq] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [Uq_ResultAcq] UNIQUE NONCLUSTERED 
(
	[WaferResultId] ASC,
	[ChamberId] ASC,
	[ActorType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ResultAcqItem]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResultAcqItem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ResultAcqId] [bigint] NOT NULL,
	[ResultType] [int] NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Idx] [smallint] NOT NULL,
	[State] [int] NOT NULL,
	[InternalState] [int] NOT NULL,
	[Tag] [int] NULL,
 CONSTRAINT [PK_ResultAcqItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ResultItem]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResultItem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ResultId] [bigint] NOT NULL,
	[ResultType] [int] NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Idx] [smallint] NOT NULL,
	[State] [int] NOT NULL,
	[InternalState] [int] NOT NULL,
	[Tag] [int] NULL,
 CONSTRAINT [PK_ResultItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ResultItemValue]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResultItemValue](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ResultItemId] [bigint] NOT NULL,
	[Type] [int] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Value] [float] NOT NULL,
	[UnitType] [int] NOT NULL,
 CONSTRAINT [PK_ResultValue] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Step]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Step](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ProductId] [int] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[XmlContent] [nvarchar](max) NULL,
	[IsArchived] [bit] NOT NULL,
 CONSTRAINT [PK_Layer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tag]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tag](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RecipeId] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Tag] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tool]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tool](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[ToolKey] [int] NOT NULL,
	[ToolCategory] [nvarchar](max) NOT NULL,
	[IsArchived] [bit] NOT NULL,
 CONSTRAINT [PK_Tool] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Tool_1] UNIQUE NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[User]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[ToolId] [int] NOT NULL,
	[IsArchived] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Vid]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Vid](
	[Id] [int] NOT NULL,
	[Label] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Vid_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Vid_1] UNIQUE NONCLUSTERED 
(
	[Label] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WaferCategory]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WaferCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[XmlContent] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_WaferCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WaferResult]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WaferResult](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[JobId] [int] NOT NULL,
	[SlotId] [int] NOT NULL,
	[WaferName] [nvarchar](max) NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[ProductId] [int] NOT NULL,
 CONSTRAINT [PK_WaferResult] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [Uq_WaferResult] UNIQUE NONCLUSTERED 
(
	[JobId] ASC,
	[SlotId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Dataflow]    Script Date: 05/07/2022 16:00:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Dataflow](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[KeyForAllVersion] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Comment] [nvarchar](max) NULL,
    [Created] [datetime2](7) NULL,
	[CreatorUserId] [int] NOT NULL,
	[XmlContent] [nvarchar](max) NOT NULL,
	[Version] [int] NOT NULL,
	[CreatorTool] [int] NOT NULL,
	[StepId] [int] NOT NULL,
	[IsArchived] [bit] NOT NULL,
	[IsShared] [bit] NOT NULL,
    [IsValidated] [bit] NOT NULL,
 CONSTRAINT [PK_Dataflow] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [Dataflow_Unique_Key_Version] UNIQUE NONCLUSTERED 
(
	[KeyForAllVersion] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [Dataflow_Unique_GuidKey_Step_Name_Version] UNIQUE NONCLUSTERED 
(
	[KeyForAllVersion] ASC,
	[Name] ASC,
	[StepId] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Index [IX_ResultItem]    Script Date: 05/07/2022 16:00:30 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_ResultItem] ON [dbo].[ResultItem]
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Chamber]  WITH NOCHECK ADD  CONSTRAINT [FK_Chamber_Configuration] FOREIGN KEY([PhysicalConfigurationId])
REFERENCES [dbo].[ConfigurationData] ([Id])
GO
ALTER TABLE [dbo].[Chamber] CHECK CONSTRAINT [FK_Chamber_Configuration]
GO
ALTER TABLE [dbo].[Chamber]  WITH NOCHECK ADD  CONSTRAINT [FK_Chamber_Tool] FOREIGN KEY([ToolId])
REFERENCES [dbo].[Tool] ([Id])
GO
ALTER TABLE [dbo].[Chamber] CHECK CONSTRAINT [FK_Chamber_Tool]
GO
ALTER TABLE [dbo].[ConfigurationHistory]  WITH NOCHECK ADD  CONSTRAINT [FK_ConfigurationHistory_Configuration] FOREIGN KEY([ConfigurationId])
REFERENCES [dbo].[ConfigurationData] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ConfigurationHistory] CHECK CONSTRAINT [FK_ConfigurationHistory_Configuration]
GO
ALTER TABLE [dbo].[ConfigurationHistory]  WITH NOCHECK ADD  CONSTRAINT [FK_ConfigurationHistory_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ConfigurationHistory] CHECK CONSTRAINT [FK_ConfigurationHistory_User]
GO
ALTER TABLE [dbo].[Input]  WITH NOCHECK ADD  CONSTRAINT [FK_Input_Recipe] FOREIGN KEY([RecipeId])
REFERENCES [dbo].[Recipe] ([Id])
GO
ALTER TABLE [dbo].[Input] CHECK CONSTRAINT [FK_Input_Recipe]
GO
ALTER TABLE [dbo].[Job]  WITH NOCHECK ADD  CONSTRAINT [FK_Job_Tool] FOREIGN KEY([ToolId])
REFERENCES [dbo].[Tool] ([Id])
GO
ALTER TABLE [dbo].[Job] CHECK CONSTRAINT [FK_Job_Tool]
GO
ALTER TABLE [dbo].[Layer]  WITH NOCHECK ADD  CONSTRAINT [FK_Layer_Material1] FOREIGN KEY([MaterialId])
REFERENCES [dbo].[Material] ([Id])
GO
ALTER TABLE [dbo].[Layer] CHECK CONSTRAINT [FK_Layer_Material1]
GO
ALTER TABLE [dbo].[Layer]  WITH NOCHECK ADD  CONSTRAINT [FK_Layer_Step] FOREIGN KEY([StepId])
REFERENCES [dbo].[Step] ([Id])
GO
ALTER TABLE [dbo].[Layer] CHECK CONSTRAINT [FK_Layer_Step]
GO
ALTER TABLE [dbo].[Log]  WITH NOCHECK ADD  CONSTRAINT [FK_Log_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Log] CHECK CONSTRAINT [FK_Log_User]
GO
ALTER TABLE [dbo].[Output]  WITH NOCHECK ADD  CONSTRAINT [FK_Output_Recipe] FOREIGN KEY([RecipeId])
REFERENCES [dbo].[Recipe] ([Id])
GO
ALTER TABLE [dbo].[Output] CHECK CONSTRAINT [FK_Output_Recipe]
GO
ALTER TABLE [dbo].[Product]  WITH NOCHECK ADD  CONSTRAINT [FK_Product_WaferCategory] FOREIGN KEY([WaferCategoryId])
REFERENCES [dbo].[WaferCategory] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_WaferCategory]
GO
ALTER TABLE [dbo].[ProductConfiguration]  WITH NOCHECK ADD  CONSTRAINT [FK_ProductConfiguration_Chamber] FOREIGN KEY([ChamberId])
REFERENCES [dbo].[Chamber] ([Id])
GO
ALTER TABLE [dbo].[ProductConfiguration] CHECK CONSTRAINT [FK_ProductConfiguration_Chamber]
GO
ALTER TABLE [dbo].[ProductConfiguration]  WITH NOCHECK ADD  CONSTRAINT [FK_ProductConfiguration_Configuration] FOREIGN KEY([ConfigurationId])
REFERENCES [dbo].[ConfigurationData] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductConfiguration] CHECK CONSTRAINT [FK_ProductConfiguration_Configuration]
GO
ALTER TABLE [dbo].[ProductConfiguration]  WITH NOCHECK ADD  CONSTRAINT [FK_ProductConfiguration_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductConfiguration] CHECK CONSTRAINT [FK_ProductConfiguration_Product]
GO
ALTER TABLE [dbo].[Recipe]  WITH NOCHECK ADD  CONSTRAINT [FK_Recipe_Chamber] FOREIGN KEY([CreatorChamberId])
REFERENCES [dbo].[Chamber] ([Id])
GO
ALTER TABLE [dbo].[Recipe] CHECK CONSTRAINT [FK_Recipe_Chamber]
GO
ALTER TABLE [dbo].[Recipe]  WITH NOCHECK ADD  CONSTRAINT [FK_Recipe_Step] FOREIGN KEY([StepId])
REFERENCES [dbo].[Step] ([Id])
GO
ALTER TABLE [dbo].[Recipe] CHECK CONSTRAINT [FK_Recipe_Step]
GO
ALTER TABLE [dbo].[Recipe]  WITH NOCHECK ADD  CONSTRAINT [FK_Recipe_User] FOREIGN KEY([CreatorUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Recipe] CHECK CONSTRAINT [FK_Recipe_User]
GO
ALTER TABLE [dbo].[RecipeFile]  WITH NOCHECK ADD  CONSTRAINT [FK_RecipeFile_Recipe] FOREIGN KEY([RecipeID])
REFERENCES [dbo].[Recipe] ([Id])
GO
ALTER TABLE [dbo].[RecipeFile] CHECK CONSTRAINT [FK_RecipeFile_Recipe]
GO
ALTER TABLE [dbo].[RecipeFile]  WITH NOCHECK ADD  CONSTRAINT [FK_RecipeFile_User] FOREIGN KEY([CreatorUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[RecipeFile] CHECK CONSTRAINT [FK_RecipeFile_User]
GO
ALTER TABLE [dbo].[Result]  WITH NOCHECK ADD  CONSTRAINT [FK_Result_Chamber] FOREIGN KEY([ChamberId])
REFERENCES [dbo].[Chamber] ([Id])
GO
ALTER TABLE [dbo].[Result] CHECK CONSTRAINT [FK_Result_Chamber]
GO
ALTER TABLE [dbo].[Result]  WITH NOCHECK ADD  CONSTRAINT [FK_Result_Recipe] FOREIGN KEY([RecipeId])
REFERENCES [dbo].[Recipe] ([Id])
GO
ALTER TABLE [dbo].[Result] CHECK CONSTRAINT [FK_Result_Recipe]
GO
ALTER TABLE [dbo].[Result]  WITH NOCHECK ADD  CONSTRAINT [FK_Result_WaferResult] FOREIGN KEY([WaferResultId])
REFERENCES [dbo].[WaferResult] ([Id])
GO
ALTER TABLE [dbo].[Result] CHECK CONSTRAINT [FK_Result_WaferResult]
GO
ALTER TABLE [dbo].[ResultAcq]  WITH NOCHECK ADD  CONSTRAINT [FK_ResultAcq_Chamber] FOREIGN KEY([ChamberId])
REFERENCES [dbo].[Chamber] ([Id])
GO
ALTER TABLE [dbo].[ResultAcq] CHECK CONSTRAINT [FK_ResultAcq_Chamber]
GO
ALTER TABLE [dbo].[ResultAcq]  WITH NOCHECK ADD  CONSTRAINT [FK_ResultAcq_Recipe] FOREIGN KEY([RecipeId])
REFERENCES [dbo].[Recipe] ([Id])
GO
ALTER TABLE [dbo].[ResultAcq] CHECK CONSTRAINT [FK_ResultAcq_Recipe]
GO
ALTER TABLE [dbo].[ResultAcq]  WITH NOCHECK ADD  CONSTRAINT [FK_ResultAcq_WaferResult] FOREIGN KEY([WaferResultId])
REFERENCES [dbo].[WaferResult] ([Id])
GO
ALTER TABLE [dbo].[ResultAcq] CHECK CONSTRAINT [FK_ResultAcq_WaferResult]
GO
ALTER TABLE [dbo].[ResultAcqItem]  WITH NOCHECK ADD  CONSTRAINT [FK_ResultAcqItem_ResultAcq] FOREIGN KEY([ResultAcqId])
REFERENCES [dbo].[ResultAcq] ([Id])
GO
ALTER TABLE [dbo].[ResultAcqItem] CHECK CONSTRAINT [FK_ResultAcqItem_ResultAcq]
GO
ALTER TABLE [dbo].[ResultItem]  WITH NOCHECK ADD  CONSTRAINT [FK_ResultItem_Result] FOREIGN KEY([ResultId])
REFERENCES [dbo].[Result] ([Id])
GO
ALTER TABLE [dbo].[ResultItem] CHECK CONSTRAINT [FK_ResultItem_Result]
GO
ALTER TABLE [dbo].[ResultItemValue]  WITH NOCHECK ADD  CONSTRAINT [FK_ResultValue_ResultItem] FOREIGN KEY([ResultItemId])
REFERENCES [dbo].[ResultItem] ([Id])
GO
ALTER TABLE [dbo].[ResultItemValue] CHECK CONSTRAINT [FK_ResultValue_ResultItem]
GO
ALTER TABLE [dbo].[Step]  WITH NOCHECK ADD  CONSTRAINT [FK_Step_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[Step] CHECK CONSTRAINT [FK_Step_Product]
GO
ALTER TABLE [dbo].[Tag]  WITH NOCHECK ADD  CONSTRAINT [FK_Tag_Recipe] FOREIGN KEY([RecipeId])
REFERENCES [dbo].[Recipe] ([Id])
GO
ALTER TABLE [dbo].[Tag] CHECK CONSTRAINT [FK_Tag_Recipe]
GO
ALTER TABLE [dbo].[User]  WITH NOCHECK ADD  CONSTRAINT [FK_User_Tool] FOREIGN KEY([ToolId])
REFERENCES [dbo].[Tool] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_Tool]
GO
ALTER TABLE [dbo].[WaferResult]  WITH NOCHECK ADD  CONSTRAINT [FK_WaferResult_Job1] FOREIGN KEY([JobId])
REFERENCES [dbo].[Job] ([Id])
GO
ALTER TABLE [dbo].[WaferResult] CHECK CONSTRAINT [FK_WaferResult_Job1]
GO
ALTER TABLE [dbo].[WaferResult]  WITH NOCHECK ADD  CONSTRAINT [FK_WaferResult_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[WaferResult] CHECK CONSTRAINT [FK_WaferResult_Product]
GO
ALTER TABLE [dbo].[Dataflow]  WITH NOCHECK ADD  CONSTRAINT [FK_Dataflow_Step] FOREIGN KEY([StepId])
REFERENCES [dbo].[Step] ([Id])
GO
ALTER TABLE [dbo].[Dataflow] CHECK CONSTRAINT [FK_Dataflow_Step]
GO
ALTER TABLE [dbo].[Dataflow]  WITH NOCHECK ADD  CONSTRAINT [FK_Dataflow_Tool] FOREIGN KEY([CreatorTool])
REFERENCES [dbo].[Tool] ([Id])
GO
ALTER TABLE [dbo].[Dataflow] CHECK CONSTRAINT [FK_Dataflow_Tool]
GO
ALTER TABLE [dbo].[Dataflow]  WITH NOCHECK ADD  CONSTRAINT [FK_Dataflow_User] FOREIGN KEY([CreatorUserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Dataflow] CHECK CONSTRAINT [FK_Dataflow_User]
GO
/****** Object:  StoredProcedure [dbo].[DeleteAllData]    Script Date: 05/07/2022 16:00:30 ******/
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

set @DbVersion =CONCAT('''', N'8.0.0','''')
set @DbDateVersion =CONCAT('''', N'2023-06-19 12:30:00','''')
set @InsertVersionQuery = 'INSERT INTO [dbo].[DatabaseVersion] (Version, Date) VALUES ( ' + @DbVersion + ' , ' +  @DbDateVersion + ' )'

EXEC sp_executesql @InsertVersionQuery

END


GO
/****** Object:  StoredProcedure [dbo].[sp_GetJobResults]    Script Date: 09/06/2023 17:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		YSI / RTI
-- Create date: 21/06/2019
-- Update date: 05/07/2022
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
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Pour optiomisation perf.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Job', @level2type=N'COLUMN',@level2name=N'ToolId'
GO
USE [master]
GO
ALTER DATABASE [UnityControlv8] SET  READ_WRITE 
GO

USE [UnityControlv8]
GO
/****** Object:  Update DatabaseVersion Script Date: 05/07/2022 13:56:23 ******/
declare @DbVersion varchar(50)
declare @DbDateVersion varchar(50)
declare @InsertVersionQuery nvarchar(500)

set @DbVersion =CONCAT('''', N'8.0.0','''')
set @DbDateVersion =CONCAT('''', N'2023-06-09 14:00:00','''')
set @InsertVersionQuery = 'INSERT INTO [dbo].[DatabaseVersion] (Version, Date) VALUES ( ' + @DbVersion + ' , ' +  @DbDateVersion + ' )'

EXEC sp_executesql @InsertVersionQuery
GO

/****** Script de la commande SelectTopNRows à partir de SSMS  ******/
SELECT TOP 10 [Id]
      ,[Version]
      ,[Date]
  FROM [UnityControlv8].[dbo].[DatabaseVersion]