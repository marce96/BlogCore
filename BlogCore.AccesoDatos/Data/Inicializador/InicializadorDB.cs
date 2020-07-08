using BlogCore.Models;
using BlogCore.Utilidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlogCore.AccesoDatos.Data.Inicializador
{
    public class InicializadorDB : IInicializadorDB
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public InicializadorDB(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Inicializar()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {
            }
            if (_db.Roles.Any(r => r.Name == CNT.Roles.Admin.ToString())) return;

            _roleManager.CreateAsync(new IdentityRole(CNT.Roles.Admin.ToString())).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(CNT.Roles.Usuario.ToString())).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
                { 
                    UserName = "admin@blogcore.com",
                    Email = "admin@blogcore.com",
                    EmailConfirmed = true,
                    Nombre = "Admin"
            }, "Admin123!").GetAwaiter().GetResult();

            ApplicationUser usuario = _db.ApplicationUser
                .Where(u => u.Email == "admin@blogcore.com")
                .FirstOrDefault();

            _userManager.AddToRoleAsync(usuario, CNT.Roles.Admin.ToString()).GetAwaiter().GetResult();
        }
    }
}
