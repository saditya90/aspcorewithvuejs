using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;

namespace VueWebApp.Pages
{
    public class CustomersModel : PageModel
    {
        private readonly IMemoryCache _memoryCache;
        public List<CustomerViewModel> Customers { get; set; } = new();
        public IEnumerable<SelectListItem> SelectListItems { get; internal set; } = VueWebAppKeys.GetItems();
        public string PageTitle { get; init; } = "Available Customer(s)";
        public CustomersModel(IMemoryCache memoryCache)
        => _memoryCache = memoryCache;

        public void OnGet() => SeedCustomers();

        private void SeedCustomers()
        {
            if (_memoryCache.TryGetValue<List<CustomerViewModel>>(VueWebAppKeys.DataKey, out var customers))
            {
                Customers = customers; return;
            }

            Customers.Add(new CustomerViewModel { Id = 1, First = "Jhon", Last = "E", Email = "jhon@yahoo.co.uk" });
            Customers.Add(new CustomerViewModel { Id = 2, First = "Rick", Last = "L", Email = "rick@yahoo.co.uk" });
            Customers.Add(new CustomerViewModel { Id = 3, First = "Elena", Last = "G", Email = "elena@yahoo.co.uk" });
            Customers.Add(new CustomerViewModel { Id = 4, First = "Emly", Last = "S", Email = "emly@yahoo.co.uk" });
            _memoryCache.Set(VueWebAppKeys.DataKey, Customers);
        }

        public IActionResult OnGetRemove(int id) 
        {
            if (_memoryCache.TryGetValue<List<CustomerViewModel>>(VueWebAppKeys.DataKey, out var customers))
            {
                var customer = customers.Find(q => q.Id == id);
                if (customer is not null)
                {
                    customers.Remove(customer);
                    Customers = customers;
                }
            }

            return RedirectToPage();
        }
    }

    public class CustomerViewModel
    {  
        public int Id { get; set; }
        [Required(ErrorMessage = "*Required")]
        public string First { get; set; }
        [Required(ErrorMessage = "*Required")]
        public string Last { get; set; }
        [Required(ErrorMessage = "*Required"), EmailAddress(ErrorMessage = "*Invalid Email")]
        public string Email { get; set; }
        public List<ContactDetails> ContactDetails { get; set; } = new();
        public List<AddressDetails> AddressDetails { get; set; } = new();
    }

    public class ContactDetails
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "*Required")]
        public PhoneNumberOrAddressType? NumberType { get; set; }
        [Required(ErrorMessage = "*Required")]
        public string PhoneNumber { get; set; }
    }

    public class AddressDetails
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "*Required")]
        public PhoneNumberOrAddressType? AddressType { get; set; }
        [Required(ErrorMessage = "*Required")]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        [Required(ErrorMessage = "*Required")]
        public string PinCode { get; set; }
        [Required(ErrorMessage = "*Required")]
        public string City { get; set; }
        [Required(ErrorMessage = "*Required")]
        public string State { get; set; }
        [Required(ErrorMessage = "*Required")]
        public string Country { get; set; }
    }

    public enum PhoneNumberOrAddressType
    {
        Work = 1,
        Home,
        Other
    }
}
