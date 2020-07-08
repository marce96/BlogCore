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
    public class SlidersController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hosting;

        public SlidersController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hosting)
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
            return Json(new { data = _contenedorTrabajo.Slider.GetAll() });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id != null)
            {
                var slider = _contenedorTrabajo.Slider.Get(id.GetValueOrDefault());
                return View(slider);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Slider slider)
        {
            if (ModelState.IsValid)
            {
                string ruta = _hosting.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                var articulodesdeDb = _contenedorTrabajo.Slider.Get(slider.Id);

                if (archivos.Count > 0)
                {
                    //Editar imagen
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(ruta, @"imagenes\sliders");
                    var extension = Path.GetExtension(archivos[0].FileName);
                    var nuevaExtension = Path.GetExtension(archivos[0].FileName);

                    var rutaImagen = Path.Combine(ruta, articulodesdeDb.UlrImagen.TrimStart('\\'));

                    if (System.IO.File.Exists(rutaImagen))
                    {
                        System.IO.File.Delete(rutaImagen);
                    }

                    //subir archivo
                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                   slider.UlrImagen = @"\imagenes\sliders\" + nombreArchivo + nuevaExtension;

                    _contenedorTrabajo.Slider.Update(slider);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //IF image already exists and it is not replaced, must to keep the one on db
                    slider.UlrImagen = articulodesdeDb.UlrImagen;
                }
                _contenedorTrabajo.Slider.Update(slider);
                _contenedorTrabajo.Save();

                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Slider.Get(id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error eliminando Slider" });
            }
           
            _contenedorTrabajo.Slider.Remove(objFromDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Slider eliminado" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Slider slider)
        {
            if (ModelState.IsValid)
            {
                string ruta = _hosting.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;
                        string nombreArchivo = Guid.NewGuid().ToString();
                        var subidas = Path.Combine(ruta, @"imagenes\sliders");
                        var extension = Path.GetExtension(archivos[0].FileName);

                        using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                        {
                            archivos[0].CopyTo(fileStreams);
                        }

                        slider.UlrImagen = @"\imagenes\sliders\" + nombreArchivo + extension;

                        _contenedorTrabajo.Slider.Add(slider);
                        _contenedorTrabajo.Save();

                        return RedirectToAction(nameof(Index));
            }

            return View();
        }
        #endregion
    }
}
