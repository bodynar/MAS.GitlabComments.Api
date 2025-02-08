DO $$
BEGIN
	IF NOT EXISTS (
		SELECT 1 FROM "ScriptLog"
		WHERE "ScriptName" = '005_AddRetraction'
	)
		THEN
			----
			-- Add new columns into StoryRecords
			----

			ALTER TABLE "StoryRecords"
				ADD "IsRetracted" BOOLEAN
					NOT NULL
					DEFAULT ('f')
			;

			ALTER TABLE "StoryRecords"
				ALTER COLUMN "IsRetracted" DROP DEFAULT
			;

			---------
			--- Add RetractionToken table to store temp retraction tokens
			---------
			CREATE TABLE "RetractionTokens"
			(
				"Id" UUID NOT NULL
					CONSTRAINT DF_RetractionTokens_Id
						DEFAULT (gen_random_uuid())
					CONSTRAINT PK_RetractionTokens
					PRIMARY KEY
				,
				"CreatedOn" TIMESTAMP(6) NOT NULL,
				"ModifiedOn" TIMESTAMP(6) NULL,

				"ValidUntil" TIMESTAMP(6) NOT NULL,
				"CommentId" UUID NOT NULL,
					CONSTRAINT FK_RetractionTokens_Comments
						FOREIGN KEY("CommentId")
							REFERENCES "Comments" ("Id"),
				"StoryRecordId" UUID NOT NULL,
					CONSTRAINT "FK_RetractionTokens_StoryRecords"
						FOREIGN KEY("StoryRecordId")
							REFERENCES "StoryRecords" ("Id")
			)
			;

			CREATE OR REPLACE FUNCTION fn_delete_comments_related_storyRecords()
				RETURNS trigger AS
			$tr$
				BEGIN
					DELETE FROM "RetractionTokens"
					WHERE "CommentId" = OLD."Id";

					DELETE FROM "StoryRecords"
					WHERE "CommentId" = OLD."Id";
					
					RETURN OLD;
				END;
			$tr$
			LANGUAGE 'plpgsql';

			INSERT INTO "ScriptLog"
				("CreatedOn", "ScriptName")
			VALUES
				(NOW(), '005_AddRetraction')
			;
	END IF;
EXCEPTION WHEN OTHERS
THEN
    ROLLBACK;
    RAISE EXCEPTION '[Error due script execution, transaction aborted]: %', sqlerrm;
END;
$$;
