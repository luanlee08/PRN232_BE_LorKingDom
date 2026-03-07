-- Add missing notification templates required for review rejection and custom notifications
-- Uses MERGE so this script is safe to run more than once.
MERGE INTO [Notification].[Templates] AS target
USING (VALUES
    ('REVIEW_REJECTED', N'Đánh giá của bạn bị từ chối',  N'Đánh giá của bạn về ''#{productName}'' đã bị từ chối. Lý do: #{reason}'),
    ('CUSTOM',          N'#{title}',                       N'#{message}')
) AS source ([TemplateCode], [TitleTemplate], [MessageTemplate])
ON target.[TemplateCode] = source.[TemplateCode]
WHEN NOT MATCHED THEN
    INSERT ([TemplateCode], [TitleTemplate], [MessageTemplate])
    VALUES (source.[TemplateCode], source.[TitleTemplate], source.[MessageTemplate]);
