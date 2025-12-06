USE InfiniteInfluence;
GO


-- USERS (10 users)
INSERT INTO Users (loginEmail, passwordHash) VALUES
('anna@mail.com', 'hash1'),
('tom@mail.com', 'hash2'),
('influencer3@mail.com', 'hash3'),
('influencer4@mail.com', 'hash4'),
('influencer5@mail.com', 'hash5'),
('company1@mail.com', 'hash6'),
('company2@mail.com', 'hash7'),
('company3@mail.com', 'hash8'),
('company4@mail.com', 'hash9'),
('company5@mail.com', 'hash10');


-- INFLUENCERS (5 influencers)
INSERT INTO Influencers (
    userId, isInfluencerVerified, verificationDate, displayName, firstName, lastName,
    profileImageUrl, age, gender, country, influencerState, city, influencerLanguage,
    biography, instagramProfileUrl, instagramFollowers, youTubeProfileUrl,
    youTubeFollowers, tikTokProfileUrl, tikTokFollower, snapchatProfileUrl,
    snapchatFollowers, xProfileUrl, xFollowers, contactPhoneNumber,
    contactEmailAddress
)
VALUES
(1, 1, GETDATE(), 'AnnaStyle', 'Anna', 'Larsen', NULL, 25, 'Female', 'Denmark', 'Hovedstaden', 'Copenhagen', 'Danish',
 'Fashion content creator.', 'insta.com/anna', 50000, NULL, 0, 'tiktok.com/annastyle', 90000, NULL, 0, NULL, 0, '+4511111111', 'anna@mail.com'),

(2, 0, GETDATE(), 'TechTom', 'Tom', 'Hansen', NULL, 29, 'Male', 'Denmark', 'Midtjylland', 'Aarhus', 'English',
 'Tech reviewer and gadget expert.', NULL, 0, 'youtube.com/techtom', 150000, NULL, 0, NULL, 0, NULL, 0, '+4522222222', 'tom@mail.com'),

(3, 1, GETDATE(), 'FoodieMia', 'Mia', 'Olsen', NULL, 31, 'Female', 'Sweden', 'Stockholm', 'Stockholm', 'Swedish',
 'Food lover sharing recipes.', 'insta.com/mia', 80000, NULL, 0, NULL, 0, NULL, 0, NULL, 0, '+4683333333', 'mia@mail.com'),

(4, 0, GETDATE(), 'FitnessJon', 'Jon', 'Nielsen', NULL, 27, 'Male', 'Norway', 'Oslo', 'Oslo', 'Norwegian',
 'Fitness, workouts, and healthy lifestyle.', NULL, 0, NULL, 0, 'tiktok.com/fitnessjon', 60000, NULL, 0, NULL, 0, '+4744444444', 'jon@mail.com'),

(5, 1, GETDATE(), 'BeautySara', 'Sara', 'Christensen', NULL, 23, 'Female', 'Denmark', 'Sjælland', 'Roskilde', 'Danish',
 'Beauty tutorials and skincare tips.', 'insta.com/sara', 70000, NULL, 0, NULL, 0, NULL, 0, NULL, 0, '+4555555555', 'sara@mail.com');


-- COMPANIES (5 companies)
INSERT INTO Companys (
    userId, isCompanyVerified, verificationDate, companyName, companyLogoUrl,
    ceoName, dateOfEstablishment, organisationNumber, standardIndustryClassification,
    websiteUrl, companyEmail, companyPhoneNumber, country, companyState, city,
    companyAddress, companyLanguage, biography, contactPerson, contactEmailAddress,
    contactPhoneNumber
)
VALUES
(6, 1, GETDATE(), 'NordicTech', NULL, 'Peter Jensen', '2010-01-01', 'NT12345', 1400,
 'nordictech.com', 'contact@nordictech.com', '+4588888888', 'Denmark', 'Hovedstaden', 'Copenhagen',
 'Tech Street 10', 'English', 'Leading Nordic tech firm.', 'Line Sørensen', 'line@nordictech.com', '+4544444444'),

(7, 0, GETDATE(), 'FreshSnacks', NULL, 'Maria Gustafsson', '2018-04-10', 'FS98765', 1100,
 'freshsnacks.com', 'info@freshsnacks.com', '+4677777777', 'Sweden', 'Stockholm', 'Stockholm',
 'Food Road 5', 'Swedish', 'Healthy snack company.', 'Jonas Berg', 'jonas@freshsnacks.com', '+4675555555'),

(8, 1, GETDATE(), 'EcoWear', NULL, 'Nina Rasmussen', '2015-06-12', 'EW54321', 1329,
 'ecowear.com', 'support@ecowear.com', '+4533333333', 'Denmark', 'Fyn', 'Odense',
 'Green Street 12', 'Danish', 'Sustainable clothing brand.', 'Olivia Madsen', 'olivia@ecowear.com', '+4577777777'),

(9, 0, GETDATE(), 'FitGear', NULL, 'Karl Olsson', '2012-09-20', 'FG11223', 285,
 'fitgear.com', 'help@fitgear.com', '+4699999999', 'Norway', 'Oslo', 'Oslo',
 'Energy Road 22', 'Norwegian', 'High-quality sports gear.', 'Erik Sørlie', 'erik@fitgear.com', '+4788888888'),

(10, 1, GETDATE(), 'BeautyBox', NULL, 'Emma Thomsen', '2017-11-15', 'BB73648', 780,
 'beautybox.com', 'service@beautybox.com', '+4566666666', 'Denmark', 'Hovedstaden', 'Copenhagen',
 'Glow Avenue 3', 'Danish', 'Cosmetics and beauty products.', 'Sofie Lauritsen', 'sofie@beautybox.com', '+4561111111');


-- COMPANY DOMAINS
INSERT INTO CompanyDomains (userId, domain) VALUES
(6, 'Fashion'), (6, 'Lifestyle'), (6, 'Beauty'),
(7, 'Technology'), (7, 'Gadgets'), (7, 'Gaming'),
(8, 'Food'), (8, 'Cooking'), (8, 'Health'),
(9, 'Fitness'), (9, 'Wellness'), (9, 'Motivation'),
(10, 'Beauty'), (10, 'Makeup'), (10, 'Skincare');

-- INFLUENCER DOMAINS
INSERT INTO InfluencerDomains (userId, domain) VALUES
(1, 'Fashion'), (1, 'Beauty'), (1, 'Lifestyle'),
(2, 'Technology'), (2, 'Gadgets'), (2, 'Software'),
(3, 'Food'), (3, 'Recipes'), (3, 'Health'),
(4, 'Fitness'), (4, 'Training'), (4, 'Motivation'),
(5, 'Beauty'), (5, 'Makeup'), (5, 'Skincare');


-- ANNOUNCEMENTS (5 announcements)
INSERT INTO Announcements (
    userId, title, creationDateTime, lastEditDateTime,
    startDisplayDateTime, endDisplayDateTime, currentApplicants, maximumApplicants,
    minimumFollowersRequired, communicationType, announcementlanguage, isKeepProducts,
    isPayoutNegotiable, totalPayoutAmount, shortDescriptionText,
    additionalInformationText, statusType, isVisible
)
VALUES
-- Announcement 1 (NordicTech - Technology)
(6, 'Review Our New Smart Device', GETDATE(), GETDATE(),
 '2025-08-20 09:00:00', DATEADD(DAY, 30, GETDATE()), 3, 10, 20000, 'Email', 'English',
 0, 1, 5000.00, 'We are seeking tech influencers to review a new smart device.',
 'You will receive the product for testing.', 'Active', 1),

-- Announcement 2 (FreshSnacks - Food)
(7, 'Healthy Snack Promotion', GETDATE(), GETDATE(),
 '2024-03-10 08:30:00', DATEADD(DAY, 20, GETDATE()), 3, 8, 0, 'Email', 'Swedish',
 1, 0, 0.00, 'Promote our new healthy snack box.',
 'Influencer keeps products after collaboration.', 'Active', 1),

-- Announcement 3 (EcoWear - Fashion)
(8, 'Eco-Friendly Clothing Campaign', GETDATE(), GETDATE(),
 '2025-01-25 09:00:00', DATEADD(DAY, 25, GETDATE()), 3, 12, 15000, 'Phone', 'Danish',
 1, 0, 0.00, 'Showcase our sustainable clothing line.',
 'Free clothing included.', 'Active', 1),

-- Announcement 4 (FitGear - Sports)
(9, 'Fitness Gear Review', GETDATE(), GETDATE(),
 '2024-06-15 07:30:00', DATEADD(DAY, 28, GETDATE()), 3, 6, 10000, 'Email', 'Norwegian',
 0, 1, 2500.00, 'Review our newest training equipment.',
 'Equipment must be returned unless otherwise agreed.', 'Active', 1),

-- Announcement 5 (BeautyBox - Beauty)
(10, 'Beauty Product Showcase', GETDATE(), GETDATE(),
 '2024-10-06 09:00:00', DATEADD(DAY, 35, GETDATE()), 2, 5, 12000, 'Email', 'Danish',
 1, 1, 3500.00, 'Showcase our new beauty product line.',
 'Products included for free.', 'Active', 1);



-- ANNOUNCEMENT SUBJECTS
INSERT INTO AnnouncementSubjects (announcementId, announcementSubject) VALUES
-- For NordicTech
(1, 'Technology'),
(1, 'Electronics'),
(1, 'Gadgets'),

-- For FreshSnacks
(2, 'Food'),
(2, 'Healthy Living'),
(2, 'Snacks'),

-- For EcoWear
(3, 'Fashion'),
(3, 'Sustainability'),
(3, 'Clothing'),

-- For FitGear
(4, 'Fitness'),
(4, 'Training'),
(4, 'Sports Equipment'),

-- For BeautyBox
(5, 'Beauty'),
(5, 'Skincare'),
(5, 'Cosmetics');


-- INFLUENCER ANNOUNCEMENTS (applications)
INSERT INTO InfluencerAnnouncements (userId, announcementId, applicationState) VALUES
-- For NordicTech (tech-oriented influencers)
(2, 1, 'Pending'),    -- TechTom
(4, 1, 'Pending'),    -- FitnessJon (relevant secondary)
(1, 1, 'Rejected'),   -- AnnaStyle (not tech, example)

-- For FreshSnacks (food)
(3, 2, 'Approved'),   -- FoodieMia
(1, 2, 'Pending'),    -- AnnaStyle
(5, 2, 'Pending'),    -- BeautySara (lifestyle/food adjacent)

-- For EcoWear (fashion)
(1, 3, 'Approved'),   -- AnnaStyle
(5, 3, 'Pending'),    -- BeautySara
(3, 3, 'Rejected'),   -- FoodieMia

-- For FitGear (sports)
(4, 4, 'Approved'),   -- FitnessJon
(2, 4, 'Pending'),    -- TechTom
(3, 4, 'Pending'),    -- FoodieMia (secondary relevance)

-- For BeautyBox (beauty)
(5, 5, 'Approved'),   -- BeautySara
(1, 5, 'Pending');    -- AnnaStyle