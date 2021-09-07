using System.Collections.Generic;

namespace WebApplication2.Models
{
    public class AppSettingKey
    {
        public static string THAAPIPath = "THAAPIPath";

        public static string GRANT_TYPE = "GRANT_TYPE";

        public static string USERNAME = "USERNAME";

        public static string PASSWORD = "PASSWORD";

        public static string CLIENT_ID = "CLIENT_ID";

        public static string CLIENT_SECRET = "CLIENT_SECRET";

        public static string TOKEN_END_POINT = "TOKEN_END_POINT";

        public static string API_URL_TOKEN = "API_URL_TOKEN";
    }

    /// <summary>
    /// Mapping các key để lấy ra api đồng bộ báo cáo
    /// </summary>
    public class MappingApi
    {
        public static Dictionary<string, string> apiKeys = new Dictionary<string, string>()
        {
            {"rpt11bcktsc", "aprserver/report/rpt11bcktsc/sync"},
            {"rpt08adktsc01", "aprserver/report/rpt08adktsc01/sync"},
            {"rpt08adktsc02", "aprserver/report/rpt08adktsc02/sync"},
            {"rpt08adktsc03", "aprserver/report/rpt08adktsc03/sync"},
            {"rpt08bdktsc01", "aprserver/report/rpt08bdktsc01/sync"},
            {"rpt08bdktsc02", "aprserver/report/rpt08bdktsc02/sync"},
            {"rpt08bdktsc03", "aprserver/report/rpt08bdktsc03/sync"},
            {"rpt09acktsc", "aprserver/report/rpt09acktsc/sync"},
            {"rpt09bcktsc", "aprserver/report/rpt09bcktsc/sync"},
            {"rpt09ccktsc", "aprserver/report/rpt09ccktsc/sync"},
            {"rpt09dcktsc", "aprserver/report/rpt09dcktsc/sync"},
            {"rpt09ddcktsc", "aprserver/report/rpt09ddcktsc/sync"},
            {"rpt10acktsc", "aprserver/report/rpt10acktsc/sync"},
            {"rpt10bcktsc", "aprserver/report/rpt10bcktsc/sync"},
            {"rpt10ccktsc", "aprserver/report/rpt10ccktsc/sync"},
            {"rpt10dcktsc", "aprserver/report/rpt10dcktsc/sync"},
            {"rpt08adktskcht01", "aprserver/report/rpt08adktskcht01/sync"},
            {"rpt08adktskcht02", "aprserver/report/rpt08adktskcht02/sync"},
            {"rpt08adktskcht03", "aprserver/report/rpt08adktskcht03/sync"},
        };
    }
}