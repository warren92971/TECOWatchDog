namespace TESCWatchDog;

public sealed record CdbIniSettings(string Server, string Database, string User, string Password,
    string Schema, bool IntegratedSecurity, bool TrustServerCertificate, int QueryTimeoutSeconds = 10)
{
    public static CdbIniSettings Parse(string text)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string section = "";
        foreach (var raw in text.Split('\n'))
        {
            var line = raw.Trim().TrimStart('\uFEFF');
            if (line.Length == 0 || line.StartsWith(';') || line.StartsWith('#')) continue;
            if (line.StartsWith('[') && line.EndsWith(']')) { section = line[1..^1].Trim(); continue; }
            if (!section.Equals("CDB", StringComparison.OrdinalIgnoreCase)) continue;
            int separator = line.IndexOf('=');
            if (separator <= 0) throw new FormatException("CDB 設定格式錯誤，請使用 key = value。");
            string key = line[..separator].Trim(), value = line[(separator + 1)..].Trim();
            if (value.StartsWith('"'))
            {
                if (value.Length < 2 || !value.EndsWith('"')) throw new FormatException("CDB 設定的雙引號未成對。");
                value = value[1..^1];
            }
            if (!values.TryAdd(key, value)) throw new FormatException("CDB 設定包含重複欄位。");
        }
        string Get(string key, string fallback = "") => values.GetValueOrDefault(key, fallback);
        bool Flag(string key)
        {
            string value = Get(key, "false");
            if (value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase)) return true;
            if (value == "0" || value.Equals("false", StringComparison.OrdinalIgnoreCase)) return false;
            throw new FormatException($"{key} 必須是 true 或 false。");
        }
        if (!Get("Type", "SQLServer").Equals("SQLServer", StringComparison.OrdinalIgnoreCase))
            throw new FormatException("目前只支援 Type = SQLServer。");
        if (string.IsNullOrWhiteSpace(Get("IP")) || string.IsNullOrWhiteSpace(Get("Database")))
            throw new FormatException("[CDB] 必須提供 IP 與 Database；DSN 是連線名稱，不能代替資料庫名稱。");
        string schema = Get("TableUserName", "dbo");
        if (string.IsNullOrWhiteSpace(schema) || schema.Length > 128 || schema.Contains('\0'))
            throw new FormatException("TableUserName 必須是有效的 Schema 名稱。");
        if (!int.TryParse(Get("QueryTimeoutSeconds", "10"), out int queryTimeout) || queryTimeout < 1 || queryTimeout > 600)
            throw new FormatException("QueryTimeoutSeconds 必須介於 1 到 600 秒。");
        // 僅在第一個等號切割，不移除行內 ; 或 #，避免破壞密碼。
        return new(Get("IP"), Get("Database"), Get("UID"), Get("PWD"), schema,
            Flag("IntegratedSecurity"), Flag("TrustServerCertificate"), queryTimeout);
    }
}
