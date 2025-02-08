DO $$
BEGIN
	IF NOT EXISTS (
		SELECT 1 FROM "ScriptLog"
		WHERE "ScriptName" = '003_UpdateEntities'
	)
		THEN
			----
			-- Add base columns into StoryRecords
			----
			ALTER TABLE "StoryRecords"
				ADD "ModifiedOn" TIMESTAMP(6) NULL
			;
		
			----
			-- Add new columns into Comments
			----
			ALTER TABLE "Comments"
				ADD "CommentWithLinkToRule" TEXT
					NOT NULL
					DEFAULT ('')
			;
		
			ALTER TABLE "Comments"
				ALTER COLUMN "CommentWithLinkToRule" DROP DEFAULT
			;

			INSERT INTO "ScriptLog"
				("CreatedOn", "ScriptName")
			VALUES
				(NOW(), '003_UpdateEntities')
			;
	END IF;
EXCEPTION WHEN OTHERS
THEN
    ROLLBACK;
    RAISE EXCEPTION '[Error due script execution, transaction aborted]: %', sqlerrm;
END;
$$;
