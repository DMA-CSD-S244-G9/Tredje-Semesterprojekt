USE InfiniteInfluence;
GO

-- Disable foreign key checks (safest for teardown)
EXEC sp_msforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT ALL";
GO


-- DROP TABLES IN CORRECT ORDER
-- Lowest-level FK tables first
IF OBJECT_ID('dbo.InfluencerAnnouncements', 'U') IS NOT NULL
    DROP TABLE InfluencerAnnouncements;

IF OBJECT_ID('dbo.AnnouncementSubjects', 'U') IS NOT NULL
    DROP TABLE AnnouncementSubjects;

IF OBJECT_ID('dbo.Announcements', 'U') IS NOT NULL
    DROP TABLE Announcements;

IF OBJECT_ID('dbo.InfluencerDomains', 'U') IS NOT NULL
    DROP TABLE InfluencerDomains;

IF OBJECT_ID('dbo.CompanyDomains', 'U') IS NOT NULL
    DROP TABLE CompanyDomains;

IF OBJECT_ID('dbo.Companys', 'U') IS NOT NULL
    DROP TABLE Companys;

IF OBJECT_ID('dbo.Influencers', 'U') IS NOT NULL
    DROP TABLE Influencers;

IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
    DROP TABLE Users;


-- CLEAN UP
-- clean up leftover constraints
EXEC sp_msforeachtable "ALTER TABLE ? CHECK CONSTRAINT ALL";
GO
