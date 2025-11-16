USE master;

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'InfiniteInfluence')
BEGIN
    ALTER DATABASE InfiniteInfluence SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE InfiniteInfluence;
END
