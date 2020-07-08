using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BlogCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogCore.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Required(ErrorMessage = "Teléfono es obligatorio.")]
            [Display(Name = "Teléfono")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Nombre es obligatorio.")]
            public string Nombre { get; set; }

            [Required(ErrorMessage = "Direccion es obligatoria.")]
            public string Direccion { get; set; }

            [Required(ErrorMessage = "Ciudad es obligatoria.")]
            public string Ciudad { get; set; }

            [Required(ErrorMessage = "Pais es obligatorio.")]
            [Display(Name = "País")]
            public string Pais { get; set; }
        }

        //private async Task LoadAsync(ApplicationUser user)
        //{
        //    var userName = await _userManager.GetUserNameAsync(user);
        //    var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        //    Username = userName;

        //    Input = new InputModel
        //    {
        //        PhoneNumber = phoneNumber,
        //        Nombre = user.Nombre,
        //        Ciudad = user.Ciudad,
        //        Direccion = user.Direccion,
        //        Pais = user.Pais
        //    };
        //}

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Nombre = user.Nombre,
                Ciudad = user.Ciudad,
                Direccion = user.Direccion,
                Pais = user.Pais
            };

            //await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            //if (!ModelState.IsValid)
            //{
            //    await LoadAsync(user);
            //    return Page();
            //}

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            user.Nombre = Input.Nombre;
            user.PhoneNumber = Input.PhoneNumber;
            user.Ciudad = Input.Ciudad;
            user.Direccion = Input.Direccion;
            user.Pais = Input.Pais;

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Perfil actualizado!";
            return RedirectToPage();
        }
    }
}
