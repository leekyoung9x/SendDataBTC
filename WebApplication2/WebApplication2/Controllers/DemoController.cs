using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class DemoController : ApiController
    {
        // GET api/values
        public Task<object> Get()
        {
            return CommonFunction.SendDataBTC("http://14.248.83.163:9001/", "aprserver/report/rpt11bcktsc/sync", data());
        }

        private object data()
        {
            var data = new
            {
                reportTypeCode = "RPT_11B_CK_TSC_03",
                reportYear = 2021
            };

            return data;
        }
    }
}
