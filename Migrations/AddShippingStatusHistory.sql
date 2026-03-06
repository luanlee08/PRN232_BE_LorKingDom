-- ============================================================
-- Migration: AddShippingStatusHistory
-- Purpose  : Add GHN-level shipping status audit trail table
--            and extend ShippingProviderTransactions with
--            polling/concurrency columns.
-- Run on   : ASP_LorKingDom4 (or whatever DB is active)
-- ============================================================

-- -------------------------------------------------------
-- 1. Extend ShippingProviderTransactions
-- -------------------------------------------------------
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('ShippingProviderTransactions')
      AND name = 'LastPolledAt'
)
BEGIN
    ALTER TABLE ShippingProviderTransactions
        ADD LastPolledAt DATETIME2 NULL;
    PRINT 'Added LastPolledAt to ShippingProviderTransactions';
END

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('ShippingProviderTransactions')
      AND name = 'RetryCount'
)
BEGIN
    ALTER TABLE ShippingProviderTransactions
        ADD RetryCount INT NOT NULL DEFAULT 0;
    PRINT 'Added RetryCount to ShippingProviderTransactions';
END

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('ShippingProviderTransactions')
      AND name = 'LastErrorMessage'
)
BEGIN
    ALTER TABLE ShippingProviderTransactions
        ADD LastErrorMessage NVARCHAR(500) NULL;
    PRINT 'Added LastErrorMessage to ShippingProviderTransactions';
END

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('ShippingProviderTransactions')
      AND name = 'RowVersion'
)
BEGIN
    ALTER TABLE ShippingProviderTransactions
        ADD RowVersion ROWVERSION NOT NULL;
    PRINT 'Added RowVersion (optimistic concurrency) to ShippingProviderTransactions';
END

-- -------------------------------------------------------
-- 2. Filtered index for efficient polling queries
--    Covers: Provider + active Status + OrderId
-- -------------------------------------------------------
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID('ShippingProviderTransactions')
      AND name = 'IX_ShippingProviderTransactions_Polling'
)
BEGIN
    CREATE INDEX IX_ShippingProviderTransactions_Polling
        ON ShippingProviderTransactions (Provider, LastPolledAt, CreatedAt)
        INCLUDE (ProviderOrderCode, OrderID, Status, UpdatedAt)
        WHERE Status NOT IN ('delivered','returned','exception','cancel');
    PRINT 'Created IX_ShippingProviderTransactions_Polling';
END

-- -------------------------------------------------------
-- 3. Create ShippingStatusHistories table
--    Stores GHN-level provider status audit trail,
--    separate from OrderStatusHistories (which tracks
--    business-level order states).
-- -------------------------------------------------------
IF NOT EXISTS (
    SELECT 1 FROM sys.tables WHERE name = 'ShippingStatusHistories'
)
BEGIN
    CREATE TABLE ShippingStatusHistories (
        HistoryId        BIGINT IDENTITY(1,1)  NOT NULL,
        ShippingTxId     BIGINT                NOT NULL,
        OrderId          INT                   NOT NULL,
        PreviousStatus   NVARCHAR(50)          NOT NULL,
        NewStatus        NVARCHAR(50)          NOT NULL,
        -- 'Polling' | 'Webhook' | 'ManualSync' | 'Demo'
        Source           NVARCHAR(20)          NOT NULL DEFAULT 'Polling',
        RawPayload       NVARCHAR(MAX)         NULL,
        ProcessedAt      DATETIME2             NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_ShippingStatusHistories PRIMARY KEY CLUSTERED (HistoryId),

        CONSTRAINT FK_ShippingStatusHistories_ShippingTxn
            FOREIGN KEY (ShippingTxId)
            REFERENCES ShippingProviderTransactions (ShippingTransactionID)
            ON DELETE CASCADE,

        CONSTRAINT FK_ShippingStatusHistories_Orders
            FOREIGN KEY (OrderId)
            REFERENCES Orders (OrderID)
            ON DELETE NO ACTION
    );

    CREATE INDEX IX_ShippingStatusHistories_ShippingTxId
        ON ShippingStatusHistories (ShippingTxId)
        INCLUDE (NewStatus, ProcessedAt);

    CREATE INDEX IX_ShippingStatusHistories_OrderId
        ON ShippingStatusHistories (OrderId, ProcessedAt DESC);

    PRINT 'Created ShippingStatusHistories table with indexes';
END

PRINT '✅ Migration AddShippingStatusHistory completed successfully.';
