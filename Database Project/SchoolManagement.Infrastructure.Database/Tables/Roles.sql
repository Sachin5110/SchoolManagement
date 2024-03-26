CREATE TABLE [dbo].[Roles]
(
	[Id] INT NOT NULL, 
    [Role] [nchar](100) NOT NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NOT NULL DEFAULT GETDATE(),
	[ModifiedBy] [uniqueidentifier] NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[DeletedBy][uniqueidentifier] NULL,
	[DeletedDate] [datetime2](7) NULL,
	[Status] [bit] NULL,
)
