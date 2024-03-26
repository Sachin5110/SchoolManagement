CREATE TABLE [dbo].[Jwt_Tokens]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Token] [nvarchar](max) NULL,
	[Status] [nvarchar](100) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[ModifiedBy] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[DeletedDate] [datetime2](7) NULL,
)
