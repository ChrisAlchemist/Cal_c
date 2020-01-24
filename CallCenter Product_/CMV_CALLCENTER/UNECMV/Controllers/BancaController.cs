using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CMV_CALLCENTER.Entidad;
using CMV_CALLCENTER.Models;
using System.Web.Configuration;
using System.Collections.Generic;

namespace CMV_CALLCENTER.Controllers
{
    public class BancaController : Controller
    {
        //
        // GET: /Banca/
        private ContextUneBD dbHape = new ContextUneBD();
        private ContextUne uneDB = new ContextUne();
        private Funciones funcion = new Funciones();
        private ContextBanca bancaDB = new ContextBanca();

        [HttpPost]
        public ActionResult GenerarFormatoPDF(RequestForOpeNoAplicadas requestForOpeNoAplicadas_)
        {
            try
            {
                string pathFormatosBanca = Server.MapPath("~" + WebConfigurationManager.AppSettings["pathFormatosBanca"].ToString());
                CLAVES cl = Session["SesionUsuario"] as CLAVES;
                GenerarPDF generarUnPDF = new GenerarPDF();
                uneDB = new ContextUne();
                TBL_UNE_REPORTE reporte = uneDB.TBL_UNE_REPORTE.Where(x => x.folio_banca == requestForOpeNoAplicadas_.reporteFolioBanca).First();
                

                string rutaArchivo = "";//= generarUnPDF.formatoAcionesNoReconocidas(reporte, cl,pathFormatosBanca);
                if(reporte.ID_TIPO_CUENTA == 6)
                {
                    //reporte.IMPORTE_SOLUCION = requestForOpeNoAplicadas_.montoADepositar;
                    uneDB.SaveChanges();
                    if (reporte.ID_ESTATUS_REPORTE == 5)
                    {
                        List<ReporteMultifolio> reportesSocio = funcion.ObtenerMultifoliosSocio(Convert.ToInt32(reporte.NUM_FOLIO), cl.Usuario, cl.Contrasena);
                        rutaArchivo = generarUnPDF.formatoDictamen(reportesSocio, reporte, cl, pathFormatosBanca, requestForOpeNoAplicadas_);
                        //reporte.folio_autorizacion_banca = requestForOpeNoAplicadas_.folioAutorizacion;
                        //uneDB.SaveChanges();
                    }
                    else
                    {
                        rutaArchivo = generarUnPDF.formatoOperacionesNoAplicadas(reporte, cl, pathFormatosBanca, requestForOpeNoAplicadas_);
                    }
                    
                }
                else if(reporte.ID_CUENTA == 33)
                {
                    rutaArchivo = generarUnPDF.formatoAcionesNoReconocidas(reporte, cl, pathFormatosBanca);
                }
                
                if(cl != null)
                {
                    if (!string.IsNullOrEmpty(rutaArchivo))
                        return Json(new { estatus = true, ruta = rutaArchivo, mensaje = "Se ha generado correctamente la url del formato.", existeSesion = true }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { estatus = false, ruta = rutaArchivo, mensaje = "No se logro generado correctamente la url del formato.", existeSesion = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                    //return Json(new { estatus = false, ruta = rutaArchivo, mensaje = "La sesión ha caducado.", existeSesion = false }, JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
}

        public ActionResult ModificarMonto(TBL_UNE_REPORTE reporte)
        {
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;
                if (cl != null)
                {
                    uneDB = new ContextUne();
                    bancaDB = new ContextBanca();
                    var DatosSocio = uneDB.SP_BANCA_OBTIENE_SOCIO(reporte.NUMERO, cl.Numusuario, 1).FirstOrDefault();

                    SP_BANCA_OBTENER_MONTO_MAXIMO_Result BancaMontoMaximo = bancaDB.SP_BANCA_OBTENER_MONTO_MAXIMO(1).First();
                    //ViewBag.tiposNotificacion = //obtienes el catalogo de tippos notificacion
                    if (DatosSocio.estatus == 1)
                    {
                        ViewData["listaMediosNotificacion"] = funcion.MostrarTiposNotificacion();
                        ViewData["BancaMontoMaximo"] = BancaMontoMaximo;
                        ViewBag.reporte = reporte;
                        //return View(Tuple.Create(DatosSocio, BancaMontoMaximo));
                        return View(DatosSocio);
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
                else
                    return RedirectToAction("Index", "Login");
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult BloquearBanca(TBL_UNE_REPORTE reporte)
        {
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;
                if(cl != null)
                {
                    uneDB = new ContextUne();
                    var DatosSocio = uneDB.SP_BANCA_OBTIENE_SOCIO(reporte.NUMERO, cl.Numusuario, 1).FirstOrDefault();
                    if (DatosSocio.estatus == 1)
                    {
                        ViewData["listaMediosNotificacion"] = funcion.MostrarTiposNotificacion();
                        ViewBag.reporte = reporte;
                        TBL_UNE_REPORTE repor = uneDB.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporte.FOLIO).FirstOrDefault(); ;
                        ViewBag.reporteUNE = repor;
                        return View(DatosSocio);
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
                else
                    return RedirectToAction("Index", "Login");

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult ReposicionSoftoken(TBL_UNE_REPORTE reporte)
        {
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;

                if(cl != null)
                {
                    uneDB = new ContextUne();
                    bancaDB = new ContextBanca();
                    SP_BANCA_OBTIENE_SOCIO_Result DatosSocio = uneDB.SP_BANCA_OBTIENE_SOCIO(reporte.NUMERO, cl.Numusuario, 1).FirstOrDefault();

                    //SP_BANCA_OBTENER_MONTO_MAXIMO_Result BancaMontoMaximo = bancaDB.SP_BANCA_OBTENER_MONTO_MAXIMO(1).First();
                    //ViewBag.tiposNotificacion = //obtienes el catalogo de tippos notificacion
                    if (DatosSocio.estatus == 1)
                    {
                        //ViewData["listaMediosNotificacion"] = funcion.MostrarTiposNotificacion();
                        //ViewData["BancaMontoMaximo"] = BancaMontoMaximo;
                        //ViewBag.reporte = reporte;
                        //return View(Tuple.Create(DatosSocio, BancaMontoMaximo));
                        ViewData["listaMediosNotificacion"] = funcion.MostrarTiposNotificacion();
                        ViewBag.reporte = reporte;
                        return View(DatosSocio);
                    }
                    else
                    {
                        return HttpNotFound();
                    }
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
        public ActionResult BloquearBanca(int numeroSocio, string descripcionBloqueo, string reporte, int idTipoNotificacion, int idCuenta)
        {
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;
                TBL_UNE_REPORTE repor = new JavaScriptSerializer().Deserialize<TBL_UNE_REPORTE>(reporte);
                SP_BANCA_BLOQUEAR_SOCIO_Result result = bancaDB.SP_BANCA_BLOQUEAR_SOCIO(numeroSocio,2,cl.Numusuario.ToString(),descripcionBloqueo,8,6).First();                
                TBL_UNE_REPORTE reporteUpdate = uneDB.TBL_UNE_REPORTE.Where(x => x.FOLIO == repor.FOLIO).FirstOrDefault();
                if (cl != null)
                {


                    if (idCuenta == 33)
                    {
                        reporteUpdate.ID_ESTATUS_REPORTE = 2;

                        TBL_UNE_USUARIOS_ASIGNADOS usuarioResponsable = new TBL_UNE_USUARIOS_ASIGNADOS();

                    }
                    else
                    {
                        reporteUpdate.ID_ESTATUS_REPORTE = 6;
                        reporteUpdate.Fecha_cierre = DateTime.Now;
                        reporteUpdate.ID_SATISFACTORIO = 1;

                        TBL_UNE_CANALIZACIONES canalizacion = new TBL_UNE_CANALIZACIONES();
                        canalizacion.FOLIO = reporteUpdate.FOLIO;
                        canalizacion.fecha_alta = DateTime.Now;
                        canalizacion.COMENTARIOS = "";
                        canalizacion.numusuario = reporteUpdate.USUARIO_REGISTRA;
                        canalizacion.ID_TIPO_COMENTARIO = 9;

                        uneDB.TBL_UNE_CANALIZACIONES.Add(canalizacion);
                        //funcion.ValidaNotificaciones(repor, 63, idTipoNotificacion, repor.folio_banca, null, "");

                    }

                    uneDB.SaveChanges();
                    funcion.ValidaNotificaciones(repor, 8, idTipoNotificacion, repor.folio_banca, DateTime.Now, "");

                    return Json(new { estatus = result.status, mensaje = result.error_message }, JsonRequestBehavior.AllowGet);
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
        public ActionResult ModificarMonto(string montoMaximo, string reporte, int idTipoNotificacion)
        {
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;
                if (cl != null)
                {


                    /*
                     * @numero_Socio int,
                  @tipo_origen int = null,
                  @numero_usuario varchar(20) = null,
                  @monto_maximo_transferencia money
                     */
                    TBL_UNE_REPORTE repor = new JavaScriptSerializer().Deserialize<TBL_UNE_REPORTE>(reporte);
                    //SP_BANCA_BLOQUEAR_SOCIO_Result result = bancaDB.SP_BANCA_BLOQUEAR_SOCIO(numeroSocio, 2, cl.Numusuario.ToString(), descripcionBloqueo, 8, 6).First();
                    SP_BANCA_MODIFICAR_MONTO_MAXIMO_Result result = bancaDB.SP_BANCA_MODIFICAR_MONTO_MAXIMO(repor.NUMERO, 2, cl.Numusuario.ToString(), Convert.ToDecimal(montoMaximo)).First();

                    //Se registra el reporte 
                    //SP_CALLCENTER_INSERTAR_REGISTRO_REPORTE_Result insertaReporte = bancaDB.SP_CALLCENTER_INSERTAR_REGISTRO_REPORTE(repor.NUMERO, repor.ID_SUPUESTOS_REPORTE, repor.DESCRIPCION_REPORTE, cl.Numusuario.ToString(), 63).First();

                    //Actualizamos el estaus del reporte a 5
                    TBL_UNE_REPORTE reporteUpdate = uneDB.TBL_UNE_REPORTE.Where(x => x.FOLIO == repor.FOLIO).FirstOrDefault();
                    reporteUpdate.ID_ESTATUS_REPORTE = 6;
                    reporteUpdate.Fecha_cierre = DateTime.Now;

                    TBL_UNE_CANALIZACIONES canalizacion = new TBL_UNE_CANALIZACIONES();
                    canalizacion.FOLIO = reporteUpdate.FOLIO;
                    canalizacion.fecha_alta = DateTime.Now;
                    canalizacion.COMENTARIOS = "";
                    canalizacion.numusuario = reporteUpdate.USUARIO_REGISTRA;
                    canalizacion.ID_TIPO_COMENTARIO = 9;
                    reporteUpdate.ID_SATISFACTORIO = 1;
                    uneDB.TBL_UNE_CANALIZACIONES.Add(canalizacion);

                    uneDB.SaveChanges();

                    funcion.ValidaNotificaciones(repor, 44, idTipoNotificacion, repor.folio_banca, DateTime.Now, "");

                    return Json(new { estatus = result.status, mensaje = result.error_message }, JsonRequestBehavior.AllowGet);
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
        public ActionResult ReposicionSoftoken(int NumeroSocio, string reporte, int idTipoNotificacion)
        {
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;
                TBL_UNE_REPORTE repor = new JavaScriptSerializer().Deserialize<TBL_UNE_REPORTE>(reporte);
                SP_BANCA_OBTIENE_SOCIO_Result DatosSocio = uneDB.SP_BANCA_OBTIENE_SOCIO(NumeroSocio, cl.Numusuario, 1).FirstOrDefault();                
                string resultEliminarToken = Funciones.EliminarToken(DatosSocio.Numero.ToString());
                if (cl != null)
                {


                    if (string.Equals("Success", resultEliminarToken, StringComparison.CurrentCultureIgnoreCase))
                    {
                        string resultAprovisionarToken = Funciones.AgregarAprovisionarToken(DatosSocio);
                        if (string.Equals("Success", resultAprovisionarToken, StringComparison.CurrentCultureIgnoreCase))
                        {
                            //Se registra el reporte 
                            //SP_CALLCENTER_INSERTAR_REGISTRO_REPORTE_Result insertaReporte = bancaDB.SP_CALLCENTER_INSERTAR_REGISTRO_REPORTE(repor.NUMERO, repor.ID_SUPUESTOS_REPORTE, repor.DESCRIPCION_REPORTE, cl.Numusuario.ToString(), 63).First();

                            //Actualizamos el estaus del reporte a 5
                            TBL_UNE_REPORTE reporteUpdate = uneDB.TBL_UNE_REPORTE.Where(x => x.FOLIO == repor.FOLIO).FirstOrDefault();
                            reporteUpdate.ID_ESTATUS_REPORTE = 6;
                            reporteUpdate.ID_SATISFACTORIO = 1;
                            reporteUpdate.Fecha_cierre = DateTime.Now;

                            TBL_UNE_CANALIZACIONES canalizacion = new TBL_UNE_CANALIZACIONES();
                            canalizacion.FOLIO = reporteUpdate.FOLIO;
                            canalizacion.fecha_alta = DateTime.Now;
                            canalizacion.COMENTARIOS = "";
                            canalizacion.numusuario = reporteUpdate.USUARIO_REGISTRA;
                            canalizacion.ID_TIPO_COMENTARIO = 9;
                            uneDB.TBL_UNE_CANALIZACIONES.Add(canalizacion);

                            uneDB.SaveChanges();

                            funcion.ValidaNotificaciones(repor, 43, idTipoNotificacion, repor.folio_banca, DateTime.Now, "");

                            return Json(new { estatus = true, mensaje = "Se ha reenviado correctamente la liga de enrolamiento Softoken." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { estatus = false, mensaje = "Por el momento no es posible realizar el reenvio de la liga de enrolamiento, espere un momento y vuelva a intentar.\n\n" + resultAprovisionarToken }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { estatus = false, mensaje = "Por el momento no es posible realizar el reenvio de la liga de enrolamiento, espere un momento y vuelva a intentar.\n\n" + resultEliminarToken }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                    return RedirectToAction("Index", "Login");

                //return Json(new { estatus = "1", mensaje = "Reposición de softoken exitosa" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
           

        }

        [HttpPost]
        public ActionResult ValidarFolioBanca( Int64 numeroSocio, Int64 folioAutorizacion,int folioBanca)
        {
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;
                
                uneDB = new ContextUne();
                TBL_UNE_REPORTE reporte = uneDB.TBL_UNE_REPORTE.Where(x => x.folio_banca == folioBanca).First();
                
                bool folioValido = funcion.ValidarFolioBanca(folioAutorizacion, numeroSocio);

                //return Json(new { estatus = folioValido }, JsonRequestBehavior.AllowGet);
                if (cl != null)
                {
                    if (folioValido)
                        return Json(new { estatus = true, mensaje = "" }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { estatus = false, mensaje = "El folio ingresado no pertenece al socio o ya se le dío un dictamen anteriormente." }, JsonRequestBehavior.AllowGet);

                }
                else
                    return RedirectToAction("Index", "Login");
            }
            catch (Exception e)
            {
                //return Json(new { estatus = false, mensaje = e.Message }, JsonRequestBehavior.AllowGet);
                throw e;
            }
        }

        ///

        //Funciones funciones = new Funciones();
        //bool tinePrestamo;
        //tinePrestamo = funciones.ValidaTienePrestamo(Convert.ToInt32(reporteUpdate.NUMERO));
        [HttpPost]
        public ActionResult ValidarTienePrestamoSocio(Int64 numeroSocio)
        {
            try
            {
                Funciones funciones = new Funciones();
                bool tienePrestamo;
                tienePrestamo = funciones.ValidaTienePrestamo(Convert.ToInt64(numeroSocio));
                return Json(new { socioTienePrestamo = tienePrestamo, mensaje = "Falla al tratar obtener un prestamo." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ObtenerReportesBancaJson(int folioUNE)
        {
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;
                
                //TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.folio_banca == requestForOpeNoAplicadas_.reporteFolioBanca).First();
                List<ReporteMultifolio> reportesSocio = funcion.ObtenerMultifoliosSocio(folioUNE, cl.Usuario, cl.Contrasena);

                if (cl != null)
                {

                    return Json(new { reportesSocio = reportesSocio, mensaje = "Consulta exitosa", existeSesion = true }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return RedirectToAction("Index", "Login");
                    //return Json(new { reportesSocio = reportesSocio, mensaje = "La sesión ha caducado.", existeSesion = false }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult InsertarReportesBancaJson(ReporteMultifolio reporteMultifolio)
        {
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;

                
                //TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.folio_banca == requestForOpeNoAplicadas_.reporteFolioBanca).First();
                int estatus = funcion.InsertarReporteMultifolio(reporteMultifolio, cl.Numusuario, cl.Usuario, cl.Contrasena);

                if (cl != null)
                {
                    return Json(new { estatus = estatus, mensaje = "ejecucion correcta", existeSesion = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                    //return Json(new { estatus = estatus, mensaje = "La sesión ha caducado", existeSesion = false }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }


        [HttpPost]
        public ActionResult EliminarReportesBancaJson(int idIncidenciaReporte)
        {
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;
                
                int estatus = funcion.EliminarReporteMultifolio(idIncidenciaReporte, cl.Usuario, cl.Contrasena);

                if (cl != null)
                {
                    return Json(new { estatus = estatus, mensaje = "ejecucion correcta", existeSesion = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                    //return Json(new { estatus = estatus, mensaje = "La sesión ha caducado", existeSesion = false }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ObtenerDetalleReporteBancaJson(int IdIncidenciaReporte)
        {
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;

                //TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.folio_banca == requestForOpeNoAplicadas_.reporteFolioBanca).First();
                ReporteMultifolio reportesSocio = new ReporteMultifolio();
                reportesSocio = funcion.ObtenerDetalleFolioBanca(IdIncidenciaReporte, cl.Usuario, cl.Contrasena);
                

                if (cl != null)
                {

                    return Json(new { reportesSocio = reportesSocio, mensaje = "Consulta exitosa", existeSesion = true }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return RedirectToAction("Index", "Login");
                    //return Json(new { reportesSocio = reportesSocio, mensaje = "La sesión ha caducado.", existeSesion = false }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }


        [HttpPost]
        public ActionResult EditarReportesBancaJson(ReporteMultifolio reporteMultifolio)
        {
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;


                //TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.folio_banca == requestForOpeNoAplicadas_.reporteFolioBanca).First();
                int estatus = funcion.EditarReporteMultifolio(reporteMultifolio, cl.Usuario, cl.Contrasena);

                if (cl != null)
                {
                    return Json(new { estatus = estatus, mensaje = "ejecucion correcta", existeSesion = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                    //return Json(new { estatus = estatus, mensaje = "La sesión ha caducado", existeSesion = false }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }


        [HttpPost]
        public ActionResult ObtenerMultifoliosPendientesJson(int folioUNE)
        {
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;

                //TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.folio_banca == requestForOpeNoAplicadas_.reporteFolioBanca).First();
                
                int reportesPendientes = funcion.ObtenerMultifoliosPendientes(folioUNE, cl.Usuario, cl.Contrasena);


                if (cl != null)
                {

                    return Json(new { reportesPendientes = reportesPendientes, mensaje = "Consulta exitosa", existeSesion = true }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return RedirectToAction("Index", "Login");
                    //return Json(new { reportesPendientes = reportesPendientes, mensaje = "La sesión ha caducado.", existeSesion = false }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult ValidarReporteProcedenteBancaJson(int folioUNE)
        {
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;


                //TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.folio_banca == requestForOpeNoAplicadas_.reporteFolioBanca).First();
                int estatus = funcion.ValidarReporteProcedenteBanca(folioUNE);

                if (cl != null)
                {
                    return Json(new { estatus = estatus, mensaje = "ejecucion correcta", existeSesion = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                    //return Json(new { estatus = estatus, mensaje = "La sesión ha caducado", existeSesion = false }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }

    }
}


