USE [ASP_LorKingDom3]
GO

-- === THÊM LỆNH NÀY ĐỂ TẠO SCHEMA ===
CREATE SCHEMA [Notification]
GO
CREATE SCHEMA [System]
GO
-- =====================================

/****** Object:  Table [dbo].[Accounts]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accounts](
	[AccountID] [int] IDENTITY(1,1) NOT NULL,
	[RoleID] [int] NULL,
	[AccountName] [nvarchar](100) NOT NULL,
	[PhoneNumber] [varchar](15) NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Image] [nvarchar](500) NULL,
	[Password] [nvarchar](255) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Status] [nvarchar](10) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[Provider] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Addresses]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Addresses](
	[AddressID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[AddressLine] [nvarchar](500) NOT NULL,
	[City] [nvarchar](100) NOT NULL,
	[Ward] [nvarchar](100) NULL,
	[IsDefault] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[District] [nvarchar](100) NULL,
	[RecipientName] [nvarchar](100) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[DistrictId] [int] NULL,
	[WardCode] [varchar](20) NULL,
	[ProvinceId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[AddressID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ages]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ages](
	[AgeID] [int] IDENTITY(1,1) NOT NULL,
	[AgeRange] [nvarchar](50) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AgeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[AgeRange] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BlogCategories]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlogCategories](
	[BlogCategoryID] [int] IDENTITY(1,1) NOT NULL,
	[BlogCategoryName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[BlogCategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[BlogCategoryName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BlogPostCategories]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlogPostCategories](
	[BlogPostID] [int] NOT NULL,
	[BlogCategoryID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BlogPostID] ASC,
	[BlogCategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BlogPosts]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlogPosts](
	[BlogPostID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[BlogTitle] [nvarchar](255) NOT NULL,
	[BlogContent] [nvarchar](max) NOT NULL,
	[BlogThumbnail] [nvarchar](500) NULL,
	[BlogUrl] [nvarchar](max) NULL,
	[IsPublished] [bit] NOT NULL,
	[IsFeatured] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[BlogPostID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Brands]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Brands](
	[BrandID] [int] IDENTITY(1,1) NOT NULL,
	[BrandName] [nvarchar](100) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BrandID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[BrandName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cart]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cart](
	[CartID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[CartID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CartItems]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CartItems](
	[CartItemID] [int] IDENTITY(1,1) NOT NULL,
	[CartID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[PriceAtThatTime] [decimal](12, 2) NOT NULL,
	[Status] [nvarchar](15) NOT NULL,
	[AddedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CartItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[SuperCategoryID] [int] NOT NULL,
	[CategoryName] [nvarchar](255) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmailOtps]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailOtps](
	[EmailOtpID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NULL,
	[Email] [nvarchar](255) NULL,
	[Purpose] [nvarchar](50) NOT NULL,
	[OtpCode] [nvarchar](10) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ExpiresAt] [datetime] NOT NULL,
	[IsUsed] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[EmailOtpID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExternalApiLogs]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExternalApiLogs](
	[LogID] [bigint] IDENTITY(1,1) NOT NULL,
	[Provider] [nvarchar](50) NOT NULL,
	[Endpoint] [nvarchar](500) NULL,
	[Method] [nvarchar](10) NULL,
	[RequestPayload] [nvarchar](max) NULL,
	[ResponsePayload] [nvarchar](max) NULL,
	[StatusCode] [int] NULL,
	[CreatedAt] [datetime] NOT NULL,
	[CreatedBy] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[LogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExternalLogins]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExternalLogins](
	[ExternalLoginID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[Provider] [nvarchar](100) NOT NULL,
	[ProviderKey] [nvarchar](255) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ExternalLoginID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_ExternalLogins] UNIQUE NONCLUSTERED 
(
	[Provider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Materials]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Materials](
	[MaterialID] [int] IDENTITY(1,1) NOT NULL,
	[MaterialName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaterialID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDetails]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetails](
	[OrderDetailID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[UnitPrice] [decimal](12, 2) NOT NULL,
	[Total]  AS ([Quantity]*[UnitPrice]) PERSISTED,
	[Reviewed] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderRefunds]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderRefunds](
	[RefundID] [bigint] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[RequestedBy] [int] NULL,
	[ApprovedBy] [int] NULL,
	[WalletTransactionID] [bigint] NULL,
	[RefundMode] [nvarchar](20) NOT NULL,
	[RefundStatus] [nvarchar](20) NOT NULL,
	[TotalAmount] [decimal](12, 2) NOT NULL,
	[RefundAmount] [decimal](12, 2) NOT NULL,
	[Reason] [nvarchar](500) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ApprovedAt] [datetime] NULL,
	[ProcessedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[RefundID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[VoucherID] [int] NULL,
	[StatusID] [int] NOT NULL,
	[ShippingName] [nvarchar](100) NULL,
	[ShippingPhone] [nvarchar](20) NULL,
	[ShippingAddressLine] [nvarchar](500) NULL,
	[ShippingCity] [nvarchar](100) NULL,
	[ShippingWard] [nvarchar](100) NULL,
	[ShippingMethod] [nvarchar](100) NULL,
	[ShippingFee] [decimal](12, 2) NOT NULL,
	[OrderDate] [datetime] NOT NULL,
	[TotalAmount] [decimal](12, 2) NOT NULL,
	[PaidByWalletAmount] [decimal](14, 2) NOT NULL,
	[PaidByExternalAmount] [decimal](14, 2) NOT NULL,
	[PaymentCompletedAt] [datetime] NULL,
	[RefundStatus] [nvarchar](15) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[ShippingDistrict] [nvarchar](255) NULL,
	[ShippingDistrictId] [int] NULL,
	[ShippingWardCode] [varchar](20) NULL,
	[ShippingProvinceId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderStatusHistory]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderStatusHistory](
	[OrderStatusHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NOT NULL,
	[StatusID] [int] NULL,
	[ChangedAt] [datetime] NOT NULL,
	[ChangedBy] [int] NULL,
	[Note] [nvarchar](500) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderStatusHistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Origins]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Origins](
	[OriginID] [int] IDENTITY(1,1) NOT NULL,
	[OriginName] [nvarchar](255) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OriginID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[OriginName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentGatewayTransactions]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentGatewayTransactions](
	[GatewayTransactionID] [bigint] IDENTITY(1,1) NOT NULL,
	[PaymentHistoryID] [bigint] NOT NULL,
	[Provider] [varchar](50) NOT NULL,
	[TransactionID] [varchar](200) NULL,
	[PaymentUrl] [varchar](1000) NULL,
	[Status] [varchar](50) NOT NULL,
	[Amount] [decimal](14, 2) NOT NULL,
	[GatewayResponse] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[CompletedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[GatewayTransactionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentHistory]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentHistory](
	[PaymentHistoryID] [bigint] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[PaymentMethod] [nvarchar](50) NOT NULL,
	[PaymentStatus] [nvarchar](50) NOT NULL,
	[TransactionCode] [nvarchar](100) NULL,
	[Amount] [decimal](14, 2) NOT NULL,
	[Currency] [nvarchar](3) NOT NULL,
	[WalletTransactionID] [bigint] NULL,
	[Note] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PaymentHistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PriceRanges]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PriceRanges](
	[PriceRangeID] [int] IDENTITY(1,1) NOT NULL,
	[PriceRangeMin] [decimal](12, 2) NOT NULL,
	[PriceRangeMax] [decimal](12, 2) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PriceRangeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductImages]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductImages](
	[ImageID] [int] IDENTITY(1,1) NOT NULL,
	[ProductID] [int] NOT NULL,
	[ImageUrl] [nvarchar](500) NOT NULL,
	[IsMain] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[SKU] [varchar](50) NOT NULL,
	[CategoryID] [int] NULL,
	[MaterialID] [int] NULL,
	[AgeID] [int] NULL,
	[SexID] [int] NULL,
	[PriceRangeID] [int] NULL,
	[BrandID] [int] NULL,
	[OriginID] [int] NULL,
	[ProductName] [nvarchar](255) NOT NULL,
	[Price] [decimal](12, 2) NOT NULL,
	[Quantity] [int] NOT NULL,
	[ProductStatus] [nvarchar](15) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[SKU] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReviewBlogReactions]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewBlogReactions](
	[ReactionBlogID] [int] IDENTITY(1,1) NOT NULL,
	[ReviewBlogID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[ReactionType] [nvarchar](10) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ReactionBlogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_ReviewBlogReactions] UNIQUE NONCLUSTERED 
(
	[ReviewBlogID] ASC,
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReviewBlogReplies]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewBlogReplies](
	[ReplyBlogID] [int] IDENTITY(1,1) NOT NULL,
	[ReviewBlogID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[Content] [nvarchar](1000) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ReplyBlogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReviewBlogs]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewBlogs](
	[ReviewBlogID] [int] IDENTITY(1,1) NOT NULL,
	[BlogPostID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[Rating] [int] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[IsBlocked] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ReviewBlogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_ReviewBlogs_PostAccount] UNIQUE NONCLUSTERED 
(
	[AccountID] ASC,
	[BlogPostID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReviewModerationLogs]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewModerationLogs](
	[ModerationLogID] [int] IDENTITY(1,1) NOT NULL,
	[ReviewProductID] [int] NOT NULL,
	[Stage] [nvarchar](50) NOT NULL,
	[Result] [nvarchar](50) NOT NULL,
	[Score] [decimal](3, 2) NULL,
	[Details] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ModerationLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReviewProductImages]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewProductImages](
	[ReviewProductImageID] [int] IDENTITY(1,1) NOT NULL,
	[ReviewProductID] [int] NOT NULL,
	[ImageUrl] [nvarchar](500) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReviewProductImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReviewProductReactions]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewProductReactions](
	[ReactionProductID] [int] IDENTITY(1,1) NOT NULL,
	[ReviewProductID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[ReactionType] [nvarchar](10) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ReactionProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_ReviewProductReactions] UNIQUE NONCLUSTERED 
(
	[ReviewProductID] ASC,
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReviewProductReplies]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewProductReplies](
	[ReplyProductID] [int] IDENTITY(1,1) NOT NULL,
	[ReviewProductID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[Content] [nvarchar](1000) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ReplyProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReviewProducts]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReviewProducts](
	[ReviewProductID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[OrderDetailID] [int] NULL,
	[Rating] [int] NOT NULL,
	[Comment] [nvarchar](1000) NULL,
	[IsVerifiedPurchase] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[Visibility] [nvarchar](20) NOT NULL,
	[ModerationScore] [decimal](3, 2) NULL,
	[ModerationDetail] [nvarchar](max) NULL,
	[EditCount] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReviewProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sexes]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sexes](
	[SexID] [int] IDENTITY(1,1) NOT NULL,
	[SexName] [nvarchar](20) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SexID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[SexName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ShippingProviderTransactions]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShippingProviderTransactions](
	[ShippingTransactionID] [bigint] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NOT NULL,
	[Provider] [varchar](50) NOT NULL,
	[ProviderOrderCode] [varchar](100) NULL,
	[TrackingNumber] [varchar](100) NULL,
	[ServiceType] [nvarchar](100) NULL,
	[Status] [varchar](50) NULL,
	[ShippingFee] [decimal](12, 2) NULL,
	[EstimatedDelivery] [datetime] NULL,
	[ActualDelivery] [datetime] NULL,
	[Metadata] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ShippingTransactionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StatusOrders]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StatusOrders](
	[StatusID] [int] IDENTITY(1,1) NOT NULL,
	[StatusName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[StatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SuperCategories]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SuperCategories](
	[SuperCategoryID] [int] IDENTITY(1,1) NOT NULL,
	[SuperCategoryName] [nvarchar](255) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SuperCategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[SuperCategoryName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemConfigs]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemConfigs](
	[ConfigKey] [nvarchar](100) NOT NULL,
	[ConfigValue] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ConfigKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Voucher]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Voucher](
	[VoucherID] [int] IDENTITY(1,1) NOT NULL,
	[VoucherTypeID] [int] NOT NULL,
	[CreateBy] [int] NULL,
	[VoucherCode] [nvarchar](50) NOT NULL,
	[DiscountType] [nvarchar](10) NOT NULL DEFAULT 'Fixed',
	[DiscountValue] [decimal](12, 2) NOT NULL,
	[MaxDiscountAmount] [decimal](12, 2) NULL,
	[MinOrderAmount] [decimal](12, 2) NULL,
	[UsageLimitPerUser] [int] NULL,
	[IsStackable] [bit] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[Status] [nvarchar](15) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[VoucherID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[VoucherCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VoucherTypes]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VoucherTypes](
	[VoucherTypeID] [int] IDENTITY(1,1) NOT NULL,
	[VoucherTypeName] [nvarchar](255) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[VoucherTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Wallets]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Wallets](
	[WalletID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[Currency] [nvarchar](3) NOT NULL,
	[Balance] [decimal](14, 2) NOT NULL,
	[Status] [nvarchar](10) NOT NULL,
	[LastTransactionAt] [datetime] NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[WalletID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WalletTransactions]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WalletTransactions](
	[WalletTransactionID] [bigint] IDENTITY(1,1) NOT NULL,
	[WalletID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[TxnType] [nvarchar](20) NOT NULL,
	[Direction] [nvarchar](3) NOT NULL,
	[Amount] [decimal](14, 2) NOT NULL,
	[BalanceBefore] [decimal](14, 2) NOT NULL,
	[BalanceAfter] [decimal](14, 2) NOT NULL,
	[RelatedOrderID] [int] NULL,
	[RelatedPaymentHistoryID] [bigint] NULL,
	[Method] [nvarchar](30) NULL,
	[ExternalRef] [nvarchar](100) NULL,
	[IdempotencyKey] [nvarchar](100) NULL,
	[Status] [nvarchar](12) NOT NULL,
	[Reason] [nvarchar](255) NULL,
	[Metadata] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[CompletedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[WalletTransactionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebhookEvents]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebhookEvents](
	[WebhookEventID] [bigint] IDENTITY(1,1) NOT NULL,
	[Provider] [varchar](50) NOT NULL,
	[EventType] [varchar](100) NOT NULL,
	[Payload] [nvarchar](max) NOT NULL,
	[Signature] [varchar](500) NULL,
	[Status] [varchar](20) NOT NULL,
	[ProcessedAt] [datetime] NULL,
	[ErrorMessage] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[WebhookEventID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Wishlists]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Wishlists](
	[WishlistID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[WishlistID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Wishlists] UNIQUE NONCLUSTERED 
(
	[AccountID] ASC,
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Notification].[Deliveries]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Notification].[Deliveries](
	[DeliveryID] [bigint] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[CreatedByJobID] [int] NULL,
	[TemplateCode] [varchar](50) NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Message] [nvarchar](500) NOT NULL,
	[Payload] [nvarchar](1000) NOT NULL,
	[Status] [varchar](10) NOT NULL,
	[CreatedAt] [datetime2](0) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DeliveryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Notification].[Templates]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Notification].[Templates](
	[TemplateID] [smallint] IDENTITY(1,1) NOT NULL,
	[TemplateCode] [varchar](50) NOT NULL,
	[TitleTemplate] [nvarchar](255) NOT NULL,
	[MessageTemplate] [nvarchar](500) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](0) NOT NULL,
	[UpdatedAt] [datetime2](0) NULL,
PRIMARY KEY CLUSTERED 
(
	[TemplateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[TemplateCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [System].[BackgroundJobs]    Script Date: 2/25/2026 7:52:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [System].[BackgroundJobs](
	[JobID] [int] IDENTITY(1,1) NOT NULL,
	[JobName] [nvarchar](100) NOT NULL,
	[CronExpression] [nvarchar](100) NULL,
	[IsEnabled] [bit] NOT NULL,
	[LastRunTime] [datetime2](7) NULL,
	[NextRunTime] [datetime2](7) NULL,
	[LastRunStatus] [nvarchar](20) NULL,
	[LastRunMessage] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[JobID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[JobName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Accounts] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Accounts] ADD  DEFAULT ('Active') FOR [Status]
GO
ALTER TABLE [dbo].[Accounts] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Addresses] ADD  DEFAULT ((0)) FOR [IsDefault]
GO
ALTER TABLE [dbo].[Addresses] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Addresses] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Ages] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Ages] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[BlogCategories] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[BlogCategories] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[BlogPosts] ADD  DEFAULT ((0)) FOR [IsPublished]
GO
ALTER TABLE [dbo].[BlogPosts] ADD  DEFAULT ((0)) FOR [IsFeatured]
GO
ALTER TABLE [dbo].[BlogPosts] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[BlogPosts] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Brands] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Brands] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Cart] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[CartItems] ADD  DEFAULT ('Active') FOR [Status]
GO
ALTER TABLE [dbo].[CartItems] ADD  DEFAULT (getdate()) FOR [AddedAt]
GO
ALTER TABLE [dbo].[Categories] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Categories] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[EmailOtps] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[EmailOtps] ADD  DEFAULT ((0)) FOR [IsUsed]
GO
ALTER TABLE [dbo].[ExternalApiLogs] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ExternalLogins] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Materials] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Materials] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[OrderDetails] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[OrderDetails] ADD  DEFAULT ((0)) FOR [Reviewed]
GO
ALTER TABLE [dbo].[OrderDetails] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[OrderRefunds] ADD  DEFAULT ('Wallet') FOR [RefundMode]
GO
ALTER TABLE [dbo].[OrderRefunds] ADD  DEFAULT ('Requested') FOR [RefundStatus]
GO
ALTER TABLE [dbo].[OrderRefunds] ADD  DEFAULT ((0)) FOR [RefundAmount]
GO
ALTER TABLE [dbo].[OrderRefunds] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ((0)) FOR [ShippingFee]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getdate()) FOR [OrderDate]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ((0)) FOR [PaidByWalletAmount]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ((0)) FOR [PaidByExternalAmount]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ('None') FOR [RefundStatus]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[OrderStatusHistory] ADD  DEFAULT (getdate()) FOR [ChangedAt]
GO
ALTER TABLE [dbo].[OrderStatusHistory] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Origins] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Origins] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[PaymentGatewayTransactions] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[PaymentHistory] ADD  DEFAULT ('Failed') FOR [PaymentStatus]
GO
ALTER TABLE [dbo].[PaymentHistory] ADD  DEFAULT ((0)) FOR [Amount]
GO
ALTER TABLE [dbo].[PaymentHistory] ADD  DEFAULT ('VND') FOR [Currency]
GO
ALTER TABLE [dbo].[PaymentHistory] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[PriceRanges] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[PriceRanges] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ProductImages] ADD  DEFAULT ((0)) FOR [IsMain]
GO
ALTER TABLE [dbo].[ProductImages] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [Quantity]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ('Available') FOR [ProductStatus]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewBlogReactions] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewBlogReplies] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewBlogs] ADD  DEFAULT ((0)) FOR [IsBlocked]
GO
ALTER TABLE [dbo].[ReviewBlogs] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewModerationLogs] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewProductImages] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ReviewProductImages] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewProductReactions] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ReviewProductReactions] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewProductReplies] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ReviewProductReplies] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ReviewProducts] ADD  DEFAULT ((0)) FOR [IsVerifiedPurchase]
GO
ALTER TABLE [dbo].[ReviewProducts] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ReviewProducts] ADD  DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[ReviewProducts] ADD  DEFAULT ('AuthorOnly') FOR [Visibility]
GO
ALTER TABLE [dbo].[ReviewProducts] ADD  DEFAULT ((0)) FOR [EditCount]
GO
ALTER TABLE [dbo].[ReviewProducts] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Roles] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Sexes] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Sexes] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ShippingProviderTransactions] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[StatusOrders] ADD  DEFAULT ('Pending') FOR [StatusName]
GO
ALTER TABLE [dbo].[StatusOrders] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SuperCategories] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[SuperCategories] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SystemConfigs] ADD  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Voucher] ADD  DEFAULT ((0)) FOR [IsStackable]
GO
ALTER TABLE [dbo].[Voucher] ADD  DEFAULT ('Active') FOR [Status]
GO
ALTER TABLE [dbo].[Voucher] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Voucher] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[VoucherTypes] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Wallets] ADD  DEFAULT ('VND') FOR [Currency]
GO
ALTER TABLE [dbo].[Wallets] ADD  DEFAULT ((0)) FOR [Balance]
GO
ALTER TABLE [dbo].[Wallets] ADD  DEFAULT ('Active') FOR [Status]
GO
ALTER TABLE [dbo].[Wallets] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[WalletTransactions] ADD  DEFAULT ('Completed') FOR [Status]
GO
ALTER TABLE [dbo].[WalletTransactions] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[WebhookEvents] ADD  DEFAULT ('Pending') FOR [Status]
GO
ALTER TABLE [dbo].[WebhookEvents] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Wishlists] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [Notification].[Deliveries] ADD  DEFAULT ('Unread') FOR [Status]
GO
ALTER TABLE [Notification].[Deliveries] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [Notification].[Templates] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [Notification].[Templates] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [System].[BackgroundJobs] ADD  DEFAULT ((1)) FOR [IsEnabled]
GO
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD  CONSTRAINT [FK_Accounts_Roles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [FK_Accounts_Roles]
GO
ALTER TABLE [dbo].[Addresses]  WITH CHECK ADD  CONSTRAINT [FK_Addresses_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Addresses] CHECK CONSTRAINT [FK_Addresses_Accounts]
GO
ALTER TABLE [dbo].[BlogPostCategories]  WITH CHECK ADD  CONSTRAINT [FK_BlogPostCategories_Categories] FOREIGN KEY([BlogCategoryID])
REFERENCES [dbo].[BlogCategories] ([BlogCategoryID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BlogPostCategories] CHECK CONSTRAINT [FK_BlogPostCategories_Categories]
GO
ALTER TABLE [dbo].[BlogPostCategories]  WITH CHECK ADD  CONSTRAINT [FK_BlogPostCategories_Posts] FOREIGN KEY([BlogPostID])
REFERENCES [dbo].[BlogPosts] ([BlogPostID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BlogPostCategories] CHECK CONSTRAINT [FK_BlogPostCategories_Posts]
GO
ALTER TABLE [dbo].[BlogPosts]  WITH CHECK ADD  CONSTRAINT [FK_BlogPosts_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[BlogPosts] CHECK CONSTRAINT [FK_BlogPosts_Accounts]
GO
ALTER TABLE [dbo].[Cart]  WITH CHECK ADD  CONSTRAINT [FK_Cart_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Cart] CHECK CONSTRAINT [FK_Cart_Accounts]
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD  CONSTRAINT [FK_CartItems_Cart] FOREIGN KEY([CartID])
REFERENCES [dbo].[Cart] ([CartID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CartItems] CHECK CONSTRAINT [FK_CartItems_Cart]
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD  CONSTRAINT [FK_CartItems_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[CartItems] CHECK CONSTRAINT [FK_CartItems_Products]
GO
ALTER TABLE [dbo].[Categories]  WITH CHECK ADD  CONSTRAINT [FK_Categories_SuperCategories] FOREIGN KEY([SuperCategoryID])
REFERENCES [dbo].[SuperCategories] ([SuperCategoryID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Categories] CHECK CONSTRAINT [FK_Categories_SuperCategories]
GO
ALTER TABLE [dbo].[EmailOtps]  WITH CHECK ADD  CONSTRAINT [FK_EmailOtps_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmailOtps] CHECK CONSTRAINT [FK_EmailOtps_Accounts]
GO
ALTER TABLE [dbo].[ExternalLogins]  WITH CHECK ADD  CONSTRAINT [FK_ExternalLogins_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ExternalLogins] CHECK CONSTRAINT [FK_ExternalLogins_Accounts]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Orders]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Products]
GO
ALTER TABLE [dbo].[OrderRefunds]  WITH CHECK ADD  CONSTRAINT [FK_Refunds_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[OrderRefunds] CHECK CONSTRAINT [FK_Refunds_Accounts]
GO
ALTER TABLE [dbo].[OrderRefunds]  WITH CHECK ADD  CONSTRAINT [FK_Refunds_ApprovedBy] FOREIGN KEY([ApprovedBy])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[OrderRefunds] CHECK CONSTRAINT [FK_Refunds_ApprovedBy]
GO
ALTER TABLE [dbo].[OrderRefunds]  WITH CHECK ADD  CONSTRAINT [FK_Refunds_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderRefunds] CHECK CONSTRAINT [FK_Refunds_Orders]
GO
ALTER TABLE [dbo].[OrderRefunds]  WITH CHECK ADD  CONSTRAINT [FK_Refunds_RequestedBy] FOREIGN KEY([RequestedBy])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[OrderRefunds] CHECK CONSTRAINT [FK_Refunds_RequestedBy]
GO
ALTER TABLE [dbo].[OrderRefunds]  WITH CHECK ADD  CONSTRAINT [FK_Refunds_WalletTxn] FOREIGN KEY([WalletTransactionID])
REFERENCES [dbo].[WalletTransactions] ([WalletTransactionID])
GO
ALTER TABLE [dbo].[OrderRefunds] CHECK CONSTRAINT [FK_Refunds_WalletTxn]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Accounts]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_StatusOrder] FOREIGN KEY([StatusID])
REFERENCES [dbo].[StatusOrders] ([StatusID])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_StatusOrder]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Vouchers] FOREIGN KEY([VoucherID])
REFERENCES [dbo].[Voucher] ([VoucherID])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Vouchers]
GO
ALTER TABLE [dbo].[OrderStatusHistory]  WITH CHECK ADD  CONSTRAINT [FK_OSH_ChangedBy] FOREIGN KEY([ChangedBy])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[OrderStatusHistory] CHECK CONSTRAINT [FK_OSH_ChangedBy]
GO
ALTER TABLE [dbo].[OrderStatusHistory]  WITH CHECK ADD  CONSTRAINT [FK_OSH_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderStatusHistory] CHECK CONSTRAINT [FK_OSH_Orders]
GO
ALTER TABLE [dbo].[OrderStatusHistory]  WITH CHECK ADD  CONSTRAINT [FK_OSH_StatusOrder] FOREIGN KEY([StatusID])
REFERENCES [dbo].[StatusOrders] ([StatusID])
GO
ALTER TABLE [dbo].[OrderStatusHistory] CHECK CONSTRAINT [FK_OSH_StatusOrder]
GO
ALTER TABLE [dbo].[PaymentGatewayTransactions]  WITH CHECK ADD  CONSTRAINT [FK_GatewayTxn_PaymentHistory] FOREIGN KEY([PaymentHistoryID])
REFERENCES [dbo].[PaymentHistory] ([PaymentHistoryID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PaymentGatewayTransactions] CHECK CONSTRAINT [FK_GatewayTxn_PaymentHistory]
GO
ALTER TABLE [dbo].[PaymentHistory]  WITH CHECK ADD  CONSTRAINT [FK_PaymentHistory_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[PaymentHistory] CHECK CONSTRAINT [FK_PaymentHistory_Accounts]
GO
ALTER TABLE [dbo].[PaymentHistory]  WITH CHECK ADD  CONSTRAINT [FK_PaymentHistory_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PaymentHistory] CHECK CONSTRAINT [FK_PaymentHistory_Orders]
GO
ALTER TABLE [dbo].[PaymentHistory]  WITH CHECK ADD  CONSTRAINT [FK_PaymentHistory_WalletTxn] FOREIGN KEY([WalletTransactionID])
REFERENCES [dbo].[WalletTransactions] ([WalletTransactionID])
GO
ALTER TABLE [dbo].[PaymentHistory] CHECK CONSTRAINT [FK_PaymentHistory_WalletTxn]
GO
ALTER TABLE [dbo].[ProductImages]  WITH CHECK ADD  CONSTRAINT [FK_ProductImages_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductImages] CHECK CONSTRAINT [FK_ProductImages_Products]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Ages] FOREIGN KEY([AgeID])
REFERENCES [dbo].[Ages] ([AgeID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Ages]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Brands] FOREIGN KEY([BrandID])
REFERENCES [dbo].[Brands] ([BrandID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Brands]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Categories] FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Categories] ([CategoryID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Categories]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Materials] FOREIGN KEY([MaterialID])
REFERENCES [dbo].[Materials] ([MaterialID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Materials]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Origins] FOREIGN KEY([OriginID])
REFERENCES [dbo].[Origins] ([OriginID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Origins]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_PriceRanges] FOREIGN KEY([PriceRangeID])
REFERENCES [dbo].[PriceRanges] ([PriceRangeID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_PriceRanges]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Sexes] FOREIGN KEY([SexID])
REFERENCES [dbo].[Sexes] ([SexID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Sexes]
GO
ALTER TABLE [dbo].[ReviewBlogReactions]  WITH CHECK ADD  CONSTRAINT [FK_ReviewBlogReactions_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[ReviewBlogReactions] CHECK CONSTRAINT [FK_ReviewBlogReactions_Accounts]
GO
ALTER TABLE [dbo].[ReviewBlogReactions]  WITH CHECK ADD  CONSTRAINT [FK_ReviewBlogReactions_ReviewBlogs] FOREIGN KEY([ReviewBlogID])
REFERENCES [dbo].[ReviewBlogs] ([ReviewBlogID])
GO
ALTER TABLE [dbo].[ReviewBlogReactions] CHECK CONSTRAINT [FK_ReviewBlogReactions_ReviewBlogs]
GO
ALTER TABLE [dbo].[ReviewBlogReplies]  WITH CHECK ADD  CONSTRAINT [FK_ReviewBlogReplies_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[ReviewBlogReplies] CHECK CONSTRAINT [FK_ReviewBlogReplies_Accounts]
GO
ALTER TABLE [dbo].[ReviewBlogReplies]  WITH CHECK ADD  CONSTRAINT [FK_ReviewBlogReplies_ReviewBlogs] FOREIGN KEY([ReviewBlogID])
REFERENCES [dbo].[ReviewBlogs] ([ReviewBlogID])
GO
ALTER TABLE [dbo].[ReviewBlogReplies] CHECK CONSTRAINT [FK_ReviewBlogReplies_ReviewBlogs]
GO
ALTER TABLE [dbo].[ReviewBlogs]  WITH CHECK ADD  CONSTRAINT [FK_ReviewBlogs_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReviewBlogs] CHECK CONSTRAINT [FK_ReviewBlogs_Accounts]
GO
ALTER TABLE [dbo].[ReviewBlogs]  WITH CHECK ADD  CONSTRAINT [FK_ReviewBlogs_BlogPosts] FOREIGN KEY([BlogPostID])
REFERENCES [dbo].[BlogPosts] ([BlogPostID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReviewBlogs] CHECK CONSTRAINT [FK_ReviewBlogs_BlogPosts]
GO
ALTER TABLE [dbo].[ReviewModerationLogs]  WITH CHECK ADD  CONSTRAINT [FK_RML_ReviewProduct] FOREIGN KEY([ReviewProductID])
REFERENCES [dbo].[ReviewProducts] ([ReviewProductID])
GO
ALTER TABLE [dbo].[ReviewModerationLogs] CHECK CONSTRAINT [FK_RML_ReviewProduct]
GO
ALTER TABLE [dbo].[ReviewProductImages]  WITH CHECK ADD  CONSTRAINT [FK_ReviewProdImages_ReviewProducts] FOREIGN KEY([ReviewProductID])
REFERENCES [dbo].[ReviewProducts] ([ReviewProductID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReviewProductImages] CHECK CONSTRAINT [FK_ReviewProdImages_ReviewProducts]
GO
ALTER TABLE [dbo].[ReviewProductReactions]  WITH CHECK ADD  CONSTRAINT [FK_ReviewProdReactions_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[ReviewProductReactions] CHECK CONSTRAINT [FK_ReviewProdReactions_Accounts]
GO
ALTER TABLE [dbo].[ReviewProductReactions]  WITH CHECK ADD  CONSTRAINT [FK_ReviewProdReactions_ReviewProducts] FOREIGN KEY([ReviewProductID])
REFERENCES [dbo].[ReviewProducts] ([ReviewProductID])
GO
ALTER TABLE [dbo].[ReviewProductReactions] CHECK CONSTRAINT [FK_ReviewProdReactions_ReviewProducts]
GO
ALTER TABLE [dbo].[ReviewProductReplies]  WITH CHECK ADD  CONSTRAINT [FK_ReviewProdReplies_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[ReviewProductReplies] CHECK CONSTRAINT [FK_ReviewProdReplies_Accounts]
GO
ALTER TABLE [dbo].[ReviewProductReplies]  WITH CHECK ADD  CONSTRAINT [FK_ReviewProdReplies_ReviewProducts] FOREIGN KEY([ReviewProductID])
REFERENCES [dbo].[ReviewProducts] ([ReviewProductID])
GO
ALTER TABLE [dbo].[ReviewProductReplies] CHECK CONSTRAINT [FK_ReviewProdReplies_ReviewProducts]
GO
ALTER TABLE [dbo].[ReviewProducts]  WITH CHECK ADD  CONSTRAINT [FK_ReviewProducts_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ReviewProducts] CHECK CONSTRAINT [FK_ReviewProducts_Accounts]
GO
ALTER TABLE [dbo].[ReviewProducts]  WITH CHECK ADD  CONSTRAINT [FK_ReviewProducts_OrderDetails] FOREIGN KEY([OrderDetailID])
REFERENCES [dbo].[OrderDetails] ([OrderDetailID])
GO
ALTER TABLE [dbo].[ReviewProducts] CHECK CONSTRAINT [FK_ReviewProducts_OrderDetails]
GO
ALTER TABLE [dbo].[ReviewProducts]  WITH CHECK ADD  CONSTRAINT [FK_ReviewProducts_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[ReviewProducts] CHECK CONSTRAINT [FK_ReviewProducts_Products]
GO
ALTER TABLE [dbo].[ShippingProviderTransactions]  WITH CHECK ADD  CONSTRAINT [FK_ShippingTxn_Orders] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShippingProviderTransactions] CHECK CONSTRAINT [FK_ShippingTxn_Orders]
GO
ALTER TABLE [dbo].[Voucher]  WITH CHECK ADD  CONSTRAINT [FK_Voucher_Accounts_CreateBy] FOREIGN KEY([CreateBy])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Voucher] CHECK CONSTRAINT [FK_Voucher_Accounts_CreateBy]
GO
ALTER TABLE [dbo].[Voucher]  WITH CHECK ADD  CONSTRAINT [FK_Voucher_VoucherTypes] FOREIGN KEY([VoucherTypeID])
REFERENCES [dbo].[VoucherTypes] ([VoucherTypeID])
GO
ALTER TABLE [dbo].[Voucher] CHECK CONSTRAINT [FK_Voucher_VoucherTypes]
GO
ALTER TABLE [dbo].[Wallets]  WITH CHECK ADD  CONSTRAINT [FK_Wallets_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Wallets] CHECK CONSTRAINT [FK_Wallets_Accounts]
GO
ALTER TABLE [dbo].[WalletTransactions]  WITH CHECK ADD  CONSTRAINT [FK_WalletTxn_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[WalletTransactions] CHECK CONSTRAINT [FK_WalletTxn_Accounts]
GO
ALTER TABLE [dbo].[WalletTransactions]  WITH CHECK ADD  CONSTRAINT [FK_WalletTxn_Orders] FOREIGN KEY([RelatedOrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[WalletTransactions] CHECK CONSTRAINT [FK_WalletTxn_Orders]
GO
ALTER TABLE [dbo].[WalletTransactions]  WITH CHECK ADD  CONSTRAINT [FK_WalletTxn_Wallets] FOREIGN KEY([WalletID])
REFERENCES [dbo].[Wallets] ([WalletID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WalletTransactions] CHECK CONSTRAINT [FK_WalletTxn_Wallets]
GO
ALTER TABLE [dbo].[Wishlists]  WITH CHECK ADD  CONSTRAINT [FK_Wishlists_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Wishlists] CHECK CONSTRAINT [FK_Wishlists_Accounts]
GO
ALTER TABLE [dbo].[Wishlists]  WITH CHECK ADD  CONSTRAINT [FK_Wishlists_Products] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Wishlists] CHECK CONSTRAINT [FK_Wishlists_Products]
GO
ALTER TABLE [Notification].[Deliveries]  WITH CHECK ADD  CONSTRAINT [FK_NotificationDeliveries_Accounts] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [Notification].[Deliveries] CHECK CONSTRAINT [FK_NotificationDeliveries_Accounts]
GO
ALTER TABLE [Notification].[Deliveries]  WITH CHECK ADD  CONSTRAINT [FK_NotificationDeliveries_BackgroundJobs] FOREIGN KEY([CreatedByJobID])
REFERENCES [System].[BackgroundJobs] ([JobID])
GO
ALTER TABLE [Notification].[Deliveries] CHECK CONSTRAINT [FK_NotificationDeliveries_BackgroundJobs]
GO
ALTER TABLE [Notification].[Deliveries]  WITH CHECK ADD  CONSTRAINT [FK_NotificationDeliveries_Templates] FOREIGN KEY([TemplateCode])
REFERENCES [Notification].[Templates] ([TemplateCode])
GO
ALTER TABLE [Notification].[Deliveries] CHECK CONSTRAINT [FK_NotificationDeliveries_Templates]
GO
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD CHECK  (([Status]='Blocked' OR [Status]='Inactive' OR [Status]='Active'))
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD CHECK  (([PriceAtThatTime]>=(0)))
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD CHECK  (([Quantity]>(0)))
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD CHECK  (([Status]='Purchased' OR [Status]='Removed' OR [Status]='Active'))
GO
ALTER TABLE [dbo].[EmailOtps]  WITH CHECK ADD  CONSTRAINT [CK_EmailOtps_Target] CHECK  (([AccountID] IS NOT NULL OR [Email] IS NOT NULL))
GO
ALTER TABLE [dbo].[EmailOtps] CHECK CONSTRAINT [CK_EmailOtps_Target]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD CHECK  (([Quantity]>(0)))
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD CHECK  (([UnitPrice]>=(0)))
GO
ALTER TABLE [dbo].[OrderRefunds]  WITH CHECK ADD  CONSTRAINT [CK_OrderRefunds_Amounts] CHECK  (([RefundAmount]<=[TotalAmount]))
GO
ALTER TABLE [dbo].[OrderRefunds] CHECK CONSTRAINT [CK_OrderRefunds_Amounts]
GO
ALTER TABLE [dbo].[OrderRefunds]  WITH CHECK ADD  CONSTRAINT [CK_OrderRefunds_Mode] CHECK  (([RefundMode]='Cash' OR [RefundMode]='BankTransfer' OR [RefundMode]='OriginalPayment' OR [RefundMode]='Wallet'))
GO
ALTER TABLE [dbo].[OrderRefunds] CHECK CONSTRAINT [CK_OrderRefunds_Mode]
GO
ALTER TABLE [dbo].[OrderRefunds]  WITH CHECK ADD  CONSTRAINT [CK_OrderRefunds_RefundAmount] CHECK  (([RefundAmount]>=(0)))
GO
ALTER TABLE [dbo].[OrderRefunds] CHECK CONSTRAINT [CK_OrderRefunds_RefundAmount]
GO
ALTER TABLE [dbo].[OrderRefunds]  WITH CHECK ADD  CONSTRAINT [CK_OrderRefunds_Status] CHECK  (([RefundStatus]='Cancelled' OR [RefundStatus]='Rejected' OR [RefundStatus]='Refunded' OR [RefundStatus]='Processing' OR [RefundStatus]='Approved' OR [RefundStatus]='Requested'))
GO
ALTER TABLE [dbo].[OrderRefunds] CHECK CONSTRAINT [CK_OrderRefunds_Status]
GO
ALTER TABLE [dbo].[OrderRefunds]  WITH CHECK ADD  CONSTRAINT [CK_OrderRefunds_TotalAmount] CHECK  (([TotalAmount]>=(0)))
GO
ALTER TABLE [dbo].[OrderRefunds] CHECK CONSTRAINT [CK_OrderRefunds_TotalAmount]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD CHECK  (([RefundStatus]='Full' OR [RefundStatus]='Rejected' OR [RefundStatus]='Requested' OR [RefundStatus]='None'))
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD CHECK  (([TotalAmount]>=(0)))
GO
ALTER TABLE [dbo].[PaymentHistory]  WITH CHECK ADD CHECK  (([PaymentStatus]='Pending' OR [PaymentStatus]='Failed' OR [PaymentStatus]='Paid'))
GO
ALTER TABLE [dbo].[PriceRanges]  WITH CHECK ADD  CONSTRAINT [CK_PriceRanges_MinMax] CHECK  (([PriceRangeMin]<=[PriceRangeMax]))
GO
ALTER TABLE [dbo].[PriceRanges] CHECK CONSTRAINT [CK_PriceRanges_MinMax]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD CHECK  (([Price]>=(0)))
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD CHECK  (([ProductStatus]='Discontinued' OR [ProductStatus]='OutOfStock' OR [ProductStatus]='Available'))
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD CHECK  (([Quantity]>=(0)))
GO
ALTER TABLE [dbo].[ReviewBlogReactions]  WITH CHECK ADD CHECK  (([ReactionType]='Dislike' OR [ReactionType]='Like'))
GO
ALTER TABLE [dbo].[ReviewBlogs]  WITH CHECK ADD CHECK  (([Rating]>=(1) AND [Rating]<=(5)))
GO
ALTER TABLE [dbo].[ReviewProductReactions]  WITH CHECK ADD CHECK  (([ReactionType]='Dislike' OR [ReactionType]='Like'))
GO
ALTER TABLE [dbo].[ReviewProducts]  WITH CHECK ADD CHECK  (([Rating]>=(1) AND [Rating]<=(5)))
GO
ALTER TABLE [dbo].[StatusOrders]  WITH CHECK ADD CHECK  (([StatusName]='Cancelled' OR [StatusName]='Confirmed' OR [StatusName]='Delivered' OR [StatusName]='Shipped' OR [StatusName]='Pending' OR [StatusName]='Completed' OR [StatusName]='Refunded'))
GO
ALTER TABLE [dbo].[Voucher]  WITH CHECK ADD CHECK  (([DiscountValue]>=(0)))
GO
ALTER TABLE [dbo].[Voucher]  WITH CHECK ADD CHECK  (([MinOrderAmount] IS NULL OR [MinOrderAmount]>=(0)))
GO
ALTER TABLE [dbo].[Voucher]  WITH CHECK ADD CHECK  (([Status]='Expired' OR [Status]='Inactive' OR [Status]='Active'))
GO
ALTER TABLE [dbo].[Voucher]  WITH CHECK ADD CHECK  (([UsageLimitPerUser] IS NULL OR [UsageLimitPerUser]>(0)))
GO
ALTER TABLE [dbo].[Voucher]  WITH CHECK ADD  CONSTRAINT [CK_Voucher_Dates] CHECK  (([StartDate]<[EndDate]))
GO
ALTER TABLE [dbo].[Voucher] CHECK CONSTRAINT [CK_Voucher_Dates]
GO
ALTER TABLE [dbo].[Wallets]  WITH CHECK ADD CHECK  (([Balance]>=(0)))
GO
ALTER TABLE [dbo].[Wallets]  WITH CHECK ADD CHECK  (([Status]='Closed' OR [Status]='Frozen' OR [Status]='Active'))
GO
ALTER TABLE [dbo].[WalletTransactions]  WITH CHECK ADD CHECK  (([Amount]>(0)))
GO
ALTER TABLE [dbo].[WalletTransactions]  WITH CHECK ADD CHECK  (([BalanceBefore]>=(0)))
GO
ALTER TABLE [dbo].[WalletTransactions]  WITH CHECK ADD CHECK  (([BalanceAfter]>=(0)))
GO
ALTER TABLE [dbo].[WalletTransactions]  WITH CHECK ADD CHECK  (([Direction]='DR' OR [Direction]='CR'))
GO
ALTER TABLE [dbo].[WalletTransactions]  WITH CHECK ADD CHECK  (([Method] IS NULL OR ([Method]='Wallet' OR [Method]='Cash' OR [Method]='EWallet' OR [Method]='Bank')))
GO
ALTER TABLE [dbo].[WalletTransactions]  WITH CHECK ADD CHECK  (([Status]='Cancelled' OR [Status]='Failed' OR [Status]='Completed' OR [Status]='Pending'))
GO
ALTER TABLE [dbo].[WalletTransactions]  WITH CHECK ADD CHECK  (([TxnType]='Refund' OR [TxnType]='Payment' OR [TxnType]='TopUp'))
GO
ALTER TABLE [dbo].[WebhookEvents]  WITH CHECK ADD CHECK  (([Status]='Failed' OR [Status]='Processed' OR [Status]='Pending'))
GO