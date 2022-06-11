using System.Web;
using System.Web.Mvc;

namespace Amorim_P3_Books_Finalforreal
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
