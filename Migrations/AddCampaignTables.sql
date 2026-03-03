-- =====================================================================
-- Migration: AddCampaignTables
-- Date: 2026-03-03
-- Adds: Notification.Campaigns, Notification.CampaignTargets,
--       Notification.DeliveryActions
-- Alters: Notification.Deliveries (adds CampaignID nullable FK)
-- =====================================================================

-- ─────────────────────────────────────────────────────────────────────
-- 1. Notification.Campaigns
-- ─────────────────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES
               WHERE TABLE_SCHEMA = 'Notification' AND TABLE_NAME = 'Campaigns')
BEGIN
    CREATE TABLE [Notification].[Campaigns] (
        [CampaignID]          INT             NOT NULL IDENTITY(1,1),
        [CampaignName]        NVARCHAR(255)   NOT NULL,
        [TemplateCode]        VARCHAR(50)     NULL,
        [TitleOverride]       NVARCHAR(255)   NULL,
        [MessageOverride]     NVARCHAR(500)   NULL,
        [SourceType]          VARCHAR(10)     NOT NULL DEFAULT 'ADMIN',    -- 'ADMIN' | 'SYSTEM' | 'WORKER'
        [TargetType]          VARCHAR(10)     NOT NULL DEFAULT 'ALL',      -- 'ALL' | 'GROUP' | 'CUSTOM' | 'SINGLE'
        [Status]              VARCHAR(15)     NOT NULL DEFAULT 'Draft',    -- 'Draft' | 'Scheduled' | 'Processing' | 'Completed' | 'Failed'
        [ScheduledAt]         DATETIME2(0)    NULL,
        [EventKey]            VARCHAR(100)    NULL,
        [ImageUrl]            NVARCHAR(500)   NULL,
        [ActionType]          NVARCHAR(20)    NULL,
        [ActionTarget]        NVARCHAR(500)   NULL,
        [CreatedByAccountID]  INT             NOT NULL,
        [CreatedAt]           DATETIME2(0)    NOT NULL DEFAULT GETDATE(),
        [UpdatedAt]           DATETIME2(0)    NULL,

        CONSTRAINT [PK_Campaigns] PRIMARY KEY CLUSTERED ([CampaignID]),

        CONSTRAINT [FK_Campaigns_Accounts]
            FOREIGN KEY ([CreatedByAccountID]) REFERENCES [dbo].[Accounts]([AccountID]),

        CONSTRAINT [FK_Campaigns_Templates]
            FOREIGN KEY ([TemplateCode]) REFERENCES [Notification].[Templates]([TemplateCode])
            ON DELETE SET NULL
    );

    PRINT 'Created Notification.Campaigns';
END
ELSE
    PRINT 'Notification.Campaigns already exists – skipped';


-- ─────────────────────────────────────────────────────────────────────
-- 2. Notification.CampaignTargets
-- ─────────────────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES
               WHERE TABLE_SCHEMA = 'Notification' AND TABLE_NAME = 'CampaignTargets')
BEGIN
    CREATE TABLE [Notification].[CampaignTargets] (
        [CampaignTargetID]  INT             NOT NULL IDENTITY(1,1),
        [CampaignID]        INT             NOT NULL,
        [TargetValue]       VARCHAR(200)    NOT NULL,   -- AccountId, GroupCode, or "ALL"

        CONSTRAINT [PK_CampaignTargets] PRIMARY KEY CLUSTERED ([CampaignTargetID]),

        CONSTRAINT [FK_CampaignTargets_Campaigns]
            FOREIGN KEY ([CampaignID]) REFERENCES [Notification].[Campaigns]([CampaignID])
            ON DELETE CASCADE
    );

    PRINT 'Created Notification.CampaignTargets';
END
ELSE
    PRINT 'Notification.CampaignTargets already exists – skipped';


-- ─────────────────────────────────────────────────────────────────────
-- 3. Alter Notification.Deliveries — add CampaignID nullable FK
-- ─────────────────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_SCHEMA = 'Notification'
                 AND TABLE_NAME   = 'Deliveries'
                 AND COLUMN_NAME  = 'CampaignID')
BEGIN
    ALTER TABLE [Notification].[Deliveries]
        ADD [CampaignID] INT NULL;

    ALTER TABLE [Notification].[Deliveries]
        ADD CONSTRAINT [FK_NotificationDeliveries_Campaigns]
            FOREIGN KEY ([CampaignID]) REFERENCES [Notification].[Campaigns]([CampaignID])
            ON DELETE SET NULL;

    -- Helps analytics queries filter deliveries by campaign
    CREATE NONCLUSTERED INDEX [IX_Deliveries_CampaignID]
        ON [Notification].[Deliveries] ([CampaignID])
        WHERE [CampaignID] IS NOT NULL;

    PRINT 'Added CampaignID to Notification.Deliveries';
END
ELSE
    PRINT 'Notification.Deliveries.CampaignID already exists – skipped';


-- ─────────────────────────────────────────────────────────────────────
-- 4. Notification.DeliveryActions  (click/read event log)
-- ─────────────────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES
               WHERE TABLE_SCHEMA = 'Notification' AND TABLE_NAME = 'DeliveryActions')
BEGIN
    CREATE TABLE [Notification].[DeliveryActions] (
        [ActionID]      BIGINT          NOT NULL IDENTITY(1,1),
        [DeliveryID]    BIGINT          NOT NULL,
        [AccountID]     INT             NOT NULL,
        [ActionType]    VARCHAR(10)     NOT NULL,   -- 'Read' | 'Click'
        [ActionTarget]  NVARCHAR(500)   NULL,       -- URL/product/voucher clicked
        [OccurredAt]    DATETIME2(0)    NOT NULL DEFAULT GETDATE(),

        CONSTRAINT [PK_DeliveryActions] PRIMARY KEY CLUSTERED ([ActionID]),

        CONSTRAINT [FK_DeliveryActions_Deliveries]
            FOREIGN KEY ([DeliveryID]) REFERENCES [Notification].[Deliveries]([DeliveryID])
            ON DELETE CASCADE,

        CONSTRAINT [FK_DeliveryActions_Accounts]
            FOREIGN KEY ([AccountID]) REFERENCES [dbo].[Accounts]([AccountID])
    );

    CREATE NONCLUSTERED INDEX [IX_DeliveryActions_DeliveryID]
        ON [Notification].[DeliveryActions] ([DeliveryID]);

    CREATE NONCLUSTERED INDEX [IX_DeliveryActions_AccountID_ActionType]
        ON [Notification].[DeliveryActions] ([AccountID], [ActionType])
        INCLUDE ([OccurredAt]);

    PRINT 'Created Notification.DeliveryActions';
END
ELSE
    PRINT 'Notification.DeliveryActions already exists – skipped';


PRINT '=== Migration AddCampaignTables complete ===';
