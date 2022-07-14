---------
--- Add comments table with constraints
---------
CREATE TABLE [dbo].[Comments]
(
	[Id] [uniqueidentifier] NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedOn] [datetime2](7) NULL,
	[Message] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[AppearanceCount] [int] NOT NULL,
)
;
GO

ALTER TABLE [dbo].[Comments]
	ADD CONSTRAINT [PK_Comments]
	PRIMARY KEY CLUSTERED ([Id] ASC)
;
GO

ALTER TABLE [dbo].[Comments]
	ADD CONSTRAINT [DF_Comments_Id]
	DEFAULT (NEWID()) FOR [Id]
;
GO

ALTER TABLE [dbo].[Comments]
	ADD CONSTRAINT [DF_Comments_AppearanceCount]
	DEFAULT ((1)) FOR [AppearanceCount]
;
GO


---------
--- Add script log to track version of db
---------
CREATE TABLE [dbo].[ScriptLog]
(
	[Id] UNIQUEIDENTIFIER NOT NULL,
	[CreatedOn] DATETIME2(7) NOT NULL,
	[ScriptName] NVARCHAR(255) NOT NULL
)
;

ALTER TABLE [dbo].[ScriptLog]
	ADD CONSTRAINT [DF_ScriptLog_Id]
	DEFAULT NEWID() FOR [Id]
;
GO

INSERT INTO [ScriptLog]
	([CreatedOn], [ScriptName])
VALUES
	(GETDATE(), '001_Init')