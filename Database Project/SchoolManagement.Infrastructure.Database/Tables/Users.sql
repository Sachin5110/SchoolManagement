CREATE TABLE [dbo].[Users]
(
	[UserId] [uniqueidentifier] NOT NULL Primary key DEFAULT NEWID(),
	[UserName] [nchar](100) NOT NULL,
	[Password] [nchar](100) NOT NULL,
	[RoleId] [int] NOT NULL,
	[Email] [nchar](100) NOT NULL,
	[Phone] [nchar](10) NOT NULL,
	[SchoolId] [uniqueidentifier] NULL,
	[TokenId] [bigint] NULL,
	[CreatedBy] [uniqueidentifier] NULL ,
	[CreatedDate] [datetime2](7) NULL DEFAULT GETDATE(),
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedDate] [datetime2](7) NULL,
	[Status] [bit] NULL,
)
