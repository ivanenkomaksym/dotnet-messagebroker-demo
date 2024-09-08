using Microsoft.AspNetCore.Mvc;

namespace WebUI
{
    public static class Helpers
    {
        public static string GetLocalUrl(IUrlHelper urlHelper, string? localUrl)
        {
            ArgumentNullException.ThrowIfNull(urlHelper);
            if (!urlHelper.IsLocalUrl(localUrl))
            {
                return urlHelper!.Page("/Index") ?? string.Empty;
            }

            return localUrl;
        }
    }
}
