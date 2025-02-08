DO $$
BEGIN
	IF NOT EXISTS (
		SELECT 1 FROM "ScriptLog"
		WHERE "ScriptName" = '007_RemoveStoryRecordColumn'
	)
		THEN
			----
			-- Remove unused column "IsIncrementAction"
			----

			ALTER TABLE "StoryRecords"
				DROP COLUMN "IsIncrementAction"
			;

			INSERT INTO "ScriptLog"
				("CreatedOn", "ScriptName")
			VALUES
				(NOW(), '007_RemoveStoryRecordColumn')
			;
	END IF;
EXCEPTION WHEN OTHERS
THEN
    ROLLBACK;
    RAISE EXCEPTION '[Error due script execution, transaction aborted]: %', sqlerrm;
END;
$$;
