
---------
--- Add StoryRecords table to track actions
---------
CREATE TABLE [dbo].[StoryRecords]
(
	[Id] [uniqueidentifier] NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[CommentId] [uniqueidentifier] NOT NULL,
	[IsIncrementAction] [bit] NOT NULL,
)
GO

ALTER TABLE [dbo].[StoryRecords]
	ADD CONSTRAINT [PK_StoryRecords]
	PRIMARY KEY CLUSTERED ([Id] ASC)
;
GO

ALTER TABLE [dbo].[StoryRecords]
	ADD CONSTRAINT [DF_StoryRecords_Id]
	DEFAULT (NEWID()) FOR [Id]
GO

ALTER TABLE [dbo].[StoryRecords]
	WITH CHECK
	ADD CONSTRAINT [FK_StoryRecords_Comments]
	FOREIGN KEY([CommentId])
		REFERENCES [dbo].[Comments] ([Id])
GO

---------
--- Add trigger for cascade delete comment and its story records
---------
CREATE TRIGGER [dbo].[Comments_Delete]
    ON [dbo].[Comments]
    INSTEAD OF DELETE
AS
    DELETE FROM [dbo].[StoryRecords]
    WHERE [CommentId] in (select id from deleted)

	DELETE FROM [dbo].[Comments]
	WHERE [Id] in (select id from deleted)
;
GO

ALTER TABLE [dbo].[Comments]
ENABLE TRIGGER [Comments_Delete]
;
GO

INSERT INTO [ScriptLog]
	([CreatedOn], [ScriptName])
VALUES
	(GETDATE(), '002_AddStoryRecord')
;
GO