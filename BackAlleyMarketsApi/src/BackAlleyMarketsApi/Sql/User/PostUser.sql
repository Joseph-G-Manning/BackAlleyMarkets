SET XACT_ABORT ON;

BEGIN TRANSACTION;

IF EXISTS (SELECT 1 FROM Profile_Login WITH (UPDLOCK, HOLDLOCK) WHERE UserName = @UserName)
BEGIN
    ROLLBACK TRANSACTION;
    THROW 50001, 'That username is already taken.', 1;
END

DECLARE @UserId INT;

INSERT INTO User_Profile (FirstName, LastName)
VALUES (@FirstName, @LastName);

SET @UserId = SCOPE_IDENTITY();

INSERT INTO Profile_Login (UserId, UserName, UserPassword)
VALUES (@UserId, @UserName, @UserPassword);

COMMIT TRANSACTION;