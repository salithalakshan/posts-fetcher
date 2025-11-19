
BEGIN
    CREATE DATABASE ExternalCacheService;
END



BEGIN
    CREATE TABLE dbo.CacheEntry
    (
        Id            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CacheKey      NVARCHAR(200)     NOT NULL UNIQUE,
        JsonValue     NVARCHAR(MAX)     NOT NULL,
        CreatedAtUtc  DATETIME2(3)      NOT NULL,
        ExpiresAtUtc  DATETIME2(3)      NOT NULL
    );

	CREATE NONCLUSTERED INDEX IX_CacheEntry_ExpiresAtUtc
        ON dbo.CacheEntry (ExpiresAtUtc);
END