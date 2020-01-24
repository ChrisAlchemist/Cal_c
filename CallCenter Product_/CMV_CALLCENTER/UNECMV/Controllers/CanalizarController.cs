using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Models;
using CMV_CALLCENTER.Entidad;
using System.Configuration;
using System.Globalization;
using System.IO;

namespace CMV_CALLCENTER.Controllers
{
    public class CanalizarController : Controller
    {
        private ContextUne db = new ContextUne();
        private Funciones funciones = new Funciones();
        private ContextBanca bancaDB = new ContextBanca();
        //
        // GET: /Canalizar/

        public ActionResult Canalizar(TBL_UNE_REPORTE reporte)
        {
            
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);

                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                if (permisos.USUARIO_UNE == 1 || permisos.USUARIO_CALL_CENTER == true)
                {
                    TBL_UNE_REPORTE reporteCan = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporte.FOLIO).FirstOrDefault();
                    if (reporteCan.folio_banca == null)
                        reporteCan.folio_banca = 0;
                    TBL_UNE_USUARIOS_ASIGNADOS users = new TBL_UNE_USUARIOS_ASIGNADOS();
                    TBL_UNE_CANALIZACIONES canalizaciones = new TBL_UNE_CANALIZACIONES();
                    ViewData["tipoCuentas"] = db.CAT_UNE_TIPO_CUENTA.ToList();
                    ViewData["cuentas"] = db.CAT_UNE_CUENTAS.Where(x => x.ID_TIPO_CUENTA == reporte.ID_TIPO_CUENTA).ToList();
                    ViewData["sucursales"] = funciones.ListaSucursales(cl.Usuario, cl.Contrasena);
                    ViewData["listaEntidades"] = funciones.ListaEntidades(cl.Usuario, cl.Contrasena);
                    ViewData["listaTipoReportes"] = funciones.obtenerTipoReporte(cl.Usuario, cl.Contrasena);
                    
                    if (permisos.USUARIO_SUCURSAL == true)
                    {
                        var listaMediosContacto = funciones.obtenerMediosContacto(cl.Usuario, cl.Contrasena);
                        ViewData["listaMediosContacto"] = listaMediosContacto.Where(x => x.ID_MEDIO_CONTACTO == 1).ToList();
                    }
                    else
                    {
                        ViewData["listaMediosContacto"] = funciones.obtenerMediosContacto(cl.Usuario, cl.Contrasena);
                    }
                    ViewData["listaTipoCuentaBanca"] = db.CAT_UNE_TIPO_CUENTA_BANCA.ToList();
                    Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.ToString();
                    Session["segmento1"] = System.Web.HttpContext.Current.Request.Url.Segments[2];
                    ViewData["numEvidenciasDebito"] = db.TBL_UNE_ARCHIVOS_ADJUNTOS.Count(X => X.FOLIO == reporte.FOLIO && X.ID_TIPO_ARCHIVO == 9);

                    List<SP_UNE_OBTENER_USUARIOS_Result> listaUsuarios = db.SP_UNE_OBTENER_USUARIOS().ToList();

                    if (permisos.USUARIO_CALL_CENTER == false || permisos.USUARIO_CALL_CENTER == null)
                        ViewData["listaUsuarios"] = listaUsuarios;
                    else
                        ViewData["listaUsuarios"] = listaUsuarios.Where(x => x.Numusuario == 518 || x.Numusuario == 116).ToList();

                    ViewData["listaSupuestos"] = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_TIPO_REPORTE == reporteCan.ID_TIPO_REPORTE).ToList();
                    ViewData["listaMedioMovimiento"] = bancaDB.CAT_CALLCENTER_MEDIO_MOVIMIENTO.ToList();
                    ViewData["listaCanales"] = db.CAT_UNE_CANAL.ToList();
                    ViewData["listaMotivoCancelacion"] = db.CAT_UNE_MOTIVO_CANCELACION.ToList();
                    ViewData["listaProductos"] = db.CAT_UNE_PRODUCTO.ToList();

                    //List<ReporteMultifolio> Rep = funciones.ObtenerMultifoliosSocio(2000, cl.Usuario, cl.Contrasena);

                    return View(Tuple.Create(reporteCan, users, canalizaciones));
                }
                else
                    return RedirectToAction("Permiso", "Error");

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        public List<SP_UNE_OBTENER_USUARIOS_Result> regresaUsuarios(string nomUsuario, string contrasena)
        {

            db = new ContextUne(nomUsuario, contrasena);
            var listaClaves = db.SP_UNE_OBTENER_USUARIOS().ToList();

            return listaClaves;
        }

        [HttpPost]
        public ActionResult canalizaUsuario(int folio, int numusuario, int responsable, string COresponsables, string comentario, int idMedioDeteccion, string fechTransaccion)
        {
            try
            {

                CLAVES cl = Session["SesionUsuario"] as CLAVES;
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                if (cl != null)
                {


                    db = new ContextUne(cl.Usuario, cl.Contrasena);
                    string[] usuarios = COresponsables.Split(',');

                    string[] CC = new string[usuarios.Length];
                    string correoResponsable;
                    string nombreResponsable;
                    TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == folio).FirstOrDefault();
                    TBL_UNE_CANALIZACIONES canalizacion = new TBL_UNE_CANALIZACIONES();
                    canalizacion.FOLIO = folio;
                    canalizacion.COMENTARIOS = comentario;
                    canalizacion.numusuario = numusuario;
                    canalizacion.fecha_alta = DateTime.Now;
                    canalizacion.ID_TIPO_COMENTARIO = 2;
                    db.TBL_UNE_CANALIZACIONES.Add(canalizacion);

                    TBL_UNE_USUARIOS_ASIGNADOS usuarioResponsable = new TBL_UNE_USUARIOS_ASIGNADOS();
                    usuarioResponsable.folio = folio;
                    usuarioResponsable.numusuario = responsable;
                    usuarioResponsable.responsable = 1;
                    usuarioResponsable.fecha_asignacion = DateTime.Now;
                    db.TBL_UNE_USUARIOS_ASIGNADOS.Add(usuarioResponsable);
                    var usuario = db.CLAVES.Where(x => x.Numusuario == usuarioResponsable.numusuario).Select(x => new { x.Nombre_s, x.Apellido_paterno, x.Apellido_materno, x.Numusuario, x.Id_de_sucursal, x.Correo }).FirstOrDefault();
                    nombreResponsable = usuario.Nombre_s + " " + usuario.Apellido_paterno + " " + usuario.Apellido_materno;
                    correoResponsable = usuario.Correo;
                    var puestoUser = db.SP_UNE_OBTENER_AREA_SUCURSAL(usuario.Numusuario, usuario.Id_de_sucursal).FirstOrDefault();
                    SUCURSALES suc = db.SUCURSALES.Where(x => x.Id_de_Sucursal == cl.Id_de_sucursal).FirstOrDefault();

                    TBL_UNE_USUARIOS_ASIGNADOS coResponsable;

                    int numero;
                    if (usuarios[0] != "null")
                    {
                        for (int i = 0; i < usuarios.Length; i++)
                        {
                            coResponsable = new TBL_UNE_USUARIOS_ASIGNADOS();
                            coResponsable.folio = folio;
                            coResponsable.numusuario = Convert.ToInt32(usuarios[i]);
                            if (permisos.ADMINISTRADOR_CALL_CENTER == true)
                            {
                                //List<TBL_UNE_PERMISOS_ADMIN> administradoresCallCenter = new List<TBL_UNE_PERMISOS_ADMIN>();
                                //administradoresCallCenter = db.TBL_UNE_PERMISOS_ADMIN.Where(x => x.ADMINISTRADOR_CALL_CENTER == true).ToList();

                                // foreach (var administrador in administradoresCallCenter)
                                //{

                                //}
                                coResponsable.responsable = 1;
                                coResponsable.folio = folio;
                                coResponsable.fecha_asignacion = DateTime.Now;
                                if (usuarioResponsable.numusuario == 518)
                                {                                    
                                    coResponsable.numusuario = 116;
                                    db.TBL_UNE_USUARIOS_ASIGNADOS.Add(coResponsable);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    coResponsable.numusuario = 518;
                                    db.TBL_UNE_USUARIOS_ASIGNADOS.Add(coResponsable);
                                    db.SaveChanges();
                                }
                                coResponsable.numusuario = 1266;
                                db.TBL_UNE_USUARIOS_ASIGNADOS.Add(coResponsable);

                                //coResponsable.responsable = 1;
                                //coResponsable.folio = folio;
                                //coResponsable.numusuario = administrador.numer;
                                //usuarioResponsable.responsable = 1;
                                //coResponsable.fecha_asignacion = DateTime.Now;

                            }
                            else
                            {
                                //    coResponsable.responsable = 0;
                                //}

                                coResponsable.fecha_asignacion = DateTime.Now;
                                db.TBL_UNE_USUARIOS_ASIGNADOS.Add(coResponsable);
                            }
                            numero = Convert.ToInt32(usuarios[i]);

                            if (numero == 0)
                            {
                                String correoMediosPago = ConfigurationSettings.AppSettings["CorreoMediosPago"];
                                CC[i] = correoMediosPago;
                            }
                            else
                            {
                                var claves = db.CLAVES.Where(x => x.Numusuario == numero).Select(x => new { x.Correo }).FirstOrDefault();
                                CC[i] = claves.Correo;
                            }

                        }
                    }


                    int estatus = new Correo().EnviarCanalizacion(correoResponsable, CC, reporte, suc.Descripcion, nombreResponsable, Server.MapPath("~/Estilos/Imagenes/firmaUNE.jpg"));
                    CAT_UNE_SUPUESTOS_REPORTE cat = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).FirstOrDefault();
                    reporte.DIAS_RESTANTES_AREA_ESPECIALIZADA = cat.DIAS_REQUERIDOS;
                    reporte.ID_ESTATUS_REPORTE = 3;
                    reporte.id_medio_deteccion_movimiento = idMedioDeteccion;

                    // reporte.FECHA_TRANSACCION = Convert.ToDateTime(fechTransaccion);

                    db.SaveChanges();
                    return Json(estatus, JsonRequestBehavior.AllowGet);
                }
                else
                    return RedirectToAction("Index", "Login");
            }
            
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message + "Fecha recibida: " + fechTransaccion);
            }
        }

        [HttpPost]
        public ActionResult canalizaUsuarioForm([Bind(Prefix = "Item1")] TBL_UNE_REPORTE reporte, List<HttpPostedFileBase> files)
        {
            TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {


                db = new ContextUne(cl.Usuario, cl.Contrasena);
                //ContextUne db2 = new ContextUne();
                //HttpPostedFileBase file = Request.Files[0];
                TBL_UNE_REPORTE rep = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporte.FOLIO).FirstOrDefault();
                rep.folio_autorizacion_banca = reporte.folio_autorizacion_banca;
                //rep.IMPORTE_RECLAMACION = reporte.IMPORTE_RECLAMACION;
                rep.id_tipo_cuenta_banca = reporte.id_tipo_cuenta_banca;

                if (rep.reporte_banca == true)
                {
                    rep.FECHA_TRANSACCION = reporte.FECHA_TRANSACCION;
                    rep.Observaciones_cierre = reporte.Observaciones_cierre;
                }

                db.SaveChanges();
                foreach (HttpPostedFileBase file in files)
                {
                    if (file != null)
                    {

                        TBL_UNE_ARCHIVOS_ADJUNTOS adjunto = new TBL_UNE_ARCHIVOS_ADJUNTOS();
                        Random rnd = new Random();
                        int rdn = rnd.Next(0, 10000);
                        String nombre = rdn + "_" + Path.GetFileName(file.FileName);
                        string ruta = funciones.obtenerRuta(Convert.ToInt32(rep.NUM_FOLIO));
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
                        adjunto.ID_TIPO_ARCHIVO = 3;
                        adjunto.NUMUSUARIO = cl.Numusuario;
                        db.TBL_UNE_ARCHIVOS_ADJUNTOS.Add(adjunto);
                        file.SaveAs(archivo);
                    }
                }

                db.SaveChanges();
                if (rep.reporte_banca == true)
                {
                    if (permisos.USUARIO_SUCURSAL != true)
                    {
                        return RedirectToAction("InicioCallcenter", "Inicio");
                    }
                    else
                    {
                        return RedirectToAction("Registros", "Registros");
                    }

                }
                else
                {
                    return RedirectToAction("Inicio", "Inicio");
                }
            }
            else
                return RedirectToAction("Index", "Login");

        }
        
    }
}
