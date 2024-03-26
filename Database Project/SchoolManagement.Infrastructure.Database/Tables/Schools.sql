CREATE TABLE [dbo].[Schools]
(
	[Id] [uniqueidentifier] NOT NULL Primary key DEFAULT NEWID(),
	[SchoolName] [nchar](100) NULL,
	[SchoolAddress] [nchar](500) NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[DeletedBy][uniqueidentifier] NULL,
	[DeletedDate] [datetime2](7) NULL,
	[Status] [bit] NULL,
)
