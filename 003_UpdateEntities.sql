----
-- Add base columns into StoryRecords
----
ALTER TABLE [dbo].[StoryRecords]
	ADD [ModifiedOn] [datetime2](7) NULL
;
GO


----
-- Add new columns into Comments
----
ALTER TABLE [dbo].[Comments]
	ADD [CommentWithLinkToRule] [nvarchar](max) NOT NULL
		CONSTRAINT [DF_Comments_Link] DEFAULT ('')
;
GO

ALTER TABLE [dbo].[Comments]
	DROP CONSTRAINT [DF_Comments_Link]
;
GO

INSERT INTO [ScriptLog]
	([CreatedOn], [ScriptName])
VALUES
	(GETDATE(), '003_UpdateEntities')
;
GO