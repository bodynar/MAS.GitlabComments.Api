BEGIN TRY
    BEGIN TRAN
        IF NOT EXISTS (
			SELECT NULL FROM [ScriptLog]
			WHERE [ScriptName] = '006_AddActionToSysVariable'
		)
		BEGIN
			----
			-- Add new columns into SystemVariables
			----

			ALTER TABLE [dbo].[SystemVariables]
				ADD [ActionCaption] nvarchar(max) NULL
			;

			EXECUTE('
				UPDATE [dbo].[SystemVariables]
				SET [ActionCaption] = ''Recalculate''
				WHERE [Code] = ''LastCommentNumber''
			')
			;

			INSERT INTO [ScriptLog]
				([CreatedOn], [ScriptName])
			VALUES
				(GETDATE(), '006_AddActionToSysVariable')
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