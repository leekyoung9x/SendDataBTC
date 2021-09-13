using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class DemoController : ApiController
    {
        /// <summary>
        /// Đầu api gửi dữ liệu báo cáo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("report")]
        public Task<object> Post([FromBody] DataReport param)
        {
            try
            {
                string apiPath = CommonFunction.GetAppSetting(AppSettingKey.API_URL_TOKEN);
                string urlTemplate = CommonFunction.GetAppSetting(AppSettingKey.API_URL_REPORT);
                string endPoint = string.Format(urlTemplate, param.Key);

                if (!string.IsNullOrEmpty(apiPath) && !string.IsNullOrEmpty(endPoint))
                {
                    return CommonFunction.SendDataBTC(apiPath, endPoint, param.Data, HttpMethod.Post);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Có lỗi xảy ra: ");
                Console.WriteLine(e);
            }

            return null;
        }

        /// <summary>
        /// Đầu api gửi dữ liệu tài sản
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("asset")]
        public Task<object> PostAsset([FromBody] DataReport param)
        {
            try
            {
                string apiPath = CommonFunction.GetAppSetting(AppSettingKey.API_URL_TOKEN);
                string urlTemplate = CommonFunction.GetAppSetting(AppSettingKey.API_URL_ASSET);
                string endPoint = string.Format(urlTemplate, param.Key);

                if (!string.IsNullOrEmpty(apiPath) && !string.IsNullOrEmpty(endPoint))
                {
                    return CommonFunction.SendDataBTC(apiPath, endPoint, param.Data, HttpMethod.Post);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Có lỗi xảy ra: ");
                Console.WriteLine(e);
            }

            return null;
        }

        /// <summary>
        /// Đầu api gửi dữ liệu báo cáo
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("asset")]
        public Task<object> GetAsset([FromUri] string syncDate)
        {
            try
            {
                string apiPath = CommonFunction.GetAppSetting(AppSettingKey.API_URL_TOKEN);
                string urlTemplate = CommonFunction.GetAppSetting(AppSettingKey.API_URL_ASSET);
                string endPoint = string.Format(urlTemplate, string.Format("getSynchronizedData?syncDate={0}", syncDate));

                if (!string.IsNullOrEmpty(apiPath) && !string.IsNullOrEmpty(endPoint))
                {
                    return CommonFunction.SendDataBTC(apiPath, endPoint, null, HttpMethod.Get);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Có lỗi xảy ra: ");
                Console.WriteLine(e);
            }

            return null;
        }
    }
}