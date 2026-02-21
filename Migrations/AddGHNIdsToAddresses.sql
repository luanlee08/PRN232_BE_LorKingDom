-- =============================================
-- Migration: Add GHN Master Data IDs to Address & Order Tables
-- Purpose: Store GHN district_id and ward_code for reliable shipping integration
-- Date: 2026-02-21
-- =============================================

-- Add GHN columns to Addresses table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Addresses]') AND name = 'DistrictId')
BEGIN
    ALTER TABLE [dbo].[Addresses]
    ADD [DistrictId] INT NULL;
    
    PRINT '✅ Added DistrictId column to Addresses table';
END
ELSE
BEGIN
    PRINT '⚠️ DistrictId column already exists in Addresses table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Addresses]') AND name = 'WardCode')
BEGIN
    ALTER TABLE [dbo].[Addresses]
    ADD [WardCode] VARCHAR(20) NULL;
    
    PRINT '✅ Added WardCode column to Addresses table';
END
ELSE
BEGIN
    PRINT '⚠️ WardCode column already exists in Addresses table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Addresses]') AND name = 'ProvinceId')
BEGIN
    ALTER TABLE [dbo].[Addresses]
    ADD [ProvinceId] INT NULL;
    
    PRINT '✅ Added ProvinceId column to Addresses table';
END
ELSE
BEGIN
    PRINT '⚠️ ProvinceId column already exists in Addresses table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Addresses]') AND name = 'RecipientName')
BEGIN
    ALTER TABLE [dbo].[Addresses]
    ADD [RecipientName] NVARCHAR(100) NULL;
    
    PRINT '✅ Added RecipientName column to Addresses table';
END
ELSE
BEGIN
    PRINT '⚠️ RecipientName column already exists in Addresses table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Addresses]') AND name = 'PhoneNumber')
BEGIN
    ALTER TABLE [dbo].[Addresses]
    ADD [PhoneNumber] VARCHAR(15) NULL;
    
    PRINT '✅ Added PhoneNumber column to Addresses table';
END
ELSE
BEGIN
    PRINT '⚠️ PhoneNumber column already exists in Addresses table';
END

-- Add GHN columns to Orders table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'ShippingDistrictId')
BEGIN
    ALTER TABLE [dbo].[Orders]
    ADD [ShippingDistrictId] INT NULL;
    
    PRINT '✅ Added ShippingDistrictId column to Orders table';
END
ELSE
BEGIN
    PRINT '⚠️ ShippingDistrictId column already exists in Orders table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'ShippingWardCode')
BEGIN
    ALTER TABLE [dbo].[Orders]
    ADD [ShippingWardCode] VARCHAR(20) NULL;
    
    PRINT '✅ Added ShippingWardCode column to Orders table';
END
ELSE
BEGIN
    PRINT '⚠️ ShippingWardCode column already exists in Orders table';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'ShippingProvinceId')
BEGIN
    ALTER TABLE [dbo].[Orders]
    ADD [ShippingProvinceId] INT NULL;
    
    PRINT '✅ Added ShippingProvinceId column to Orders table';
END
ELSE
BEGIN
    PRINT '⚠️ ShippingProvinceId column already exists in Orders table';
END

-- Create indexes for better query performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Addresses_DistrictId' AND object_id = OBJECT_ID(N'[dbo].[Addresses]'))
BEGIN
    CREATE INDEX IX_Addresses_DistrictId ON [dbo].[Addresses]([DistrictId])
    WHERE [DistrictId] IS NOT NULL;
    
    PRINT '✅ Created index IX_Addresses_DistrictId';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_ShippingDistrictId' AND object_id = OBJECT_ID(N'[dbo].[Orders]'))
BEGIN
    CREATE INDEX IX_Orders_ShippingDistrictId ON [dbo].[Orders]([ShippingDistrictId])
    WHERE [ShippingDistrictId] IS NOT NULL;
    
    PRINT '✅ Created index IX_Orders_ShippingDistrictId';
END

PRINT '';
PRINT '🎉 Migration completed successfully!';
PRINT '';
PRINT '📝 Next Steps:';
PRINT '1. Run EF Core scaffold to update models (or manually add properties)';
PRINT '2. Update DTOs to accept/return GHN IDs';
PRINT '3. Implement LocationService with Redis caching';
PRINT '4. Update frontend to send GHN IDs along with text names';
PRINT '';
PRINT '⚠️ Legacy Data: Existing addresses will have NULL values.';
PRINT '   Use the backfill script or implement best-effort mapping on first use.';
