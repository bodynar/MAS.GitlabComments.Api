DO $$
BEGIN
	IF NOT EXISTS (
		SELECT 1 FROM "ScriptLog"
		WHERE "ScriptName" = '006_AddActionToSysVariable'
	)
		THEN
			----
			-- Add new columns into SystemVariables
			----

			ALTER TABLE "SystemVariables"
				ADD "ActionCaption" TEXT NULL
			;

			
			UPDATE "SystemVariables"
			SET "ActionCaption" = 'Recalculate'
			WHERE "Code" = 'LastCommentNumber'
			;

			INSERT INTO "ScriptLog"
				("CreatedOn", "ScriptName")
			VALUES
				(NOW(), '006_AddActionToSysVariable')
			;
	END IF;
EXCEPTION WHEN OTHERS
THEN
    ROLLBACK;
    RAISE EXCEPTION '[Error due script execution, transaction aborted]: %', sqlerrm;
END;
$$;
