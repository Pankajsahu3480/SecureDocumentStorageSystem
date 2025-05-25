--CREATE TABLE Users (
--    Id UNIQUEIDENTIFIER PRIMARY KEY,
--    Username NVARCHAR(100) UNIQUE NOT NULL,
--    PasswordHash NVARCHAR(255) NOT NULL
--);

--CREATE TABLE Documents (

--    Id UNIQUEIDENTIFIER PRIMARY KEY,
--    UserId UNIQUEIDENTIFIER NOT NULL,
--    FileName NVARCHAR(255) NOT NULL,
--    Content VARBINARY(MAX) NOT NULL,
--    Version INT NOT NULL,
--    UploadedAt DATETIME NOT NULL,
--    CONSTRAINT FK_Documents_Users FOREIGN KEY(UserId) REFERENCES Users(Id)
--);

--USE SecureDocumentStorageSystem
--GO
--CREATE PROCEDURE spUploadDocument
--    @UserId UNIQUEIDENTIFIER,
--    @FileName NVARCHAR(255),
--    @Content VARBINARY(MAX)
--AS
--BEGIN
--    SET NOCOUNT ON;

--    DECLARE @NextVersion INT;

--    SELECT @NextVersion = ISNULL(MAX(Version) + 1, 0)
--    FROM Documents
--    WHERE UserId = @UserId AND FileName = @FileName;

--    INSERT INTO Documents (Id, UserId, FileName, Content, Version, UploadedAt)
--    VALUES (NEWID(), @UserId, @FileName, @Content, @NextVersion, GETUTCDATE());
--END


--GO
--CREATE PROCEDURE spGetLatestDocument
--    @UserId UNIQUEIDENTIFIER,
--    @FileName NVARCHAR(255)
--AS
--BEGIN
--    SET NOCOUNT ON;

--    SELECT TOP 1 *
--    FROM Documents
--    WHERE UserId = @UserId AND FileName = @FileName
--    ORDER BY Version DESC;
--END



--GO
--CREATE PROCEDURE spGetDocumentByRevision
--    @UserId UNIQUEIDENTIFIER,
--    @FileName NVARCHAR(255),
--    @Revision INT
--AS
--BEGIN
--    SET NOCOUNT ON;

--    SELECT *
--    FROM Documents
--    WHERE UserId = @UserId AND FileName = @FileName AND Version = @Revision;
--END


--Go
--CREATE PROCEDURE spListUserDocuments
--    @UserId UNIQUEIDENTIFIER
--AS
--BEGIN
--    SET NOCOUNT ON;

--    SELECT DISTINCT FileName
--    FROM Documents
--    WHERE UserId = @UserId;
--END


-- Get user by username
CREATE PROCEDURE GetUserByUsername
    @Username NVARCHAR(100)
AS
BEGIN
    SELECT * FROM Users WHERE Username = @Username;
END;
GO

-- Add new user
CREATE PROCEDURE AddUser
    @Id UNIQUEIDENTIFIER,
    @Username NVARCHAR(100),
    @PasswordHash NVARCHAR(200)
AS
BEGIN
    INSERT INTO Users (Id, Username, PasswordHash)
    VALUES (@Id, @Username, @PasswordHash);
END;
GO

-- Add document with versioning
CREATE PROCEDURE AddDocument
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @FileName NVARCHAR(200),
    @Content VARBINARY(MAX),
    @UploadDate DATETIME,
    @Revision INT
AS
BEGIN
    INSERT INTO Documents (Id, UserId, FileName, Content, UploadDate, Revision)
    VALUES (@Id, @UserId, @FileName, @Content, @UploadDate, @Revision);
END;
GO

-- Get latest revision
CREATE PROCEDURE GetLatestDocument
    @UserId UNIQUEIDENTIFIER,
    @FileName NVARCHAR(200)
AS
BEGIN
    SELECT TOP 1 * FROM Documents
    WHERE UserId = @UserId AND FileName = @FileName
    ORDER BY Revision DESC;
END;
GO

-- Get specific revision
CREATE PROCEDURE GetDocumentByRevision
    @UserId UNIQUEIDENTIFIER,
    @FileName NVARCHAR(200),
    @Revision INT
AS
BEGIN
    SELECT * FROM Documents
    WHERE UserId = @UserId AND FileName = @FileName AND Revision = @Revision;
END;
GO
