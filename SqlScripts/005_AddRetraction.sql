BEGIN TRY
    BEGIN TRAN
        IF NOT EXISTS (
			SELECT NULL FROM [ScriptLog]
			WHERE [ScriptName] = '005_AddRetraction'
		)
		BEGIN
			----
			-- Add new columns into StoryRecords
			----

			ALTER TABLE [dbo].[StoryRecords]
				ADD [IsRetracted] [bit] NOT NULL
					CONSTRAINT [DF_StoryRecords_IsRetracted] DEFAULT (0)
			;

			ALTER TABLE [dbo].[StoryRecords]
				DROP CONSTRAINT [DF_StoryRecords_IsRetracted]
			;

			---------
			--- Add RetractionToken table to store temp retraction tokens
			---------
			CREATE TABLE [dbo].[RetractionTokens]
			(
				[Id] [uniqueidentifier] NOT NULL
					CONSTRAINT [DF_RetractionTokens_Id]
						DEFAULT (NEWID())
					CONSTRAINT [PK_RetractionTokens]
					PRIMARY KEY CLUSTERED ([Id] ASC)
				,
				[CreatedOn] [datetime2](7) NOT NULL,
				[ModifiedOn] [datetime2](7) NULL,

				[ValidUntil] [datetime2](7) NOT NULL,
				[CommentId] [uniqueidentifier] NOT NULL
					CONSTRAINT [FK_RetractionTokens_Comments]
						FOREIGN KEY([CommentId])
							REFERENCES [dbo].[Comments] ([Id]),
				[StoryRecordId] [uniqueidentifier] NOT NULL
					CONSTRAINT [FK_RetractionTokens_StoryRecords]
						FOREIGN KEY([StoryRecordId])
							REFERENCES [dbo].[StoryRecords] ([Id]),
			)
			;

			---------
			--- Add trigger for cascade delete comment and its related records
			---------
			EXEC(
				'ALTER TRIGGER [dbo].[Comments_Delete]
					ON [dbo].[Comments]
					INSTEAD OF DELETE
				AS
					DELETE FROM [dbo].[RetractionTokens]
					WHERE [CommentId] in (select id from deleted)

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
				(GETDATE(), '005_AddRetraction')
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