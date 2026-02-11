/* =============================================
   Migration: Remove old Notification tables
   Description: Drops Notifications, UserNotifications, NotificationLogs tables
                since system now uses Notification.Deliveries table instead
   Created: February 2026
   ============================================= */

USE [ASP_LorKingDom]
GO

PRINT 'Starting migration: Remove old notification tables...'
GO

/* =============================================
   BACKUP EXISTING DATA (Optional - Run if needed)
   ============================================= */
-- SELECT * INTO Notifications_BACKUP_20260211 FROM [dbo].[Notifications];
-- SELECT * INTO UserNotifications_BACKUP_20260211 FROM [dbo].[UserNotifications];
-- SELECT * INTO NotificationLogs_BACKUP_20260211 FROM [dbo].[NotificationLogs];
GO

/* =============================================
   DROP OLD NOTIFICATION TABLES
   ============================================= */

-- Drop dependent tables first (foreign key constraints)
IF OBJECT_ID('[dbo].[UserNotifications]', 'U') IS NOT NULL
BEGIN
    PRINT 'Dropping table [dbo].[UserNotifications]...'
    DROP TABLE [dbo].[UserNotifications];
    PRINT '✓ Table [dbo].[UserNotifications] dropped successfully'
END
GO

IF OBJECT_ID('[dbo].[NotificationLogs]', 'U') IS NOT NULL
BEGIN
    PRINT 'Dropping table [dbo].[NotificationLogs]...'
    DROP TABLE [dbo].[NotificationLogs];
    PRINT '✓ Table [dbo].[NotificationLogs] dropped successfully'
END
GO

-- Drop main table last
IF OBJECT_ID('[dbo].[Notifications]', 'U') IS NOT NULL
BEGIN
    PRINT 'Dropping table [dbo].[Notifications]...'
    DROP TABLE [dbo].[Notifications];
    PRINT '✓ Table [dbo].[Notifications] dropped successfully'
END
GO

PRINT 'Migration completed successfully!'
PRINT 'Old notification tables removed. System now uses [Notification].[Deliveries] table.'
GO

/* =============================================
   VERIFICATION
   ============================================= */
-- Verify tables are dropped
IF OBJECT_ID('[dbo].[Notifications]', 'U') IS NULL
   AND OBJECT_ID('[dbo].[UserNotifications]', 'U') IS NULL
   AND OBJECT_ID('[dbo].[NotificationLogs]', 'U') IS NULL
BEGIN
    PRINT '✓ VERIFICATION PASSED: All old notification tables have been removed'
END
ELSE
BEGIN
    PRINT '✗ VERIFICATION FAILED: Some tables still exist'
END
GO

-- Verify Deliveries table exists
IF OBJECT_ID('[Notification].[Deliveries]', 'U') IS NOT NULL
BEGIN
    PRINT '✓ [Notification].[Deliveries] table exists and ready to use'
    
    -- Show table structure
    SELECT 
 '→ Deliveries table has ' + CAST(COUNT(*) AS VARCHAR) + ' columns' AS Info
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_SCHEMA = 'Notification' AND TABLE_NAME = 'Deliveries';
    
    -- Show current record count
    DECLARE @DeliveryCount INT;
    SELECT @DeliveryCount = COUNT(*) FROM [Notification].[Deliveries];
    PRINT '→ Current delivery records: ' + CAST(@DeliveryCount AS VARCHAR);
END
ELSE
BEGIN
    PRINT '✗ WARNING: [Notification].[Deliveries] table does not exist!'
    PRINT '  Please ensure your database schema includes this table.'
END
GO

/* =============================================
   ROLLBACK SCRIPT (IF NEEDED)
   ============================================= */
/*
-- If you need to restore the old tables from backup:

SELECT * INTO [dbo].[Notifications] FROM Notifications_BACKUP_20260211;
SELECT * INTO [dbo].[UserNotifications] FROM UserNotifications_BACKUP_20260211;
SELECT * INTO [dbo].[NotificationLogs] FROM NotificationLogs_BACKUP_20260211;

-- Recreate foreign keys and indexes as needed
*/
