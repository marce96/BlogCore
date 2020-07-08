using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlogCore.Models;
using BlogCore.AccesoDatos.Data.Repository;
using BlogCore.Models.ViewModels;

namespace BlogCore.Controllers
{
    [Area("Cliente")]
    public class HomeController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        public HomeController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        public IActionResult Index()
        {
            HomeVM home = new HomeVM() {
                Slider = _contenedorTrabajo.Slider.GetAll(),
                Articulo = _contenedorTrabajo.Articulo.GetAll()
            };
            return View(home);
        }

        public IActionResult Details(int id)
        {
            var articuloDb = _contenedorTrabajo.Articulo.GetFirstOrDefault(p => p.Id.Equals(id));
            return View(articuloDb);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
