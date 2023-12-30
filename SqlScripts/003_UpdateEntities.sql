IF NOT EXISTS (
	SELECT * FROM [ScriptLog]
	WHERE [ScriptName] = '003_UpdateEntities'
)
BEGIN
    ----
	-- Add base columns into StoryRecords
	----
	ALTER TABLE [dbo].[StoryRecords]
		ADD [ModifiedOn] [datetime2](7) NULL
	;

	----
	-- Add new columns into Comments
	----
	ALTER TABLE [dbo].[Comments]
		ADD [CommentWithLinkToRule] [nvarchar](255) NOT NULL
			CONSTRAINT [DF_Comments_Link] DEFAULT ('')
	;

	ALTER TABLE [dbo].[Comments]
		DROP CONSTRAINT [DF_Comments_Link]
	;

	INSERT INTO [ScriptLog]
		([CreatedOn], [ScriptName])
	VALUES
		(GETDATE(), '003_UpdateEntities')
	;
END
