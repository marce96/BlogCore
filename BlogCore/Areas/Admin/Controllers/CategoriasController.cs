using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogCore.AccesoDatos.Data;
using BlogCore.AccesoDatos.Data.Repository;
using BlogCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class CategoriasController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly ApplicationDbContext _context;

        public CategoriasController(IContenedorTrabajo contenedorTrabajo, ApplicationDbContext context)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            if (ModelState.IsValid) //validaciones de campo no vacio
            {
                _contenedorTrabajo.Categoria.Add(categoria);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                    _contenedorTrabajo.Categoria.Update(categoria);
                    _contenedorTrabajo.Save();
                    return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Categoria.Get(id);
            if (objFromDb == null) return Json(new { success = false, message = "Error al borrar categoria"});

            _contenedorTrabajo.Categoria.Remove(objFromDb);
            _contenedorTrabajo.Save();
             return Json(new { success = true, message = "Categoria eliminada!" });
        }


        #region APIS GetMethods
        [HttpGet]
        public IActionResult GetAll()
        {
            var categorias = _context.Categoria.FromSqlRaw<Categoria>("spGetCategorias").ToList();
            return Json(new { data = categorias});
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Categoria categoria = new Categoria();
            categoria = _contenedorTrabajo.Categoria.Get(id);
            if (categoria == null) return NotFound();
            else return View(categoria);
        }
        #endregion
    }
}
