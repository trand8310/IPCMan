using MainClient.Infrastructure;
using MainClient.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using System.Text.Json.Nodes;
using System.Web;

namespace MainClient.Common
{
    public class UrlHelper
    {

        /// <summary>
        /// URL 宏处理
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ip"></param>
        /// <param name="task"></param>
        /// <param name="dev"></param>
        /// <param name="os"></param>
        /// <returns></returns>
        public static string URLMacroReplacement(string url, string ip, JsonNode task, JsonNode dev, OSType os, AppSettings appSettings, long timestamp = 0)
        {
            string domain = new Uri(url).Host;
            if (domain.Contains("miaozhen.com"))
            {
                return miaozhen(url, ip, task, dev, os, appSettings, timestamp);
            }
            else if (domain.Contains("gridsumdissector.com"))
            {
                return gridsumdissector(url, ip, task, dev, os, appSettings, timestamp);
            }
            else if (domain.Contains("ipinyou.com"))
            {
                return ipinyou(url, ip, task, dev, os, appSettings, timestamp); ;
            }
            else if (domain.Contains("stats.dmp.ghac.cn"))
            {
                return dmpghac(url, ip, task, dev, os, appSettings, timestamp);
            }
            else if (domain.Contains("mafengwo.cn"))
            {
                return mafengwo(url, ip, task, dev, os, appSettings, timestamp);
            }
            else
            {
                return miaozhen(url, ip, task, dev, os, appSettings);
            }
        }

        /// <summary>
        /// 秒针URL处理
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ip"></param>
        /// <param name="param"></param>
        /// <param name="os"></param>
        /// <param name="dev"></param>
        /// <returns></returns>
        private static string miaozhen(string url, string ip, JsonNode task, JsonNode dev, OSType os, AppSettings appSettings, long timestamp = 0)
        {
            if (timestamp == 0)
                timestamp = CommonHelper.UnixTimeNowSecond();

            if (url.Contains("[timestamp]"))
                url = url.Replace("[timestamp]", timestamp.ToString());
            if (url.Contains("__TS__"))
                url = url.Replace("__TS__", timestamp.ToString());

            var huichuanip = task["huichuanip"]?.GetValue<string>() ?? "";
            if (huichuanip.Equals("on") && !string.IsNullOrWhiteSpace(ip))
            {
                url = url.Replace("__IP__", ip);
            }

            var huichuan = task["huichuan"]?.GetValue<string>() ?? "";

            if (huichuan.Equals("on"))
            {
                //__OS__//1位数字,取0~3。0表示Android，1表示iOS，2表示Windows Phone，3表示其他
                if (!appSettings.NoneOS)
                {
                    if (os == OSType.ANDROID)
                        url = url.Replace("__OS__", "0");
                    else if (os == OSType.IOS)
                        url = url.Replace("__OS__", "1");
                    else if (os == OSType.WINDOWS_PHONE)
                        url = url.Replace("__OS__", "2");
                    else
                        url = url.Replace("__OS__", "3");
                }

                if (os == OSType.IOS)
                {
                    string idfa = dev["idfa"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(idfa))
                    {
                        url = url.Replace("__IDFA__", idfa);
                    }
                    if (appSettings.Using_iOS_IMEI)
                    {
                        string imei = dev["imei"]?.GetValue<string>() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(imei))
                        {
                            var imei_md5 = CommonHelper.MD5Hash(imei);
                            url = url.Replace("__IMEI__", imei_md5);
                        }
                    }
                    if (appSettings.Using_iOS_MAC)
                    {
                        string mac = dev["mac"]?.GetValue<string>() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(mac))
                        {
                            var macmd51 = CommonHelper.MD5Hash(mac);
                            var macmd52 = CommonHelper.MD5Hash(mac.Replace(":", ""));
                            url = url.Replace("__MAC1__", macmd51);
                            url = url.Replace("__MAC__", macmd52);
                        }
                    }
                }

                else if (os == OSType.ANDROID)
                {
                    string mac = dev["mac"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(mac))
                    {
                        var macmd51 = CommonHelper.MD5Hash(mac);
                        var macmd52 = CommonHelper.MD5Hash(mac.Replace(":", ""));
                        url = url.Replace("__MAC1__", macmd51);
                        url = url.Replace("__MAC__", macmd52);
                    }


                    string imei = dev["imei"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(imei))
                    {
                        var imei_md5 = CommonHelper.MD5Hash(imei);
                        url = url.Replace("__IMEI__", imei_md5);
                    }

                    string androidId = dev["androidid"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(androidId))
                    {
                        var androidId_md5 = CommonHelper.MD5Hash(androidId);
                        url = url.Replace("__ANDROIDID__", androidId_md5);
                        url = url.Replace("__ANDROIDID1__", androidId);
                    }

                    string oaid = dev["oaid"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(oaid))
                    {
                        var oaid_md5 = CommonHelper.MD5Hash(oaid);
                        url = url.Replace("__OAID__", oaid);
                        url = url.Replace("__OAID1__", oaid_md5);
                    }
                }
            }



            return url;
        }


        /// <summary>
        /// 国双URL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ip"></param>
        /// <param name="param"></param>
        /// <param name="os"></param>
        /// <param name="dev"></param>
        /// <returns></returns>
        private static string gridsumdissector(string url, string ip, JsonNode task, JsonNode dev, OSType os, AppSettings appSettings, long timestamp = 0)
        {
            //https://i.gridsumdissector.com/v/?gscmd=impress&gid=gad_155_Y9RU7SQW&os=__OS__&if=__IDFA__&oid=__OPENUDID__&aid=__ANDROIDID__&im=__IMEI__&oa=__OAID__&m=__MAC__&ip=__IP__&ts=__TS__&did=__DUID__&aaid=__AAID__&uid=__UDID__&odin=__ODIN__&ua=__UA__&lbs=__LBS__

            if (timestamp == 0)
                timestamp = CommonHelper.UnixTimeNowSecond();


            if (url.Contains("[timestamp]"))
                url = url.Replace("[timestamp]", timestamp.ToString());
            if (url.Contains("__TS__"))
                url = url.Replace("__TS__", timestamp.ToString());

            var huichuanip = task["huichuanip"]?.GetValue<string>() ?? "";
            if (huichuanip.Equals("on") && !string.IsNullOrWhiteSpace(ip))
            {
                url = url.Replace("__IP__", ip);
            }

            var huichuan = task["huichuan"]?.GetValue<string>() ?? "";
            if (huichuan.Equals("on"))
            {
                //__OS__//1位数字,取0~3。0表示Android，1表示iOS，2表示Windows Phone，3表示其他 
                if (!appSettings.NoneOS)
                {
                    if (os == OSType.ANDROID)
                        url = url.Replace("__OS__", "0");
                    else if (os == OSType.IOS)
                        url = url.Replace("__OS__", "1");
                    else if (os == OSType.WINDOWS_PHONE)
                        url = url.Replace("__OS__", "2");
                    else
                        url = url.Replace("__OS__", "3");
                }


                if (os == OSType.IOS)
                {
                    string idfa = dev["idfa"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(idfa))
                    {
                        url = url.Replace("__IDFA__", idfa);
                    }
                    if (appSettings.Using_iOS_IMEI)
                    {
                        string imei = dev["imei"]?.GetValue<string>() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(imei))
                        {
                            var imei_md5 = CommonHelper.MD5Hash(imei);
                            url = url.Replace("__IMEI__", imei_md5);
                        }
                    }
                    if (appSettings.Using_iOS_MAC)
                    {
                        string mac = dev["mac"]?.GetValue<string>() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(mac))
                        {
                            var mac_md5 = CommonHelper.MD5Hash(mac);
                            url = url.Replace("__MAC__", mac_md5);
                        }
                    }
                }
                else if (os == OSType.ANDROID)
                {
                    string mac = dev["mac"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(mac))
                    {
                        var mac_md5 = CommonHelper.MD5Hash(mac);
                        url = url.Replace("__MAC__", mac_md5);
                    }

                    string imei = dev["imei"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(imei))
                    {
                        var imei_md5 = CommonHelper.MD5Hash(imei);
                        url = url.Replace("__IMEI__", imei_md5);
                    }

                    string androidId = dev["androidid"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(androidId))
                    {
                        var androidId_md5 = CommonHelper.MD5Hash(androidId);
                        url = url.Replace("__ANDROIDID__", androidId_md5);
                    }

                    string oaid = dev["oaid"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(oaid))
                    {
                        var oaid_md5 = CommonHelper.MD5Hash(oaid);
                        url = url.Replace("__OAID__", oaid_md5);
                    }

                }
                url = url.Replace("__TS__", CommonHelper.UnixTimeNow().ToString());
            }



            return url;
        }

        /// <summary>
        /// 深演广告
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ip"></param>
        /// <param name="param"></param>
        /// <param name="os"></param>
        /// <param name="dev"></param>
        /// <returns></returns>
        private static string ipinyou(string url, string ip, JsonNode task, JsonNode dev, OSType os, AppSettings appSettings, long timestamp = 0)
        {
            //http://vt.ipinyou.com/IinK3066gI5vwOkVZ-.IcX5R_.sWLZhPIi7pbkvccpO3kUXEe5DrZWFlJbrDuAyySZ_T8kzY9epmcXfrEv_RzyW4f.txHx607mbPPtH8cJVys8k_?tmp=[timestamp]&mob_idfa=[idfa]&mob_imei=[imei]&mob_android=[androidid]&mob_os=[os]&mob_oaid=[oaid]&mob_mac=[mac]

            if (timestamp == 0)
                timestamp = CommonHelper.UnixTimeNowSecond();


            if (url.Contains("[timestamp]"))
                url = url.Replace("[timestamp]", timestamp.ToString());

            var huichuan = task["huichuan"]?.GetValue<string>() ?? "";
            if (huichuan.Equals("on"))
            {
                //[os]//1位数字,取0~3。0表示Android，1表示iOS，2表示Windows Phone，3表示其他 
                if (os == OSType.ANDROID)
                    url = url.Replace("[os]", "0");
                else if (os == OSType.IOS)
                    url = url.Replace("[os]", "1");
                else if (os == OSType.WINDOWS_PHONE)
                    url = url.Replace("[os]", "2");
                else
                    url = url.Replace("[os]", "3");

                if (os == OSType.IOS)
                {
                    string idfa = dev["idfa"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(idfa))
                    {
                        url = url.Replace("[idfa]", idfa);
                    }
                }
                else if (os == OSType.ANDROID)
                {

                    string mac = dev["mac"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(mac))
                    {
                        var mac_md5 = CommonHelper.MD5Hash(mac.Replace(":", "").ToUpper());
                        url = url.Replace("[mac]", mac_md5);
                    }

                    string imei = dev["imei"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(imei))
                    {
                        var imei_md5 = CommonHelper.MD5Hash(imei.ToLower());
                        url = url.Replace("[imei]", imei_md5);
                    }

                    string androidId = dev["androidid"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(androidId))
                    {
                        var androidId_md5 = CommonHelper.MD5Hash(androidId.ToLower());
                        url = url.Replace("[androidid]", androidId_md5);
                    }

                    string oaid = dev["oaid"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(oaid))
                    {
                        var oaid_md5 = CommonHelper.MD5Hash(oaid.ToLower());
                        url = url.Replace("[oaid]", oaid_md5);
                    }
                }
            }
            return url;
        }

        /// <summary>
        /// 广本DMP
        ///  https://stats.dmp.ghac.cn/imp/QGe3gXV_J7tzpt.MeAYoP?u=__URL__&os=__OS__&imei=__IMEI__&mac=__MAC__&mac1=__MAC1__&idfa=__IDFA__&oaid=__OAID__&aaid=__AAID__&openudid=__OPENUDID__&androidid=__ANDROIDID__&duid=__DUID__&ip=__IP__&ua=__UA__&ts=__TS__
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ip"></param>
        /// <param name="param"></param>
        /// <param name="os"></param>
        /// <param name="dev"></param>
        /// <returns></returns>
        private static string dmpghac(string url, string ip, JsonNode task, JsonNode dev, OSType os, AppSettings appSettings, long timestamp = 0)
        {
            if (timestamp == 0)
                timestamp = CommonHelper.UnixTimeNowSecond();


            var huichuanip = task["huichuanip"]?.GetValue<string>() ?? "";
            if (huichuanip.Equals("on") && !string.IsNullOrWhiteSpace(ip))
            {
                url = url.Replace("__IP__", ip);
            }
            var huichuan = task["huichuan"]?.GetValue<string>() ?? "";
            if (huichuan.Equals("on"))
            {
                //__OS__//1位数字,取0~3。0表示Android，1表示iOS，2表示Windows Phone，3表示其他
                if (os == OSType.ANDROID)
                    url = url.Replace("__OS__", "0");
                else if (os == OSType.IOS)
                    url = url.Replace("__OS__", "1");
                else if (os == OSType.WINDOWS_PHONE)
                    url = url.Replace("__OS__", "2");
                else
                    url = url.Replace("__OS__", "3");

                url = url.Replace("__TS__", timestamp.ToString());
                string ua = dev["ua"]?.GetValue<string>() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(ua))
                    url = url.Replace("__UA__", System.Web.HttpUtility.UrlEncode(ua));

                if (os == OSType.IOS)
                {
                    string idfa = dev["idfa"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(idfa))
                    {
                        url = url.Replace("__IDFA__", idfa);
                    }
                }
                else if (os == OSType.ANDROID)
                {
                    string mac = dev["mac"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(mac))
                    {
                        var macmd51 = CommonHelper.MD5Hash(mac.Replace(":", ""));
                        var macmd52 = CommonHelper.MD5Hash(mac);
                        url = url.Replace("__MAC__", macmd51);
                        url = url.Replace("__MAC1__", macmd52);
                    }


                    string imei = dev["imei"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(imei))
                    {
                        var imei_md5 = CommonHelper.MD5Hash(imei);
                        url = url.Replace("__IMEI__", imei_md5);
                    }

                    string androidId = dev["androidid"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(androidId))
                    {
                        var androidId_md5 = CommonHelper.MD5Hash(androidId);
                        url = url.Replace("__ANDROIDID__", androidId_md5);
                    }

                    string oaid = dev["oaid"]?.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(oaid))
                    {
                        var oaid_md5 = CommonHelper.MD5Hash(oaid);
                        url = url.Replace("__OAID__", oaid_md5);
                    }

                }
            }

            return url;
        }

        private static string mafengwo(string url, string ip, JsonNode task, JsonNode dev, OSType os, AppSettings appSettings, long timestamp = 0)
        {
            if (timestamp == 0)
                timestamp = CommonHelper.UnixTimeNowSecond();
            var huichuan = task["huichuan"]?.GetValue<string>() ?? "";

            if (!huichuan.Equals("on", StringComparison.OrdinalIgnoreCase))
                return url;

            var updates = new Dictionary<string, string>(StringComparer.Ordinal);

            var ts = timestamp.ToString();

            // 时间戳
            SetMacro(updates, "timestamp", ts);
            SetMacro(updates, "ts", ts);

            // IP
            if (!string.IsNullOrWhiteSpace(ip))
            {
                SetMacro(updates, "ip", ip);
            }

            // OS
            string os_val = string.Empty;

            if (os == OSType.ANDROID)
                os_val = "0";
            else if (os == OSType.IOS)
                os_val = "1";
            else if (os == OSType.WINDOWS_PHONE)
                os_val = "2";
            else
                os_val = "3";

            SetMacro(updates, "os", os_val);

            if (os == OSType.IOS)
            {
                if (appSettings.Using_iOS_IMEI)
                {
                    string imei = dev["imei"]?.GetValue<string>() ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(imei))
                    {
                        string imeiMd5 = CommonHelper.MD5Hash(imei);
                        SetMacro(updates, "imei", imeiMd5);
                    }
                }

                if (appSettings.Using_iOS_MAC)
                {
                    string mac = dev["mac"]?.GetValue<string>() ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(mac))
                    {
                        string macMd5 = CommonHelper.MD5Hash(mac.Replace(":", ""));
                        SetMacro(updates, "mac", macMd5);
                    }
                }

                string idfa = dev["idfa"]?.GetValue<string>() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(idfa))
                {
                    SetMacro(updates, "idfa", idfa);
                }
            }
            else if (os == OSType.ANDROID)
            {
                string mac = dev["mac"]?.GetValue<string>() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(mac))
                {
                    string macMd5 = CommonHelper.MD5Hash(mac.Replace(":", ""));
                    SetMacro(updates, "mac", macMd5);
                }

                string imei = dev["imei"]?.GetValue<string>() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(imei))
                {
                    string imeiMd5 = CommonHelper.MD5Hash(imei);
                    SetMacro(updates, "imei", imeiMd5);
                }

                string androidId = dev["androidid"]?.GetValue<string>() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(androidId))
                {
                    // 原值
                    SetMacro(updates, "androidid1", androidId);

                    // MD5 值
                    string androidIdMd5 = CommonHelper.MD5Hash(androidId);
                    SetMacro(updates, "androidid", androidIdMd5);
                }

                string oaid = dev["oaid"]?.GetValue<string>() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(oaid))
                {
                    // 原值
                    SetMacro(updates, "oaid", oaid);

                    // MD5 值
                    string oaidMd5 = CommonHelper.MD5Hash(oaid);
                    SetMacro(updates, "oaid1", oaidMd5);
                }
            }

            url = ReplaceUrlMacros(url, updates);

            return url;
        }
        private static void SetMacro(Dictionary<string, string> macros, string name, string value)
        {
            if (macros == null)
                return;

            if (string.IsNullOrWhiteSpace(name))
                return;

            value = value ?? string.Empty;

            // __IDFA__ / __OAID__ / __ANDROIDID__
            macros["__" + name.ToUpperInvariant() + "__"] = value;

            // [idfa] / [oaid] / [androidid]
            macros["[" + name.ToLowerInvariant() + "]"] = value;
        }
        private static string ReplaceUrlMacros(string url, Dictionary<string, string> macros)
        {
            if (string.IsNullOrWhiteSpace(url))
                return url;

            if (macros == null || macros.Count == 0)
                return url;

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                // 非标准 URL，直接整体宏替换
                return ReplaceMacros(url, macros);
            }

            string baseUrl = uri.GetLeftPart(UriPartial.Path);
            string query = uri.Query;
            string fragment = uri.Fragment;

            if (string.IsNullOrEmpty(query))
                return baseUrl + fragment;

            query = query.Substring(1);

            string[] parts = query.Split('&');

            for (int i = 0; i < parts.Length; i++)
            {
                if (string.IsNullOrEmpty(parts[i]))
                    continue;

                int eqIndex = parts[i].IndexOf('=');

                if (eqIndex < 0)
                {
                    // 没有 = 的参数，例如 ?debug
                    parts[i] = ReplaceMacros(parts[i], macros);
                    continue;
                }

                string key = parts[i].Substring(0, eqIndex);
                string value = parts[i].Substring(eqIndex + 1);

                if (string.IsNullOrEmpty(value))
                    continue;

                // 先解码，这样可以处理 target 里面的嵌套 URL
                string decodedValue = HttpUtility.UrlDecode(value) ?? string.Empty;

                // 只替换参数值里的宏，不替换参数名
                string replacedValue = ReplaceMacros(decodedValue, macros);

                // 再编码回去
                string encodedValue = HttpUtility.UrlEncode(replacedValue);

                parts[i] = key + "=" + encodedValue;
            }

            return baseUrl + "?" + string.Join("&", parts) + fragment;
        }
        private static string ReplaceMacros(string text, Dictionary<string, string> macros)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            if (macros == null || macros.Count == 0)
                return text;

            foreach (var item in macros.OrderByDescending(x => x.Key.Length))
            {
                if (string.IsNullOrEmpty(item.Key))
                    continue;

                if (text.IndexOf(item.Key, StringComparison.Ordinal) >= 0)
                {
                    text = text.Replace(item.Key, item.Value ?? string.Empty);
                }
            }

            return text;
        }
    }
}
