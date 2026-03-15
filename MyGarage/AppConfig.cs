using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace MyGarage
{
    public class AppConfig
    {
        public SmtpConfig Smtp { get; set; } = new();
        public FreeMobileConfig FreeMobile { get; set; } = new();

        public static AppConfig Load()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (!File.Exists(path))
                return new AppConfig();
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
        }
    }

    public class SmtpConfig
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class FreeMobileConfig
    {
        public string UserId { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }
}