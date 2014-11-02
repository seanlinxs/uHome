using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Mvc;

namespace uHome
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new RequireHttpsAttribute());
        }
    }
}
