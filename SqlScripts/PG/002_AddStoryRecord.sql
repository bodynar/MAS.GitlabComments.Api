DO $$
BEGIN
	IF NOT EXISTS (
		SELECT 1 FROM "ScriptLog"
		WHERE "ScriptName" = '002_AddStoryRecord'
	)
		THEN
			---------
			--- Add StoryRecords table to track actions
			---------
			CREATE TABLE "StoryRecords"
			(
				"Id" UUID NOT NULL
					CONSTRAINT "DF_StoryRecords_Id"
						DEFAULT (gen_random_uuid())
					CONSTRAINT "PK_StoryRecords"
						PRIMARY KEY
				,
				"CreatedOn" TIMESTAMP(6) NOT NULL,
				"CommentId" UUID NOT NULL,
					CONSTRAINT "FK_StoryRecords_Comments"
						FOREIGN KEY("CommentId")
							REFERENCES "Comments"("Id")
				,
				"IsIncrementAction" BOOLEAN NOT NULL
			)
			;	
	
			---------
			--- Add trigger for cascade delete comment and its story records
			---------
			CREATE OR REPLACE FUNCTION fn_delete_comments_related_storyRecords()
				RETURNS trigger AS
			$tr$
				BEGIN
					DELETE FROM "StoryRecords"
					WHERE "CommentId" = OLD."Id";
					RETURN OLD;
				END;
			$tr$
			LANGUAGE 'plpgsql';

			CREATE TRIGGER Comments_Delete_TR
				BEFORE DELETE
				ON "Comments"
				FOR EACH ROW
				EXECUTE PROCEDURE fn_delete_comments_related_storyRecords();
	

			INSERT INTO "ScriptLog"
				("CreatedOn", "ScriptName")
			VALUES
				(NOW(), '002_AddStoryRecord')
			;
	END IF;
EXCEPTION WHEN OTHERS
THEN
    ROLLBACK;
    RAISE EXCEPTION '[Error due script execution, transaction aborted]: %', sqlerrm;
END;
$$;
