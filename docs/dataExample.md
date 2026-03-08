USE [ASP_LorKingDom]
GO

/_ =============================================
TEST DATA — INSERT THEO THỨ TỰ DEPENDENCY
============================================= _/

-- ============= LEVEL 0: Bảng độc lập =============

-- Roles
INSERT INTO [dbo].[Roles] ([RoleName], [Description]) VALUES
('Customer', N'Người dùng cuối mua và sử dụng sản phẩm'),
('Staff', N'Nhân viên xử lý đơn hàng và hỗ trợ khách hàng'),
('Warehouse', N'Nhân viên quản lý kho và tồn kho'),
('Admin', N'Quản trị viên hệ thống, toàn quyền');
GO

-- SuperCategories
INSERT INTO [dbo].[SuperCategories] ([SuperCategoryName]) VALUES
(N'Đồ chơi'),
(N'Phụ kiện trẻ em'),
(N'Thời trang trẻ em');
GO

-- Ages
INSERT INTO [dbo].[Ages] ([AgeRange]) VALUES
(N'0-1 tuổi'),
(N'1-3 tuổi'),
(N'3-6 tuổi'),
(N'6-12 tuổi'),
(N'12+ tuổi');
GO

-- Sexes
INSERT INTO [dbo].[Sexes] ([SexName]) VALUES
(N'Bé trai'),
(N'Bé gái'),
(N'Unisex');
GO

-- Brands
INSERT INTO [dbo].[Brands] ([BrandName]) VALUES
('LEGO'),
('Fisher-Price'),
('Mattel'),
('Chicco'),
('VTech');
GO

-- Origins
INSERT INTO [dbo].[Origins] ([OriginName]) VALUES
(N'Đan Mạch'),
(N'Mỹ'),
(N'Ý'),
(N'Việt Nam'),
(N'Trung Quốc');
GO

-- Materials
INSERT INTO [dbo].[Materials] ([MaterialName], [Description]) VALUES
(N'Nhựa ABS', N'Nhựa an toàn, không chứa BPA'),
(N'Gỗ tự nhiên', N'Gỗ thông hoặc gỗ bạch đàn, sơn màu an toàn'),
(N'Vải bông', N'100% cotton hữu cơ'),
(N'Silicon', N'Silicone cấp thực phẩm, chịu nhiệt tốt'),
(N'Kim loại', N'Hợp kim nhôm hoặc thép không gỉ');
GO

-- PriceRanges
INSERT INTO [dbo].[PriceRanges] ([PriceRangeMin], [PriceRangeMax]) VALUES
(0, 100000),
(100000, 300000),
(300000, 500000),
(500000, 1000000),
(1000000, 5000000);
GO

-- StatusOrders
INSERT INTO [dbo].[StatusOrders] ([StatusName], [Description]) VALUES
('Pending', N'Chờ xác nhận'),
('Confirmed', N'Đã xác nhận, đang chuẩn bị hàng'),
('Shipped', N'Đã giao cho đơn vị vận chuyển'),
('Delivered', N'Đã giao hàng thành công'),
('Completed', N'Hoàn tất đơn hàng'),
('Cancelled', N'Đã huỷ'),
('Refunded', N'Đã hoàn tiền');
GO

-- VoucherTypes
INSERT INTO [dbo].[VoucherTypes] ([VoucherTypeName]) VALUES
(N'Giảm theo giá trị cố định'),
(N'Giảm theo phần trăm'),
(N'Miễn phí vận chuyển');
GO

-- BlogCategories
INSERT INTO [dbo].[BlogCategories] ([BlogCategoryName], [Description]) VALUES
(N'Tin tức', N'Thông báo và tin tức mới nhất'),
(N'Cẩm nang', N'Hướng dẫn và lời khuyên cho phụ huynh'),
(N'Khuyến mãi', N'Thông tin về các chương trình ưu đãi'),
(N'Review sản phẩm',N'Đánh giá chi tiết sản phẩm');
GO

-- SystemConfigs
INSERT INTO [dbo].[SystemConfigs] ([ConfigKey], [ConfigValue], [Description]) VALUES
('MaxReviewImages', '5', N'Số ảnh tối đa trong 1 review'),
('RefundWindowDays', '7', N'Số ngày cho phép yêu cầu hoàn tiền'),
('WalletTopUpMin', '10000', N'Nạp tối thiểu vào ví (VND)'),
('FreeShippingMinOrder','500000',N'Giá trị đơn tối thiểu để miễn phí ship');
GO

-- System.BackgroundJobs
INSERT INTO [System].[BackgroundJobs] ([JobName], [CronExpression], [IsEnabled]) VALUES
('SendScheduledNotifications', '0 \* \* \* _', 1),
('ExpireVouchers', '0 0 _ \* _', 1),
('CleanupOldLogs', '0 3 _ \* 0', 1);
GO

-- Notification.Templates
INSERT INTO [Notification].[Templates] ([TemplateCode], [TitleTemplate], [MessageTemplate]) VALUES
('ORDER_CONFIRMED', N'Đơn hàng #{OrderId} đã xác nhận', N'Đơn hàng #{OrderId} của bạn đã được xác nhận. Chúng tôi đang chuẩn bị hàng.'),
('ORDER_SHIPPED', N'Đơn hàng #{OrderId} đang giao', N'Đơn hàng #{OrderId} đã được bàn giao cho đơn vị vận chuyển. Mã vận đơn: #{TrackingNumber}.'),
('ORDER_DELIVERED', N'Đơn hàng #{OrderId} đã giao thành công',N'Đơn hàng #{OrderId} đã được giao thành công. Cảm ơn bạn đã mua hàng!'),
('ORDER_CANCELLED', N'Đơn hàng #{OrderId} đã huỷ', N'Đơn hàng #{OrderId} đã bị huỷ. Lý do: #{Reason}.'),
('WALLET_TOPUP', N'Nạp tiền thành công', N'Ví của bạn đã được nạp #{Amount} VND. Số dư hiện tại: #{Balance} VND.'),
('WALLET_PAYMENT', N'Thanh toán đơn hàng #{OrderId}', N'Ví của bạn đã bị trừ #{Amount} VND cho đơn hàng #{OrderId}.'),
('REFUND_APPROVED', N'Hoàn tiền đơn hàng #{OrderId} thành công',N'#{Amount} VND đã được hoàn vào ví của bạn.'),
('REVIEW_APPROVED', N'Đánh giá của bạn đã được duyệt', N'Cảm ơn bạn đã chia sẻ trải nghiệm. Đánh giá của bạn đã được hiển thị công khai.'),
('REVIEW_REJECTED', N'Đánh giá của bạn bị từ chối', N'Đánh giá của bạn về ''#{productName}'' đã bị từ chối. Lý do: #{reason}'),
('CUSTOM', N'#{title}', N'#{message}');
GO

-- ============= LEVEL 1: Accounts & Categories =============

-- Accounts (Password đã hash bằng bcrypt cost=11)
INSERT INTO [dbo].[Accounts]
([RoleID], [AccountName], [PhoneNumber], [Email], [Password], [Status], [Provider])
VALUES
-- Admin
(4, 'admin01', '0900000003', 'admin@lorkingdom.com',
'$2b$11$puQ/vlg2ytfmh3VgmCK/wePQdbILFqu5/VkRByoQpHbvs91tHgFCG', 'Active', 'Email'),
-- Staff
(2, 'staff01', '0900000001', 'staff@lorkingdom.com',
'$2b$11$g6.SyR4fqS86LEl2nM1H/OIm11oCEpMukHOW6I0/C9ypBmIR50zfS', 'Active', 'Email'),
-- Warehouse
(3, 'warehouse01', '0900000002', 'warehouse@lorkingdom.com',
'$2b$11$ACDZHTdYJs7tFYezEOO1VOJUilPqZv8aaLz28J5GWwCjSIsUlSKtS', 'Active', 'Email'),
-- Customers
(1, 'nguyenvana', '0912345678', 'nguyenvana@gmail.com',
'$2b$11$g6.SyR4fqS86LEl2nM1H/OIm11oCEpMukHOW6I0/C9ypBmIR50zfS', 'Active', 'Email'),
(1, 'tranthib', '0987654321', 'tranthib@gmail.com',
'$2b$11$g6.SyR4fqS86LEl2nM1H/OIm11oCEpMukHOW6I0/C9ypBmIR50zfS', 'Active', 'Email'),
(1, 'lehongc', '0978123456', 'lehongc@gmail.com',
'$2b$11$g6.SyR4fqS86LEl2nM1H/OIm11oCEpMukHOW6I0/C9ypBmIR50zfS', 'Active', 'Google');
GO

-- Categories (SuperCategoryID: 1=Đồ chơi, 2=Phụ kiện, 3=Thời trang)
INSERT INTO [dbo].[Categories] ([SuperCategoryID], [CategoryName]) VALUES
(1, N'Đồ chơi lắp ráp'),
(1, N'Đồ chơi nhồi bông'),
(1, N'Đồ chơi giáo dục'),
(2, N'Xe đẩy & ghế ngồi'),
(2, N'Bình sữa & phụ kiện ăn dặm'),
(3, N'Áo & quần'),
(3, N'Giày dép trẻ em');
GO

-- ============= LEVEL 2: Phụ thuộc vào Accounts =============

-- Addresses (AccountID: 4=nguyenvana, 5=tranthib, 6=lehongc)
INSERT INTO [dbo].[Addresses]
([AccountID], [AddressLine], [City], [District], [Ward],
[IsDefault], [RecipientName], [PhoneNumber], [ProvinceId], [DistrictId], [WardCode])
VALUES
(4, N'123 Nguyễn Trãi', N'Hồ Chí Minh', N'Quận 1', N'Phường Bến Nghé', 1, N'Nguyễn Văn A', '0912345678', 202, 1442, '21208'),
(4, N'45 Lê Lợi', N'Hồ Chí Minh', N'Quận 3', N'Phường 6', 0, N'Nguyễn Văn A', '0912345678', 202, 1444, '21211'),
(5, N'88 Trần Hưng Đạo',N'Hà Nội', N'Hoàn Kiếm', N'Phường Hàng Bài', 1, N'Trần Thị B', '0987654321', 201, 1478, '10102'),
(6, N'12 Bạch Đằng', N'Đà Nẵng', N'Hải Châu', N'Phường Thạch Thang',1,N'Lê Hồng C', '0978123456', 490, 1809, '49113');
GO

-- Wallets
INSERT INTO [dbo].[Wallets] ([AccountID], [Currency], [Balance], [Status]) VALUES
(4, 'VND', 500000, 'Active'),
(5, 'VND', 1200000, 'Active'),
(6, 'VND', 0, 'Active');
GO

-- Cart (1 giỏ / account)
INSERT INTO [dbo].[Cart] ([AccountID]) VALUES (4), (5), (6);
GO

-- Vouchers
INSERT INTO [dbo].[Voucher]
([VoucherTypeID], [CreateBy], [VoucherCode], [DiscountValue],
[MinOrderAmount], [UsageLimitPerUser], [IsStackable],
[StartDate], [EndDate], [Status])
VALUES
(1, 1, 'WELCOME50K', 50000, 200000, 1, 0, '2026-01-01', '2026-12-31', 'Active'),
(2, 1, 'SALE20PCT', 20, 500000, 2, 0, '2026-02-01', '2026-03-31', 'Active'),
(3, 1, 'FREESHIP', 0, 300000, 1, 1, '2026-01-01', '2026-06-30', 'Active');
GO

-- BlogPosts (AccountID 2 = staff)
INSERT INTO [dbo].[BlogPosts]
([AccountID], [BlogTitle], [BlogContent], [BlogThumbnail], [IsPublished], [IsFeatured])
VALUES
(2, N'Top 5 đồ chơi LEGO 2026',
N'LEGO tiếp tục ra mắt nhiều bộ lắp ráp thú vị trong năm 2026...',
'https://cdn.lorkingdom.com/blog/lego2026.jpg', 1, 1),
(2, N'Hướng dẫn chọn đồ chơi phù hợp theo độ tuổi',
N'Việc chọn đồ chơi đúng lứa tuổi rất quan trọng cho sự phát triển của trẻ...',
'https://cdn.lorkingdom.com/blog/guide-age.jpg', 1, 0);
GO

-- BlogPostCategories (join post với category)
INSERT INTO [dbo].[BlogPostCategories] ([BlogPostID], [BlogCategoryID]) VALUES
(1, 4), -- post 1 → Review sản phẩm
(2, 2); -- post 2 → Cẩm nang
GO

-- ============= LEVEL 3: Products =============

-- Products
INSERT INTO [dbo].[Products]
([SKU], [CategoryID], [MaterialID], [AgeID], [SexID], [PriceRangeID],
[BrandID], [OriginID], [ProductName], [Price], [Quantity],
[ProductStatus], [Description])
VALUES
('LEGO-CITY-001', 1, 1, 4, 3, 3, 1, 1,
N'LEGO City Đồn Cảnh Sát', 450000, 50, 'Available',
N'Bộ lắp ráp LEGO City với đồn cảnh sát, xe tuần tra và 4 minifigure.'),

('FP-ROCK-002', 2, 3, 2, 3, 2, 2, 2,
N'Fisher-Price Con Thú Nhồi Bông Gấu Teddy', 250000, 100, 'Available',
N'Gấu bông mềm mịn, an toàn cho bé từ 1 tuổi trở lên.'),

('VT-LEARN-003', 3, 1, 3, 3, 2, 5, 5,
N'VTech Bảng Học Thông Minh', 280000, 75, 'Available',
N'Bảng học tương tác với hơn 80 trò chơi, giúp bé học chữ và số.'),

('CH-STROLLER-004', 4, 5, 1, 3, 4, 4, 3,
N'Chicco Xe Đẩy Trẻ Em Sprint', 850000, 20, 'Available',
N'Xe đẩy gọn nhẹ, mái che rộng, phù hợp bé từ 6 tháng tuổi.'),

('MTL-BARBIE-005', 2, 3, 4, 2, 2, 3, 2,
N'Mattel Búp Bê Barbie Thời Trang', 220000, 80, 'Available',
N'Búp bê Barbie với trang phục thời trang mùa hè 2026.'),

('LEGO-DUPLO-006', 1, 1, 2, 3, 2, 1, 1,
N'LEGO DUPLO Bộ Xây Dựng Cơ Bản', 199000, 60, 'Available',
N'Bộ DUPLO 30 mảnh ghép lớn, an toàn cho bé từ 18 tháng.'),

('CH-BOTTLE-007', 5, 4, 1, 3, 1, 4, 3,
N'Chicco Bình Sữa Silicone 150ml', 85000, 200, 'Available',
N'Bình sữa silicone mềm, núm vú giả mô phỏng ti mẹ.');
GO

-- ProductImages
INSERT INTO [dbo].[ProductImages] ([ProductID], [ImageUrl], [IsMain]) VALUES
(1, 'https://cdn.lorkingdom.com/products/lego-city-001-main.jpg', 1),
(1, 'https://cdn.lorkingdom.com/products/lego-city-001-side.jpg', 0),
(2, 'https://cdn.lorkingdom.com/products/fp-rock-002-main.jpg', 1),
(3, 'https://cdn.lorkingdom.com/products/vt-learn-003-main.jpg', 1),
(4, 'https://cdn.lorkingdom.com/products/ch-stroller-004-main.jpg',1),
(5, 'https://cdn.lorkingdom.com/products/mtl-barbie-005-main.jpg', 1),
(6, 'https://cdn.lorkingdom.com/products/lego-duplo-006-main.jpg', 1),
(7, 'https://cdn.lorkingdom.com/products/ch-bottle-007-main.jpg', 1);
GO

-- CartItems (CartID: 1=nguyenvana, 2=tranthib)
INSERT INTO [dbo].[CartItems]
([CartID], [ProductID], [Quantity], [PriceAtThatTime], [Status])
VALUES
(1, 1, 1, 450000, 'Active'),
(1, 7, 2, 85000, 'Active'),
(2, 3, 1, 280000, 'Active');
GO

-- Wishlists
INSERT INTO [dbo].[Wishlists] ([AccountID], [ProductID]) VALUES
(4, 4),
(4, 5),
(5, 1),
(6, 2);
GO

-- ============= LEVEL 4: Orders =============

-- Orders (AccountID 4=nguyenvana, 5=tranthib)
INSERT INTO [dbo].[Orders]
([AccountID], [VoucherID], [StatusID],
[ShippingName], [ShippingPhone], [ShippingAddressLine],
[ShippingCity], [ShippingDistrict], [ShippingWard],
[ShippingMethod], [ShippingFee], [TotalAmount],
[PaidByWalletAmount], [PaidByExternalAmount],
[PaymentCompletedAt], [RefundStatus])
VALUES
-- Order 1: nguyenvana, đã completed, dùng voucher WELCOME50K
(4, 1, 5,
N'Nguyễn Văn A', '0912345678', N'123 Nguyễn Trãi',
N'Hồ Chí Minh', N'Quận 1', N'Phường Bến Nghé',
'GHN', 25000, 535000,
0, 535000, GETDATE(), 'None'),

-- Order 2: tranthib, shipped
(5, NULL, 3,
N'Trần Thị B', '0987654321', N'88 Trần Hưng Đạo',
N'Hà Nội', N'Hoàn Kiếm', N'Phường Hàng Bài',
'GHTK', 30000, 310000,
310000, 0, GETDATE(), 'None'),

-- Order 3: nguyenvana, pending
(4, NULL, 1,
N'Nguyễn Văn A', '0912345678', N'45 Lê Lợi',
N'Hồ Chí Minh', N'Quận 3', N'Phường 6',
'GHN', 20000, 390000,
0, 0, NULL, 'None');
GO

-- OrderDetails
INSERT INTO [dbo].[OrderDetails]
([OrderID], [ProductID], [Quantity], [UnitPrice], [Reviewed])
VALUES
(1, 1, 1, 450000, 1), -- Order 1: LEGO City
(1, 7, 1, 85000, 1), -- Order 1: Bình sữa
(2, 3, 1, 280000, 0), -- Order 2: VTech
(3, 2, 1, 250000, 0), -- Order 3: Gấu bông
(3, 7, 2, 85000, 0); -- Order 3: Bình sữa x2
GO

-- OrderStatusHistory
INSERT INTO [dbo].[OrderStatusHistory]
([OrderID], [StatusID], [ChangedBy], [Note])
VALUES
(1, 1, 2, N'Đơn hàng được tạo'),
(1, 2, 2, N'Đã xác nhận và chuẩn bị hàng'),
(1, 3, 3, N'Bàn giao GHN - Mã VĐ: GHN123456'),
(1, 4, 3, N'Giao hàng thành công'),
(1, 5, 2, N'Hoàn tất đơn hàng'),
(2, 1, 2, N'Đơn hàng được tạo'),
(2, 2, 2, N'Đã xác nhận'),
(2, 3, 3, N'Bàn giao GHTK'),
(3, 1, 2, N'Đơn hàng được tạo');
GO

-- WalletTransactions
INSERT INTO [dbo].[WalletTransactions]
([WalletID], [AccountID], [TxnType], [Direction],
[Amount], [BalanceBefore], [BalanceAfter],
[RelatedOrderID], [Method], [Status], [Reason])
VALUES
-- nạp tiền vào ví nguyenvana
(1, 4, 'TopUp', 'CR', 500000, 0, 500000, NULL, 'Bank', 'Completed', N'Nạp tiền qua ngân hàng'),
-- tranthib thanh toán order 2 bằng ví
(2, 5, 'TopUp', 'CR', 1500000, 0, 1500000,NULL, 'Bank', 'Completed', N'Nạp tiền qua ngân hàng'),
(2, 5, 'Payment', 'DR', 310000, 1500000, 1200000, 2, 'Wallet', 'Completed', N'Thanh toán đơn hàng #2');
GO

-- PaymentHistory
INSERT INTO [dbo].[PaymentHistory]
([OrderID], [AccountID], [PaymentMethod], [PaymentStatus],
[TransactionCode], [Amount], [Currency], [WalletTransactionID])
VALUES
(1, 4, 'VNPay', 'Paid', 'VNP20260220001', 535000, 'VND', NULL),
(2, 5, 'Wallet', 'Paid', NULL, 310000, 'VND', 3), -- WalletTransactionID=3
(3, 4, 'VNPay', 'Pending', NULL, 390000, 'VND', NULL);
GO

-- ShippingProviderTransactions
INSERT INTO [dbo].[ShippingProviderTransactions]
([OrderID], [Provider], [ProviderOrderCode], [TrackingNumber],
[ServiceType], [Status], [ShippingFee], [ActualDelivery])
VALUES
(1, 'GHN', 'GHN-ORD-001', 'GHN123456789', N'Giao hàng nhanh', 'delivered', 25000, GETDATE()),
(2, 'GHTK', 'GHTK-ORD-001','GHTK987654321',N'Tiết kiệm', 'delivering',30000, NULL);
GO

-- ============= LEVEL 5: Reviews =============

-- ReviewProducts (OrderDetailID 1=LEGO City của nguyenvana)
INSERT INTO [dbo].[ReviewProducts]
([AccountID], [ProductID], [OrderDetailID], [Rating], [Comment],
[IsVerifiedPurchase], [Status], [Visibility], [EditCount])
VALUES
(4, 1, 1, 5,
N'Sản phẩm tuyệt vời, bé nhà mình rất thích. Các mảnh ghép chắc chắn, màu sắc đẹp. Đóng gói cẩn thận.',
1, 'Approved', 'Public', 0),
(4, 7, 2, 4,
N'Bình sữa chất lượng tốt, silicone mềm và an toàn. Tuy nhiên hơi khó rửa.',
1, 'Approved', 'Public', 0);
GO

-- ReviewProductImages
INSERT INTO [dbo].[ReviewProductImages]
([ReviewProductID], [ImageUrl])
VALUES
(1, 'https://cdn.lorkingdom.com/reviews/rv1-photo1.jpg'),
(1, 'https://cdn.lorkingdom.com/reviews/rv1-photo2.jpg');
GO

-- ReviewProductReactions (tranthib like review của nguyenvana)
INSERT INTO [dbo].[ReviewProductReactions]
([ReviewProductID], [AccountID], [ReactionType])
VALUES
(1, 5, 'Like'),
(1, 6, 'Like'),
(2, 5, 'Like');
GO

-- ReviewProductReplies (staff trả lời review)
INSERT INTO [dbo].[ReviewProductReplies]
([ReviewProductID], [AccountID], [Content])
VALUES
(1, 2, N'Cảm ơn bạn đã tin tưởng và chia sẻ trải nghiệm! Chúc bé chơi vui nhé 🎉'),
(2, 2, N'Cảm ơn phản hồi của bạn! Bạn có thể dùng bàn chải nhỏ để vệ sinh núm van bình nhé.');
GO

-- ReviewModerationLogs
INSERT INTO [dbo].[ReviewModerationLogs]
([ReviewProductID], [Stage], [Result], [Score], [Details])
VALUES
(1, 'Stage1', 'Passed', 0.05, N'Không phát hiện nội dung vi phạm'),
(1, 'Stage2', 'Passed', 0.03, N'Sentiment tích cực, không spam'),
(2, 'Stage1', 'Passed', 0.08, N'Không phát hiện nội dung vi phạm');
GO

-- ReviewBlogs
INSERT INTO [dbo].[ReviewBlogs]
([BlogPostID], [AccountID], [Rating], [Comment])
VALUES
(1, 4, 5, N'Bài viết rất bổ ích! Mình đã mua được bộ LEGO phù hợp cho bé.'),
(2, 5, 4, N'Hướng dẫn rõ ràng, có ích cho các phụ huynh mới.');
GO

-- ReviewBlogReactions
INSERT INTO [dbo].[ReviewBlogReactions]
([ReviewBlogID], [AccountID], [ReactionType])
VALUES
(1, 5, 'Like'),
(2, 4, 'Like'),
(2, 6, 'Like');
GO

-- ReviewBlogReplies
INSERT INTO [dbo].[ReviewBlogReplies]
([ReviewBlogID], [AccountID], [Content])
VALUES
(1, 2, N'Cảm ơn bạn đã đọc! Chúc bé chơi vui nhé 😊');
GO

-- ============= Notification.Deliveries =============

INSERT INTO [Notification].[Deliveries]
([AccountID], [CreatedByJobID], [TemplateCode], [Title], [Message], [Payload], [Status])
VALUES
(4, NULL, 'ORDER_CONFIRMED',
N'Đơn hàng #1 đã xác nhận',
N'Đơn hàng #1 của bạn đã được xác nhận. Chúng tôi đang chuẩn bị hàng.',
'{"OrderId":1}', 'Read'),

(4, NULL, 'ORDER_DELIVERED',
N'Đơn hàng #1 đã giao thành công',
N'Đơn hàng #1 đã được giao thành công. Cảm ơn bạn đã mua hàng!',
'{"OrderId":1}', 'Read'),

(5, NULL, 'ORDER_SHIPPED',
N'Đơn hàng #2 đang giao',
N'Đơn hàng #2 đã được bàn giao cho đơn vị vận chuyển. Mã vận đơn: GHTK987654321.',
'{"OrderId":2,"TrackingNumber":"GHTK987654321"}', 'Unread'),

(5, NULL, 'WALLET_TOPUP',
N'Nạp tiền thành công',
N'Ví của bạn đã được nạp 1.500.000 VND. Số dư hiện tại: 1.500.000 VND.',
'{"Amount":1500000,"Balance":1500000}', 'Read');
GO
