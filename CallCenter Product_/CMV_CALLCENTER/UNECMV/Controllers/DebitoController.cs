using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Models;

namespace CMV_CALLCENTER.Controllers
{
    public class DebitoController : Controller
    {
        private ContextUne db = new ContextUne();
        //
        // GET: /Debito/

        public ActionResult Debito()
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                if (permisos.PERMISO_DEBITO == 1)
                {
                    db = new ContextUne(cl.Usuario, cl.Contrasena);
                    ViewData["causas"] = db.CAT_UNE_SUPUESTOS_REPORTE.ToList();
                    ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => x.BANDEJA_DEBITO == 1).ToList();
                    Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.ToString();
                    Session["segmento1"] = System.Web.HttpContext.Current.Request.Url.Segments[2];
                    return View();
                }
                else
                    return RedirectToAction("Permiso", "Error");
            }
            else
                return RedirectToAction("Index", "Login");
        }

        public int FinalizaReporte(int folio, string comentarios)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);

            TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == folio).FirstOrDefault();

            reporte.ID_FINALIZADO_DEBITO = 2;

            TBL_UNE_CANALIZACIONES canalizacion = new TBL_UNE_CANALIZACIONES();
            canalizacion.FOLIO = folio;
            canalizacion.fecha_alta = DateTime.Now;
            canalizacion.COMENTARIOS = comentarios;
            canalizacion.numusuario = cl.Numusuario;
            canalizacion.ID_TIPO_COMENTARIO = 10;
            db.TBL_UNE_CANALIZACIONES.Add(canalizacion);
            db.SaveChanges();

            return 1;

        }

    }
}
