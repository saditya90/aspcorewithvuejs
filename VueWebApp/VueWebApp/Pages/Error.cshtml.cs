using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace VueWebApp.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }

    public class VueWebAppKeys
    {
        public const string DataKey = "_VueWebAppDataKey";

        public static IEnumerable<SelectListItem> GetItems()
        => Enum.GetNames(typeof(PhoneNumberOrAddressType)).Select(q => new SelectListItem
        {
            Text = q,
            Value = ((int)Enum.Parse(typeof(PhoneNumberOrAddressType), q)).ToString()
        });
    }
}
