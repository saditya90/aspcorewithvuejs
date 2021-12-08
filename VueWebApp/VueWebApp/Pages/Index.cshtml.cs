using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace VueWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public IndexViewModel ViewModel { get; set; } = new();
        public string PageTitle { get; init; } = "Home page";

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            _logger.LogInformation(JsonSerializer.Serialize(ViewModel));
        }

        public class IndexViewModel
        {
            public string WelcomeText { get; init; } = "Welcome";
            public string LearnText { get; init; } = @"Learn about <a href=""https://vuejs.org/"">building Web apps with Vue Js</a>.";
            public string ExampleAbout { get; init; } = @"This page is running on Vue Js Version <a href=""https://unpkg.com/browse/vue@2.6.14/"">2.6.14<a>. All content is binding through Vue Js.";
        }
    }
}
