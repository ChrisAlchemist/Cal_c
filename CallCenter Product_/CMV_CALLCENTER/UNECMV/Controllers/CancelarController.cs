using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Models;
using System.Data.Entity;
using System.Configuration;
using CMV_CALLCENTER.Entidad;
using System.Globalization;
using System.IO;

namespace CMV_CALLCENTER.Controllers
{
    public class CancelarController : Controller
    {
        private ContextUne db = new ContextUne();
        private Funciones funcion = new Funciones();
        private ContextBanca bancaDB = new ContextBanca();
        //
        // GET: /Cancelar/

        public ActionResult Cancelar(TBL_UNE_REPORTE reporte)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if(cl != null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                TBL_UNE_REPORTE rep = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporte.FOLIO).FirstOrDefault();

                if ((rep.ID_ESTATUS_REPORTE == 1 && rep.USUARIO_REGISTRA == cl.Numusuario) || (rep.ID_ESTATUS_REPORTE == 2 && permisos.USUARIO_UNE == 1) || (rep.ID_ESTATUS_REPORTE == 2 && permisos.USUARIO_CALL_CENTER==true) || (rep.ID_ESTATUS_REPORTE == 3 && permisos.ADMINISTRADOR_CALL_CENTER == true))
                {
                    ViewData["sucursales"] = funcion.ListaSucursales(cl.Usuario, cl.Contrasena);
                    ViewData["tipoCuentas"] = db.CAT_UNE_TIPO_CUENTA.ToList();
                    ViewData["cuentas"] = db.CAT_UNE_CUENTAS.Where(x => x.ID_TIPO_CUENTA == reporte.ID_TIPO_CUENTA).ToList();
                    ViewData["listaEntidades"] = funcion.ListaEntidades(cl.Usuario, cl.Contrasena);
                    ViewData["listaMediosContacto"] = funcion.obtenerMediosContacto(cl.Usuario, cl.Contrasena);
                    ViewData["listaTipoReportes"] = funcion.obtenerTipoReporte(cl.Usuario, cl.Contrasena);
                    ViewData["listaSupuestos"] = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_TIPO_REPORTE == rep.ID_TIPO_REPORTE).ToList();
                    ViewData["listaCanales"] = db.CAT_UNE_CANAL.ToList();
                    ViewData["listaMotivoCancelacion"] = db.CAT_UNE_MOTIVO_CANCELACION.ToList();
                    ViewData["listaProductos"] = db.CAT_UNE_PRODUCTO.ToList();
                    ViewData["listaResolucion"] = db.CAT_UNE_RESOLUCION.ToList();
                    ViewData["ListaCausaResolucion"] = db.CAT_UNE_CAUSA_RESOLUCION.ToList();
                    Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.ToString();
                    ViewData["listaMedioMovimiento"] = bancaDB.CAT_CALLCENTER_MEDIO_MOVIMIENTO.ToList();
                    return View(rep);
                }
                else
                    return RedirectToAction("Permiso", "Error");
            }
            else
                return RedirectToAction("Index", "Login");

        }

        [HttpPost]
        public ActionResult CancelarReporte(TBL_UNE_REPORTE reporteCancelar)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {


                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporteCancelar.FOLIO).FirstOrDefault();

                HttpPostedFileBase file = Request.Files[0];
                if (file.ContentLength > 0)
                {
                    TBL_UNE_ARCHIVOS_ADJUNTOS adjunto = new TBL_UNE_ARCHIVOS_ADJUNTOS();
                    Random rnd = new Random();
                    int rdn = rnd.Next(0, 10000);
                    String nombre = rdn + "_" + Path.GetFileName(file.FileName);
                    string ruta = funcion.obtenerRuta(Convert.ToInt32(reporte.NUM_FOLIO));
                    string archivo = ruta + "\\" + nombre;
                    if (!System.IO.Directory.Exists(ruta))
                    {
                        System.IO.Directory.CreateDirectory(ruta);
                    }

                    if (System.IO.File.Exists(archivo))
                    {
                        System.IO.File.Delete(archivo);
                    }
                    adjunto.FECHA_ALTA = DateTime.Now;
                    string rutaSQL = archivo;
                    rutaSQL = rutaSQL.Replace("\\", "//").Replace("\"", "/");
                    adjunto.RUTA_ARCHIVO = rutaSQL;
                    adjunto.NOMBRE_ARCHIVO = Path.GetFileName(file.FileName);
                    adjunto.FOLIO = reporte.FOLIO;
                    adjunto.ID_TIPO_ARCHIVO = 6;
                    adjunto.NUMUSUARIO = cl.Numusuario;
                    db.TBL_UNE_ARCHIVOS_ADJUNTOS.Add(adjunto);
                    db.SaveChanges();
                    file.SaveAs(archivo);
                }

                DateTime hoy = DateTime.Now;
                reporte.Fecha_cierre = hoy;
                reporte.Comentarios_cancelacion = reporteCancelar.Comentarios_cancelacion;
                reporte.ID_ESTATUS_REPORTE = 9;

                reporte.ID_RESOLUCION = reporteCancelar.ID_RESOLUCION;
                reporte.ID_CAUSA_RESOLUCION = 0;

                TBL_UNE_CANALIZACIONES canalizaciones = new TBL_UNE_CANALIZACIONES();
                canalizaciones.FOLIO = reporte.FOLIO;
                canalizaciones.fecha_alta = hoy;
                canalizaciones.COMENTARIOS = reporteCancelar.Comentarios_cancelacion;
                canalizaciones.numusuario = cl.Numusuario;
                canalizaciones.ID_TIPO_COMENTARIO = 8;
                db.TBL_UNE_CANALIZACIONES.Add(canalizaciones);

                db.SaveChanges();

                return RedirectToAction("Registros", "Registros");
            }
            else
                return RedirectToAction("Index", "Login");

        }
    }
}
