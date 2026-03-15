using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Service.Services
{
    public class UpdateService
    {
        private const string GithubApiUrl = "https://api.github.com/repos/Joris-Tazamoucht/MyGarage/releases/latest";
        public async Task<UpdateInfo?> CheckForUpdateAsync(Version currentVersion)
        {
            using var http = new HttpClient();
            // GitHub exige un User-Agent
            http.DefaultRequestHeaders.Add("User-Agent", "MyGarage-App");

            var release = await http.GetFromJsonAsync<GithubRelease>(GithubApiUrl);
            if (release == null) return null;

            // Nettoyer le tag "v1.2.3" → "1.2.3"
            string versionStr = release.TagName.TrimStart('v');
            if (!Version.TryParse(versionStr, out var latestVersion)) return null;

            if (latestVersion <= currentVersion) return null;

            // Trouver l'asset .exe
            var asset = release.Assets.FirstOrDefault(a => a.Name.EndsWith(".exe"));
            if (asset == null) return null;

            return new UpdateInfo
            {
                CurrentVersion = currentVersion.ToString(),
                NewVersion = latestVersion.ToString(),
                DownloadUrl = asset.BrowserDownloadUrl,
                ReleaseNotes = release.Body ?? string.Empty,
                FileName = asset.Name
            };
        }

        public async Task<string> DownloadUpdateAsync(UpdateInfo update, IProgress<int>? progress = null)
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("User-Agent", "MyGarage-App");

            string tempPath = Path.Combine(Path.GetTempPath(), update.FileName);

            using var response = await http.GetAsync(update.DownloadUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            long? totalBytes = response.Content.Headers.ContentLength;
            using var stream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write);

            byte[] buffer = new byte[8192];
            long downloaded = 0;
            int read;

            while ((read = await stream.ReadAsync(buffer)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, read));
                downloaded += read;
                if (totalBytes.HasValue)
                    progress?.Report((int)(downloaded * 100 / totalBytes.Value));
            }

            return tempPath;
        }
    }

    public class UpdateInfo
    {
        public string CurrentVersion { get; set; } = string.Empty;
        public string NewVersion { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public string ReleaseNotes { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }

    public class GithubRelease
    {
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; } = string.Empty;

        [JsonPropertyName("body")]
        public string? Body { get; set; }

        [JsonPropertyName("assets")]
        public List<GithubAsset> Assets { get; set; } = new();
    }

    public class GithubAsset
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("browser_download_url")]
        public string BrowserDownloadUrl { get; set; } = string.Empty;
    }
}