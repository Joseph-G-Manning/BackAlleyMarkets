SET
    XACT_ABORT ON;

BEGIN TRANSACTION;

-- 1. Reject duplicate item names
IF EXISTS (
    SELECT
        1
    FROM
        dbo.Item WITH (UPDLOCK, HOLDLOCK)
    WHERE
        ItemName = @ItemName
) BEGIN ROLLBACK TRANSACTION;

THROW 50001,
'An item with this name already exists.',
1;

END -- 2. Find or create the category
DECLARE @CategoryId INT;

SELECT
    @CategoryId = CategoryId
FROM
    dbo.Category WITH (UPDLOCK, HOLDLOCK)
WHERE
    CategoryName = @CategoryName;

IF @CategoryId IS NULL BEGIN
INSERT INTO
    dbo.Category (CategoryName)
VALUES
    (@CategoryName);

SET
    @CategoryId = SCOPE_IDENTITY();

END -- 3. Insert the item
INSERT INTO
    dbo.Item (ItemName, Description, CategoryId)
VALUES
    (@ItemName, @Description, @CategoryId);

DECLARE @ItemId INT = SCOPE_IDENTITY();

-- 4. Unpack the tag array (trimmed, blanks removed, duplicates removed)
DECLARE @TagNames TABLE (
    TagName NVARCHAR(100) COLLATE DATABASE_DEFAULT PRIMARY KEY
);

INSERT INTO
    @TagNames (TagName)
SELECT
    DISTINCT LTRIM(RTRIM([value]))
FROM
    OPENJSON(@Tags)
WHERE
    LTRIM(RTRIM([value])) <> '';

-- 5. Create only the tags that don't exist yet
INSERT INTO
    dbo.Tag (TagName)
SELECT
    t.TagName
FROM
    @TagNames t
WHERE
    NOT EXISTS (
        SELECT
            1
        FROM
            dbo.Tag WITH (UPDLOCK, HOLDLOCK)
        WHERE
            TagName = t.TagName
    );

-- 6. Link every tag (new or existing) to the new item
INSERT INTO
    dbo.Item_Tag (ItemId, TagId)
SELECT
    @ItemId,
    tg.TagId
FROM
    dbo.Tag tg
    JOIN @TagNames t ON t.TagName = tg.TagName;

COMMIT TRANSACTION;

-- Result set 1: the item
SELECT
    @ItemId AS ItemId,
    @ItemName AS ItemName,
    @Description AS Description,
    @CategoryId AS CategoryId;

-- Result set 2: its tags
SELECT
    tg.TagId,
    tg.TagName
FROM
    dbo.Item_Tag it
    JOIN dbo.Tag tg ON tg.TagId = it.TagId
WHERE
    it.ItemId = @ItemId;