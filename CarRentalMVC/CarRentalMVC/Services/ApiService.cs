using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CarRentalMVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "http://localhost:5000/"; // URL của API backend
        public string BaseUrl => _baseUrl;

        public ApiService()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };
        }

        // ✅ Clear Authorization để tránh lỗi reuse token cũ
        private void ResetAuth() => _client.DefaultRequestHeaders.Authorization = null;

        // 🟢 Gán token Bearer
        private void SetAuth(string? token)
        {
            if (!string.IsNullOrEmpty(token))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // 🟢 Helper: Format endpoint cho an toàn
        private string BuildUrl(string endpoint)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentNullException(nameof(endpoint));
            if (endpoint.StartsWith("/")) endpoint = endpoint.TrimStart('/');
            return $"{_baseUrl}{endpoint}";
        }

        // 🟢 GET trả về object kiểu T
        public async Task<T?> GetAsync<T>(string endpoint, string? token = null)
        {
            ResetAuth();
            SetAuth(token);
            var url = BuildUrl(endpoint);

            var response = await _client.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"GET {url} → {response.StatusCode}\n{result}");

            return JsonConvert.DeserializeObject<T>(result);
        }

        // 🟢 GET trả về string (nếu muốn xử lý JSON thủ công)
        public async Task<string> GetAsync(string endpoint, string? token = null)
        {
            ResetAuth();
            SetAuth(token);
            var url = BuildUrl(endpoint);

            var response = await _client.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"GET {url} → {response.StatusCode}\n{result}");

            return result;
        }

        // 🟢 POST (JSON)
        public async Task<string> PostAsync(string endpoint, object data, string? token = null)
        {
            ResetAuth();
            SetAuth(token);
            var url = BuildUrl(endpoint);

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"POST {url} → {response.StatusCode}\n{result}");

            return result;
        }

        // 🟢 POST (Multipart - upload ảnh / file)
        public async Task<string> PostMultipartAsync(string endpoint, MultipartFormDataContent data, string? token = null)
        {
            ResetAuth();
            SetAuth(token);
            var url = BuildUrl(endpoint);

            var response = await _client.PostAsync(url, data);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"POST(Multipart) {url} → {response.StatusCode}\n{result}");

            return result;
        }

        // 🟢 PUT (JSON)
        public async Task<string> PutAsync(string endpoint, object data, string? token = null)
        {
            ResetAuth();
            SetAuth(token);
            var url = BuildUrl(endpoint);

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"PUT {url} → {response.StatusCode}\n{result}");

            return result;
        }

        // 🟢 PUT (Multipart)
        public async Task<string> PutMultipartAsync(string endpoint, MultipartFormDataContent data, string? token = null)
        {
            ResetAuth();
            SetAuth(token);
            var url = BuildUrl(endpoint);

            var response = await _client.PutAsync(url, data);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"PUT(Multipart) {url} → {response.StatusCode}\n{result}");

            return result;
        }

        // 🟢 DELETE
        public async Task<string> DeleteAsync(string endpoint, string? token = null)
        {
            ResetAuth();
            SetAuth(token);
            var url = BuildUrl(endpoint);

            var response = await _client.DeleteAsync(url);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"DELETE {url} → {response.StatusCode}\n{result}");

            return result;
        }

        // 🟢 LOGIN → lấy JWT Token
        public async Task<string> LoginAsync(string endpoint, object loginRequest)
        {
            var url = BuildUrl(endpoint);
            var json = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"LOGIN Error: {result}");

            return result; // trả về token JSON string
        }

        // 🟢 Kiểm tra JWT token hợp lệ
        public async Task<bool> VerifyTokenAsync(string endpoint, string token)
        {
            try
            {
                var result = await GetAsync(endpoint, token);
                return !string.IsNullOrEmpty(result);
            }
            catch
            {
                return false;
            }
        }

        // 🆕 🟣 Thêm tiện ích: Parse JSON sang object hoặc dynamic
        public static T? ParseResponse<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default;
            }
        }

        // 🆕 🟣 Thêm log helper (chỉ hiển thị khi debug)
        private void Log(string message)
        {
#if DEBUG
            Console.WriteLine($"[ApiService] {DateTime.Now:HH:mm:ss} → {message}");
#endif
        }
    }
}

