using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public static class Helper
    {
        public static string GetQueryString(Dictionary<string, string> query)
        {
            var keyValues = query
                .Select(e => $"{Uri.EscapeDataString(e.Key)}={Uri.EscapeDataString(e.Value)}")
                .ToArray();

            return string.Join("&", keyValues);
        }
    }
}
