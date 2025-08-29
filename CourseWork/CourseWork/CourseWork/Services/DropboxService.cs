using Dropbox.Api;
using Dropbox.Api.Files;

namespace CourseWork.Services
{
    public class DropboxService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public DropboxService(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var appKey = _config["Dropbox:AppKey"];
            var appSecret = _config["Dropbox:AppSecret"];
            var refreshToken = _config["Dropbox:RefreshToken"];

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropbox.com/oauth2/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"refresh_token", refreshToken},
                {"client_id", appKey},
                {"client_secret", appSecret}
            });

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            return data["access_token"].ToString();
        }

        public async Task<string> UploadUserAvatarAsync(Stream fileStream, string fileName, string userId)
        {
            var accessToken = await GetAccessTokenAsync();
            using var dbx = new DropboxClient(accessToken);

            string dropboxPath = $"/avatars/{userId}_avatar_{fileName}";

            var uploaded = await dbx.Files.UploadAsync(
                dropboxPath,
                WriteMode.Overwrite.Instance,
                body: fileStream);


            var link = await dbx.Sharing.CreateSharedLinkWithSettingsAsync(dropboxPath);

            string rawUrl = link.Url.Replace("&dl=0", "&raw=1");

            return rawUrl;
        }

        public async Task<string> UploadInventoryImageAsync(Stream fileStream, string fileName, string inventoryId)
        {
            var accessToken = await GetAccessTokenAsync();
            using var dbx = new DropboxClient(accessToken);

            string dropboxPath = $"/inventories/{inventoryId}_image_{fileName}";


            var uploaded = await dbx.Files.UploadAsync(
                dropboxPath,
                WriteMode.Overwrite.Instance,
                body: fileStream);

            var link = await dbx.Sharing.CreateSharedLinkWithSettingsAsync(dropboxPath);

            string rawUrl = link.Url.Replace("&dl=0", "&raw=1");

            return rawUrl;
        }


        public async Task DeleteFileAsync(string filePath)
        {
            var accessToken = await GetAccessTokenAsync();
            using var dbx = new DropboxClient(accessToken);

            if (!string.IsNullOrEmpty(filePath))
                await dbx.Files.DeleteV2Async(filePath);
        }

        public async Task<string> GetPublicUrlAsync(string filePath)
        {
            var accessToken = await GetAccessTokenAsync();
            using var dbx = new DropboxClient(accessToken);

            var link = await dbx.Sharing.CreateSharedLinkWithSettingsAsync(filePath);
            return link.Url;
        }
    }
}
