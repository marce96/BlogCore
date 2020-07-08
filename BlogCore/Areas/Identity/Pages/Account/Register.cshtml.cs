using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BlogCore.Models;
using BlogCore.Utilidades;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace BlogCore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email es obligatorio")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password es obligatorio")]
            [StringLength(100, ErrorMessage = "El {0} debe ser al menos de {2} y maximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar password")]
            [Compare("Password", ErrorMessage = "El password y confirmation password deben de ser iguales.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Nombre es obligatorio")]
            public string Nombre { get; set; }

            [Required(ErrorMessage = "Direccion es obligatorio")]
            public string Direccion { get; set; }

            [Required(ErrorMessage = "Ciudad es obligatorio")]
            public string Ciudad { get; set; }

            [Required(ErrorMessage = "Pais es obligatorio")]
            public string Pais { get; set; }

            [Required(ErrorMessage = "Telefono es obligatorio")]
            [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Solo se permiten numeros")]
            [StringLength(8, MinimumLength = 8, ErrorMessage = "Telefono debe de contener 8 digitos")]
            [Display(Name = "Telefono")]
            public string PhoneNumber { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {
                    Nombre = Input.Nombre,
                    UserName = Input.Email,
                    Email = Input.Email,
                    Pais = Input.Pais,
                    Ciudad = Input.Ciudad,
                    Direccion = Input.Direccion,
                    PhoneNumber = Input.PhoneNumber,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {

                    //Validacion si roles existen o no
                    if (!await _roleManager.RoleExistsAsync(CNT.Roles.Admin.ToString()))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(CNT.Roles.Admin.ToString()));
                        await _roleManager.CreateAsync(new IdentityRole(CNT.Roles.Usuario.ToString()));
                    }

                    //Obtener role seleccionado

                    string rol = Request.Form["radUsuarioRole"].ToString();

                    //validar si el role es admin y agregar
                    if (rol == CNT.Roles.Admin.ToString()) await _userManager.AddToRoleAsync(user, CNT.Roles.Admin.ToString());
                    else
                    {
                        if (rol == CNT.Roles.Usuario.ToString()) await _userManager.AddToRoleAsync(user, CNT.Roles.Usuario.ToString());
                    }

                    _logger.LogInformation("Usuario creado exitosamente.");

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    //}
                    //else
                    //{
                    //    await _signInManager.SignInAsync(user, isPersistent: false);
                    //    return LocalRedirect(returnUrl);
                    //}
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
