using System;
using System.Web;

namespace SymHack.Controllers
{
    // From http://www.chwe.at/2009/01/use-wrappers-to-access-your-cookies-sessions/
    public static class CookieWrapper
    {
        public static string GuestLog
        {
            get { return GetValue("GuestLog"); }
            set { SetValue("GuestLog", value, DateTime.Now.AddDays(1)); }
        }

        public static string MusicStyle
        {
            get { return GetValue("MusicStyle"); }
            set { SetValue("MusicStyle", value, DateTime.Now.AddDays(1)); }
        }

        public static string MusicVolume
        {
            get { return GetValue("MusicVolume"); }
            set { SetValue("MusicVolume", value, DateTime.Now.AddDays(1)); }
        }

        private static string GetValue(string key)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];
            if (cookie != null)
                return cookie.Value;
            return null;
        }

        private static void SetValue(string key, string value, DateTime expires)
        {
            HttpContext.Current.Response.Cookies[key].Value = value;
            HttpContext.Current.Response.Cookies[key].Expires = expires;
        }
    }
}