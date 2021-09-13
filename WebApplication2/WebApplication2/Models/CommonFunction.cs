using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class CommonFunction

    {
        /// <summary>
        /// Lấy token của Bộ Tài Chính trước khi call đẩy dữ liệu
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetTokenBTC()
        {
            string token = string.Empty;

            try
            {
                // Các tham số gọi server
                var param = new
                {
                    grant_type = GetAppSetting(AppSettingKey.GRANT_TYPE),
                    username = GetAppSetting(AppSettingKey.USERNAME),
                    password = GetAppSetting(AppSettingKey.PASSWORD),
                    client_id = GetAppSetting(AppSettingKey.CLIENT_ID),
                    client_secret = GetAppSetting(AppSettingKey.CLIENT_SECRET),
                    apiUrl = GetAppSetting(AppSettingKey.API_URL_TOKEN),
                    token_endpoint = GetAppSetting(AppSettingKey.TOKEN_END_POINT)
                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(param.apiUrl);

                    // Thêm Authorization
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{param.client_id}:{param.client_secret}")));

                    var headerParams = new List<KeyValuePair<string, string>>();
                    headerParams.Add(new KeyValuePair<string, string>("grant_type", param.grant_type));
                    headerParams.Add(new KeyValuePair<string, string>("username", param.username));
                    headerParams.Add(new KeyValuePair<string, string>("password", param.password));

                    // Tạo một request post
                    var request = new HttpRequestMessage(HttpMethod.Post, param.token_endpoint);
                    request.Content = new FormUrlEncodedContent(headerParams);

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseResult = JsonConvert.DeserializeObject<ResponseApi>(response.Content.ReadAsStringAsync().Result);

                        token = responseResult.access_token;
                    }
                }
            }
            catch (Exception e)
            {
                // Có lỗi khi lấy Token
            }

            return token;
        }

        /// <summary>
        /// Hàm gửi dữ liệu đi
        /// </summary>
        /// <param name="url">Có dạng http://14.248.83.163:9001/ </param>
        /// <param name="api">Có dạng aprserver/report/rpt11bcktsc/sync </param>
        /// <param name="data">Dữ liệu gửi đi</param>
        /// <returns></returns>
        public static async Task<object> SendDataBTC(string url, string api, object data, HttpMethod type)
        {
            string responseResult = string.Empty;

            // Lấy Token trước khi call
            string token = await GetTokenBTC();

            // Nếu lấy được Token thì gửi dữ liệu đi
            if (!string.IsNullOrWhiteSpace(token))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    // Thêm Authorization
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Tạo một request post
                    var request = new HttpRequestMessage(type, api);

                    if (type == HttpMethod.Post)
                    {
                        request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                    }

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        responseResult = response.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            else
            {
                // Chưa lấy được Token
            }

            return responseResult;
        }

        /// <summary>
        /// Lấy app setting
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSetting(string key)
        {
            string res = string.Empty;

            try
            {
                res = System.Configuration.ConfigurationManager.AppSettings[key];
            }
            catch (Exception)
            {
                Console.WriteLine("Key config không tồn tại");
            }

            return res;
        }
    }
}