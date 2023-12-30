IF NOT EXISTS (
	SELECT * FROM [ScriptLog]
	WHERE [ScriptName] = '002_AddStoryRecord'
)
BEGIN
	---------
	--- Add StoryRecords table to track actions
	---------
	CREATE TABLE [dbo].[StoryRecords]
	(
		[Id] [uniqueidentifier] NOT NULL
			CONSTRAINT [DF_StoryRecords_Id]
				DEFAULT (NEWID())
			CONSTRAINT [PK_StoryRecords]
			PRIMARY KEY CLUSTERED ([Id] ASC)
		,
		[CreatedOn] [datetime2](7) NOT NULL,
		[CommentId] [uniqueidentifier] NOT NULL
			CONSTRAINT [FK_StoryRecords_Comments] 
				FOREIGN KEY([CommentId])
					REFERENCES [dbo].[Comments] ([Id])
		,
		[IsIncrementAction] [bit] NOT NULL,
	)
	;	
	
	---------
	--- Add trigger for cascade delete comment and its story records
	---------
	EXEC(
		'CREATE TRIGGER [dbo].[Comments_Delete]
			ON [dbo].[Comments]
			INSTEAD OF DELETE
		AS
			DELETE FROM [dbo].[StoryRecords]
			WHERE [CommentId] in (select id from deleted)

			DELETE FROM [dbo].[Comments]
			WHERE [Id] in (select id from deleted)
		;'
	)
	
	ALTER TABLE [dbo].[Comments]
		ENABLE TRIGGER [Comments_Delete]
	

	INSERT INTO [ScriptLog]
		([CreatedOn], [ScriptName])
	VALUES
		(GETDATE(), '002_AddStoryRecord')
	
END
