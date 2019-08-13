using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AppCocina.Controllers
{
    public class HomeController : AsyncController
    {
        public async Task<ActionResult> Index()
        {
            DataTable ingredientes = new DataTable();
            DataTable platillos = new DataTable();
            Models.CapaDatos misDatos = null;

            using (misDatos = new Models.CapaDatos())
            {
                ingredientes = await misDatos.getIngredientes();
                platillos = await misDatos.getPlatillos();
            }
            ViewBag.Ingredientes = ingredientes;
            ViewBag.Platillos = platillos;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> updateStock(string ingredientes)
        {
            Models.CapaDatos misDatos = null;
            bool respuesta = false;
            using (misDatos = new Models.CapaDatos())
            {
                respuesta = await misDatos.updateStock(ingredientes);
            }
            return Json(respuesta.ToString(), JsonRequestBehavior.AllowGet);
        }
    }
}