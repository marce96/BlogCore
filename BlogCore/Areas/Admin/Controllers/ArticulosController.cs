using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlogCore.AccesoDatos.Data.Repository;
using BlogCore.Models;
using BlogCore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

namespace BlogCore.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ArticulosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hosting;

        public ArticulosController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hosting)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hosting = hosting;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #region APIS GetMethods
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Articulo.GetAll(includeProperties: "Categoria") });
        }

        [HttpGet]
        public IActionResult Create()
        {
            ArticuloVM articulovm = new ArticuloVM()
            {
                Articulo = new Models.Articulo(),
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias()
            };

            return View(articulovm);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ArticuloVM artivm = new ArticuloVM()
            {
                Articulo = new Models.Articulo(),
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias()
            };

            if (id != null)
            {
                artivm.Articulo = _contenedorTrabajo.Articulo.Get(id.GetValueOrDefault());
            }
            return View(artivm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ArticuloVM artivm)
        {
            if (ModelState.IsValid)
            {
                string ruta = _hosting.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                var articulodesdeDb = _contenedorTrabajo.Articulo.Get(artivm.Articulo.Id);

                if (archivos.Count > 0)
                {
                    //Editar imagen
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(ruta, @"imagenes\articulos");
                    var extension = Path.GetExtension(archivos[0].FileName);
                    var nuevaExtension = Path.GetExtension(archivos[0].FileName);

                    var rutaImagen = Path.Combine(ruta, articulodesdeDb.UrlImagen.TrimStart('\\'));

                    if (System.IO.File.Exists(rutaImagen))
                    {
                        System.IO.File.Delete(rutaImagen);
                    }

                    //subir archivo
                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                    artivm.Articulo.UrlImagen = @"\imagenes\articulos\" + nombreArchivo + nuevaExtension;
                    artivm.Articulo.FechaCreacion = DateTime.Now.ToString();

                    _contenedorTrabajo.Articulo.Update(artivm.Articulo);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //IF image already exists and it is not replaced, must to keep the one on db
                    artivm.Articulo.UrlImagen = articulodesdeDb.UrlImagen;
                }
                _contenedorTrabajo.Articulo.Update(artivm.Articulo);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var articuloDesdeDb = _contenedorTrabajo.Articulo.Get(id);
            string rutaDirectorio = _hosting.WebRootPath; //obtener ruta de imagen
            var rutaImagen = Path.Combine(rutaDirectorio, articuloDesdeDb.UrlImagen.TrimStart('\\'));

            if (System.IO.File.Exists(rutaImagen))
            {
                System.IO.File.Delete(rutaImagen);
            }

            if (articuloDesdeDb == null)
            {
                return Json(new { success = false, message = "Error borrando articulo"});
            }

            _contenedorTrabajo.Articulo.Remove(articuloDesdeDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Articulo eliminado"});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ArticuloVM artivm)
        {
            artivm.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
            if (ModelState.IsValid)
            {
                string ruta = _hosting.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                if (archivos.Count > 0)
                {
                    if (artivm.Articulo.Id == 0)
                    {
                        //nuevo articulo
                        string nombreArchivo = Guid.NewGuid().ToString();
                        var subidas = Path.Combine(ruta, @"imagenes\articulos");
                        var extension = Path.GetExtension(archivos[0].FileName);

                        using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                        {
                            archivos[0].CopyTo(fileStreams);
                        }

                        artivm.Articulo.UrlImagen = @"\imagenes\articulos\" + nombreArchivo + extension;
                        artivm.Articulo.FechaCreacion = DateTime.Now.ToString();

                        _contenedorTrabajo.Articulo.Add(artivm.Articulo);
                        _contenedorTrabajo.Save();

                        return RedirectToAction(nameof(Index));
                    }
                }
                
                return View(artivm);
            }

            return View(artivm);
        }
        #endregion
    }
}
