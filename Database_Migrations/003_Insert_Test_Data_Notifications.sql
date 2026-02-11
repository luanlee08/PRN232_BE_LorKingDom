-- =============================================
-- Test Data for Notification System
-- Created: 2026-02-11
-- =============================================

USE ASP_LorKingDom;
GO

-- =============================================
-- 1. INSERT NOTIFICATION TEMPLATES
-- =============================================
PRINT 'Inserting Notification Templates...';

DECLARE @TemplateCount INT = 0;

-- Template 1: ORDER_CONFIRMED
IF NOT EXISTS (SELECT 1 FROM Notification.Templates WHERE TemplateCode = 'ORDER_CONFIRMED')
BEGIN
    INSERT INTO Notification.Templates (TemplateCode, TitleTemplate, MessageTemplate, IsActive, CreatedAt)
    VALUES 
    ('ORDER_CONFIRMED', 
     'Đơn hàng #{{orderCode}} đã được xác nhận', 
     'Chào {{customerName}}, đơn hàng #{{orderCode}} của bạn đã được xác nhận và đang được xử lý. Tổng giá trị: {{totalAmount}} VNĐ.',
     1, GETUTCDATE());
    SET @TemplateCount = @TemplateCount + 1;
END

-- Template 2: ORDER_SHIPPED
IF NOT EXISTS (SELECT 1 FROM Notification.Templates WHERE TemplateCode = 'ORDER_SHIPPED')
BEGIN
    INSERT INTO Notification.Templates (TemplateCode, TitleTemplate, MessageTemplate, IsActive, CreatedAt)
    VALUES 
    ('ORDER_SHIPPED', 
     'Đơn hàng #{{orderCode}} đang được giao', 
     'Đơn hàng #{{orderCode}} của bạn đã được giao cho đơn vị vận chuyển {{shippingUnit}}. Mã vận đơn: {{trackingCode}}',
     1, GETUTCDATE());
    SET @TemplateCount = @TemplateCount + 1;
END

-- Template 3: ORDER_DELIVERED
IF NOT EXISTS (SELECT 1 FROM Notification.Templates WHERE TemplateCode = 'ORDER_DELIVERED')
BEGIN
    INSERT INTO Notification.Templates (TemplateCode, TitleTemplate, MessageTemplate, IsActive, CreatedAt)
    VALUES 
    ('ORDER_DELIVERED', 
     'Đơn hàng #{{orderCode}} đã giao thành công', 
     'Đơn hàng #{{orderCode}} đã được giao thành công. Cảm ơn bạn đã mua hàng!',
     1, GETUTCDATE());
    SET @TemplateCount = @TemplateCount + 1;
END

-- Template 4: PROMOTION
IF NOT EXISTS (SELECT 1 FROM Notification.Templates WHERE TemplateCode = 'PROMOTION')
BEGIN
    INSERT INTO Notification.Templates (TemplateCode, TitleTemplate, MessageTemplate, IsActive, CreatedAt)
    VALUES 
    ('PROMOTION', 
     '🎉 Khuyến mãi: {{promotionName}}', 
     '{{description}} Áp dụng mã {{voucherCode}} để nhận giảm giá {{discount}}%. Có hiệu lực đến {{expireDate}}.',
     1, GETUTCDATE());
    SET @TemplateCount = @TemplateCount + 1;
END

-- Template 5: WELCOME
IF NOT EXISTS (SELECT 1 FROM Notification.Templates WHERE TemplateCode = 'WELCOME')
BEGIN
    INSERT INTO Notification.Templates (TemplateCode, TitleTemplate, MessageTemplate, IsActive, CreatedAt)
    VALUES 
    ('WELCOME', 
     'Chào mừng {{userName}} đến với LorKingDom!', 
     'Chúc mừng bạn đã tạo tài khoản thành công. Khám phá các sản phẩm tuyệt vời của chúng tôi ngay hôm nay!',
     1, GETUTCDATE());
    SET @TemplateCount = @TemplateCount + 1;
END

-- Template 6: PAYMENT_SUCCESS
IF NOT EXISTS (SELECT 1 FROM Notification.Templates WHERE TemplateCode = 'PAYMENT_SUCCESS')
BEGIN
    INSERT INTO Notification.Templates (TemplateCode, TitleTemplate, MessageTemplate, IsActive, CreatedAt)
    VALUES 
    ('PAYMENT_SUCCESS', 
     'Thanh toán thành công #{{paymentId}}', 
     'Giao dịch thanh toán {{amount}} VNĐ cho đơn hàng #{{orderCode}} đã được xử lý thành công.',
     1, GETUTCDATE());
    SET @TemplateCount = @TemplateCount + 1;
END

PRINT 'Inserted ' + CAST(@TemplateCount AS VARCHAR) + ' templates.';
GO

-- =============================================
-- 2. INSERT BACKGROUND JOBS (for testing scheduled notifications)
-- =============================================
PRINT 'Inserting Background Jobs...';

DECLARE @JobCount INT = 0;

-- Job 1: Daily Promotion Reminder
IF NOT EXISTS (SELECT 1 FROM System.BackgroundJobs WHERE JobName = 'Test: Daily Promotion Reminder')
BEGIN
    INSERT INTO System.BackgroundJobs (JobName, CronExpression, IsEnabled, LastRunTime, NextRunTime, LastRunStatus, LastRunMessage)
    VALUES 
    ('Test: Daily Promotion Reminder', '0 9 * * *', 1, GETUTCDATE(), DATEADD(DAY, 1, GETUTCDATE()), 'SUCCESS', 'Đã gửi 250 thông báo');
    SET @JobCount = @JobCount + 1;
END

-- Job 2: Weekly Newsletter
IF NOT EXISTS (SELECT 1 FROM System.BackgroundJobs WHERE JobName = 'Test: Weekly Newsletter')
BEGIN
    INSERT INTO System.BackgroundJobs (JobName, CronExpression, IsEnabled, LastRunTime, NextRunTime, LastRunStatus, LastRunMessage)
    VALUES 
    ('Test: Weekly Newsletter', '0 10 * * 1', 1, DATEADD(DAY, -7, GETUTCDATE()), DATEADD(DAY, 1, GETUTCDATE()), 'SUCCESS', 'Đã gửi 1500 thông báo');
    SET @JobCount = @JobCount + 1;
END

-- Job 3: Flash Sale Alert
IF NOT EXISTS (SELECT 1 FROM System.BackgroundJobs WHERE JobName = 'Test: Flash Sale Alert')
BEGIN
    INSERT INTO System.BackgroundJobs (JobName, CronExpression, IsEnabled, LastRunTime, NextRunTime, LastRunStatus, LastRunMessage)
    VALUES 
    ('Test: Flash Sale Alert', NULL, 1, GETUTCDATE(), NULL, 'PENDING', NULL);
    SET @JobCount = @JobCount + 1;
END

PRINT 'Inserted ' + CAST(@JobCount AS VARCHAR) + ' background jobs.';
GO

-- =============================================
-- 3. INSERT TEST DELIVERIES
-- =============================================
PRINT 'Inserting Test Deliveries...';

-- Get first 5 account IDs for testing
DECLARE @AccountId1 INT, @AccountId2 INT, @AccountId3 INT, @AccountId4 INT, @AccountId5 INT;

SELECT TOP 1 @AccountId1 = AccountId FROM dbo.Accounts ORDER BY AccountId;
SELECT @AccountId2 = AccountId FROM dbo.Accounts WHERE AccountId > @AccountId1 ORDER BY AccountId OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;
SELECT @AccountId3 = AccountId FROM dbo.Accounts WHERE AccountId > @AccountId2 ORDER BY AccountId OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;
SELECT @AccountId4 = AccountId FROM dbo.Accounts WHERE AccountId > @AccountId3 ORDER BY AccountId OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;
SELECT @AccountId5 = AccountId FROM dbo.Accounts WHERE AccountId > @AccountId4 ORDER BY AccountId OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;

-- Get JobId for testing
DECLARE @JobId1 INT, @JobId2 INT;
SELECT TOP 1 @JobId1 = JobId FROM System.BackgroundJobs WHERE JobName LIKE 'Test:%' ORDER BY JobId;
SELECT @JobId2 = JobId FROM System.BackgroundJobs WHERE JobId > @JobId1 ORDER BY JobId OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;

-- Insert test deliveries if accounts exist
IF @AccountId1 IS NOT NULL
BEGIN
    -- Delivery 1: ORDER_CONFIRMED (Unread, manual send)
    INSERT INTO Notification.Deliveries (AccountId, CreatedByJobId, TemplateCode, Title, Message, Payload, Status, CreatedAt)
    VALUES 
    (@AccountId1, NULL, 'ORDER_CONFIRMED', 
     'Đơn hàng #ORD20260211001 đã được xác nhận', 
     'Chào Nguyễn Văn A, đơn hàng #ORD20260211001 của bạn đã được xác nhận và đang được xử lý. Tổng giá trị: 2,500,000 VNĐ.',
     '{"orderCode":"ORD20260211001","customerName":"Nguyễn Văn A","totalAmount":"2,500,000"}',
     'Unread', GETUTCDATE());

    -- Delivery 2: ORDER_SHIPPED (Read, from background job)
    INSERT INTO Notification.Deliveries (AccountId, CreatedByJobId, TemplateCode, Title, Message, Payload, Status, CreatedAt)
    VALUES 
    (@AccountId1, @JobId1, 'ORDER_SHIPPED', 
     'Đơn hàng #ORD20260210050 đang được giao', 
     'Đơn hàng #ORD20260210050 của bạn đã được giao cho đơn vị vận chuyển Giao Hàng Nhanh. Mã vận đơn: GHN987654321',
     '{"orderCode":"ORD20260210050","shippingUnit":"Giao Hàng Nhanh","trackingCode":"GHN987654321"}',
     'Read', DATEADD(HOUR, -12, GETUTCDATE()));

    -- Delivery 3: PROMOTION (Unread)
    IF @AccountId2 IS NOT NULL
    BEGIN
        INSERT INTO Notification.Deliveries (AccountId, CreatedByJobId, TemplateCode, Title, Message, Payload, Status, CreatedAt)
        VALUES 
        (@AccountId2, @JobId2, 'PROMOTION', 
         '🎉 Khuyến mãi: Flash Sale 50% Cuối Tuần', 
         'Giảm giá cực sốc lên đến 50% cho tất cả sản phẩm đồ chơi. Áp dụng mã FLASHSALE50 để nhận giảm giá 50%. Có hiệu lực đến 31/12/2026.',
         '{"promotionName":"Flash Sale 50% Cuối Tuần","description":"Giảm giá cực sốc lên đến 50% cho tất cả sản phẩm đồ chơi","voucherCode":"FLASHSALE50","discount":"50","expireDate":"31/12/2026"}',
         'Unread', DATEADD(MINUTE, -30, GETUTCDATE()));
    END

    -- Delivery 4: WELCOME (Read)
    IF @AccountId3 IS NOT NULL
    BEGIN
        INSERT INTO Notification.Deliveries (AccountId, CreatedByJobId, TemplateCode, Title, Message, Payload, Status, CreatedAt)
        VALUES 
        (@AccountId3, NULL, 'WELCOME', 
         'Chào mừng user123 đến với LorKingDom!', 
         'Chúc mừng bạn đã tạo tài khoản thành công. Khám phá các sản phẩm tuyệt vời của chúng tôi ngay hôm nay!',
         '{"userName":"user123"}',
         'Read', DATEADD(DAY, -3, GETUTCDATE()));
    END

    -- Delivery 5: PAYMENT_SUCCESS (Unread)
    IF @AccountId4 IS NOT NULL
    BEGIN
        INSERT INTO Notification.Deliveries (AccountId, CreatedByJobId, TemplateCode, Title, Message, Payload, Status, CreatedAt)
        VALUES 
        (@AccountId4, NULL, 'PAYMENT_SUCCESS', 
         'Thanh toán thành công #PAY123456', 
         'Giao dịch thanh toán 1,200,000 VNĐ cho đơn hàng #ORD20260211002 đã được xử lý thành công.',
         '{"paymentId":"PAY123456","amount":"1,200,000","orderCode":"ORD20260211002"}',
         'Unread', DATEADD(MINUTE, -5, GETUTCDATE()));
    END

    -- Delivery 6: ORDER_DELIVERED (Read)
    IF @AccountId5 IS NOT NULL
    BEGIN
        INSERT INTO Notification.Deliveries (AccountId, CreatedByJobId, TemplateCode, Title, Message, Payload, Status, CreatedAt)
        VALUES 
        (@AccountId5, @JobId1, 'ORDER_DELIVERED', 
         'Đơn hàng #ORD20260209123 đã giao thành công', 
         'Đơn hàng #ORD20260209123 đã được giao thành công. Cảm ơn bạn đã mua hàng!',
         '{"orderCode":"ORD20260209123"}',
         'Read', DATEADD(DAY, -1, GETUTCDATE()));
    END

    -- Additional deliveries for the first account
    INSERT INTO Notification.Deliveries (AccountId, CreatedByJobId, TemplateCode, Title, Message, Payload, Status, CreatedAt)
    VALUES 
    (@AccountId1, @JobId2, 'PROMOTION', 
     '🎉 Khuyến mãi: Tết Nguyên Đán 2026', 
     'Mừng Tết Nguyên Đán - Giảm giá 30% toàn bộ đồ chơi LEGO. Áp dụng mã TET2026 để nhận giảm giá 30%. Có hiệu lực đến 15/02/2026.',
     '{"promotionName":Active = 1 THEN 1 ELSE 0 END) AS Active Tết Nguyên Đán - Giảm giá 30% toàn bộ đồ chơi LEGO","voucherCode":"TET2026","discount":"30","expireDate":"15/02/2026"}',
     'Unread', DATEADD(HOUR, -2, GETUTCDATE()));

    PRINT 'Inserted test deliveries for ' + CAST(COALESCE(@AccountId1, 0) AS VARCHAR) + ' and other accounts.';
END
ELSE
BEGIN
    PRINT 'WARNING: No accounts found. Skipping delivery inserts.';
END
GO

-- =============================================
-- 4. VERIFICATION QUERIES
-- =============================================
PRINT '======================================';
PRINT 'VERIFICATION RESULTS:';
PRINT '======================================';

PRINT 'Templates Count:';
SELECT COUNT(*) AS TotalTemplates, 
       SUM(CASE WHEN IsEnabled = 1 THEN 1 ELSE 0 END) AS EnabledTemplates
FROM Notification.Templates;

PRINT '';
PRINT 'Background Jobs Count:';
SELECT COUNT(*) AS TotalJobs,
       SUM(CASE WHEN IsEnabled = 1 THEN 1 ELSE 0 END) AS EnabledJobs
FROM System.BackgroundJobs;

PRINT '';
PRINT 'Deliveries Count:';
SELECT 
    COUNT(*) AS TotalDeliveries,
    SUM(CASE WHEN Status = 'Unread' THEN 1 ELSE 0 END) AS UnreadDeliveries,
    SUM(CASE WHEN Status = 'Read' THEN 1 ELSE 0 END) AS ReadDeliveries,
    COUNT(DISTINCT AccountId) AS UniqueRecipients,
    SUM(CASE WHEN CreatedByJobId IS NULL THEN 1 ELSE 0 END) AS ManualDeliveries,
    SUM(CASE WHEN CreatedByJobId IS NOT NULL THEN 1 ELSE 0 END) AS AutomatedDeliveries
FROM Notification.Deliveries;

PRINT '';
PRINT 'Recent Deliveries:';
SELECT TOP 5
    d.DeliveryId,
    a.Email AS Recipient,
    d.TemplateCode,
    d.Title,
    d.Status,
    CASE WHEN d.CreatedByJobId IS NULL THEN 'Manual' ELSE 'Automated (#' + CAST(d.CreatedByJobId AS VARCHAR) + ')' END AS Source,
    d.CreatedAt
FROM Notification.Deliveries d
INNER JOIN dbo.Accounts a ON d.AccountId = a.AccountId
ORDER BY d.CreatedAt DESC;

PRINT '';
PRINT '======================================';
PRINT 'TEST DATA INSERTION COMPLETED!';
PRINT '======================================';
GO
