BEGIN TRY
    BEGIN TRAN
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
				ADD [CommentWithLinkToRule] [nvarchar](1024) NOT NULL
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
	COMMIT TRAN
END TRY
BEGIN CATCH
    IF (@@TRANCOUNT > 0)
        ROLLBACK TRAN;
    DECLARE @number INT = 50000 + ERROR_NUMBER();
    DECLARE @state TINYINT = ERROR_STATE();
    DECLARE @message VARCHAR(MAX) = FORMATMESSAGE('[Error due script execution, transaction aborted]: %s', ERROR_MESSAGE());
     
    THROW @number, @message, @state;
END CATCH