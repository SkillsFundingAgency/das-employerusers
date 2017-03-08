/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


update RelyingParty set LoginCallbackUrl = ApplicationUrl + 'service/signin/' where LoginCallbackUrl is null and id <> 'testrp'

update RelyingParty set LoginCallbackUrl = ApplicationUrl + 'home/login/' where id = 'testrp'