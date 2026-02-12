using System;
using nU3.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace nU3.Connectivity.Implementations
{
    /// <summary>
    /// HTTP/REST based file transfer client.
    /// Replaces the legacy implementation.
    /// </summary>
    public class HttpFileTransferClient : FileTransferClientBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public HttpFileTransferClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = TimeSpan.FromMinutes(10)
            };

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public HttpFileTransferClient(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl.TrimEnd('/');
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        protected override async Task<bool> RemoteUploadAsync(string serverPath, byte[] data)
        {
            const int maxAttempts = 3;
            Exception? lastException = null;

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    using var form = new MultipartFormDataContent();
                    using var fileContent = new ByteArrayContent(data);

                    form.Add(fileContent, "File", Path.GetFileName(serverPath));
                    form.Add(new StringContent(serverPath), "ServerPath");

                    var response = await _httpClient.PostAsync("/api/v1/files/upload", form).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                        return true;

                    int statusCode = (int)response.StatusCode;
                    // Retry on 429 (Too Many Requests) or 5xx (Server Errors)
                    if ((statusCode == 429 || statusCode >= 500) && attempt < maxAttempts)
                    {
                        var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(Math.Pow(2, attempt));
                        await Task.Delay(retryAfter).ConfigureAwait(false);
                        continue;
                    }

                    return false;
                }
                catch (HttpRequestException ex)
                {
                    lastException = ex;
                    if (attempt < maxAttempts)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt))).ConfigureAwait(false);
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Upload error: {ex.Message}", ex);
                }
            }

            if (lastException != null)
                throw new InvalidOperationException($"Upload failed after {maxAttempts} attempts: {lastException.Message}", lastException);

            return false;
        }

        protected override async Task<byte[]> RemoteDownloadAsync(string serverPath)
        {
            try
            {
                var encodedPath = Uri.EscapeDataString(serverPath);
                var response = await _httpClient.GetAsync($"/api/v1/files/download?serverPath={encodedPath}").ConfigureAwait(false);
                
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Download error: {ex.Message}", ex);
            }
        }

        protected override async Task<bool> RemoteCommandAsync(string command, object[]? args)
        {
            try
            {
                var endpoint = MapCommandToEndpoint(command);
                var requestData = CreateRequestData(command, args);

                HttpResponseMessage response;
                var url = endpoint.url;
                
                if (endpoint.method == HttpMethod.Post)
                {
                    if (requestData != null)
                    {
                        if (command == nameof(CreateDirectory) || command == nameof(CreateDirectoryAsync))
                        {
                            var path = args?[0]?.ToString() ?? string.Empty;
                            var encoded = Uri.EscapeDataString(path);
                            var separator = url.Contains("?") ? "&" : "?";
                            url = url + $"{separator}path={encoded}";
                            response = await _httpClient.PostAsync(url, null).ConfigureAwait(false);
                        }
                        else
                        {
                            response = await _httpClient.PostAsJsonAsync(url, requestData).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        response = await _httpClient.PostAsync(url, null).ConfigureAwait(false);
                    }
                }
                else if (endpoint.method == HttpMethod.Delete)
                {
                    if (args != null && args.Length > 0)
                    {
                        var encoded = Uri.EscapeDataString(args[0]?.ToString() ?? string.Empty);
                        url = url + $"?path={encoded}";
                    }

                    response = await _httpClient.DeleteAsync(url).ConfigureAwait(false);
                }
                else
                {
                    throw new NotSupportedException($"HTTP method {endpoint.method} not supported");
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<bool>(_jsonOptions).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                 throw new InvalidOperationException($"Network error executing '{command}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error executing '{command}': {ex.Message}", ex);
            }
        }

        protected override async Task<T> RemoteQueryAsync<T>(string command, object[]? args)
        {
            try
            {
                var endpoint = MapQueryToEndpoint(command);
                var url = BuildQueryUrl(endpoint, args);

                var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                if (typeof(T) == typeof(string))
                {
                    try 
                    {
                        var jsonResult = await response.Content.ReadFromJsonAsync<string>(_jsonOptions).ConfigureAwait(false);
                        return (T)(object)(jsonResult ?? string.Empty);
                    }
                    catch (JsonException)
                    {
                        var rawResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        return (T)(object)rawResult;
                    }
                }
                else
                {
                    var result = await response.Content.ReadFromJsonAsync<T>(_jsonOptions).ConfigureAwait(false);
                    return result!;
                }
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Network error querying '{command}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error querying '{command}': {ex.Message}", ex);
            }
        }

        private (string url, HttpMethod method) MapCommandToEndpoint(string command)
        {
            return command switch
            {
                nameof(SetHomeDirectory) or nameof(SetHomeDirectoryAsync) => 
                    ("/api/v1/files/directory/config", HttpMethod.Post),
                nameof(CreateDirectory) or nameof(CreateDirectoryAsync) => 
                    ("/api/v1/files/directory/create", HttpMethod.Post),
                nameof(DeleteDirectory) or nameof(DeleteDirectoryAsync) => 
                    ("/api/v1/files/directory", HttpMethod.Delete),
                nameof(RenameDirectory) or nameof(RenameDirectoryAsync) => 
                    ("/api/v1/files/directory/rename", HttpMethod.Post),
                nameof(DeleteFile) or nameof(DeleteFileAsync) => 
                    ("/api/v1/files", HttpMethod.Delete),
                _ => throw new NotSupportedException($"Command '{command}' not supported")
            };
        }

        private string MapQueryToEndpoint(string command)
        {
            return command switch
            {
                nameof(GetHomeDirectory) or nameof(GetHomeDirectoryAsync) => "/api/v1/files/directory",
                nameof(GetFileList) or nameof(GetFileListAsync) => "/api/v1/files/list",
                nameof(GetSubDirectoryList) or nameof(GetSubDirectoryListAsync) => "/api/v1/files/subdirectories",
                nameof(ExistDirectory) or nameof(ExistDirectoryAsync) => "/api/v1/files/directory/exists",
                nameof(ExistFile) or nameof(ExistFileAsync) => "/api/v1/files/exists",
                nameof(GetFileSize) or nameof(GetFileSizeAsync) => "/api/v1/files/size",
                _ => throw new NotSupportedException($"Query '{command}' not supported")
            };
        }

        private string BuildQueryUrl(string baseEndpoint, object[]? args)
        {
            if (args == null || args.Length == 0)
                return baseEndpoint;

            var path = args[0]?.ToString() ?? string.Empty;
            var encodedPath = Uri.EscapeDataString(path);
            return $"{baseEndpoint}?path={encodedPath}";
        }

        private object? CreateRequestData(string command, object[]? args)
        {
            if (args == null || args.Length == 0)
                return null;

            return command switch
            {
                nameof(SetHomeDirectory) or nameof(SetHomeDirectoryAsync) =>
                    new
                    {
                        IsUseHomePath = args.Length > 0 && args[0] is bool b && b,
                        ServerHomePath = args.Length > 1 ? args[1]?.ToString() ?? string.Empty : string.Empty
                    },

                nameof(CreateDirectory) or nameof(CreateDirectoryAsync) =>
                    new { path = args[0]?.ToString() ?? string.Empty },

                nameof(RenameDirectory) or nameof(RenameDirectoryAsync) =>
                    new
                    {
                        sourcePath = args.Length > 0 ? args[0]?.ToString() ?? string.Empty : string.Empty,
                        destPath = args.Length > 1 ? args[1]?.ToString() ?? string.Empty : string.Empty
                    },

                _ => null
            };
        }
    }
}