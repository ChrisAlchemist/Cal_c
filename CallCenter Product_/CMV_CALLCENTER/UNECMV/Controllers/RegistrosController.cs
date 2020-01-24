using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Models;


namespace CMV_CALLCENTER.Controllers
{
    public class RegistrosController : Controller
    {
        private ContextUne db = new ContextUne();
        //
        // GET: /Registros/

        public ActionResult Registros()
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if(cl!=null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                ViewData["causas"] = db.CAT_UNE_SUPUESTOS_REPORTE.ToList();
                Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.ToString();
                Session["segmento1"] = System.Web.HttpContext.Current.Request.Url.Segments[2];
                return View();
            }
            else
                return RedirectToAction("Index", "Login");

        }

        public List<TBL_UNE_REPORTE> obtenerReportes(int numusuario, String usuario, int id_rol, String contrasena, int tipoJefe, int idSucursal, TBL_UNE_PERMISOS_ADMIN permisoUsuario)
        {
            db = new ContextUne(usuario, contrasena);

            if (permisoUsuario.USUARIO_SUCURSAL == true)
            {
                var listaReportes = db.TBL_UNE_REPORTE.Where(x => (x.ID_ESTATUS_REPORTE == 1 || x.ID_ESTATUS_REPORTE == 2) && x.reporte_banca == true && x.ID_DE_SUCURSAL == idSucursal).ToList();
                return listaReportes;
            }
            if(permisoUsuario.USUARIO_CALL_CENTER == true)
            {
                var listaReportes = new List<TBL_UNE_REPORTE> { };
                if(permisoUsuario.ADMINISTRADOR_CALL_CENTER == true)
                {
                    listaReportes = db.TBL_UNE_REPORTE.Where(x => (x.ID_ESTATUS_REPORTE == 1 || x.ID_ESTATUS_REPORTE == 2) && x.reporte_banca == true).ToList();
                }
                else
                {
                    listaReportes = db.TBL_UNE_REPORTE.Where(x => (x.ID_ESTATUS_REPORTE == 1 || x.ID_ESTATUS_REPORTE == 2) && x.reporte_banca == true && x.USUARIO_REGISTRA == numusuario).ToList();
                }

                return listaReportes;
            }
            else if (permisoUsuario.USUARIO_UNE == 1)
            {
                var listaReportes = db.TBL_UNE_REPORTE.Where(x=>x.ID_ESTATUS_REPORTE == 1 && x.reporte_banca == false).ToList();
                return listaReportes;
            }
            else if(tipoJefe == 4)
            {
                string regionActual = db.SUCURSALES.Where(x => x.Id_de_Sucursal == idSucursal).Select(x => x.Region_Actual).FirstOrDefault();

                var listaReportes = (from t in db.TBL_UNE_REPORTE
                                 join s in db.SUCURSALES
                                 on t.ID_SUCURSAL_REGISTRO equals s.Id_de_Sucursal
                                 where (s.Region_Actual.Contains(regionActual) && t.reporte_banca == false)
                                 select t).ToList();

                return listaReportes;
            }
            else
            {
                var listaReportes = db.TBL_UNE_REPORTE.Where(x => x.USUARIO_REGISTRA == numusuario && x.ID_ESTATUS_REPORTE == 1).ToList();
                return listaReportes;
            }                
        }

        public String ObtenerNombreUsuarioAlta(int? numusuario)
        {
            db = new ContextUne();
            CLAVES clave = db.CLAVES.Where(x => x.Numusuario == numusuario).FirstOrDefault();
            return clave.Nombre_s + " " + clave.Apellido_paterno + " " + clave.Apellido_materno;
        }

    }
}
