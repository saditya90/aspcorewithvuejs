using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VueWebApp.Pages
{
    public class AddCustomerModel : PageModel
    {
        private readonly IMemoryCache _memoryCache;

        public AddCustomerModel(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            AddDropDownItems();
        }

        public string PageTitle { get; set; } = "Add Customer";

        [BindProperty]
        public CustomerViewModel Customer { get; set; } = new();
        public Dictionary<string, IEnumerable<string>> Errors { get; internal set; }
        public IEnumerable<SelectListItem> SelectListItems { get; internal set; }
        public void OnGet()
        {
            InitilizeDefaultValues();
        }

        private void InitilizeDefaultValues()
        {
            if (Customer.ContactDetails.Count == 0)
                Customer.ContactDetails.Add(new ContactDetails());
            if (Customer.AddressDetails.Count == 0)
                Customer.AddressDetails.Add(new AddressDetails());
        }

        public IActionResult OnPostRegisterAsync()
        {
            ValidateCustomValue();
            if (ModelState.IsValid)
            {
                AddCustomerDetails();
                return RedirectToPage("Customers");
            }
            AddErrors();
            return Page();
        }

        public IActionResult OnPostUpdateAsync()
        {
            ValidateCustomValue();
            if (ModelState.IsValid)
            {
                EditCustomerDetails();
                return RedirectToPage("Customers");
            }
            AddErrors(); UpdatePageTitle();
            return Page();
        }

        public IActionResult OnGetEdit(int id)
        {
            if (_memoryCache.TryGetValue<List<CustomerViewModel>>(VueWebAppKeys.DataKey, out var customers))
            {
                var customer = customers.FirstOrDefault(q => q.Id == id);
                if (customer is not null)
                {
                    UpdateCustomerModel(customer); 
                    UpdatePageTitle();
                    return Page();
                }
                else
                    goto error;
            }

        error:
            return RedirectToPage("Customers");
        }

        private void UpdateCustomerModel(CustomerViewModel customer)
        {  
            Customer.First = customer.First;
            Customer.Last = customer.Last;
            Customer.AddressDetails.AddRange(customer.AddressDetails);
            Customer.ContactDetails.AddRange(customer.ContactDetails);
            Customer.Email = customer.Email;
            Customer.Id = customer.Id;
            InitilizeDefaultValues();
        }

        private void EditCustomerDetails()
        {
            if (_memoryCache.TryGetValue<List<CustomerViewModel>>(VueWebAppKeys.DataKey, out var customers))
            {
                var customer = customers.Find(q => q.Id == Customer.Id);
                var index = customers.IndexOf(customer);
                customers.Insert(index, Customer);
                customers.Remove(customer);
                _memoryCache.Set(VueWebAppKeys.DataKey, customers);
            }
        }

        private void AddCustomerDetails()
        {
            if (_memoryCache.TryGetValue<List<CustomerViewModel>>(VueWebAppKeys.DataKey, out var customers))
            {
                Customer.Id = customers.Count > 0 ? customers.Max(q => q.Id) + 1 : 1;
                customers.Add(Customer);
                _memoryCache.Set(VueWebAppKeys.DataKey, customers);
            }
        }

        private void ValidateCustomValue()
        {
            //Custom validation goes here.
            Customer.ContactDetails.ForEach(c =>
            {
                if (!string.IsNullOrWhiteSpace(c.PhoneNumber) && !long.TryParse(c.PhoneNumber, out _))
                    ModelState.AddModelError($"Customer.ContactDetails[{Customer.ContactDetails.IndexOf(c)}].PhoneNumber", "Phone number should be numeric");
            });

            Customer.AddressDetails.ForEach(a =>
            {
                if (!string.IsNullOrWhiteSpace(a.PinCode) && !long.TryParse(a.PinCode, out _))
                    ModelState.AddModelError($"Customer.AddressDetails[{Customer.AddressDetails.IndexOf(a)}].PinCode", "Pincode should be numeric");
            });

            if (ModelState.IsValid)
            {
                if (_memoryCache.TryGetValue<List<CustomerViewModel>>(VueWebAppKeys.DataKey, out var customers))
                {
                    if (customers.Any(c => c.Id != Customer.Id && c.Email.Equals(Customer.Email, StringComparison.InvariantCultureIgnoreCase)))
                        ModelState.AddModelError("", "Email already exists.");
                }
            }
        }

        private void UpdatePageTitle()
        => PageTitle = "Edit Customer";

        private void AddErrors()
        {
            Errors = new();
            foreach (var item in ModelState)
            {
                if ((int)item.Value.ValidationState == 1)
                {
                    Errors.Add(item.Key.Replace(".", "_"), item.Value.Errors.Select(q => q.ErrorMessage));
                }

            }
        }

        private void AddDropDownItems()
        => SelectListItems = VueWebAppKeys.GetItems();
    }
}
