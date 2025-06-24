SELECT *
FROM Products
WHERE FarmerId IS NOT NULL AND FarmerId NOT IN (SELECT Id FROM Farmers);
DELETE FROM Products
WHERE FarmerId IS NOT NULL AND FarmerId NOT IN (SELECT Id FROM Farmers);

SELECT *
FROM Products
WHERE FarmerId IS NOT NULL
  AND FarmerId NOT IN (SELECT Id FROM Farmers);
  SELECT *
FROM Products
WHERE FarmerId IS NOT NULL
  AND FarmerId NOT IN (SELECT Id FROM Farmers);

  DELETE FROM Products
WHERE FarmerId IS NOT NULL
  AND FarmerId NOT IN (SELECT Id FROM Farmers);

  SELECT *
FROM sys.foreign_keys
WHERE name = 'FK_Products_Farmers_FarmerId';

SELECT * 
FROM Products
WHERE FarmerId IS NOT NULL 
  AND FarmerId NOT IN (SELECT Id FROM Farmers);

  DROP DATABASE AgriEnergyDB;

  DELETE FROM __EFMigrationsHistory;