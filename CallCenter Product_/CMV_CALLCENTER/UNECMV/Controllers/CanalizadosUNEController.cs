using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Models;
using CMV_CALLCENTER.Entidad;

namespace CMV_CALLCENTER.Controllers
{
    public class CanalizadosUNEController : Controller
    {
        private ContextUne db = new ContextUne();
        //
        // GET: /CanalizadosUNE/

        public ActionResult CanalizadosUNE()
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if(cl!=null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;

                if(permisos.USUARIO_UNE == 1)
                {
                    ViewData["causas"] = db.CAT_UNE_SUPUESTOS_REPORTE.ToList();
                    ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => x.ID_ESTATUS_REPORTE == 2 && x.reporte_banca == false).ToList();
                    Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.ToString();
                    Session["segmento1"] = System.Web.HttpContext.Current.Request.Url.Segments[2];
                    return View();
                }
                else if(permisos.USUARIO_CALL_CENTER == true)
                {
                    ViewData["causas"] = db.CAT_UNE_SUPUESTOS_REPORTE.ToList();
                    if(permisos.ADMINISTRADOR_CALL_CENTER == true)
                        ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => x.ID_ESTATUS_REPORTE == 2 && x.reporte_banca == true).ToList();
                    else
                        ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => x.ID_ESTATUS_REPORTE == 2 && x.reporte_banca == true && x.USUARIO_REGISTRA == cl.Numusuario).ToList();
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
