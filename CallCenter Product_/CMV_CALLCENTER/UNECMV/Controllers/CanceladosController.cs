using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Models;
using CMV_CALLCENTER.Entidad;

namespace CMV_CALLCENTER.Controllers
{
    public class CanceladosController : Controller
    {
        private ContextUne db = new ContextUne();
        //
        // GET: /Cancelados/

        public ActionResult Cancelados()
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);

                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                ViewData["causas"] = db.CAT_UNE_SUPUESTOS_REPORTE.ToList();

                if (permisos.USUARIO_SUCURSAL == true)
                {
                    ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => x.ID_ESTATUS_REPORTE == 9 && x.reporte_banca == true && x.ID_DE_SUCURSAL == cl.Id_de_sucursal).ToList();
                }
                else
                {
                    if (permisos.USUARIO_CALL_CENTER == true)
                    {
                        if (permisos.ADMINISTRADOR_CALL_CENTER == true)
                            ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => x.ID_ESTATUS_REPORTE == 9 && x.reporte_banca == true).ToList();
                        else
                            ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => x.ID_ESTATUS_REPORTE == 9 && x.reporte_banca == true && x.USUARIO_REGISTRA == cl.Numusuario).ToList();
                    }
                    else if (permisos.USUARIO_UNE == 1)
                    {
                        ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => x.ID_ESTATUS_REPORTE == 9 && x.reporte_banca == false).ToList();
                    }
                    else
                    {
                        ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => x.ID_ESTATUS_REPORTE == 9 && x.USUARIO_REGISTRA == cl.Numusuario).ToList();
                    }
                }
                Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.AbsolutePath.ToString();
                Session["segmento1"] = System.Web.HttpContext.Current.Request.Url.Segments[2];

                return View();
            }
            else
                return RedirectToAction("Index", "Login");
        }

        public DateTime obtenerFecha(int folio, String Usuario, String Contrasena, TBL_UNE_REPORTE reporte)
        {
            db = new ContextUne(Usuario, Contrasena);
            var comentario = db.SP_UNE_CARGA_COMENTARIO(folio, 4).FirstOrDefault();
            DateTime fechaCancelacion = Convert.ToDateTime(comentario.fecha_alta);

            if (Convert.ToBoolean(reporte.reporte_banca))
                fechaCancelacion = Convert.ToDateTime(reporte.Fecha_cierre);

            return fechaCancelacion;
        }

    }
}
