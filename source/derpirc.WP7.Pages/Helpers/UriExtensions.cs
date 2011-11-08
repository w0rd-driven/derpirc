using System;
using System.Collections.Generic;
using System.Net;

namespace derpirc.Helpers
{
    public static class UriExtensions
    {
        public static Dictionary<string, string> ParseQueryString(this Uri uri)
        {
            var result = new Dictionary<string, string>();
            var queryString = uri.OriginalString.Substring(uri.OriginalString.IndexOf("?"));
            string[] items = queryString.Split('&');
            foreach (string item in items)
            {
                if (item.Contains("="))
                {
                    string[] nameValue = item.Split('=');
                    if (nameValue[0].Contains("?"))
                        nameValue[0] = nameValue[0].Replace("?", "");
                    result.Add(nameValue[0], HttpUtility.UrlDecode(nameValue[1]));
                }
            }
            return result;
        }
    }
}
