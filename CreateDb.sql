
CREATE DATABASE ExternalCacheService;
Go

CREATE TABLE dbo.CacheEntry
(
    Id            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CacheKey      NVARCHAR(200)     NOT NULL UNIQUE,
    JsonValue     NVARCHAR(MAX)     NOT NULL,
    CreatedAtUtc  DATETIME2(7)      NOT NULL,
    ExpiresAtUtc  DATETIME2(7)      NOT NULL
);

CREATE NONCLUSTERED INDEX IX_CacheEntry_CacheKey
    ON dbo.CacheEntry (CacheKey);

CREATE NONCLUSTERED INDEX IX_CacheEntry_ExpiresAtUtc
    ON dbo.CacheEntry (ExpiresAtUtc);

Go