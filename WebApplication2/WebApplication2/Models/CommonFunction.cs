using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebApplication2.Models
{
    public class CommonFunction

    {
        public static string PostApiWithAuthorization(string url, String sAuthorization, object param = null)
        {
            string res = string.Empty;
            System.Text.UTF8Encoding encode = new System.Text.UTF8Encoding();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            req.Method = "POST";
            req.ContentType = "application/json";
            req.Headers.Add("Authorization", sAuthorization);

            if (param != null)
            {
                byte[] paramBye = encode.GetBytes(JsonConvert.SerializeObject(param));

                using (Stream dataStream = req.GetRequestStream())
                {
                    dataStream.Write(paramBye, 0, paramBye.Length);
                }
            }

            using (HttpWebResponse response = (HttpWebResponse)(req.GetResponse()))
            {
                res = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }

            return res;
        }

        /// <summary>
        /// Gọi tới một api khác có authorize
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="sAuthorization"></param>
        /// <returns></returns>
        public static string PostApi(string url, object param, string sAuthorization)
        {
            string res = string.Empty;

            ServicePointManager.ServerCertificateValidationCallback += (objSender, cert, chain, sslPolicyErrors) => true;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            var parameter = Encoding.Default.GetBytes(JsonConvert.SerializeObject(param));

            req.Method = "post";
            req.ContentType = "application/json";
            req.ContentLength = parameter.Length;
            req.Headers.Add("Authorization", sAuthorization);

            req.GetRequestStream().Write(parameter, 0, parameter.Length);

            HttpWebResponse response = (HttpWebResponse)req.GetResponse();

            using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
            {
                res = sr.ReadToEnd();
            }

            return res;
        }

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
        public static async Task<object> SendDataBTC(string url, string api, object data)
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
                    var request = new HttpRequestMessage(HttpMethod.Post, api);

                    request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

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
        /// Hàm lấy ra token bộ tài chính
        /// </summary>
        /// <returns></returns>
        public static string GetTokenBTC2()
        {
            string token = "";

            try
            {
                Encoding encoding = Encoding.UTF8;
                string apiPath = GetAppSetting(AppSettingKey.TOKEN_END_POINT);

                // Các tham số gọi server
                var dataParam = new
                {
                    grant_type = GetAppSetting(AppSettingKey.GRANT_TYPE),
                    username = GetAppSetting(AppSettingKey.USERNAME),
                    password = GetAppSetting(AppSettingKey.PASSWORD),
                    client_id = GetAppSetting(AppSettingKey.CLIENT_ID),
                    client_secret = GetAppSetting(AppSettingKey.CLIENT_SECRET),
                };

                string paramString = string.Format("grant_type={0}&client_id={1}&client_secret={2}&username={3}&password={4}", dataParam.grant_type, dataParam.client_id, dataParam.client_secret, dataParam.username, dataParam.password);
                string clientIdSerialize = string.Format("{0}:{1}", dataParam.client_id, dataParam.client_secret);
                string sAuthorization = "Basic " + Convert.ToBase64String(encoding.GetBytes(clientIdSerialize));

                var client = new RestClient(apiPath);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddHeader("authorization", sAuthorization);
                request.AddParameter("application/x-www-form-urlencoded", paramString, ParameterType.RequestBody);

                // Call api lấy token
                var response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseResult = JsonConvert.DeserializeObject<ResponseApi>(response.Content);

                    token = "Bearer " + responseResult.access_token;
                }
            }
            catch (Exception e)
            {

            }

            return token;
        }

        /// <summary>

        /// Lấy thông tin kết quả từ api app khác

        /// </summary>

        /// <param name="dataParam">param truyền sang api</param>

        /// <param name="apiUrl">url của API</param>

        /// <param name="serviceApiType">Loại API</param>

        /// <returns>Kết quả trả về từ API</returns>

        public async static Task<object> GetResultFromApiAppOther(object dataParam, string apiUrl, Enumaration.ServiceApiType serviceApiType)
        {
            string responseString = string.Empty;
            string sAuthorization = await GetTokenBTC();

            try
            {
                if (!string.IsNullOrWhiteSpace(sAuthorization))
                {
                    System.Text.UTF8Encoding encode = new System.Text.UTF8Encoding();
                    byte[] data = encode.GetBytes(JsonConvert.SerializeObject(dataParam));
                    string apiPath = string.Format("{0}/{1}", apiUrl, GetApiNameForApiType(serviceApiType));

                    ServicePointManager.ServerCertificateValidationCallback += (objSender, cert, chain, sslPolicyErrors) => true;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiPath);

                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.ContentLength = data.Length;
                    request.Headers.Add("Authorization", sAuthorization);

                    using (Stream dataStream = request.GetRequestStream())

                    {

                        dataStream.Write(data, 0, data.Length);

                    }

                    //Lấy kết quả trả về từ service
                    using (HttpWebResponse response = (HttpWebResponse)(request.GetResponse()))
                    {
                        responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return responseString;
        }

        /// <summary>

        /// Lấy tên service theo Type

        /// </summary>

        /// <param name="serviceApiType">serviceApiType</param>

        public static string GetApiNameForApiType(Enumaration.ServiceApiType serviceApiType)

        {

            string apiName = string.Empty;

            switch (serviceApiType)

            {

                case Enumaration.ServiceApiType.THAReceiveTaisan:

                    apiName = "api/tdtt/ReceiveTaisan";

                    break;

                case Enumaration.ServiceApiType.THAReceiveLoaitaisan:

                    apiName = "api/tdtt/ReceiveLoaitaisan";

                    break;

                case Enumaration.ServiceApiType.THAReceiveLotaisan:

                    apiName = "api/tdtt/ReceiveLotaisan";

                    break;

                case Enumaration.ServiceApiType.THAReceivePhuongthucHT:

                    apiName = "api/tdtt/ReceivePhuongthucHT";

                    break;

                case Enumaration.ServiceApiType.THAReceiveKytinhkhauhao:

                    apiName = "api/tdtt/ReceiveKytinhkhauhao";

                    break;

                case Enumaration.ServiceApiType.THAReceiveBophansudung:

                    apiName = "api/tdtt/ReceiveBophansudung";

                    break;

                case Enumaration.ServiceApiType.THAReceiveNguonkinhphi:

                    apiName = "api/tdtt/ReceiveNguonkinhphi";

                    break;

                case Enumaration.ServiceApiType.THAReceiveHinhthucmua:

                    apiName = "api/tdtt/ReceiveHinhthucmua";

                    break;

                case Enumaration.ServiceApiType.THAReceiveHientrangsudung:

                    apiName = "api/tdtt/ReceiveHientrangsudung";

                    break;

                case Enumaration.ServiceApiType.THAReceiveDuan:

                    apiName = "api/tdtt/ReceiveDuan";

                    break;

                case Enumaration.ServiceApiType.THAReceiveNhacungcap:

                    apiName = "api/tdtt/ReceiveNhacungcap";

                    break;

                case Enumaration.ServiceApiType.THAReceiveBiendong:

                    apiName = "api/tdtt/ReceiveBiendong";

                    break;

                case Enumaration.ServiceApiType.THAReceiveLoaixe:

                    apiName = "api/tdtt/ReceiveLoaixe";

                    break;

                case Enumaration.ServiceApiType.THAReceiveCapHangNha:

                    apiName = "api/tdtt/ReceiveCapHangNha";

                    break;

                case Enumaration.ServiceApiType.THAReceiveTrangthaipd:

                    apiName = "api/tdtt/ReceiveTrangthaipd";

                    break;

                case Enumaration.ServiceApiType.THAReceiveCongNangNha:

                    apiName = "api/tdtt/ReceiveCongNangNha";

                    break;

                case Enumaration.ServiceApiType.THAReceiveMucdichsdd:

                    apiName = "api/tdtt/ReceiveMucdichsdd ";

                    break;

                case Enumaration.ServiceApiType.THAReceiveDonvi:

                    apiName = "api/tdtt/ReceiveDonvi";

                    break;

                case Enumaration.ServiceApiType.THAReceiveNuocsanxuat:

                    apiName = "api/tdtt/ReceiveNuocsanxuat";

                    break;

                case Enumaration.ServiceApiType.THAReceiveNhanhieu:

                    apiName = "api/tdtt/ReceiveNhanhieu";

                    break;

                case Enumaration.ServiceApiType.THAReceiveChucdanh:

                    apiName = "api/tdtt/ReceiveChucdanh";

                    break;

            }

            return apiName;

        }

        /// <summary>

        /// Lấy app setting

        /// </summary>

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
                throw;
            }

            return res;
        }
    }
}