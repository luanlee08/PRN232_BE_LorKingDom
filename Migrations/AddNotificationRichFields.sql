-- Migration: Add rich fields to Deliveries table for the redesigned notification system
-- Run against the Notification schema
-- Date: 2026-03-03

USE [LorKingDom]; -- adjust to your DB name if different

-- 1. Add new columns to Notification.Deliveries
ALTER TABLE [Notification].[Deliveries]
ADD
    [ImageUrl]     NVARCHAR(500)  NULL,
    [ActionType]   NVARCHAR(20)   NULL,   -- 'product' | 'voucher' | 'url' | 'none'
    [ActionTarget] NVARCHAR(500)  NULL;   -- productId, voucherCode, or full URL

-- 2. Verify columns were added
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'Notification'
  AND TABLE_NAME   = 'Deliveries'
  AND COLUMN_NAME  IN ('ImageUrl', 'ActionType', 'ActionTarget');
