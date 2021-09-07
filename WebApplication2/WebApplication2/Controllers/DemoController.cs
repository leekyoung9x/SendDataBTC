using System.Threading.Tasks;
using System.Web.Http;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class DemoController : ApiController
    {
        [HttpPost]
        public Task<object> Post([FromBody] DataReport param)
        {
            return CommonFunction.SendDataBTC(CommonFunction.GetAppSetting(AppSettingKey.API_URL_TOKEN), MappingApi.apiKeys[param.Key], param.Data);
        }
    }
}