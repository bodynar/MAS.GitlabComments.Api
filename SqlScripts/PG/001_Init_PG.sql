DO $$
BEGIN
	IF NOT EXISTS (
		SELECT 1
		FROM pg_catalog.pg_tables
		WHERE schemaname = 'public'
		AND tablename  = 'ScriptLog'
	)
		THEN
			---------
			--- Add comments table with constraints
			---------
			CREATE TABLE "Comments"
			(
				"Id" UUID NOT NULL
					CONSTRAINT "PK_Comments"
						PRIMARY KEY
					CONSTRAINT "DF_Comments_Id"
						DEFAULT (gen_random_uuid())
				,
				"CreatedOn" TIMESTAMP(6) NOT NULL,
				"ModifiedOn" TIMESTAMP(6) NULL,
				"Message" TEXT NOT NULL,
				"Description" TEXT NULL,
				"AppearanceCount" INT NOT NULL
					CONSTRAINT "DF_Comments_AppearanceCount"
						DEFAULT ((1))
			
			);

			---------
			--- Add script log to track version of db
			---------
			CREATE TABLE "ScriptLog"
			(
				"Id" UUID NOT NULL
					CONSTRAINT "DF_ScriptLog_Id"
						DEFAULT gen_random_uuid()
				,
				"CreatedOn" TIMESTAMP(6) NOT NULL,
				"ScriptName" VARCHAR(255) NOT NULL
			);


			INSERT INTO "ScriptLog"
				("CreatedOn", "ScriptName")
			VALUES
				(NOW(), '001_Init')
			;
	END IF;
EXCEPTION WHEN OTHERS
THEN
    ROLLBACK;
    RAISE EXCEPTION '[Error due script execution, transaction aborted]: %', sqlerrm;
END;
$$;
