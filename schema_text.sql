/****** Object:  StoredProcedure [dbo].[usp_AccessingAuditLogs]    Script Date: 2023-09-23 20:45:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[usp_AccessingAuditLogs]
(
	@UserIp			nvarchar(50) ,
	@UserHostName	nvarchar(150),
	@RDPUserName	nvarchar(150),
	@ServerIp		nvarchar(50),
	@ServerHostName	nvarchar(150),
	@AccessTime		datetime	,
	@LevelOfEvent	nvarchar(50),
	@RawXMLData		xml,
	@Description	nvarchar(800),
	@IsAbnormal		bit
)
as
begin
	INSERT INTO tblServerAuditLogs
    (
		 [UserIp]
		,[UserHostName]
		,[RDPUserName]
		,[ServerIp]
		,[ServerHostName]
		,[AccessTime]
		,[LevelOfEvent]
		,[RawXMLData]
		,[Description]
		,[IsAbnormal]
	)
     VALUES
    (
		 @UserIp
		,@UserHostName
		,@RDPUserName
		,@ServerIp
		,@ServerHostName
		,@AccessTime
		,@LevelOfEvent
		,@RawXMLData
		,@Description
		,@IsAbnormal
	)
end
/****** Object:  Table [dbo].[tblServerAuditLogs]    Script Date: 2023-09-23 22:20:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblServerAuditLogs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserIp] [nvarchar](50) NULL,
	[UserHostName] [nvarchar](150) NULL,
	[RDPUserName] [nvarchar](150) NULL,
	[ServerIp] [nvarchar](50) NULL,
	[ServerHostName] [nvarchar](150) NULL,
	[AccessTime] [datetime] NULL,
	[LevelOfEvent] [nvarchar](50) NULL,
	[RawXMLData] [xml] NULL,
	[Description] [nvarchar](800) NULL,
	[IsAbnormal] [bit] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_tblServerAuditLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[tblServerAuditLogs] ADD  CONSTRAINT [DF_tblServerAuditLogs_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO





