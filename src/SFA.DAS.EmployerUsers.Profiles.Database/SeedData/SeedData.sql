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

if NOT EXISTS(Select 1 FROM PasswordProfile WHERE [key] = 'RFpVdndIQk1FZHRHTWk2Q0NAdFJyRnJjajdzSnhb' and Id = 'b1fae38b-2325-4aa9-b0c3-3a31ef367210')
BEGIN
	INSERT INTO PasswordProfile
	(Id, [Key], WorkFactor, SaltLength, StorageLength)
	VALUES
	('b1fae38b-2325-4aa9-b0c3-3a31ef367210', 'RFpVdndIQk1FZHRHTWk2Q0NAdFJyRnJjajdzSnhb', 10000, 16, 256)
END