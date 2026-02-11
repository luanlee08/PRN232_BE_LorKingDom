# Database Migrations

Thư mục này chứa các migration scripts để cập nhật database schema.

## Migration History

### 002_Remove_Old_Notification_Tables.sql

**Date**: February 11, 2026  
**Status**: ⚠️ READY TO RUN  
**Description**: Xóa các bảng notification cũ (Notifications, UserNotifications, NotificationLogs)

**Changes**:

- ❌ DROP TABLE `dbo.Notifications`
- ❌ DROP TABLE `dbo.UserNotifications`
- ❌ DROP TABLE `dbo.NotificationLogs`
- ✅ Hệ thống giờ sử dụng `Notification.Deliveries` (đã tồn tại trong schema gốc)

**Why**: Hệ thống cũ bị trùng lặp - có 2 cách gửi notification. Deliveries table có structure tốt hơn và gọn hơn.

**Impact**:

- Code đã được refactor hoàn toàn
- API endpoints thay đổi (sử dụng DeliveryResponse thay vì NotificationResponse)
- Backward incompatible (không thể rollback dễ dàng)

**How to run**:

```sql
sqlcmd -S your_server -d ASP_LorKingDom -i "002_Remove_Old_Notification_Tables.sql"
```

Hoặc chạy trực tiếp bằng SSMS

---

### 001_Add_Template_Support_To_Notifications.sql

**Date**: February 2026  
**Status**: ⚠️ NO LONGER NEEDED (bảng Notifications sẽ bị xóa bởi migration 002)  
**Description**: Thêm Template support vào bảng Notifications (deprecated)

**Note**: Migration này không còn cần thiết vì bảng Notifications sẽ bị xóa. Template system giờ chỉ dùng với Deliveries table.

---

## Current Schema

### Notification System Tables (ACTIVE)

**Notification.Templates** - Template definitions

- TemplateCode (PK) - VARCHAR(50)
- TitleTemplate, MessageTemplate
- IsActive, CreatedAt, UpdatedAt

**Notification.Deliveries** - Delivered notifications to users

- DeliveryID (PK) - BIGINT
- AccountID (FK to Accounts)
- CreatedByJobID (FK to BackgroundJobs) - NULL if manual
- TemplateCode (FK to Templates)
- Title, Message, Payload
- Status (Unread/Read)
- CreatedAt

**System.BackgroundJobs** - Hangfire job tracking

- JobID, JobName, CronExpression
- IsEnabled, LastRunTime, NextRunTime
- LastRunStatus, LastRunMessage

### Legacy Tables (TO BE REMOVED)

~~**dbo.Notifications**~~ - Scheduled notifications (❌ Will be dropped)  
~~**dbo.UserNotifications**~~ - User-notification mapping (❌ Will be dropped)  
~~**dbo.NotificationLogs**~~ - Delivery logs (❌ Will be dropped)

---

## How Notification System Works Now

### Old System (Before refactor):

1. Admin creates `Notification` record → persisted in DB
2. Hangfire worker checks scheduled notifications every minute
3. Worker sends → creates `UserNotifications` records
4. Logs in `NotificationLogs`

### New System (After refactor):

1. Admin sends request via API endpoint (just JSON, not persisted immediately)
2. System determines target users
3. For each user:
   - **Immediate**: Create `Delivery` records right away
   - **Scheduled**: Hangfire schedules job → creates `Delivery` records at scheduled time
4. No intermediate tables needed - `Deliveries` is the source of truth

**Benefits**:

- ✅ 3 bảng → 1 bảng duy nhất (Deliveries)
- ✅ Không cần recurring worker check database
- ✅ Hangfire quản lý scheduling trực tiếp
- ✅ Dễ track: mỗi delivery = 1 notification đã gửi cho 1 user

---

## API Changes

### Old Endpoints (Deprecated):

```
POST   /api/admin/notifications          Create notification
GET    /api/admin/notifications          List notifications
GET    /api/admin/notifications/{id}     Get notification
PUT    /api/admin/notifications/{id}     Update notification
DELETE /api/admin/notifications/{id}     Cancel notification
```

### New Endpoints (Current):

```
POST   /api/admin/notifications          Send notification (creates deliveries)
GET    /api/admin/notifications          List deliveries (admin view)
GET    /api/admin/notifications/{id}     Get delivery by ID
GET    /api/admin/notifications/stats    Get stats

// User endpoints (to be added to user controller)
GET    /api/user/notifications           Get my notifications
GET    /api/user/notifications/unread-count  Get unread count
POST   /api/user/notifications/{id}/read Mark as read
POST   /api/user/notifications/read-all  Mark all as read
DELETE /api/user/notifications/{id}      Delete notification
```

---

## DTO Changes

### Old DTOs:

- `CreateNotificationRequest`
- `UpdateNotificationRequest`
- `NotificationResponse`
- `NotificationQuery`
- `NotificationStatsResponse`

### New DTOs:

- `SendNotificationRequest` - Request to send notifications
- `MarkAsReadRequest` - Mark delivery as read
- `DeliveryResponse` - Delivered notification info
- `DeliveryQuery` - Query deliveries
- `DeliveryStatsResponse` - Delivery statistics

---

## Migration Order

1. ✅ **Code refactor** (Done - build thành công)
2. ⚠️ **Run migration 002** - Xóa bảng cũ
3. ✅ **Template system** still works (no changes needed)
4. 🔧 **Optional**: Add user controller endpoints for notification viewing

---

## Rollback Strategy

⚠️ **WARNING**: Rollback khó vì code đã thay đổi hoàn toàn.

### If you need to rollback:

1. Khôi phục code cũ từ git
2. Chạy migration script để tạo lại tables:
   ```sql
   -- Create Notifications table
   -- Create UserNotifications table
   -- Create NotificationLogs table
   -- Restore indexes and FKs
   ```
3. Rebuild và deploy

→ Khuyến nghị: Test kỹ trên staging environment trước khi chạy production

---

## Next Steps

1. **Run Migration 002** trên database
2. **Test endpoints** với Postman/Swagger
3. **Create user controller** cho user xem notifications của họ
4. **Update frontend** để call API mới
5. **Monitor Hangfire dashboard** tại `/hangfire`

---

Last Updated: February 11, 2026
