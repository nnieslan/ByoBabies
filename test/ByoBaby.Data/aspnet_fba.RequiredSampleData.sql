-- =============================================
-- Script Template
-- =============================================
/* Load [dbo].[aspnet_SchemaVersions] */
IF EXISTS(SELECT 1 FROM [dbo].[aspnet_SchemaVersions])
BEGIN
    PRINT '[dbo].[aspnet_SchemaVersions] records exist. Bypassing record creation.'
END
ELSE
BEGIN
    PRINT 'Loading [dbo].[aspnet_SchemaVersions]';

    INSERT INTO [dbo].[aspnet_SchemaVersions] ([Feature], [CompatibleSchemaVersion], [IsCurrentVersion])
    SELECT 'common', N'1', 1
    UNION ALL
    SELECT 'health monitoring', N'1', 1
    UNION ALL
    SELECT 'membership', N'1', 1
    UNION ALL
    SELECT 'personalization', N'1', 1
    UNION ALL
    SELECT 'profile', N'1', 1
    UNION ALL
    SELECT 'role manager', N'1', 1
END;
GO

/* Load [dbo].[SecurityRole] 
IF EXISTS(SELECT 1 FROM [dbo].[SecurityRole])
BEGIN
    PRINT '[dbo].[SecurityRole] records exist. Bypassing record creation.'
END
ELSE
BEGIN
    PRINT 'Loading [dbo].[SecurityRole]';

    SET IDENTITY_INSERT [dbo].[SecurityRole] ON;

    INSERT INTO [dbo].[SecurityRole] ([SecurityRoleId], [Name])
    SELECT 1, N'SystemAdministrator'
    UNION ALL
    SELECT 2, N'OrganizationAdministrator'
    UNION ALL
    SELECT 3, N'OrganizationMember'

    SET IDENTITY_INSERT [dbo].[SecurityRole] OFF;
END;
GO
*/

IF EXISTS(SELECT 1 FROM [dbo].[aspnet_Applications])
BEGIN
    PRINT '[dbo].[aspnet_Applications] records exist - updating master application record.'
	/* This change allows for upgrade of databases that used a different app id */
	UPDATE [dbo].[aspnet_Applications] SET ApplicationId='1456D370-0210-44DA-B508-18F6ED920398' WHERE ApplicationName='/'
END
ELSE
BEGIN
    PRINT 'Loading [dbo].[aspnet_Applications]';

	INSERT INTO [dbo].[aspnet_Applications] (ApplicationName, LoweredApplicationName, ApplicationId, Description)
	SELECT '/', '/', '1456D370-0210-44DA-B508-18F6ED920398', NULL
END;
GO

IF EXISTS(SELECT 1 FROM [dbo].[aspnet_Users])
BEGIN
    PRINT '[dbo].[aspnet_Users] records exist. Bypassing record creation.'
END
ELSE
BEGIN
    PRINT 'Loading [dbo].[aspnet_Users]';

	INSERT INTO [dbo].[aspnet_Users] (ApplicationId, UserId, UserName, LoweredUserName, IsAnonymous, LastActivityDate)
	SELECT '1456D370-0210-44DA-B508-18F6ED920398', '1023CD80-38E7-4D29-9430-CFDA844BFB08', 'admin', 'admin', 0, GETDATE()
END;
GO

IF EXISTS(SELECT 1 FROM [dbo].[aspnet_Membership])
BEGIN
    PRINT '[dbo].[aspnet_Membership] records exist. Bypassing record creation.'
END
ELSE
BEGIN
    PRINT 'Loading [dbo].[aspnet_Membership]';

	INSERT INTO [dbo].[aspnet_Membership] (ApplicationId, UserId, Password, PasswordFormat, PasswordSalt, Email, LoweredEmail, IsApproved, IsLockedOut, CreateDate, LastLoginDate, LastPasswordChangedDate, LastLockoutDate, FailedPasswordAttemptCount, FailedPasswordAttemptWindowStart, FailedPasswordAnswerAttemptCount, FailedPasswordAnswerAttemptWindowStart)
	SELECT '1456D370-0210-44DA-B508-18F6ED920398', '1023CD80-38E7-4D29-9430-CFDA844BFB08', '+PGIQ1btkif1eEWrawk8RXANZpQ=', 1, 'xv6aBEZJ7+o44cnL4Tpqbw==', 'admin@byobabies.com', 'admin@byobabies.com', 1, 0, GETDATE(), '1754-01-01 00:00:00.000', '1754-01-01 00:00:00.000', '1754-01-01 00:00:00.000', 0, '1754-01-01 00:00:00.000', 0, '1754-01-01 00:00:00.000'
END;
GO

PRINT 'Loading [dbo].[aspnet_Users] with sample data';
	INSERT INTO [dbo].[aspnet_Users] (ApplicationId, UserId, UserName, LoweredUserName, IsAnonymous, LastActivityDate)
	SELECT '1456D370-0210-44DA-B508-18F6ED920398', '98BFBB36-0742-41F0-818F-FC217B2E5553', 'nicknieslanik@gmail.com', 'nicknieslanik@gmail.com', 0, GETDATE()
/* Seed test users */

PRINT 'Loading [dbo].[aspnet_Membership] with sample data';

INSERT INTO [dbo].[aspnet_Membership] (ApplicationId, UserId, Password, PasswordFormat, PasswordSalt, Email, LoweredEmail, IsApproved, IsLockedOut, CreateDate, LastLoginDate, LastPasswordChangedDate, LastLockoutDate, FailedPasswordAttemptCount, FailedPasswordAttemptWindowStart, FailedPasswordAnswerAttemptCount, FailedPasswordAnswerAttemptWindowStart)
SELECT '1456D370-0210-44DA-B508-18F6ED920398', '98BFBB36-0742-41F0-818F-FC217B2E5553', '+PGIQ1btkif1eEWrawk8RXANZpQ=', 1, 'xv6aBEZJ7+o44cnL4Tpqbw==', 'nicknieslanik@gmail.com', 'nicknieslanik@gmail.com', 1, 0, GETDATE(), '1754-01-01 00:00:00.000', '1754-01-01 00:00:00.000', '1754-01-01 00:00:00.000', 0, '1754-01-01 00:00:00.000', 0, '1754-01-01 00:00:00.000'
