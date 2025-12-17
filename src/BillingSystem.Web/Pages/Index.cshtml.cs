using Microsoft.AspNetCore.Authorization;

namespace BillingSystem.Web.Pages
{
    [Authorize]
    public class IndexModel : AuthPage
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
