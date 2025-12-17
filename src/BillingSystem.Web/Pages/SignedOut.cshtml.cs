using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BillingSystem.Web.Pages
{
    [AllowAnonymous]
    public class SignedOutModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
