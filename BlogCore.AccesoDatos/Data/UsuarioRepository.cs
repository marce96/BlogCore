using BlogCore.AccesoDatos.Data.Repository;
using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlogCore.AccesoDatos.Data
{
    public class UsuarioRepository : Repository<ApplicationUser>, IUsuarioRepository
    {
        private readonly ApplicationDbContext _db;

        public UsuarioRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void BloqueaUsuario(string IdUsuario)
        {
            var usuario = _db.ApplicationUser.FirstOrDefault(u => u.Id == IdUsuario);
            usuario.LockoutEnd = DateTime.Now.AddDays(1);
            _db.SaveChanges(); 
        }

        public void DesbloquearUsuario(string IdUsuario)
        {
            var usuario = _db.ApplicationUser.FirstOrDefault(u => u.Id == IdUsuario);
            usuario.LockoutEnd = DateTime.Now;
            _db.SaveChanges();
        }
    }
}

