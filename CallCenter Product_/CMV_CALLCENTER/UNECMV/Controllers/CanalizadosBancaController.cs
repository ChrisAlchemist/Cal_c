using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Models;

namespace CMV_CALLCENTER.Controllers
{
    public class CanalizadosBancaController : Controller
    {
        private ContextUne db = new ContextUne();
        //
        // GET: /CanalizadosBanca/

        public ActionResult CanalizadosBanca()
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;


            if (cl != null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;

                if (permisos.USUARIO_CALL_CENTER == true)
                {
                    ViewData["causas"] = db.CAT_UNE_SUPUESTOS_REPORTE.ToList();
                    ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => (x.ID_ESTATUS_REPORTE == 2 || x.ID_ESTATUS_REPORTE == 4 || x.ID_ESTATUS_REPORTE == 5) && x.reporte_banca == true).ToList();
                    Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.ToString();
                    Session["segmento1"] = System.Web.HttpContext.Current.Request.Url.Segments[2];
                    return View();
                }
                else
                    return RedirectToAction("Permiso", "Error");
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

    }
}
