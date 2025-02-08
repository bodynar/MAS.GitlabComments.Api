BEGIN TRY
    BEGIN TRAN
        IF NOT EXISTS (
			SELECT NULL FROM [ScriptLog]
			WHERE [ScriptName] = '007_RemoveStoryRecordColumn'
		)
		BEGIN
			----
			-- Remove unused column "IsIncrementAction"
			----

			ALTER TABLE [StoryRecords]
				DROP COLUMN [IsIncrementAction]
			;

			INSERT INTO [ScriptLog]
				([CreatedOn], [ScriptName])
			VALUES
				(GETDATE(), '007_RemoveStoryRecordColumn')
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