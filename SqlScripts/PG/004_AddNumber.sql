DO $$
BEGIN
	IF NOT EXISTS (
		SELECT 1 FROM "ScriptLog"
		WHERE "ScriptName" = '004_AddNumber'
	)
		THEN
			----
			-- Add new columns into Comments
			----
		
			ALTER TABLE "Comments"
				ADD "Number" VARCHAR(255)
					NOT NULL
					DEFAULT ('')
			;
		
			ALTER TABLE "Comments"
				ALTER COLUMN "Number" DROP DEFAULT
			;
		
			---------
			--- Add SystemVariables table to store system parameters
			---------
			CREATE TABLE "SystemVariables"
			(
				"Id" UUID NOT NULL
					CONSTRAINT "DF_SystemVariables_Id"
						DEFAULT (gen_random_uuid())
					CONSTRAINT "PK_SystemVariables"
					PRIMARY KEY
				,
				"CreatedOn" TIMESTAMP(6) NOT NULL,
				"ModifiedOn" TIMESTAMP(6) NULL,
		
				"Code" VARCHAR(255) NOT NULL,
					CONSTRAINT "UQ_SystemVariables_Code" UNIQUE("Code"),
				"Caption" TEXT NULL,
				"Type" VARCHAR(255) NOT NULL,
				"RawValue" TEXT NOT NULL
			)
			;
		
			INSERT INTO "SystemVariables"
				("CreatedOn", "Code", "Type", "RawValue", "Caption")
			VALUES
				(NOW(), 'LastCommentNumber', 'Int', '0', 'Sequence number of latest added comment'),
				(NOW(), 'IsChangeNumberUnique', 'Bool', 'false', 'Is comments table modified with number column unique constraint')
			;

			INSERT INTO "ScriptLog"
				("CreatedOn", "ScriptName")
			VALUES
				(NOW(), '004_AddNumber')
			;
	END IF;
EXCEPTION WHEN OTHERS
THEN
    ROLLBACK;
    RAISE EXCEPTION '[Error due script execution, transaction aborted]: %', sqlerrm;
END;
$$;
