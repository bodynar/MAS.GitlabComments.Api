IF NOT EXISTS (
	SELECT * FROM [ScriptLog]
	WHERE [ScriptName] = '004_AddNumber'
)
BEGIN
	----
	-- Add new columns into Comments
	----

	ALTER TABLE [dbo].[Comments]
		ADD [Number] [nvarchar](max) NOT NULL
			CONSTRAINT [DF_Comments_Number] DEFAULT ('')
	;

	ALTER TABLE [dbo].[Comments]
		DROP CONSTRAINT [DF_Comments_Number]
	;

	---------
	--- Add SystemVariables table to store system parameters
	---------
	CREATE TABLE [dbo].[SystemVariables]
	(
		[Id] [uniqueidentifier] NOT NULL
			CONSTRAINT [DF_SystemVariables_Id]
				DEFAULT (NEWID())
			CONSTRAINT [PK_SystemVariables]
			PRIMARY KEY CLUSTERED ([Id] ASC)
		,
		[CreatedOn] [datetime2](7) NOT NULL,
		[ModifiedOn] [datetime2](7) NULL,

		[Code] [nvarchar](255) NOT NULL
			CONSTRAINT [UQ_SystemVariables_Code] UNIQUE([Code]),
		[Caption] [nvarchar](max) NULL,
		[Type] [nvarchar](max) NOT NULL,
		[RawValue] [nvarchar](max) NOT NULL,
	)
	;

	INSERT INTO [SystemVariables]
		([CreatedOn], [Code], [Type], [RawValue])
	VALUES
		(GETDATE(), 'LastCommentNumber', 'Int', '0')
	;

	INSERT INTO [ScriptLog]
		([CreatedOn], [ScriptName])
	VALUES
		(GETDATE(), '004_AddNumber')
	;
END
