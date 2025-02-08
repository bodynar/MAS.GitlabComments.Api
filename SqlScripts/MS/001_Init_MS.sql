BEGIN TRY
    BEGIN TRAN
		IF NOT EXISTS (
			SELECT NULL FROM sys.objects
			WHERE object_id = OBJECT_ID(N'[dbo].[ScriptLog]')
		)
		BEGIN
			---------
			--- Add comments table with constraints
			---------
			CREATE TABLE [dbo].[Comments]
			(
				[Id] [uniqueidentifier] NOT NULL
					CONSTRAINT [PK_Comments]
						PRIMARY KEY CLUSTERED
					CONSTRAINT [DF_Comments_Id]
						DEFAULT (NEWID())
				,
				[CreatedOn] [datetime2](7) NOT NULL,
				[ModifiedOn] [datetime2](7) NULL,
				[Message] [nvarchar](max) NOT NULL,
				[Description] [nvarchar](max) NULL,
				[AppearanceCount] [int] NOT NULL
					CONSTRAINT [DF_Comments_AppearanceCount]
						DEFAULT ((1))
				,
			)
			;

			---------
			--- Add script log to track version of db
			---------
			CREATE TABLE [dbo].[ScriptLog]
			(
				[Id] UNIQUEIDENTIFIER NOT NULL
					CONSTRAINT [DF_ScriptLog_Id]
						DEFAULT NEWID()
				,
				[CreatedOn] DATETIME2(7) NOT NULL,
				[ScriptName] NVARCHAR(255) NOT NULL
			)
			;


			INSERT INTO [ScriptLog]
				([CreatedOn], [ScriptName])
			VALUES
				(GETDATE(), '001_Init')
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