using System;

using System.Diagnostics;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Models;
using CMV_CALLCENTER.Entidad;
using System.Data.Entity.Validation;
using SmsMailUtils;
using System.Web.Script.Serialization;

namespace CMV_CALLCENTER.Controllers
{
    public class PreviewController : Controller
    {
        private ContextUne db = new ContextUne();
        private Funciones funcion = new Funciones();
        private ContextUneBD dbUne = new ContextUneBD();
        private ContextBanca bancaDB = new ContextBanca();
        //
        // GET: /Preview/

        [ValidateInput(false)]
        public ActionResult Preview(TBL_UNE_REPORTE repFolio)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                

                List<CAT_UNE_TIPO_REPORTE> listaTipoReporte = null;
                List<CAT_UNE_MEDIO_CONTACTO> listaMedioContacto = null;
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == repFolio.FOLIO).FirstOrDefault();
                ViewData["cuentas"] = db.CAT_UNE_CUENTAS.Where(x => x.ID_TIPO_CUENTA == reporte.ID_TIPO_CUENTA).ToList();
                ViewData["sucursales"] = funcion.ListaSucursales(cl.Usuario, cl.Contrasena);
                ViewData["listaEntidades"] = funcion.ListaEntidades(cl.Usuario, cl.Contrasena);
                ViewData["archivosAdjuntos"] = db.TBL_UNE_ARCHIVOS_ADJUNTOS.Where(x => x.FOLIO == reporte.FOLIO && x.ID_TIPO_ARCHIVO != 1).ToList();
                ViewData["conteoArchivosAdjuntos"] = db.TBL_UNE_ARCHIVOS_ADJUNTOS.Count(x => x.FOLIO == reporte.FOLIO && x.ID_TIPO_ARCHIVO != 1);
                ViewData["listaSupuestos"] = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_TIPO_REPORTE == reporte.ID_TIPO_REPORTE && x.ID_TIPO_CUENTA == reporte.ID_TIPO_CUENTA && x.ID_CUENTA == reporte.ID_CUENTA).ToList();
                ViewData["listaMedioMovimiento"] = bancaDB.CAT_CALLCENTER_MEDIO_MOVIMIENTO.ToList();
                ViewData["listaTipoCuentaBanca"] = db.CAT_UNE_TIPO_CUENTA_BANCA.ToList();
                ViewData["listaCuentasNoAfectadas"] = bancaDB.CAT_CALLCENTER_TIPO_CUENTA_NO_AFECTADA.ToList();
                //Se cargan la lista de tipos de reporte y segun el usuario se cargan ciertos elementos
                listaTipoReporte = funcion.obtenerTipoReporte(cl.Usuario, cl.Contrasena);
                //Se cargan la lista de tipos de medios de contacto y segun el usuario se cargan ciertos elementos
                listaMedioContacto = funcion.obtenerMediosContacto(cl.Usuario, cl.Contrasena);

                ViewData["listaTipoReportes"] = funcion.obtenerTipoReporte(cl.Usuario, cl.Contrasena);
                ViewData["listaMediosContacto"] = funcion.obtenerMediosContacto(cl.Usuario, cl.Contrasena);
                ViewData["listaCanales"] = db.CAT_UNE_CANAL.ToList();
                ViewData["listaMotivoCancelacion"] = db.CAT_UNE_MOTIVO_CANCELACION.ToList();
                ViewData["listaProductos"] = db.CAT_UNE_PRODUCTO.ToList();
                ViewData["listaResolucion"] = db.CAT_UNE_RESOLUCION.ToList();
                ViewData["ListaCausaResolucion"] = db.CAT_UNE_CAUSA_RESOLUCION.ToList();


                if (permisos.USUARIO_CALL_CENTER == false || permisos.USUARIO_CALL_CENTER == null)
                {
                    ViewData["tipoCuentas"] = db.CAT_UNE_TIPO_CUENTA.Where(x => x.ID_TIPO_CUENTA <= 3).ToList();
                    ViewData["listaTipoReportes"] = listaTipoReporte.Where(x => x.ID_TIPO_REPORTE <= 3).ToList();
                    ViewData["listaMediosContacto"] = listaMedioContacto.ToList();
                }
                else
                {
                    ViewData["tipoCuentas"] = db.CAT_UNE_TIPO_CUENTA.Where(x => x.ID_TIPO_CUENTA > 3).ToList();
                    ViewData["listaTipoReportes"] = listaTipoReporte.Where(x => x.ID_TIPO_REPORTE > 3).ToList();
                    if(permisos.USUARIO_SUCURSAL == true)
                    {
                        ViewData["listaMediosContacto"] = listaMedioContacto.Where(x => x.ID_MEDIO_CONTACTO == 1).ToList();
                    }
                    else
                    {
                        if( repFolio.ID_ESTATUS_REPORTE == 3 || repFolio.ID_ESTATUS_REPORTE == 4 || repFolio.ID_ESTATUS_REPORTE == 5)
                        {
                            ViewData["listaMediosContacto"] = listaMedioContacto.Where(x => x.ID_MEDIO_CONTACTO == repFolio.ID_MEDIO_CONTACTO).ToList();
                        }
                        else
                        {
                            ViewData["listaMediosContacto"] = listaMedioContacto.Where(x => x.ID_MEDIO_CONTACTO == 2).ToList();
                        }
                        
                    }
                    
                }

                if (reporte.ID_ESTATUS_REPORTE == 6 || reporte.ID_ESTATUS_REPORTE == 7 || reporte.ID_ESTATUS_REPORTE == 8)
                {
                    dbUne = new ContextUneBD(cl.Usuario, cl.Contrasena);
                    String numFolio = reporte.NUM_FOLIO.ToString();
                    ViewData["listaLlamadas"] = dbUne.CMV_REC_DATA.Where(x => x.CRM_FOLIO == numFolio && x.P_STATUS == 1).OrderBy(x => x.INB_OUT_ID).ToList();
                    ViewData["ConteoListaLlamadas"] = dbUne.CMV_REC_DATA.Count(x => x.CRM_FOLIO == numFolio && x.P_STATUS == 1);
                }

                ViewData["numEvidenciasDebito"] = db.TBL_UNE_ARCHIVOS_ADJUNTOS.Count(X => X.FOLIO == repFolio.FOLIO && X.ID_TIPO_ARCHIVO == 9);

                if (reporte.ID_TIPO_CUENTA == 5 && reporte.ID_ESTATUS_REPORTE == 2)
                {
                    if (reporte.ID_CUENTA == 37)
                    {
                        return RedirectToAction("ModificarMonto", "Banca", reporte);
                    }
                    else if (reporte.ID_CUENTA == 36)
                    {
                        if (reporte.ID_SUPUESTOS_REPORTE == 2173)
                        {
                            return RedirectToAction("ReposicionSoftoken", "Banca", reporte);
                        }
                        else
                        {
                            return View(reporte);
                        }
                    }
                    else if ((reporte.ID_CUENTA == 33 || reporte.ID_CUENTA == 34) && reporte.ID_ESTATUS_REPORTE == 2)
                    {
                        return RedirectToAction("BloquearBanca", "Banca", reporte);
                    }

                    else
                        return View(reporte);
                }
                else
                    return View(reporte);
            }
            else
                return RedirectToAction("Index", "Login");
        }

        public ActionResult RedirectPreview(int folio)
        {
            try
            {
                TBL_UNE_REPORTE reporte = null;
                CLAVES cl = Session["SesionUsuario"] as CLAVES;
                
                if (cl != null)
                {
                    db = new ContextUne(cl.Usuario, cl.Contrasena);

                    List<CAT_UNE_TIPO_REPORTE> listaTipoReporte = null;
                    List<CAT_UNE_MEDIO_CONTACTO> listaMedioContacto = null;
                    TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                    reporte = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == folio).FirstOrDefault();
                    ViewData["cuentas"] = db.CAT_UNE_CUENTAS.Where(x => x.ID_TIPO_CUENTA == reporte.ID_TIPO_CUENTA).ToList();
                    ViewData["sucursales"] = funcion.ListaSucursales(cl.Usuario, cl.Contrasena);
                    ViewData["listaEntidades"] = funcion.ListaEntidades(cl.Usuario, cl.Contrasena);
                    ViewData["archivosAdjuntos"] = db.TBL_UNE_ARCHIVOS_ADJUNTOS.Where(x => x.FOLIO == reporte.FOLIO && x.ID_TIPO_ARCHIVO != 1).ToList();
                    ViewData["conteoArchivosAdjuntos"] = db.TBL_UNE_ARCHIVOS_ADJUNTOS.Count(x => x.FOLIO == reporte.FOLIO && x.ID_TIPO_ARCHIVO != 1);
                    ViewData["listaSupuestos"] = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_TIPO_REPORTE == reporte.ID_TIPO_REPORTE && x.ID_TIPO_CUENTA == reporte.ID_TIPO_CUENTA && x.ID_CUENTA == reporte.ID_CUENTA).ToList();
                    ViewData["listaMedioMovimiento"] = bancaDB.CAT_CALLCENTER_MEDIO_MOVIMIENTO.ToList();
                    ViewData["listaTipoCuentaBanca"] = db.CAT_UNE_TIPO_CUENTA_BANCA.ToList();

                    //Se cargan la lista de tipos de reporte y segun el usuario se cargan ciertos elementos
                    listaTipoReporte = funcion.obtenerTipoReporte(cl.Usuario, cl.Contrasena);
                    //Se cargan la lista de tipos de medios de contacto y segun el usuario se cargan ciertos elementos
                    listaMedioContacto = funcion.obtenerMediosContacto(cl.Usuario, cl.Contrasena);

                    ViewData["listaProductos"] = db.CAT_UNE_PRODUCTO.ToList();
                    ViewData["listaCanales"] = db.CAT_UNE_CANAL.ToList();
                    ViewData["listaMotivoCancelacion"] = db.CAT_UNE_MOTIVO_CANCELACION.ToList();
                    ViewData["listaTipoReportes"] = funcion.obtenerTipoReporte(cl.Usuario, cl.Contrasena);
                    ViewData["listaMediosContacto"] = funcion.obtenerMediosContacto(cl.Usuario, cl.Contrasena);
                    ViewData["listaCanales"] = db.CAT_UNE_CANAL.ToList();
                    ViewData["listaMotivoCancelacion"] = db.CAT_UNE_MOTIVO_CANCELACION.ToList();
                    ViewData["listaProductos"] = db.CAT_UNE_PRODUCTO.ToList();
                    ViewData["listaResolucion"] = db.CAT_UNE_RESOLUCION.ToList();
                    ViewData["ListaCausaResolucion"] = db.CAT_UNE_CAUSA_RESOLUCION.ToList();

                    if (permisos.USUARIO_CALL_CENTER == false)
                    {
                        ViewData["tipoCuentas"] = db.CAT_UNE_TIPO_CUENTA.ToList();
                        ViewData["listaTipoReportes"] = listaTipoReporte.Where(x => x.ID_TIPO_REPORTE <= 3).ToList();
                        ViewData["listaMediosContacto"] = listaMedioContacto.ToList();
                    }
                    else
                    {
                        ViewData["tipoCuentas"] = db.CAT_UNE_TIPO_CUENTA.Where(x => x.ID_TIPO_CUENTA > 3).ToList();
                        ViewData["listaTipoReportes"] = listaTipoReporte.Where(x => x.ID_TIPO_REPORTE > 3).ToList();
                        if (permisos.USUARIO_SUCURSAL == true)
                        {
                            ViewData["listaMediosContacto"] = listaMedioContacto.Where(x => x.ID_MEDIO_CONTACTO == 1).ToList();
                        }
                        else
                        {
                            if (reporte.ID_ESTATUS_REPORTE == 3 || reporte.ID_ESTATUS_REPORTE == 4 || reporte.ID_ESTATUS_REPORTE == 5)
                            {
                                ViewData["listaMediosContacto"] = listaMedioContacto.Where(x => x.ID_MEDIO_CONTACTO == reporte.ID_MEDIO_CONTACTO).ToList();
                            }
                            else
                            {
                                ViewData["listaMediosContacto"] = listaMedioContacto.Where(x => x.ID_MEDIO_CONTACTO == 2).ToList();
                            }

                        }
                    }
                    ViewData["numEvidenciasDebito"] = db.TBL_UNE_ARCHIVOS_ADJUNTOS.Count(X => X.FOLIO == folio && X.ID_TIPO_ARCHIVO == 9);
                    SP_BANCA_OBTIENE_SOCIO_Result DatosSocio = db.SP_BANCA_OBTIENE_SOCIO(reporte.NUMERO, cl.Numusuario, 1).FirstOrDefault();

                    if (reporte.ID_CUENTA == 33 && DatosSocio.id_motivo_bloqueo == 1)
                    {
                        return RedirectToAction("BloquearBanca", "Banca", reporte);
                    }
                    else
                        return View("Preview", reporte);
                }
                else
                    return RedirectToAction("Index", "Login");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult ModificarReporte(TBL_UNE_REPORTE reporte, List<HttpPostedFileBase> files)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {


                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_REPORTE reporteModificar = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporte.FOLIO).FirstOrDefault();

                reporteModificar.TELEFONO = reporte.TELEFONO;
                reporteModificar.TEL_CELULAR = reporte.TEL_CELULAR;
                reporteModificar.DESCRIPCION_REPORTE = reporte.DESCRIPCION_REPORTE;
                reporteModificar.ID_TIPO_REPORTE = reporte.ID_TIPO_REPORTE;
                reporteModificar.ID_SUPUESTOS_REPORTE = reporte.ID_SUPUESTOS_REPORTE;
                reporteModificar.ID_MEDIO_CONTACTO = reporte.ID_MEDIO_CONTACTO;
                reporteModificar.ID_TIPO_CUENTA = reporte.ID_TIPO_CUENTA;
                reporteModificar.ID_CUENTA = reporte.ID_CUENTA;
                reporteModificar.IMPORTE_RECLAMACION = reporte.IMPORTE_RECLAMACION;
                reporteModificar.ID_PRODUCTO = reporte.ID_PRODUCTO;
                reporteModificar.ID_MOTIVO_CANCELACION = reporte.ID_MOTIVO_CANCELACION;
                reporteModificar.ID_CANAL = reporte.ID_CANAL;
                reporteModificar.NUMERO_CUENTA_PTMO = reporte.NUMERO_CUENTA_PTMO;

                if (reporteModificar.ID_TIPO_REPORTE == 3)
                {
                    reporteModificar.ID_CANAL = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).Select(x => x.ID_CANAL).FirstOrDefault();
                    reporteModificar.ID_MOTIVO_CANCELACION = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).Select(x => x.ID_MOTIVO_CANCELACION).FirstOrDefault();
                    reporteModificar.ID_PRODUCTO = db.CAT_UNE_CUENTAS.Where(x => x.ID_CUENTA == reporte.ID_CUENTA).Select(x => x.ID_PRODUCTO).FirstOrDefault();
                }


                int numEvidencias = db.TBL_UNE_ARCHIVOS_ADJUNTOS.Count(X => X.FOLIO == reporteModificar.FOLIO && X.ID_TIPO_ARCHIVO == 9);

                foreach (HttpPostedFileBase file in files)
                {
                    if (file != null)
                    {
                        TBL_UNE_ARCHIVOS_ADJUNTOS adjunto = new TBL_UNE_ARCHIVOS_ADJUNTOS();
                        Random rnd = new Random();
                        int rdn = rnd.Next(0, 10000);
                        string ruta = funcion.obtenerRuta(Convert.ToInt32(reporteModificar.NUM_FOLIO));
                        string nombre = rdn + "_" + Path.GetFileName(file.FileName);
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
                        adjunto.FOLIO = reporteModificar.FOLIO;

                        if ((reporteModificar.ID_TIPO_REPORTE == 1 || reporteModificar.ID_TIPO_REPORTE == 3) && reporteModificar.ID_CUENTA == 5 && numEvidencias < 2)
                        {
                            adjunto.ID_TIPO_ARCHIVO = 9;
                        }
                        else
                            adjunto.ID_TIPO_ARCHIVO = 7;

                        adjunto.NUMUSUARIO = cl.Numusuario;
                        db.TBL_UNE_ARCHIVOS_ADJUNTOS.Add(adjunto);
                        file.SaveAs(archivo);
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Preview", "Preview", reporteModificar);
            }
            else
                return RedirectToAction("Index", "Login");
        }

        public List<CAT_UNE_SUPUESTOS_REPORTE> cargarSupuestos(int idTipoReporte, String usuario, String contrasena)
        {
            db = new ContextUne(usuario, contrasena);
            var supuestos = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_TIPO_REPORTE == idTipoReporte).ToList();
            return supuestos;
        }

        public String obtenerComentarioArea(int folio, String Usuario, String Contrasena)
        {
            db = new ContextUne(Usuario, Contrasena);
            var comentario = db.SP_UNE_CARGA_COMENTARIO(folio, 1).FirstOrDefault();

            if (comentario != null)
            {
                return comentario.COMENTARIOS;
            }
            else
                return "";
        }

        public String obtenerComentario(int folio, String Usuario, String Contrasena, int caso)
        {
            db = new ContextUne(Usuario, Contrasena);
            var comentario = db.SP_UNE_CARGA_COMENTARIO(folio, caso).FirstOrDefault();

            if (comentario != null)
            {
                return comentario.COMENTARIOS;
            }
            else
                return "";
        }

        public ActionResult agregarDocumentoFinalizar(TBL_UNE_REPORTE rep)
        {
            HttpPostedFileBase file = Request.Files[0];

            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {


                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == rep.FOLIO).FirstOrDefault();
                //TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;

                if (file.ContentLength > 0)
                {
                    TBL_UNE_ARCHIVOS_ADJUNTOS adjunto = new TBL_UNE_ARCHIVOS_ADJUNTOS();
                    string ruta = funcion.obtenerRuta(Convert.ToInt32(reporte.NUM_FOLIO));
                    string nombre = Path.GetFileName(file.FileName);
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
                    adjunto.NOMBRE_ARCHIVO = nombre;
                    adjunto.FOLIO = reporte.FOLIO;
                    adjunto.ID_TIPO_ARCHIVO = 5;
                    adjunto.NUMUSUARIO = cl.Numusuario;
                    db.TBL_UNE_ARCHIVOS_ADJUNTOS.Add(adjunto);
                    file.SaveAs(archivo);
                    db.SaveChanges();
                }

                return RedirectToAction("Finalizados", "Finalizados");
            }
            else
                return RedirectToAction("Index", "Login");

        }

        public int EliminaArchivo(int IdArchivo)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            TBL_UNE_ARCHIVOS_ADJUNTOS archivos = new TBL_UNE_ARCHIVOS_ADJUNTOS();
            archivos = db.TBL_UNE_ARCHIVOS_ADJUNTOS.Where(x => x.ID_ARCHIVO_ADJUNTAR == IdArchivo).FirstOrDefault();
            string rutaSQL = archivos.RUTA_ARCHIVO;
            db.TBL_UNE_ARCHIVOS_ADJUNTOS.Remove(archivos);

            try
            {
                db.SaveChanges();
                System.IO.File.Delete(rutaSQL);
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 1;
        }

        public JsonResult NumArchivosDebito(int folio)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            int numEvidencias = db.TBL_UNE_ARCHIVOS_ADJUNTOS.Count(X => X.FOLIO == folio && X.ID_TIPO_ARCHIVO == 9);
            return Json(numEvidencias);
        }

        public JsonResult ValidarArchivoAudio(int idLlamada)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            dbUne = new ContextUneBD(cl.Usuario, cl.Contrasena);
            Audio audio = new Audio();
            int existe = db.TBL_UNE_CONVERSION_LLAMADAS.Count(x => x.INB_OUT_ID == idLlamada);
            CMV_REC_DATA prescence = dbUne.CMV_REC_DATA.Where(x => x.INB_OUT_ID == idLlamada).FirstOrDefault();

            if (existe > 0)
            {
                try
                {
                    audio.ruta = db.TBL_UNE_CONVERSION_LLAMADAS.Where(x => x.INB_OUT_ID == idLlamada).Select(x => x.RUTA).FirstOrDefault();
                    audio.idLlamada = idLlamada;
                    audio.estatus = 1;
                    audio.rutaOriginal = prescence.PATH_REC.Replace("\\", "//").Replace("\"", "/");
                    audio.fechaLlamada = prescence.PATH_REC;
                    audio.folio = Convert.ToInt32(prescence.CRM_FOLIO);
                    audio.telefono = prescence.PHONE;
                    audio.TipoLlamada = Convert.ToInt32(prescence.CALLTYPE);
                    return Json(audio);
                }
                catch (Exception ex)
                {
                    audio.estatus = 0;
                    return Json(audio);
                }
            }
            else
            {
                try
                {
                    String rutaAudio = ConfigurationSettings.AppSettings["rutaArchivosAudio"] + "\\" + prescence.CRM_FOLIO;
                    String rutaAudioOrigen = prescence.PATH_REC + "\\" + prescence.CALL_ID + ".gsm";
                    if (!Directory.Exists(rutaAudio))
                    {
                        Directory.CreateDirectory(rutaAudio);
                    }

                    String destfilename = rutaAudio + "\\" + prescence.CALL_ID + ".gsm";

                    if (System.IO.File.Exists(destfilename))
                        System.IO.File.Delete(destfilename);

                    string u = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                    System.IO.File.Copy(rutaAudioOrigen, destfilename);

                    string destFullPath = destfilename.Replace("gsm", "wav");
                    var soxProcInf = GetSoxProcInf(rutaAudioOrigen, destFullPath);
                    using (var soxProc = Process.Start(soxProcInf))
                    {
                        if (!soxProc.WaitForExit(30000))
                            soxProc.Kill();
                    }

                    var wavBytes = System.IO.File.ReadAllBytes(destFullPath);
                    var mime = "audio/wav";
                    HttpResponseMessage result = new HttpResponseMessage();
                    result.Content = new StreamContent(new System.IO.MemoryStream(wavBytes));
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue(mime);

                    System.IO.File.Delete(destfilename);

                    TBL_UNE_CONVERSION_LLAMADAS llamada = new TBL_UNE_CONVERSION_LLAMADAS();
                    llamada.INB_OUT_ID = prescence.INB_OUT_ID;
                    llamada.NUM_FOLIO = prescence.CRM_FOLIO;
                    llamada.FECHA_CONVERSION = DateTime.Now;
                    llamada.RUTA = destFullPath.Replace("\\", "//").Replace("\"", "/");
                    llamada.NOMBRE_ARCHIVO = prescence.CALL_ID;
                    db.TBL_UNE_CONVERSION_LLAMADAS.Add(llamada);
                    db.SaveChanges();

                    audio.estatus = 1;
                    audio.ruta = destFullPath.Replace("\\", "//").Replace("\"", "/");
                    audio.idLlamada = idLlamada;
                    audio.rutaOriginal = prescence.PATH_REC.Replace("\\", "//").Replace("\"", "/");
                    audio.fechaLlamada = prescence.PATH_REC;
                    audio.folio = Convert.ToInt32(prescence.CRM_FOLIO);
                    audio.telefono = prescence.PHONE;
                    audio.TipoLlamada = Convert.ToInt32(prescence.CALLTYPE);
                    return Json(audio);
                }
                catch (Exception ex)
                {
                    audio.estatus = 0;
                    return Json(audio);
                }
            }
        }

        ProcessStartInfo GetSoxProcInf(string sourceFullPath, string destFullPath)
        {
            try
            {
                var soxProcInf = new ProcessStartInfo();
                soxProcInf.CreateNoWindow = false;
                soxProcInf.UseShellExecute = false;
                soxProcInf.FileName = Server.MapPath("~/" + ConfigurationManager.AppSettings["Sox"]);
                soxProcInf.WindowStyle = ProcessWindowStyle.Normal;
                //soxProcInf.Arguments = string.("\"{0}\" -b 16 \"{1}\" channels 2 rate 128k fade 3 norm", sourceFullPath, destFullPath);
                soxProcInf.Arguments = string.Format("\"{0}\" -b 16 \"{1}\" channels 2 rate 44100 dither -s", sourceFullPath, destFullPath);
                if (string.IsNullOrEmpty(soxProcInf.FileName))
                    throw new Exception("Invalid SOX path(null / empty)");
                return soxProcInf;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult EnviarEnlacePassword(int numero, int folio)
        {
            bool resultado = true;
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            TBL_UNE_REPORTE rep = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == folio).FirstOrDefault();
            //SP_CALLCENTER_INSERTAR_REGISTRO_REPORTE_Result insertaReporte = bancaDB.SP_CALLCENTER_INSERTAR_REGISTRO_REPORTE(rep.NUMERO, rep.ID_SUPUESTOS_REPORTE, rep.DESCRIPCION_REPORTE, cl.Numusuario.ToString(), 63).First();
            //bancaDB.SaveChanges();
            List<Notificacion> listaNotificaciones = funcion.ObtenerNotificacionSocio(numero, 31, 0, rep.folio_banca,DateTime.Now,"", null);

            try
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(numero.ToString());
                String cuerpo = "";
                foreach (var item in listaNotificaciones)
                {
                    switch (item.idTipoNotificacion)
                    {
                        //case TIPO_NOTIFICACION.SMS:
                        //    Dictionary<string, string> resul = SmsMailUtils.SmsMail.SendSms(new SmsMailUtils.Notificacion { celular = item.celular, cuerpo = item.cuerpo });
                        //    resultado = Convert.ToBoolean(resul["result"].ToString() == "1" ? true : false);
                        //    break;

                        //////////////////////////////////////////Solo se debe de enviar el correo///////////////////////////////////////////////////////////////////////

                        case TIPO_NOTIFICACION.CORREO_ELECTRONICO:
                            cuerpo = item.cuerpo.Replace("@Numero", System.Convert.ToBase64String(plainTextBytes));
                            Dictionary<string, string> resul_ = SmsMailUtils.SmsMail.SendCorreExterno(new SmsMailUtils.Notificacion { cuerpo = cuerpo, para = item.para, asunto = item.asunto });
                            resultado = Convert.ToBoolean(resul_["result"].ToString() == "1" ? true : false);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                resultado = false;
            }

            return Json(resultado);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folio"></param>
        /// <param name="comentarios"></param>
        /// <returns>
        /// valida = 1 -- Retorno correcto
        /// valida = 0 -- Error interno de la aplicación
        /// valida = 2 -- Sesión vencida
        /// </returns>
        public JsonResult FinalizarReporteEnlacePassword(int folio, String comentarios)
        {
            int valida = 1;
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
            if (cl != null)
            {
                try
                {
                    db = new ContextUne(cl.Usuario, cl.Contrasena);
                    TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == folio).FirstOrDefault();
                    DateTime ahora = DateTime.Now;
                    reporte.Observaciones_cierre = comentarios;
                    reporte.Fecha_cierre = ahora;
                    reporte.ID_ESTATUS_REPORTE = 6;
                    reporte.ID_SATISFACTORIO = 1;

                    //if (permisos.USUARIO_CALL_CENTER == false)
                    //{
                    TBL_UNE_CANALIZACIONES canalizacion = new TBL_UNE_CANALIZACIONES();
                        canalizacion.FOLIO = reporte.FOLIO;
                        canalizacion.fecha_alta = DateTime.Now;
                        canalizacion.COMENTARIOS = comentarios;
                        canalizacion.numusuario = reporte.USUARIO_REGISTRA;
                        canalizacion.ID_TIPO_COMENTARIO = 7;
                        db.TBL_UNE_CANALIZACIONES.Add(canalizacion);
                    //}

                    
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    valida = 0;
                }
            }
            else
                valida = 2;

            return Json(valida);
        }

        public ActionResult RegresarLogin()
        {
            return RedirectToAction("Index", "Login");
        }
    }
}
