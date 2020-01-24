using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Models;
using CMV_CALLCENTER.Entidad;
using System.Globalization;
using System.IO;

namespace CMV_CALLCENTER.Controllers
{
    public class ResponderController : Controller
    {
        private ContextUne db = new ContextUne();
        private Funciones funcion = new Funciones();
        private ContextBanca bancaDB = new ContextBanca();
        //
        // GET: /Responder/

        public ActionResult Responder(TBL_UNE_REPORTE reporte)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if(cl!=null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_USUARIOS_ASIGNADOS can = db.TBL_UNE_USUARIOS_ASIGNADOS.Where(x => x.folio == reporte.FOLIO && x.responsable == 1).FirstOrDefault();
                TBL_UNE_REPORTE reporteResp = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporte.FOLIO).FirstOrDefault();
                if (can.numusuario == cl.Numusuario || reporteResp.reporte_banca == true)
                {
                    ViewData["sucursales"] = funcion.ListaSucursales(cl.Usuario, cl.Contrasena);
                    ViewData["tipoCuentas"] = db.CAT_UNE_TIPO_CUENTA.ToList();
                    ViewData["cuentas"] = db.CAT_UNE_CUENTAS.Where(x => x.ID_TIPO_CUENTA == reporte.ID_TIPO_CUENTA).ToList();
                    ViewData["debito"] = db.CAT_UNE_PROCEDE_DEBITO.Where(x => x.ID_PROCEDE_DEBITO > 0).ToList();
                    ViewData["comision"] = db.TBL_UNE_COMISIONES.Where(x => x.ID_COMISION == 1).FirstOrDefault();
                    ViewData["archivosAdjuntos"] = db.TBL_UNE_ARCHIVOS_ADJUNTOS.Where(x => x.FOLIO == reporte.FOLIO && x.ID_TIPO_ARCHIVO != 1).ToList();
                    ViewData["listaEntidades"] = funcion.ListaEntidades(cl.Usuario, cl.Contrasena);
                    ViewData["listaTipoReportes"] = funcion.obtenerTipoReporte(cl.Usuario, cl.Contrasena);
                    ViewData["listaMediosContacto"] = funcion.obtenerMediosContacto(cl.Usuario, cl.Contrasena);
                    ViewData["corresponsalias"] = db.CAT_UNE_PROCEDE_CORRESPONSALIAS.Where(x => x.ID_PROCEDE_CORRESPONSALIAS > 0).ToList();
                    ViewData["listaSupuestos"] = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_TIPO_REPORTE == reporteResp.ID_TIPO_REPORTE).ToList();
                    ViewData["listaMedioMovimiento"] = bancaDB.CAT_CALLCENTER_MEDIO_MOVIMIENTO.ToList();
                    ViewData["listaTipoCuentaBanca"] = db.CAT_UNE_TIPO_CUENTA_BANCA.ToList();
                    ViewData["listaCanales"] = db.CAT_UNE_CANAL.ToList();
                    ViewData["listaMotivoCancelacion"] = db.CAT_UNE_MOTIVO_CANCELACION.ToList();
                    ViewData["listaProductos"] = db.CAT_UNE_PRODUCTO.ToList();
                    ViewData["ListaCausaResolucion"] = db.CAT_UNE_CAUSA_RESOLUCION.ToList();
                    var supuestosvalidar = db.SP_UNE_OBTENER_SUPUESTOS_VALIDA_IMPORTE().ToList();

                    if (reporteResp.folio_banca == null)
                        reporteResp.folio_banca = 0;

                    if(supuestosvalidar.Exists(x=>x.ID_SUPUESTOS_REPORTE == reporteResp.ID_SUPUESTOS_REPORTE))
                    {
                        ViewData["supuestoRequiereMonto"] = 1;
                    }
                    else
                        ViewData["supuestoRequiereMonto"] = 0;

                    return View(Tuple.Create(reporteResp, new TBL_UNE_CANALIZACIONES()));
                }
                else
                    return RedirectToAction("Permiso", "Error");

            }
            else
                return RedirectToAction("Index", "Login");
        }

        public String puesto(int idSucursal, int numusuario, string usuario,string contrasena)
        {
            db = new ContextUne(usuario, contrasena);
            var puestoUser = db.SP_UNE_OBTENER_AREA_SUCURSAL(numusuario, idSucursal).FirstOrDefault();

            return puestoUser.PUESTO;
        }

        public String responsable(int folio, String usuario, String contrasena)
        {
            db = new ContextUne(usuario, contrasena);
            var responsableUser = db.TBL_UNE_USUARIOS_ASIGNADOS.Where(x => x.folio == folio && x.responsable == 1).FirstOrDefault();
            String nombre_s = "";
            if (responsableUser!=null)
            {
                var nombre = db.CLAVES.Where(x => x.Numusuario == responsableUser.numusuario).Select(x=> new { x.Nombre_s, x.Apellido_paterno, x.Apellido_materno }).FirstOrDefault();
                nombre_s = nombre.Nombre_s + " " + nombre.Apellido_paterno + " " + nombre.Apellido_materno;
            }

            return nombre_s;
        }

        public ActionResult RegistrarRespuesta(int folio, int numusuario, String reclamacion, decimal monto, int procedeDebito, int? procedeCorresponsalias, int? idCausaResolucion)
        {
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_CANALIZACIONES canalizar = new TBL_UNE_CANALIZACIONES();

                canalizar.COMENTARIOS = reclamacion;
                canalizar.FOLIO = folio;
                canalizar.numusuario = numusuario;
                canalizar.fecha_alta = DateTime.Now;
                canalizar.ID_TIPO_COMENTARIO = 3;
                db.TBL_UNE_CANALIZACIONES.Add(canalizar);
                TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == folio).FirstOrDefault();
                int resultado = new Correo().EnviarRespuesta(reporte, Server.MapPath("~/Estilos/Imagenes/firmaUNE.jpg"));

                if(resultado == 1)
                {
                    reporte.ID_CAUSA_RESOLUCION = idCausaResolucion;
                    reporte.ID_ESTATUS_REPORTE = 4;
                    if (procedeDebito == 1)
                        reporte.ID_RESOLUCION = 501;
                    else if(procedeDebito != 0)
                        reporte.ID_RESOLUCION = 502;

                    if (reporte.ID_TIPO_REPORTE == 3)
                    {
                        reporte.IMPORTE_SOLUCION = monto;

                        if(reporte.ID_TIPO_REPORTE == 3 && reporte.ID_CUENTA == 5)
                        {
                            reporte.ID_PROCEDE_DEBITO = procedeDebito;
                            reporte.BANDEJA_DEBITO = 1;
                            reporte.ID_FINALIZADO_DEBITO = 1;
                        }
                        else if(reporte.ID_TIPO_REPORTE == 3 && reporte.ID_CUENTA == 27)
                        {
                            reporte.ID_PROCEDE_CORRESPONSALIAS = procedeCorresponsalias;
                            if(procedeCorresponsalias == 1)
                                reporte.ID_RESOLUCION = 501;
                            else
                                reporte.ID_RESOLUCION = 502;
                        }
                    }
                    if(reporte.ID_TIPO_REPORTE == 4 && reporte.IMPORTE_RECLAMACION > 0)
                    {
                        reporte.IMPORTE_SOLUCION = monto;
                    }
                    db.SaveChanges();
                }
                return Json(resultado);
            }
            catch (Exception ex)
            {
                return Json(0);
            }

        }

        [HttpPost]
        public ActionResult RegistrarDocumento([Bind(Prefix = "Item1")] TBL_UNE_REPORTE reporte)
        {
            TBL_UNE_PERMISOS_ADMIN permisos = (Session["permiso"] as TBL_UNE_PERMISOS_ADMIN);
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            ContextUne db2 = new ContextUne();
            HttpPostedFileBase file = Request.Files[0];

            if (file.ContentLength>0)
            {
                TBL_UNE_REPORTE rep = db2.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporte.FOLIO).FirstOrDefault();
                TBL_UNE_ARCHIVOS_ADJUNTOS adjunto = new TBL_UNE_ARCHIVOS_ADJUNTOS();
                Random rnd = new Random();
                int rdn = rnd.Next(0, 10000);
                String nombre = rdn+"_"+ Path.GetFileName(file.FileName);
                string ruta = funcion.obtenerRuta(Convert.ToInt32(rep.NUM_FOLIO));
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
                adjunto.FOLIO = rep.FOLIO;
                adjunto.ID_TIPO_ARCHIVO = 4;
                adjunto.NUMUSUARIO = cl.Numusuario;
                db.TBL_UNE_ARCHIVOS_ADJUNTOS.Add(adjunto);
                db.SaveChanges();
                file.SaveAs(archivo);
            }

         if (permisos.USUARIO_CALL_CENTER == true)
            {
                return RedirectToAction("InicioCallcenter", "Inicio");

            }
            else
            {
                return RedirectToAction("Inicio", "Inicio");
            }
        }

        public ActionResult Redireccionar()
        {
            TBL_UNE_PERMISOS_ADMIN permisos = (Session["permiso"] as TBL_UNE_PERMISOS_ADMIN);
            if (permisos.USUARIO_CALL_CENTER == true)
            {
                return RedirectToAction("InicioCallcenter", "Inicio");

            }
            else
            {
                return RedirectToAction("Inicio", "Inicio");
            }
            
        }
    }
}
