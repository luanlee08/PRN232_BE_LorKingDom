-- ============================================================
-- Full Notification Template Migration
-- Covers ALL template codes referenced by system code.
-- Safe to run multiple times (IF NOT EXISTS guards).
-- ============================================================

-- ─────────────────────────────────────────────────────────────
-- 1. FIX EXISTING ROWS
-- ─────────────────────────────────────────────────────────────

-- Fix ORDER_CONFIRMED: remove {{customerName}} that is never passed by handlers
UPDATE [Notification].[Templates]
SET [MessageTemplate] = N'Đơn hàng #{{orderCode}} của bạn đã được xác nhận.'
WHERE [TemplateCode] = 'ORDER_CONFIRMED';

-- ─────────────────────────────────────────────────────────────
-- 2. ORDER LIFECYCLE (System-only, auto-triggered)
-- ─────────────────────────────────────────────────────────────

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'ORDER_CREATED')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'ORDER_CREATED',
        N'🛍️ Đặt hàng thành công #{{orderCode}}',
        N'Đơn hàng #{{orderCode}} trị giá {{totalAmount}} VNĐ đã được tạo thành công. Phương thức thanh toán: {{paymentMethod}}.'
    );

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'ORDER_CANCELLED')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'ORDER_CANCELLED',
        N'❌ Đơn hàng #{{orderCode}} đã bị hủy',
        N'Đơn hàng #{{orderCode}} đã bị hủy. Lý do: {{reason}}.'
    );

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'ORDER_COMPLETED')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'ORDER_COMPLETED',
        N'✅ Đơn hàng #{{orderCode}} hoàn thành',
        N'Đơn hàng #{{orderCode}} đã hoàn thành. Cảm ơn bạn đã mua sắm tại LorKingDom!'
    );

-- ─────────────────────────────────────────────────────────────
-- 3. PAYMENT (System-only, auto-triggered)
-- ─────────────────────────────────────────────────────────────

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'PAYMENT_FAILED')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'PAYMENT_FAILED',
        N'❗ Thanh toán thất bại #{{orderCode}}',
        N'Thanh toán {{amount}} VNĐ cho đơn hàng #{{orderCode}} không thành công. Vui lòng thử lại hoặc chọn phương thức khác.'
    );

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'PAYMENT_PENDING')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'PAYMENT_PENDING',
        N'⏳ Đơn hàng #{{orderCode}} đang chờ thanh toán',
        N'Đơn hàng #{{orderCode}} trị giá {{amount}} VNĐ đang chờ thanh toán. Hoàn tất trước {{expiresAt}}.'
    );

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'REFUND_SUCCESS')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'REFUND_SUCCESS',
        N'💰 Hoàn tiền thành công #{{orderCode}}',
        N'Giao dịch hoàn tiền {{refundAmount}} VNĐ cho đơn hàng #{{orderCode}} đã được xử lý thành công.'
    );

-- ─────────────────────────────────────────────────────────────
-- 4. GHN SHIPPING INTERMEDIATE STATUSES (System-only, auto-triggered)
-- ─────────────────────────────────────────────────────────────

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'SHIPPING_PICKING')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'SHIPPING_PICKING',
        N'📦 Đơn hàng đang được lấy hàng',
        N'Đơn hàng #{{orderId}} đang được người giao lấy hàng. Mã vận đơn: {{trackingCode}}.'
    );

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'SHIPPING_TRANSPORTING')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'SHIPPING_TRANSPORTING',
        N'🚚 Đơn hàng đang trên đường vận chuyển',
        N'Đơn hàng #{{orderId}} đang được vận chuyển. Mã vận đơn: {{trackingCode}}.'
    );

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'SHIPPING_DELIVERING')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'SHIPPING_DELIVERING',
        N'🏠 Shipper đang giao hàng đến bạn',
        N'Đơn hàng #{{orderId}} đang được giao đến địa chỉ của bạn. Mã vận đơn: {{trackingCode}}.'
    );

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'SHIPPING_RETURNING')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'SHIPPING_RETURNING',
        N'↩️ Đơn hàng đang được hoàn trả',
        N'Đơn hàng #{{orderId}} không giao được và đang được hoàn trả về kho. Mã vận đơn: {{trackingCode}}.'
    );

-- ─────────────────────────────────────────────────────────────
-- 5. REVIEW (System-only, auto-triggered by AI moderation)
-- ─────────────────────────────────────────────────────────────

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'REVIEW_REJECTED')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'REVIEW_REJECTED',
        N'⚠️ Đánh giá của bạn không được duyệt',
        N'Đánh giá sản phẩm "{{productName}}" của bạn bị từ chối. Lý do: {{reason}}.'
    );

-- ─────────────────────────────────────────────────────────────
-- 6. INVENTORY (System-only, Hangfire worker)
-- ─────────────────────────────────────────────────────────────

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'LOW_STOCK')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'LOW_STOCK',
        N'⚠️ Cảnh báo: Sản phẩm sắp hết hàng',
        N'Sản phẩm "{{productName}}" còn {{quantity}} sản phẩm trong kho. Vui lòng nhập thêm hàng.'
    );

-- ─────────────────────────────────────────────────────────────
-- 7. ADMIN-ALLOWED TEMPLATES (Manual, admin sends via dashboard)
-- ─────────────────────────────────────────────────────────────

-- Fallback for free-form admin messages (TemplateCode = CUSTOM)
IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'CUSTOM')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES ('CUSTOM', N'{{title}}', N'{{message}}');

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'SYSTEM_ANNOUNCEMENT')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'SYSTEM_ANNOUNCEMENT',
        N'📢 {{title}}',
        N'{{message}}'
    );

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'VOUCHER')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'VOUCHER',
        N'🎟️ Voucher mới: {{voucherCode}}',
        N'Bạn nhận được voucher <b>{{voucherCode}}</b> giảm {{discountAmount}}. Sử dụng trước {{expiresAt}}.'
    );

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'COUPON')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'COUPON',
        N'🏷️ Mã giảm giá: {{couponCode}}',
        N'Sử dụng mã <b>{{couponCode}}</b> để được giảm {{discountAmount}} cho đơn hàng tiếp theo. HSD: {{expiresAt}}.'
    );

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'PRODUCT_ANNOUNCEMENT')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'PRODUCT_ANNOUNCEMENT',
        N'🛍️ {{title}}',
        N'{{message}}'
    );

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'BLOG_ANNOUNCEMENT')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'BLOG_ANNOUNCEMENT',
        N'📝 {{title}}',
        N'{{message}}'
    );

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'CONTENT_ANNOUNCEMENT')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'CONTENT_ANNOUNCEMENT',
        N'📣 {{title}}',
        N'{{message}}'
    );

IF NOT EXISTS (SELECT 1 FROM [Notification].[Templates] WHERE [TemplateCode] = 'CUSTOMER_SUPPORT')
    INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (
        'CUSTOMER_SUPPORT',
        N'🎧 Hỗ trợ khách hàng: {{title}}',
        N'{{message}}'
    );
