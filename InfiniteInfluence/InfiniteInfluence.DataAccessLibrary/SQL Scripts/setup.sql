use master;

if exists (select * from sys.databases where name='InfiniteInfluence')
	ALTER DATABASE [InfiniteInfluence] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
go
if exists (select * from sys.databases where name='InfiniteInfluence')
	drop database InfiniteInfluence;
go

create database InfiniteInfluence;
go

USE InfiniteInfluence;
GO


--  USERS
CREATE TABLE Users (
    userId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    loginEmail VARCHAR(64) NOT NULL UNIQUE,
    passwordHash VARCHAR(100) NOT NULL
);


--  INFLUENCERS
CREATE TABLE Influencers (
    userId INT NOT NULL PRIMARY KEY, -- Both primary and foreign key
    isInfluencerVerified BIT DEFAULT 0,
    verificationDate datetime2 NULL,
    displayName NVARCHAR(100) NOT NULL,
    firstName NVARCHAR(100) NOT NULL,
    lastName NVARCHAR(100) NOT NULL,
    profileImageUrl NVARCHAR(255),
    age INT NOT NULL,
    gender NVARCHAR(50) NOT NULL,
    country NVARCHAR(100) NOT NULL,
    influencerState NVARCHAR(100) NOT NULL,
    city NVARCHAR(100) NOT NULL,
    influencerLanguage NVARCHAR(100) NOT NULL,
    biography NVARCHAR(500) NOT NULL,
    instagramProfileUrl NVARCHAR(255),
    instagramFollowers INT DEFAULT 0,
    youTubeProfileUrl NVARCHAR(255),
    youTubeFollowers INT DEFAULT 0,
    tikTokProfileUrl NVARCHAR(255),
    tikTokFollower INT DEFAULT 0,
    snapchatProfileUrl NVARCHAR(255),
    snapchatFollowers INT DEFAULT 0,
    xProfileUrl NVARCHAR(255),
    xFollowers INT DEFAULT 0,
    contactPhoneNumber NVARCHAR(20) NOT NULL,
    contactEmailAddress NVARCHAR(64) NOT NULL,

--  CONSTRAINT
--  Foreign key constraint from Influencers to Users
--  The 'userId' column in this table Influencers, references the 'userId' column in the Users table.
--  If a row in the Users table is deleted, all related rows in this table will be automatically deleted (ON DELETE CASCADE).
    CONSTRAINT foreignkey_constraint_from_influencers_to_users
        FOREIGN KEY (userId)
        REFERENCES Users(userId)
        ON DELETE NO ACTION
);


--  COMPANIES
CREATE TABLE Companys (
    userId INT NOT NULL PRIMARY KEY, -- Both primary and foreign key
    isCompanyVerified BIT DEFAULT 0,
    verificationDate datetime2 NULL,
    companyName NVARCHAR(255) NOT NULL,
    companyLogoUrl NVARCHAR(255),
    ceoName NVARCHAR(255) NOT NULL,
	  dateOfEstablishment datetime2 NULL,
	  organisationNumber NVARCHAR(50) NOT NULL,
    standardIndustryClassification int NOT NULL,
    websiteUrl NVARCHAR(255) NULL,
    companyEmail NVARCHAR(64) NOT NULL,
    companyPhoneNumber NVARCHAR(20) NOT NULL,
    country NVARCHAR(100) NOT NULL,
    companyState NVARCHAR(100) NOT NULL,
    city NVARCHAR(100) NOT NULL,
    companyAddress NVARCHAR(255) NOT NULL,
    companyLanguage NVARCHAR(100) NOT NULL,
    biography NVARCHAR(500) NOT NULL,
    contactPerson NVARCHAR(100) NOT NULL,
    contactEmailAddress NVARCHAR(64) NOT NULL,
    contactPhoneNumber NVARCHAR(20) NOT NULL,


--  CONSTRAINT
--  Foreign key constraint from Companys to Users
--  The 'userId' column in this table Companys, references the 'userId' column in the Users table.
--  If a row in the Users table is deleted, all related rows in this table will be automatically deleted (ON DELETE CASCADE).
    CONSTRAINT foreignkey_constraint_from_companys_to_users
        FOREIGN KEY (userId)
        REFERENCES Users(userId)
        ON DELETE NO ACTION
);


--  COMPANY DOMAINS
CREATE TABLE CompanyDomains (
    userId INT NOT NULL, -- Both primary and foreign key
    domain VARCHAR(20) NOT NULL,
	PRIMARY KEY (userId, domain),

--  CONSTRAINT
--  Foreign key constraint from CompanyDomains to Companys
--  The 'userId' column in this table CompanyDomains, references the 'userId' column in the Companys table.
--  If a row in the Companys table is deleted, all rows in this table that reference that company will be automatically deleted (ON DELETE CASCADE).
    CONSTRAINT foreignkey_constraint_from_companydomains_to_companys
        FOREIGN KEY (userId)
        REFERENCES Companys(userId)
        ON DELETE CASCADE
);


--  INFLUENCER DOMAINS
CREATE TABLE InfluencerDomains (
    userId INT NOT NULL, -- Both primary and foreign key
    domain VARCHAR(20) NOT NULL,
	PRIMARY KEY (userId, domain),

-- CONSTRAINT
--  Foreign key constraint from InfluencerDomains to Influencers
--  The 'userId' column in this table InfluencerDomains, references the 'userId' column in the Influencers table.
--  If a row in the Influencers table is deleted, all rows in this table that reference that influencer will be automatically deleted (ON DELETE CASCADE).
    CONSTRAINT foreignkey_constraint_from_influencerdomains_to_influencer
        FOREIGN KEY (userId)
        REFERENCES Influencers(userId)
        ON DELETE CASCADE
);


CREATE TABLE Announcements (
    announcementId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    userId INT NOT NULL,
    title VARCHAR(32) NOT NULL,
    creationDateTime datetime2 NULL,
    lastEditDateTime datetime2 NULL,
    startDisplayDateTime datetime2 NULL,
    endDisplayDateTime datetime2 NULL,
    currentApplicants INT NOT NULL,
    maximumApplicants INT NOT NULL,
    minimumFollowersRequired INT NOT NULL,
    communicationType VARCHAR(100) NOT NULL,
    announcementlanguage VARCHAR(100) NOT NULL,
    isKeepProducts BIT DEFAULT 0,
    isPayoutNegotiable BIT DEFAULT 0,
    totalPayoutAmount DECIMAL(10,2) NOT NULL,
    shortDescriptionText NVARCHAR(100) NOT NULL,
    additionalInformationText NVARCHAR(1500) NOT NULL,
    statusType NVARCHAR(50) NULL,
    isVisible BIT DEFAULT 0,

--  The column to track the version of the row that is used for our optimistic concurrency
    RowVersion rowversion NOT NULL,

--  CONSTRAINT
--  Foreign key constraint from Announcements to Companys
--  The 'userId' column in this table Announcements, references the 'userId' column in the Companys table.
--  If a row in the Companys table is deleted, all rows in this table that reference that company will be automatically deleted (ON DELETE CASCADE).
    CONSTRAINT foreignkey_constraint_from_announcements_to_companys
        FOREIGN KEY (userId)
        REFERENCES Companys(userId)
        ON DELETE CASCADE
);

--  ANNOUNCEMENT SUBJECTS
CREATE TABLE AnnouncementSubjects (
    announcementId INT NOT NULL, -- Both primary and foreign key
    announcementSubject VARCHAR(20) NOT NULL,
	PRIMARY KEY (announcementId, announcementSubject),

--  CONSTRAINT
--  Foreign key constraint from AnnouncementSubjects to Announcements
--  The 'announcementId' column in this table AnnouncementSubjects, references the 'announcementId' column in the Announcements table.
--  If a row in the Announcements table is deleted, all rows in this table that reference that annoucement will be automatically deleted (ON DELETE CASCADE).
    CONSTRAINT foeignkey_constraint_from_announcementsubjects_to_announcements
        FOREIGN KEY (announcementId)
        REFERENCES Announcements(announcementId)
        ON DELETE CASCADE
);


--  INFLUENCER ANNOUNCEMENTS
CREATE TABLE InfluencerAnnouncements (
    userId INT, -- Both primary and foreign key
    announcementId INT,
    applicationState VARCHAR(50),
    PRIMARY KEY (userId, announcementId),

--  CONSTRAINT
--  Foreign key constraint from InfluencerAnnouncements to Influencers
--  The 'userId' column in this table InfluencerAnnouncements, references the 'userId' column in the Influencers table.
--  If a row in the Influencers table is deleted, all rows in this table that reference that influencer will be automatically deleted (ON DELETE CASCADE).
    CONSTRAINT foreignkey_constraint_from_influencerannouncements_to_influencers
        FOREIGN KEY (userId)
        REFERENCES Influencers(userId)
        ON DELETE CASCADE,

--  Foreign key constraint from InfluencerAnnouncements to Announcements
--  The 'announcementId' column in this table InfluencerAnnouncements, references the 'announcementId' column in the Announcements table.
--  If a row in the Announcements table is deleted, all rows in this table that reference that announcement will be automatically deleted (ON DELETE CASCADE).
    CONSTRAINT foreignkey_constraint_from_influencerannouncements_to_announcement
        FOREIGN KEY (announcementId)
        REFERENCES Announcements(announcementId)
        ON DELETE CASCADE
);

