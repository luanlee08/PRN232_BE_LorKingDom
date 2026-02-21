-- Migration: Add ShippingDistrict column to Orders table
-- Date: 2026-02-21
-- Purpose: Enable GHN API address mapping (Province → District → Ward)

USE [AspLorKingDom]
GO

-- Add ShippingDistrict column
ALTER TABLE [dbo].[Orders]
ADD [ShippingDistrict] NVARCHAR(255) NULL;
GO

-- Optional: Update existing orders with placeholder
-- You can remove this if you want NULL for old orders
UPDATE [dbo].[Orders]
SET [ShippingDistrict] = N'Chưa cập nhật'
WHERE [ShippingDistrict] IS NULL 
  AND [IsDeleted] = 0;
GO

PRINT 'Successfully added ShippingDistrict column to Orders table';
