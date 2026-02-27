-- ============================================================
-- TEST DATA SCRIPT — Orders & Related Entities
-- Database : AspLorKingDom
-- Run BEFORE: AddShippingDistrictToOrders.sql
--             AddGHNIdsToAddresses.sql
-- Safe to re-run (uses IF NOT EXISTS / MERGE guards)
-- ============================================================

USE ASP_LorKingDom3;
GO

-- ============================================================
-- 0. MIGRATION COLUMNS — already included in CreateDB-2502.sql
--    (ShippingDistrict, ShippingDistrictId, ShippingWardCode,
--     ShippingProvinceId are part of the Orders table definition)
-- ============================================================
GO

-- ============================================================
-- 1. STATUS ORDERS (Pending→Refunded)
-- ============================================================
SET IDENTITY_INSERT StatusOrders ON;

IF NOT EXISTS (SELECT 1 FROM StatusOrders WHERE StatusId = 1)
    INSERT INTO StatusOrders (StatusId, StatusName, Description, CreatedAt)
    VALUES (1, N'Pending', N'Đơn hàng chờ xác nhận', GETDATE());

IF NOT EXISTS (SELECT 1 FROM StatusOrders WHERE StatusId = 2)
    INSERT INTO StatusOrders (StatusId, StatusName, Description, CreatedAt)
    VALUES (2, N'Confirmed', N'Đã xác nhận', GETDATE());

IF NOT EXISTS (SELECT 1 FROM StatusOrders WHERE StatusId = 3)
    INSERT INTO StatusOrders (StatusId, StatusName, Description, CreatedAt)
    VALUES (3, N'Shipped', N'Đang giao hàng', GETDATE());

IF NOT EXISTS (SELECT 1 FROM StatusOrders WHERE StatusId = 4)
    INSERT INTO StatusOrders (StatusId, StatusName, Description, CreatedAt)
    VALUES (4, N'Delivered', N'Đã giao thành công', GETDATE());

IF NOT EXISTS (SELECT 1 FROM StatusOrders WHERE StatusId = 5)
    INSERT INTO StatusOrders (StatusId, StatusName, Description, CreatedAt)
    VALUES (5, N'Completed', N'Đơn hàng hoàn tất', GETDATE());

IF NOT EXISTS (SELECT 1 FROM StatusOrders WHERE StatusId = 6)
    INSERT INTO StatusOrders (StatusId, StatusName, Description, CreatedAt)
    VALUES (6, N'Cancelled', N'Đã huỷ', GETDATE());

IF NOT EXISTS (SELECT 1 FROM StatusOrders WHERE StatusId = 7)
    INSERT INTO StatusOrders (StatusId, StatusName, Description, CreatedAt)
    VALUES (7, N'Refunded', N'Đã hoàn tiền', GETDATE());

SET IDENTITY_INSERT StatusOrders OFF;
GO

-- ============================================================
-- 2. ROLES (already seeded: 1=Customer, 2=Staff, 3=Warehouse, 4=Admin)
-- ============================================================
-- No insert needed — roles already exist in the database.
GO

-- ============================================================
-- 4. VOUCHER TYPE & TEST VOUCHERS
-- VoucherTypes already seeded: 1=Order, 2=Shipping
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Voucher WHERE VoucherCode = 'TESTDISCOUNT10')
    INSERT INTO Voucher (VoucherTypeId, CreateBy, VoucherCode, DiscountType, DiscountValue,
                         MaxDiscountAmount, MinOrderAmount, UsageLimitPerUser, IsStackable,
                         StartDate, EndDate, Status, IsDeleted, CreatedAt)
    VALUES (1, NULL, 'TESTDISCOUNT10', 'Percentage', 10, 100000, 200000, 5, 0,
            DATEADD(MONTH, -1, GETDATE()), DATEADD(MONTH, 6, GETDATE()), 'Active', 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Voucher WHERE VoucherCode = 'TESTFLAT50K')
    INSERT INTO Voucher (VoucherTypeId, CreateBy, VoucherCode, DiscountType, DiscountValue,
                         MaxDiscountAmount, MinOrderAmount, UsageLimitPerUser, IsStackable,
                         StartDate, EndDate, Status, IsDeleted, CreatedAt)
    VALUES (1, NULL, 'TESTFLAT50K', 'Fixed', 50000, NULL, 150000, 3, 0,
            DATEADD(MONTH, -1, GETDATE()), DATEADD(MONTH, 3, GETDATE()), 'Active', 0, GETDATE());
GO

-- ============================================================
-- 5. TEST PRODUCTS (insert only if table is empty or product absent)
-- ============================================================
-- Category / Brand / Material / Age / Sex / PriceRange / Origin must exist first
SET IDENTITY_INSERT SuperCategories ON;
IF NOT EXISTS (SELECT 1 FROM SuperCategories WHERE SuperCategoryId = 1)
    INSERT INTO SuperCategories (SuperCategoryId, SuperCategoryName, CreatedAt)
    VALUES (1, N'Đồ chơi & Giải trí', GETDATE());
SET IDENTITY_INSERT SuperCategories OFF;

SET IDENTITY_INSERT Categories ON;
IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryId = 1)
    INSERT INTO Categories (CategoryId, SuperCategoryId, CategoryName, IsDeleted, CreatedAt)
    VALUES (1, 1, N'Đồ chơi lắp ráp', 0, GETDATE());
SET IDENTITY_INSERT Categories OFF;

SET IDENTITY_INSERT Brands ON;
IF NOT EXISTS (SELECT 1 FROM Brands WHERE BrandId = 1)
    INSERT INTO Brands (BrandId, BrandName, IsDeleted, CreatedAt)
    VALUES (1, N'Lego', 0, GETDATE());
SET IDENTITY_INSERT Brands OFF;

IF NOT EXISTS (SELECT 1 FROM Products WHERE Sku = 'TEST-SKU-001')
    INSERT INTO Products (Sku, CategoryId, BrandId, ProductName, Price, Quantity, ProductStatus, IsDeleted, CreatedAt)
    VALUES ('TEST-SKU-001', 1, 1, N'Bộ Lego Thành Phố Mini', 350000, 100, 'Available', 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Products WHERE Sku = 'TEST-SKU-002')
    INSERT INTO Products (Sku, CategoryId, BrandId, ProductName, Price, Quantity, ProductStatus, IsDeleted, CreatedAt)
    VALUES ('TEST-SKU-002', 1, 1, N'Lego Xe Cảnh Sát', 250000, 50, 'Available', 0, GETDATE());

IF NOT EXISTS (SELECT 1 FROM Products WHERE Sku = 'TEST-SKU-003')
    INSERT INTO Products (Sku, CategoryId, BrandId, ProductName, Price, Quantity, ProductStatus, IsDeleted, CreatedAt)
    VALUES ('TEST-SKU-003', 1, 1, N'Lego Phi Thuyền Vũ Trụ', 580000, 30, 'Available', 0, GETDATE());
GO

-- ============================================================
-- 6. INSERT TEST ORDERS  (9 orders covering all statuses)
-- ============================================================
DECLARE @custId   INT = (SELECT TOP 1 AccountId FROM Accounts WHERE Email = 'vuquangduc1404@gmail.com');
DECLARE @vId10    INT = (SELECT TOP 1 VoucherId  FROM Voucher  WHERE VoucherCode = 'TESTDISCOUNT10');
DECLARE @vId50k   INT = (SELECT TOP 1 VoucherId  FROM Voucher  WHERE VoucherCode = 'TESTFLAT50K');
DECLARE @p1       INT = (SELECT TOP 1 ProductId  FROM Products  WHERE Sku = 'TEST-SKU-001');
DECLARE @p2       INT = (SELECT TOP 1 ProductId  FROM Products  WHERE Sku = 'TEST-SKU-002');
DECLARE @p3       INT = (SELECT TOP 1 ProductId  FROM Products  WHERE Sku = 'TEST-SKU-003');

-- ── Order 1 · Pending
IF NOT EXISTS (SELECT 1 FROM Orders WHERE ShippingPhone = '0901234567' AND StatusId = 1 AND ShippingName = N'Order-Test-01')
BEGIN
    INSERT INTO Orders (AccountId, VoucherId, StatusId, ShippingName, ShippingPhone,
                        ShippingAddressLine, ShippingCity, ShippingDistrict, ShippingWard,
                        ShippingMethod, ShippingFee, OrderDate, TotalAmount,
                        PaidByWalletAmount, PaidByExternalAmount, RefundStatus, IsDeleted, CreatedAt)
    VALUES (@custId, NULL, 1, N'Order-Test-01', '0901234567',
            N'123 Đường Lê Lợi', N'Hồ Chí Minh', N'Quận 1', N'Phường Bến Nghé',
            N'GHN', 30000, DATEADD(DAY, -7, GETDATE()), 730000,
            0, 730000, 'None', 0, DATEADD(DAY, -7, GETDATE()));
    DECLARE @o1 INT = SCOPE_IDENTITY();
    INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, IsDeleted, Reviewed, CreatedAt)
    VALUES (@o1, @p1, 2, 350000, 0, 0, GETDATE());
    INSERT INTO OrderStatusHistory (OrderId, StatusId, ChangedAt, Note, CreatedAt)
    VALUES (@o1, 1, DATEADD(DAY, -7, GETDATE()), N'Khách hàng đặt hàng', GETDATE());
END

-- ── Order 2 · Processing
IF NOT EXISTS (SELECT 1 FROM Orders WHERE ShippingPhone = '0901234567' AND StatusId = 2 AND ShippingName = N'Order-Test-02')
BEGIN
    INSERT INTO Orders (AccountId, VoucherId, StatusId, ShippingName, ShippingPhone,
                        ShippingAddressLine, ShippingCity, ShippingDistrict, ShippingWard,
                        ShippingMethod, ShippingFee, OrderDate, TotalAmount,
                        PaidByWalletAmount, PaidByExternalAmount, RefundStatus, IsDeleted, CreatedAt)
    VALUES (@custId, @vId50k, 2, N'Order-Test-02', '0901234567',
            N'45 Nguyễn Huệ', N'Hồ Chí Minh', N'Quận 1', N'Phường Bến Thành',
            N'GHN', 25000, DATEADD(DAY, -6, GETDATE()), 475000, -- Đã trừ 50k voucher
            0, 475000, 'None', 0, DATEADD(DAY, -6, GETDATE()));
    DECLARE @o2 INT = SCOPE_IDENTITY();
    INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, IsDeleted, Reviewed, CreatedAt)
    VALUES (@o2, @p2, 2, 250000, 0, 0, GETDATE());
    INSERT INTO OrderStatusHistory (OrderId, StatusId, ChangedAt, Note, CreatedAt)
    VALUES (@o2, 1, DATEADD(DAY, -6, GETDATE()),  N'Khách hàng đặt hàng',  GETDATE()),
           (@o2, 2, DATEADD(DAY, -5, GETDATE()),  N'Đang xử lý thanh toán', GETDATE());
END

-- ── Order 3 · Confirmed
IF NOT EXISTS (SELECT 1 FROM Orders WHERE ShippingPhone = '0901234567' AND StatusId = 3 AND ShippingName = N'Order-Test-03')
BEGIN
    INSERT INTO Orders (AccountId, VoucherId, StatusId, ShippingName, ShippingPhone,
                        ShippingAddressLine, ShippingCity, ShippingDistrict, ShippingWard,
                        ShippingMethod, ShippingFee, OrderDate, TotalAmount,
                        PaidByWalletAmount, PaidByExternalAmount, RefundStatus, IsDeleted, CreatedAt)
    VALUES (@custId, @vId10, 3, N'Order-Test-03', '0901234567',
            N'88 Đinh Tiên Hoàng', N'Hồ Chí Minh', N'Bình Thạnh', N'Phường 3',
            N'GHN', 30000, DATEADD(DAY, -5, GETDATE()), 552000, -- Đã trừ 10% (58k)
            100000, 452000, 'None', 0, DATEADD(DAY, -5, GETDATE()));
    DECLARE @o3 INT = SCOPE_IDENTITY();
    INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, IsDeleted, Reviewed, CreatedAt)
    VALUES (@o3, @p3, 1, 580000, 0, 0, GETDATE());
    INSERT INTO OrderStatusHistory (OrderId, StatusId, ChangedAt, Note, CreatedAt)
    VALUES (@o3, 1, DATEADD(DAY, -5, GETDATE()),  N'Khách hàng đặt hàng',  GETDATE()),
           (@o3, 2, DATEADD(DAY, -4, GETDATE()),  N'Xử lý thành công',     GETDATE()),
           (@o3, 3, DATEADD(DAY, -3, GETDATE()),  N'Đơn hàng đã xác nhận', GETDATE());
END

-- ── Order 4 · Shipped
IF NOT EXISTS (SELECT 1 FROM Orders WHERE ShippingPhone = '0901234567' AND StatusId = 4 AND ShippingName = N'Order-Test-04')
BEGIN
    INSERT INTO Orders (AccountId, VoucherId, StatusId, ShippingName, ShippingPhone,
                        ShippingAddressLine, ShippingCity, ShippingDistrict, ShippingWard,
                        ShippingMethod, ShippingFee, OrderDate, TotalAmount,
                        PaidByWalletAmount, PaidByExternalAmount, RefundStatus, IsDeleted, CreatedAt)
    VALUES (@custId, NULL, 4, N'Order-Test-04', '0901234567',
            N'200 Lý Thường Kiệt', N'Hồ Chí Minh', N'Quận 10', N'Phường 15',
            N'GHN', 35000, DATEADD(DAY, -4, GETDATE()), 965000, -- Tính đúng tổng
            0, 965000, 'None', 0, DATEADD(DAY, -4, GETDATE()));
    DECLARE @o4 INT = SCOPE_IDENTITY();
    INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, IsDeleted, Reviewed, CreatedAt)
    VALUES (@o4, @p1, 1, 350000, 0, 0, GETDATE()),
           (@o4, @p3, 1, 580000, 0, 0, GETDATE());
    INSERT INTO OrderStatusHistory (OrderId, StatusId, ChangedAt, Note, CreatedAt)
    VALUES (@o4, 1, DATEADD(DAY, -4, GETDATE()),  N'Khách hàng đặt hàng',  GETDATE()),
           (@o4, 2, DATEADD(DAY, -3, GETDATE()),  N'Xử lý thành công',     GETDATE()),
           (@o4, 3, DATEADD(DAY, -2, GETDATE()),  N'Đơn hàng đã xác nhận', GETDATE()),
           (@o4, 4, DATEADD(DAY, -1, GETDATE()),  N'Đã bàn giao vận chuyển', GETDATE());
END

-- ── Order 5 · Delivered
IF NOT EXISTS (SELECT 1 FROM Orders WHERE ShippingPhone = '0901234567' AND StatusId = 5 AND ShippingName = N'Order-Test-05')
BEGIN
    DECLARE @deliveredDate DATETIME = DATEADD(DAY, -1, GETDATE());
    INSERT INTO Orders (AccountId, VoucherId, StatusId, ShippingName, ShippingPhone,
                        ShippingAddressLine, ShippingCity, ShippingDistrict, ShippingWard,
                        ShippingMethod, ShippingFee, OrderDate, TotalAmount,
                        PaidByWalletAmount, PaidByExternalAmount, PaymentCompletedAt,
                        RefundStatus, IsDeleted, CreatedAt)
    VALUES (@custId, @vId50k, 5, N'Order-Test-05', '0901234567',
            N'77 Trần Hưng Đạo', N'Hà Nội', N'Hoàn Kiếm', N'Phường Hàng Trống',
            N'GHTK', 28000, DATEADD(DAY, -10, GETDATE()), 828000, -- Đã trừ 50k voucher
            50000, 778000, @deliveredDate, 'None', 0, DATEADD(DAY, -10, GETDATE()));
    DECLARE @o5 INT = SCOPE_IDENTITY();
    INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, IsDeleted, Reviewed, CreatedAt)
    VALUES (@o5, @p1, 1, 350000, 0, 0, GETDATE()),
           (@o5, @p2, 2, 250000, 0, 0, GETDATE());
    INSERT INTO OrderStatusHistory (OrderId, StatusId, ChangedAt, Note, CreatedAt)
    VALUES (@o5, 1, DATEADD(DAY,-10,GETDATE()), N'Khách hàng đặt hàng',    GETDATE()),
           (@o5, 2, DATEADD(DAY, -9,GETDATE()), N'Xử lý thành công',       GETDATE()),
           (@o5, 3, DATEADD(DAY, -8,GETDATE()), N'Đơn hàng đã xác nhận',   GETDATE()),
           (@o5, 4, DATEADD(DAY, -5,GETDATE()), N'Đã bàn giao vận chuyển', GETDATE()),
           (@o5, 5, @deliveredDate,             N'Giao hàng thành công',   GETDATE());
END

-- ── Order 6 · Cancelled (by customer)
IF NOT EXISTS (SELECT 1 FROM Orders WHERE ShippingPhone = '0901234567' AND StatusId = 6 AND ShippingName = N'Order-Test-06')
BEGIN
    INSERT INTO Orders (AccountId, VoucherId, StatusId, ShippingName, ShippingPhone,
                        ShippingAddressLine, ShippingCity, ShippingDistrict, ShippingWard,
                        ShippingMethod, ShippingFee, OrderDate, TotalAmount,
                        PaidByWalletAmount, PaidByExternalAmount, RefundStatus, IsDeleted, CreatedAt)
    VALUES (@custId, NULL, 6, N'Order-Test-06', '0901234567',
            N'5 Nguyễn Trãi', N'Hồ Chí Minh', N'Quận 5', N'Phường 1',
            N'GHN', 30000, DATEADD(DAY, -3, GETDATE()), 280000,
            0, 280000, 'None', 0, DATEADD(DAY, -3, GETDATE()));
    DECLARE @o6 INT = SCOPE_IDENTITY();
    INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, IsDeleted, Reviewed, CreatedAt)
    VALUES (@o6, @p2, 1, 250000, 0, 0, GETDATE());
    INSERT INTO OrderStatusHistory (OrderId, StatusId, ChangedAt, Note, CreatedAt)
    VALUES (@o6, 1, DATEADD(DAY,-3,GETDATE()), N'Khách hàng đặt hàng',    GETDATE()),
           (@o6, 6, DATEADD(DAY,-2,GETDATE()), N'Khách hàng tự huỷ đơn',  GETDATE());
END

-- ── Order 7 · Cancelled (out of stock)
IF NOT EXISTS (SELECT 1 FROM Orders WHERE ShippingPhone = '0901234567' AND StatusId = 6 AND ShippingName = N'Order-Test-07')
BEGIN
    INSERT INTO Orders (AccountId, VoucherId, StatusId, ShippingName, ShippingPhone,
                        ShippingAddressLine, ShippingCity, ShippingDistrict, ShippingWard,
                        ShippingMethod, ShippingFee, OrderDate, TotalAmount,
                        PaidByWalletAmount, PaidByExternalAmount, RefundStatus, IsDeleted, CreatedAt)
    VALUES (@custId, @vId10, 6, N'Order-Test-07', '0901234567',
            N'99 Hai Bà Trưng', N'Hà Nội', N'Quận Hai Bà Trưng', N'Phường Bùi Thị Xuân',
            N'GHN', 25000, DATEADD(DAY, -8, GETDATE()), 547000,
            0, 547000, 'None', 0, DATEADD(DAY, -8, GETDATE()));
    DECLARE @o7 INT = SCOPE_IDENTITY();
    INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, IsDeleted, Reviewed, CreatedAt)
    VALUES (@o7, @p3, 1, 580000, 0, 0, GETDATE());
    INSERT INTO OrderStatusHistory (OrderId, StatusId, ChangedAt, Note, CreatedAt)
    VALUES (@o7, 1, DATEADD(DAY,-8,GETDATE()), N'Khách hàng đặt hàng',          GETDATE()),
           (@o7, 2, DATEADD(DAY,-7,GETDATE()), N'Xử lý thành công',             GETDATE()),
           (@o7, 6, DATEADD(DAY,-6,GETDATE()), N'Hết hàng, tự động huỷ đơn',    GETDATE());
END

-- ── Order 8 · Refunded
IF NOT EXISTS (SELECT 1 FROM Orders WHERE ShippingPhone = '0901234567' AND StatusId = 7 AND ShippingName = N'Order-Test-08')
BEGIN
    INSERT INTO Orders (AccountId, VoucherId, StatusId, ShippingName, ShippingPhone,
                        ShippingAddressLine, ShippingCity, ShippingDistrict, ShippingWard,
                        ShippingMethod, ShippingFee, OrderDate, TotalAmount,
                        PaidByWalletAmount, PaidByExternalAmount, PaymentCompletedAt,
                        RefundStatus, IsDeleted, CreatedAt)
    VALUES (@custId, NULL, 7, N'Order-Test-08', '0901234567',
            N'12 Phan Bội Châu', N'Đà Nẵng', N'Hải Châu', N'Phường Thạch Thang',
            N'GHN', 30000, DATEADD(DAY,-15,GETDATE()), 730000,
            0, 730000, DATEADD(DAY,-13,GETDATE()), 'Full', 0, DATEADD(DAY,-15,GETDATE()));
    DECLARE @o8 INT = SCOPE_IDENTITY();
    INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, IsDeleted, Reviewed, CreatedAt)
    VALUES (@o8, @p1, 2, 350000, 0, 0, GETDATE());
    INSERT INTO OrderStatusHistory (OrderId, StatusId, ChangedAt, Note, CreatedAt)
    VALUES (@o8, 1, DATEADD(DAY,-15,GETDATE()), N'Khách hàng đặt hàng',          GETDATE()),
           (@o8, 2, DATEADD(DAY,-14,GETDATE()), N'Xử lý thành công',             GETDATE()),
           (@o8, 3, DATEADD(DAY,-13,GETDATE()), N'Đơn hàng đã xác nhận',          GETDATE()),
           (@o8, 4, DATEADD(DAY,-12,GETDATE()), N'Đã bàn giao vận chuyển',        GETDATE()),
           (@o8, 5, DATEADD(DAY,-10,GETDATE()), N'Giao hàng thành công',          GETDATE()),
           (@o8, 6, DATEADD(DAY, -9,GETDATE()), N'Khách yêu cầu hoàn trả',        GETDATE()),
           (@o8, 7, DATEADD(DAY, -8,GETDATE()), N'Hoàn tiền thành công',          GETDATE());
END

-- ── Order 9 · Delivered · wallet paid
IF NOT EXISTS (SELECT 1 FROM Orders WHERE ShippingPhone = '0901234567' AND StatusId = 5 AND ShippingName = N'Order-Test-09')
BEGIN
    DECLARE @deliveredDate9 DATETIME = DATEADD(DAY, -2, GETDATE());
    INSERT INTO Orders (AccountId, VoucherId, StatusId, ShippingName, ShippingPhone,
                        ShippingAddressLine, ShippingCity, ShippingDistrict, ShippingWard,
                        ShippingMethod, ShippingFee, OrderDate, TotalAmount,
                        PaidByWalletAmount, PaidByExternalAmount, PaymentCompletedAt,
                        RefundStatus, IsDeleted, CreatedAt)
    VALUES (@custId, @vId10, 5, N'Order-Test-09', '0901234567',
            N'33 Bạch Đằng', N'Hà Nội', N'Cầu Giấy', N'Phường Dịch Vọng',
            N'GHTK', 22000, DATEADD(DAY,-12,GETDATE()), 1102000, -- Đã trừ max cap 100k
            500000, 602000, @deliveredDate9, 'None', 0, DATEADD(DAY,-12,GETDATE()));
    DECLARE @o9 INT = SCOPE_IDENTITY();
    INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice, IsDeleted, Reviewed, CreatedAt)
    VALUES (@o9, @p1, 1, 350000, 0, 0, GETDATE()),
           (@o9, @p2, 1, 250000, 0, 0, GETDATE()),
           (@o9, @p3, 1, 580000, 0, 0, GETDATE());
    INSERT INTO OrderStatusHistory (OrderId, StatusId, ChangedAt, Note, CreatedAt)
    VALUES (@o9, 1, DATEADD(DAY,-12,GETDATE()), N'Khách hàng đặt hàng',    GETDATE()),
           (@o9, 2, DATEADD(DAY,-11,GETDATE()), N'Xử lý thành công',       GETDATE()),
           (@o9, 3, DATEADD(DAY,-10,GETDATE()), N'Đơn hàng đã xác nhận',   GETDATE()),
           (@o9, 4, DATEADD(DAY, -7,GETDATE()), N'Đã bàn giao vận chuyển', GETDATE()),
           (@o9, 5, @deliveredDate9,            N'Giao hàng thành công',   GETDATE());
END
GO

-- ============================================================
-- 7. REVIEW PRODUCTS (9 reviews covering Pending / Approved / Rejected)
--    Linked to OrderDetails from Orders 4, 5, 8, 9 (Delivered / Refunded)
-- ============================================================
DECLARE @rCustId  INT = (SELECT TOP 1 AccountId FROM Accounts WHERE Email = 'vuquangduc1404@gmail.com');
DECLARE @rP1      INT = (SELECT TOP 1 ProductId FROM Products WHERE Sku = 'TEST-SKU-001');
DECLARE @rP2      INT = (SELECT TOP 1 ProductId FROM Products WHERE Sku = 'TEST-SKU-002');
DECLARE @rP3      INT = (SELECT TOP 1 ProductId FROM Products WHERE Sku = 'TEST-SKU-003');

DECLARE @odO5P1 INT = (SELECT TOP 1 od.OrderDetailId FROM OrderDetails od JOIN Orders o ON od.OrderId = o.OrderId WHERE o.ShippingName = N'Order-Test-05' AND od.ProductId = @rP1);
DECLARE @odO5P2 INT = (SELECT TOP 1 od.OrderDetailId FROM OrderDetails od JOIN Orders o ON od.OrderId = o.OrderId WHERE o.ShippingName = N'Order-Test-05' AND od.ProductId = @rP2);
DECLARE @odO9P1 INT = (SELECT TOP 1 od.OrderDetailId FROM OrderDetails od JOIN Orders o ON od.OrderId = o.OrderId WHERE o.ShippingName = N'Order-Test-09' AND od.ProductId = @rP1);
DECLARE @odO9P2 INT = (SELECT TOP 1 od.OrderDetailId FROM OrderDetails od JOIN Orders o ON od.OrderId = o.OrderId WHERE o.ShippingName = N'Order-Test-09' AND od.ProductId = @rP2);
DECLARE @odO9P3 INT = (SELECT TOP 1 od.OrderDetailId FROM OrderDetails od JOIN Orders o ON od.OrderId = o.OrderId WHERE o.ShippingName = N'Order-Test-09' AND od.ProductId = @rP3);
DECLARE @odO8P1 INT = (SELECT TOP 1 od.OrderDetailId FROM OrderDetails od JOIN Orders o ON od.OrderId = o.OrderId WHERE o.ShippingName = N'Order-Test-08' AND od.ProductId = @rP1);
DECLARE @odO4P1 INT = (SELECT TOP 1 od.OrderDetailId FROM OrderDetails od JOIN Orders o ON od.OrderId = o.OrderId WHERE o.ShippingName = N'Order-Test-04' AND od.ProductId = @rP1);
DECLARE @odO4P3 INT = (SELECT TOP 1 od.OrderDetailId FROM OrderDetails od JOIN Orders o ON od.OrderId = o.OrderId WHERE o.ShippingName = N'Order-Test-04' AND od.ProductId = @rP3);

-- R1: Approved · 5★ · Lego Thành Phố Mini (Order 5)
IF @odO5P1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM ReviewProducts WHERE OrderDetailId = @odO5P1)
    INSERT INTO ReviewProducts (AccountId, ProductId, OrderDetailId, Rating, Comment,
                                IsVerifiedPurchase, IsDeleted, Status, Visibility, CreatedAt)
    VALUES (@rCustId, @rP1, @odO5P1, 5, N'Sản phẩm chất lượng tuyệt vời, lắp ráp rất thú vị!',
            1, 0, 'Approved', 'Public', DATEADD(DAY, -9, GETDATE()));

-- R2: Approved · 4★ · Lego Xe Cảnh Sát (Order 5)
IF @odO5P2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM ReviewProducts WHERE OrderDetailId = @odO5P2)
    INSERT INTO ReviewProducts (AccountId, ProductId, OrderDetailId, Rating, Comment,
                                IsVerifiedPurchase, IsDeleted, Status, Visibility, CreatedAt)
    VALUES (@rCustId, @rP2, @odO5P2, 4, N'Xe cảnh sát rất đẹp, màu sắc tươi sáng. Phù hợp cho trẻ từ 6 tuổi.',
            1, 0, 'Approved', 'Public', DATEADD(DAY, -8, GETDATE()));

-- R3: Approved · 3★ · Lego Thành Phố Mini (Order 9)
IF @odO9P1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM ReviewProducts WHERE OrderDetailId = @odO9P1)
    INSERT INTO ReviewProducts (AccountId, ProductId, OrderDetailId, Rating, Comment,
                                IsVerifiedPurchase, IsDeleted, Status, Visibility, CreatedAt)
    VALUES (@rCustId, @rP1, @odO9P1, 3, N'Bộ ổn, nhưng số lượng mảnh ghép hơi ít so với giá tiền.',
            1, 0, 'Approved', 'Public', DATEADD(DAY, -11, GETDATE()));

-- R4: Approved · 2★ · Lego Xe Cảnh Sát (Order 9)
IF @odO9P2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM ReviewProducts WHERE OrderDetailId = @odO9P2)
    INSERT INTO ReviewProducts (AccountId, ProductId, OrderDetailId, Rating, Comment,
                                IsVerifiedPurchase, IsDeleted, Status, Visibility, CreatedAt)
    VALUES (@rCustId, @rP2, @odO9P2, 2, N'Chất lượng không như mong đợi, một số miếng ghép bị lỗi.',
            1, 0, 'Approved', 'Public', DATEADD(DAY, -10, GETDATE()));

-- R5: Pending · 5★ · Lego Phi Thuyền Vũ Trụ (Order 9)
IF @odO9P3 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM ReviewProducts WHERE OrderDetailId = @odO9P3)
    INSERT INTO ReviewProducts (AccountId, ProductId, OrderDetailId, Rating, Comment,
                                IsVerifiedPurchase, IsDeleted, Status, Visibility, CreatedAt)
    VALUES (@rCustId, @rP3, @odO9P3, 5, N'Phi thuyền vũ trụ cực kỳ ấn tượng! Đóng gói cẩn thận, giao hàng nhanh.',
            1, 0, 'Pending', 'AuthorOnly', DATEADD(DAY, -1, GETDATE()));

-- R6: Pending · 4★ · Lego Thành Phố Mini (Order 4)
IF @odO4P1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM ReviewProducts WHERE OrderDetailId = @odO4P1)
    INSERT INTO ReviewProducts (AccountId, ProductId, OrderDetailId, Rating, Comment,
                                IsVerifiedPurchase, IsDeleted, Status, Visibility, CreatedAt)
    VALUES (@rCustId, @rP1, @odO4P1, 4, N'Bộ lắp ráp đẹp nhưng hướng dẫn hơi khó hiểu với trẻ nhỏ.',
            1, 0, 'Pending', 'AuthorOnly', DATEADD(HOUR, -5, GETDATE()));

-- R7: Rejected · 1★ · Lego Phi Thuyền Vũ Trụ (Order 4)
IF @odO4P3 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM ReviewProducts WHERE OrderDetailId = @odO4P3)
    INSERT INTO ReviewProducts (AccountId, ProductId, OrderDetailId, Rating, Comment,
                                IsVerifiedPurchase, IsDeleted, Status, Visibility, ModerationDetail, CreatedAt)
    VALUES (@rCustId, @rP3, @odO4P3, 1, N'Spam test review content!!!',
            1, 0, 'Rejected', 'AuthorOnly', N'Review vi phạm chính sách: nội dung không hợp lệ',
            DATEADD(DAY, -2, GETDATE()));

-- R8: Approved · 5★ · Lego Thành Phố Mini (Order 8 – sau hoàn tiền)
IF @odO8P1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM ReviewProducts WHERE OrderDetailId = @odO8P1)
    INSERT INTO ReviewProducts (AccountId, ProductId, OrderDetailId, Rating, Comment,
                                IsVerifiedPurchase, IsDeleted, Status, Visibility, CreatedAt)
    VALUES (@rCustId, @rP1, @odO8P1, 5, N'Sản phẩm tốt nhưng giao hàng bị chậm. Đã nhận được tiền hoàn.',
            1, 0, 'Approved', 'Public', DATEADD(DAY, -9, GETDATE()));

-- R9: Approved · 4★ · Lego Xe Cảnh Sát – không gắn OrderDetail (không xác thực mua hàng)
IF NOT EXISTS (SELECT 1 FROM ReviewProducts WHERE AccountId = @rCustId AND ProductId = @rP2 AND OrderDetailId IS NULL AND Rating = 4)
    INSERT INTO ReviewProducts (AccountId, ProductId, OrderDetailId, Rating, Comment,
                                IsVerifiedPurchase, IsDeleted, Status, Visibility, CreatedAt)
    VALUES (@rCustId, @rP2, NULL, 4, N'Mình đã dùng loại này rồi, chất lượng rất tốt so với mức giá.',
            0, 0, 'Approved', 'Public', DATEADD(DAY, -20, GETDATE()));
GO

-- ============================================================
-- 8. REVIEW PRODUCT REPLIES (3 replies từ Staff/Admin)
-- ============================================================
DECLARE @staffId INT = (SELECT TOP 1 AccountId FROM Accounts
                        WHERE RoleId IN (2, 4) AND AccountId != (SELECT TOP 1 AccountId FROM Accounts WHERE Email = 'vuquangduc1404@gmail.com')
                        ORDER BY AccountId);

-- Reply cho R1 (Approved 5★ Lego Thành Phố Mini Order 5)
DECLARE @rvR1 INT = (SELECT TOP 1 rp.ReviewProductId
                     FROM ReviewProducts rp JOIN OrderDetails od ON rp.OrderDetailId = od.OrderDetailId
                     JOIN Orders o ON od.OrderId = o.OrderId
                     WHERE o.ShippingName = N'Order-Test-05' AND rp.Rating = 5);

IF @staffId IS NOT NULL AND @rvR1 IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM ReviewProductReplies WHERE ReviewProductId = @rvR1 AND AccountId = @staffId)
    INSERT INTO ReviewProductReplies (ReviewProductId, AccountId, Content, IsDeleted, CreatedAt)
    VALUES (@rvR1, @staffId, N'Cảm ơn bạn đã tin tưởng và ủng hộ sản phẩm của chúng tôi! Rất vui khi bạn hài lòng.', 0, DATEADD(DAY, -8, GETDATE()));

-- Reply cho R2 (Approved 4★ Lego Xe Cảnh Sát Order 5)
DECLARE @rvR2 INT = (SELECT TOP 1 rp.ReviewProductId
                     FROM ReviewProducts rp JOIN OrderDetails od ON rp.OrderDetailId = od.OrderDetailId
                     JOIN Orders o ON od.OrderId = o.OrderId
                     WHERE o.ShippingName = N'Order-Test-05' AND rp.Rating = 4);

IF @staffId IS NOT NULL AND @rvR2 IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM ReviewProductReplies WHERE ReviewProductId = @rvR2 AND AccountId = @staffId)
    INSERT INTO ReviewProductReplies (ReviewProductId, AccountId, Content, IsDeleted, CreatedAt)
    VALUES (@rvR2, @staffId, N'Cảm ơn bạn đã nhận xét! Chúng tôi ghi nhận góp ý về màu sắc và sẽ cải thiện trong các phiên bản tiếp theo.', 0, DATEADD(DAY, -7, GETDATE()));

-- Reply cho R4 (Approved 2★ Lego Xe Cảnh Sát Order 9)
DECLARE @rvR4 INT = (SELECT TOP 1 rp.ReviewProductId
                     FROM ReviewProducts rp JOIN OrderDetails od ON rp.OrderDetailId = od.OrderDetailId
                     JOIN Orders o ON od.OrderId = o.OrderId
                     WHERE o.ShippingName = N'Order-Test-09' AND rp.Rating = 2);

IF @staffId IS NOT NULL AND @rvR4 IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM ReviewProductReplies WHERE ReviewProductId = @rvR4 AND AccountId = @staffId)
    INSERT INTO ReviewProductReplies (ReviewProductId, AccountId, Content, IsDeleted, CreatedAt)
    VALUES (@rvR4, @staffId, N'Chúng tôi rất tiếc về trải nghiệm của bạn. Vui lòng liên hệ CSKH để được hỗ trợ đổi/trả sản phẩm lỗi.', 0, DATEADD(DAY, -9, GETDATE()));
GO

-- ============================================================
-- VERIFY
-- ============================================================
SELECT o.OrderId, o.ShippingName, so.StatusName, o.TotalAmount,
       COUNT(od.OrderDetailId) AS Items
FROM   Orders o
JOIN   StatusOrders so ON so.StatusId = o.StatusId
LEFT JOIN OrderDetails od ON od.OrderId = o.OrderId
WHERE  o.ShippingPhone = '0901234567'
GROUP  BY o.OrderId, o.ShippingName, so.StatusName, o.TotalAmount
ORDER  BY o.OrderId;

SELECT rp.ReviewProductId, p.ProductName, a.Email AS Reviewer,
       rp.Rating, LEFT(rp.Comment, 60) AS CommentPreview,
       rp.Status, rp.Visibility, rp.IsVerifiedPurchase,
       COUNT(rpr.ReplyProductId) AS Replies
FROM   ReviewProducts rp
JOIN   Products p  ON p.ProductId  = rp.ProductId
JOIN   Accounts a  ON a.AccountId  = rp.AccountId
LEFT  JOIN ReviewProductReplies rpr ON rpr.ReviewProductId = rp.ReviewProductId AND rpr.IsDeleted = 0
GROUP  BY rp.ReviewProductId, p.ProductName, a.Email, rp.Rating,
          rp.Comment, rp.Status, rp.Visibility, rp.IsVerifiedPurchase
ORDER  BY rp.ReviewProductId;
GO
