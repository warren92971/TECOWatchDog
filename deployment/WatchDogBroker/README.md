# WatchDog 專用 RabbitMQ 連網安裝包

此包安裝 MQTT Broker，不包含 SQL Server 監控程式的改版。只供全新、專用的 Windows x64 主機使用；不會更新或覆蓋其他 RabbitMQ。

## 安裝

1. 在 MCC 建模測試機解壓縮至簡短英文路徑，例如 C:\Setup\WatchDogBroker。需要能連線 GitHub 下載。
2. 若先前測試用 PowerShell 還開著，在原視窗執行 `$mqttTestListener.Stop()`，釋放 8883。
3. 以系統管理員身分執行 Start-Install.cmd。不要在你自己的操作電腦執行。
4. 外部 IP 預設 140.116.234.108；請勿輸入 Port。防火牆來源可填客戶對外 IP 或 CIDR；Any 表示不在本機限制來源，仍受學校網管規則限制。
5. 分別設定 watchdog_admin、watchdog_pub、teco_sub 的密碼（至少 12 字元），自行安全保存。

安裝到 C:\WatchDogBroker，建立 Windows RabbitMQ 自動啟動服務、本機防火牆 TCP 8883 規則及私人 CA。遇到既有 RabbitMQ/Erlang、安裝目錄或 8883 占用會停止。失敗時保留現場，不會自動刪除資料；請提供錯誤訊息排查，不要反覆刪檔重裝。

## 連線設定

|用途|設定|
|---|---|
|客戶端外部連線|140.116.234.108:18688，啟用 TLS|
|伺服器本機連線|localhost:8883，啟用 TLS|
|網管轉送|外部 TCP 18688 → 本機 TCP 8883，須由網管維持|
|WatchDog 發布帳號|watchdog_pub|
|對方訂閱帳號|teco_sub|
|Topic 範例|teco/watchdog/status|
|允許 Topic 範圍|teco/watchdog/ 開頭；建議先使用確切 Topic|
|QoS|1|
|管理頁|僅伺服器本機 http://127.0.0.1:15672；watchdog_admin|

未開放明文 MQTT 1883 或 AMQP 5672。帳號區分發布與訂閱權限。不使用匿名登入。現有 WatchDog 若尚無 TLS/帳密功能，需要另行修改才能連線。

## 檢查

安裝完成訊息只代表服務已監聽 TCP 8883，尚不代表外網、TLS、MQTT 全部通過。

在伺服器上以系統管理員 PowerShell 執行：

```powershell
& C:\WatchDogBroker\runtime\pwsh.exe -File C:\WatchDogBroker\client\Test-Mqtt.ps1 -BrokerHost localhost -Port 8883
```

輸入 watchdog_pub 密碼。PASS 表示 TCP、CA 信任、伺服器名稱/IP 驗證和 MQTT 登入成功；不會發送業務訊息，未測試發布/訂閱權限或資料流。

外部電腦使用 PowerShell 7.4 以上，將 Test-Mqtt.ps1 和 client\ca.crt 放同一資料夾，再執行：

```powershell
pwsh -File .\Test-Mqtt.ps1 -BrokerHost 140.116.234.108 -Port 18688 -UserName teco_sub
```

如 TCP 失敗，先檢查 RabbitMQ 服務、本機防火牆及網管 NAT。若 TLS 失敗，檢查 CA 檔、時間和使用的 IP/DNS；不要關閉憑證驗證。Broker 日誌在 C:\WatchDogBroker\logs。

## 給對方的檔案

只提供 C:\WatchDogBroker\client\ca.crt、連線資訊、Topic 和 teco_sub 帳號；密碼另走安全管道。對方需將 CA 設為信任來源，並保留伺服器身分驗證。

不要分享 certs 內的 .key、definitions.json 或整個安裝資料夾。此設計不要求客戶端憑證，但仍要求 TLS 和帳密；設定 verify_none 是伺服器不要求客戶端憑證，不是讓客戶端忽略伺服器憑證。

伺服器憑證有效期 365 天，CA 5 年，沒有自動續期；請安排到期前以原 CA 更新伺服器憑證，並備份私鑰至安全位置。變更連線 IP/DNS 也需更新 SAN。

## 停用與移除

僅在確認是這個專用安裝後，以管理員執行 `Stop-Service RabbitMQ` 可停止服務。完整移除請先備份必要資料，再透過 Windows「已安裝的應用程式」移除此套 RabbitMQ 和 Erlang。

移除本包防火牆規則：`Remove-NetFirewallRule -Name WatchDog-MQTTS-8883`。

服務移除後，在系統環境變數中檢查 ERLANG_HOME、RABBITMQ_BASE、RABBITMQ_CONFIG_FILE、RABBITMQ_ENABLED_PLUGINS_FILE；只移除值仍指向 C:\WatchDogBroker 的項目。ERL_EPMD_ADDRESS 如仍為本包設定的 127.0.0.1，且沒有其他 Erlang 服務依賴才移除。資料目錄保留，不提供自動遞迴刪除。

## 固定版本與驗證範圍

- RabbitMQ 4.3.6、Erlang 27.3.4.18、PowerShell 7.6.6；下載來源及 SHA256 在 downloads.json。下載後檢查雜湊，不符合即停止。
- 參考官方文件：https://www.rabbitmq.com/docs/install-windows 、https://www.rabbitmq.com/docs/mqtt 、https://www.rabbitmq.com/docs/which-erlang 。
- 製作端會做腳本語法及憑證生成檢查；尚未在目標主機實際安裝或完成端到端發布/訂閱驗收。請先於此測試機驗收，再提供正式服務。
