# Changelog

本專案版本紀錄遵循 [Semantic Versioning](https://semver.org/lang/zh-TW/)。

## [1.0.0] - 2026-10-08

### 新增

- CDB `VM_RESULT_CONTROL` 監控，透過 `MAX(TIMETAG)` 判斷是否有新資料。
- 可設定查詢間隔、無新資料門檻及單次 SQL 查詢逾時。
- 從 `system.ini` 自動載入 SQL Server、資料庫、Schema、驗證及憑證設定。
- 連線東元 MQTT Broker，預設使用 `ncku/watchdog/alert` 與 `ncku/watchdog/recovery`。
- 無新資料警報與資料恢復通知，支援自訂文字或 JSON，以及動態變數替換。
- 警報與恢復手動推送按鈕，使用 QoS 1 並顯示發布結果。
- MQTT 訂閱回收驗證，Log 顯示實際接收的 Topic 與 Payload。
- 查詢完成後逐秒顯示下一次 CDB 查詢倒數。
- 每日事件紀錄，儲存於執行檔旁的 `Log/YYYY-MM-DD.txt`。
- 離線檢查程式，涵蓋輪詢、警報、恢復、INI、安全驗證及 WinForms 建構。

### 調整

- 將 CDB 與通知設定整合為單頁橫向版面，隱藏不再使用的通用 MQTT 手動發布頁。
- CDB 查詢完整 `await`，避免查詢重疊；每輪完成後等待完整查詢間隔。
- 執行監控時鎖定 CDB 與通知設定，但保留警報及恢復手動推送功能。

### 安全性

- SQL 查詢固定為唯讀目標並安全引用 Schema。
- 密碼不寫入畫面 Log 或每日事件檔。
- `system.ini` 排除於 Git，僅提供無密碼的設定範例。
