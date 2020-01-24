using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Models;
using CMV_CALLCENTER.Entidad;
using System.Globalization;

namespace CMV_CALLCENTER.Controllers
{
    public class InicioController : Controller
    {
        private ContextUne db = new ContextUne();


        public ActionResult InicioCallcenter()
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {
                int? tipoJefe = Session["tipoJefe"] as int?;
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                ViewData["causas"] = db.CAT_UNE_SUPUESTOS_REPORTE.ToList();
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                //REPORTES DE BANCA
                if (permisos.USUARIO_CALL_CENTER == true)
                {
                    
                    List<TBL_UNE_REPORTE> a = (from t in db.TBL_UNE_REPORTE
                                join u in db.TBL_UNE_USUARIOS_ASIGNADOS
                                on t.FOLIO equals u.folio
                                where ((t.ID_ESTATUS_REPORTE == 2 || t.ID_ESTATUS_REPORTE == 3 || t.ID_ESTATUS_REPORTE == 10) && u.responsable == 1 && t.reporte_banca == true)
                                select t).ToList();

                    ViewData["listaReportes"] = a.GroupBy(p=>p.FOLIO).Select( g => g.FirstOrDefault()).ToList();

                    ViewData["listaReponsables"] = (from t in db.TBL_UNE_REPORTE
                                                    orderby t.FOLIO
                                                    join u in db.TBL_UNE_USUARIOS_ASIGNADOS
                                                    on t.FOLIO equals u.folio
                                                    where ((t.ID_ESTATUS_REPORTE == 2 || t.ID_ESTATUS_REPORTE == 3 || t.ID_ESTATUS_REPORTE == 10) && u.responsable == 1 && t.reporte_banca == true)
                                                    select u).ToList();
                }
                //REPORTES DE UNE CMV
                
                Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.AbsolutePath.ToString();
                Session["segmento1"] = System.Web.HttpContext.Current.Request.Url.Segments[2];

                return View("Inicio");
            }
            else
                return RedirectToAction("Index", "Login");
        }
        public ActionResult Inicio()
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {
                int? tipoJefe = Session["tipoJefe"] as int?;
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                ViewData["causas"] = db.CAT_UNE_SUPUESTOS_REPORTE.ToList();
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                //REPORTES DE BANCA
                //if(permisos.USUARIO_CALL_CENTER == true)
                //{

                //    ViewData["listaReportesBanca"] = (from t in db.TBL_UNE_REPORTE
                //                                 join u in db.TBL_UNE_USUARIOS_ASIGNADOS
                //                                 on t.FOLIO equals u.folio
                //                                 where ((t.ID_ESTATUS_REPORTE == 3 || t.ID_ESTATUS_REPORTE == 10) && u.responsable == 1 && t.reporte_banca == true)
                //                                 select t).ToList();
                //}
                //REPORTES DE UNE CMV
                if (permisos.USUARIO_UNE == 1)
                {
                    ViewData["listaReportes"] = (from t in db.TBL_UNE_REPORTE
                                                 join u in db.TBL_UNE_USUARIOS_ASIGNADOS
                                                 on t.FOLIO equals u.folio
                                                 where ((t.ID_ESTATUS_REPORTE == 3 || t.ID_ESTATUS_REPORTE == 10) && u.responsable == 1 && t.reporte_banca == false)
                                                 select t).ToList();
                }
                else if(tipoJefe == 4)
                {
                    string regionActual = db.SUCURSALES.Where(x => x.Id_de_Sucursal == cl.Id_de_sucursal).Select(x => x.Region_Actual).FirstOrDefault();
                    ViewData["listaReportes"] = (from t in db.TBL_UNE_REPORTE
                                                 join s in db.SUCURSALES
                                                 on t.ID_SUCURSAL_REGISTRO equals s.Id_de_Sucursal
                                                 join u in db.TBL_UNE_USUARIOS_ASIGNADOS
                                                 on t.FOLIO equals u.folio
                                                 where ((t.ID_ESTATUS_REPORTE == 3 || t.ID_ESTATUS_REPORTE == 10) && u.responsable == 1 && s.Region_Actual.Contains(regionActual) && t.reporte_banca == false)
                                                 select t).ToList();
                }
                else
                {
                    ViewData["listaReportes"] = (from t in db.TBL_UNE_REPORTE
                                                 join u in db.TBL_UNE_USUARIOS_ASIGNADOS
                                                 on t.FOLIO equals u.folio
                                                 where u.responsable == 1 && u.numusuario == cl.Numusuario && (t.ID_ESTATUS_REPORTE == 3 || t.ID_ESTATUS_REPORTE == 10) && t.reporte_banca == false
                                                 select t).ToList();
                }

                ViewData["listaReponsables"] = (from t in db.TBL_UNE_REPORTE
                                                join u in db.TBL_UNE_USUARIOS_ASIGNADOS
                                                on t.FOLIO equals u.folio
                                                where ((t.ID_ESTATUS_REPORTE == 2 || t.ID_ESTATUS_REPORTE == 3 || t.ID_ESTATUS_REPORTE == 10) && u.responsable == 1 && t.reporte_banca == false)
                                                select u).ToList();

                Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.AbsolutePath.ToString();
                Session["segmento1"] = System.Web.HttpContext.Current.Request.Url.Segments[2];

                return View();
            }
            else
                return RedirectToAction("Index", "Login");

        }

        public ActionResult CerrarSesion()
        {

            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                db.SP_UNE_REESTABLECER_FOLIOS_USUARIO_SESION(cl.Numusuario, Session.SessionID);
                Session.Abandon();
                Session["SesionUsuario"] = null;
                Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
                return RedirectToAction("Index", "Login");
            }
            else
                return RedirectToAction("Index", "Login");
        }

        public int canalizarReporte(int folio)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            DateTime hoy = DateTime.Now;

            TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == folio).FirstOrDefault();
            reporte.FECHA_Canalizacion = hoy;
            reporte.ID_ESTATUS_REPORTE = 2;
            reporte.DIAS_RESTANTES_GENERAL = 30;
            SUCURSALES suc = db.SUCURSALES.Where(x => x.Id_de_Sucursal == cl.Id_de_sucursal).FirstOrDefault();
            int resultado = new Correo().Enviar(reporte, suc.Descripcion, cl.Correo, Server.MapPath("~/Estilos/Imagenes/firmaUNE.jpg"));
            if (resultado == 1)
            {
                db.SaveChanges();
            }
            return resultado;
        }

        public int EsResponsable(int numusuario, int folio, string usuario, string contrasena)
        {
            db = new ContextUne(usuario, contrasena);
            var responsable = db.TBL_UNE_USUARIOS_ASIGNADOS.Where(x => x.numusuario == numusuario && x.folio == folio).FirstOrDefault();
            if (responsable != null)
                return Convert.ToInt32(responsable.responsable);
            else
                return 0;
        }

        public String NombreResponsable(int folio, string usuario, string contrasena)
        {
            db = new ContextUne(usuario, contrasena);
            String nombre = db.SP_UNE_OBTENER_NOMBRE_RESPONSABLE(folio).FirstOrDefault();
            nombre = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nombre.ToLower());
            return nombre;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public DateTime obtenerFecha(int folio, String Usuario, String Contrasena)
        {
            db = new ContextUne(Usuario, Contrasena);
            var comentario = db.SP_UNE_CARGA_COMENTARIO(folio, 1).FirstOrDefault();
            return Convert.ToDateTime(comentario.fecha_alta);
        }

        public int cerrarFolios(int numusuario)
        {
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                db.SP_UNE_REESTABLECER_FOLIOS_USUARIO_SESION(numusuario, Session.SessionID);
                return 1;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

    }
}