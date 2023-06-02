using Microsoft.AspNetCore.Mvc;

namespace WebUI
{
    public static class Helpers
    {
        public static string GetLocalUrl(IUrlHelper urlHelper, string localUrl)
        {
            if (!urlHelper.IsLocalUrl(localUrl))
            {
                return urlHelper!.Page("/Index");
            }

            return localUrl;
        }
    }
}
