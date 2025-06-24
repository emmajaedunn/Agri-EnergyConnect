SELECT * 
FROM sys.indexes 
WHERE name = 'IX_Farmers_UserId';

DROP TABLE IF EXISTS Farmers;

SELECT 
    f.name AS ForeignKey,
    OBJECT_NAME(f.parent_object_id) AS ReferencingTable
FROM 
    sys.foreign_keys AS f
WHERE 
    f.referenced_object_id = OBJECT_ID('Farmers');
    ALTER TABLE Products
DROP CONSTRAINT FK_Products_Farmers_FarmerId;

DROP TABLE IF EXISTS Farmers;