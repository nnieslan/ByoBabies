USE [ByoBaby.Data]
GO

INSERT INTO [dbo].[People]
           ([UserId]
           ,[MemberSince]
           ,[UserProfile_Id])
     VALUES
           ('98BFBB36-0742-41F0-818F-FC217B2E5553'
           ,GETDATE()
           ,1)
GO


INSERT INTO [dbo].[Profiles]
           ([PersonId]
           ,[Email]
           ,[FirstName]
           ,[LastName]
           ,[MobilePhone]
           ,[HomePhone]
           ,[City]
           ,[Neighborhood]
           ,[LastUpdated])
     VALUES
           (1
           ,'nicknieslanik@gmail.com'
           ,'Nick'
           ,'Nieslanik'
           ,'720-939-9808'
           ,'720-939-9808'
           ,'Denver'
           ,'North Park Hill'
           ,GETDATE())
GO

