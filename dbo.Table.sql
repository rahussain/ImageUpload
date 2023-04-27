CREATE TABLE [dbo].[ImageUpload]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [ImageName] NVARCHAR(200) NOT NULL, 
    [ImageNameonDisk] NVARCHAR(MAX) NOT NULL, 
    [IsDeleted] BIT NOT NULL, 
    [CreatedOnDate] DATETIME NULL, 
    [DeletedOnDate] DATETIME NULL
)
