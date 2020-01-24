using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Configuration;
using System.IO;
using CMV_CALLCENTER.Entidad;
using System.Globalization;
using System.Data.Objects;

namespace CMV_CALLCENTER.Controllers
{
    public class RegistrarCallCenterController : Controller
    {

        private ContextUne db = new ContextUne();
        private Funciones funcion = new Funciones();
        private ContextUneBD uneDB = new ContextUneBD();
        private ContextBanca bancaDB = new ContextBanca();
        private DatosReporteCallCenter datosReporte = new DatosReporteCallCenter();
        private Funciones funciones = new Funciones();
        //
        // GET: /Registrar/

        /// <summary>
        /// Se altera Metodo para recibir datos desde prescence
        /// </summary>
        /// <param name="iframeCallCenter"></param>
        /// <returns></returns>
        /// 

        [HttpPost]
        public ActionResult RegistrarLlamada(IframeCallCenter iframeCallCenter)
        {
            try
            {
                //http://localhost:49369/RegistrarCallCenter/RegistrarCallCenter?vCallType=1&vContactID=968596&vPhone=19996&vNumeroSocio=837648&vServiceID=3
                
                CLAVES cl = Session["SesionUsuario"] as CLAVES;
                if (cl == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    if (Request.UrlReferrer.Query.Length > 0)
                    {
                    /*vCALLTYPE=1&                          0
                     * vContactID=968920&                   1
                     * vPhone=4003&                         2   
                     * vServiceID=31012&                    3
                     * vAgente=1002&                        4
                     * vNumeroSocio=770170&                 5
                     * vID_Fraudes_CC=968918,%20968919      6
                    */
                        iframeCallCenter.vContactID = Convert.ToDecimal(Request.UrlReferrer.Query.Split('&')[1].ToString().Split('=')[1].ToString());
                        if (iframeCallCenter.vContactID != null || iframeCallCenter.vContactID.HasValue)
                        {
                            iframeCallCenter.vCallType = Convert.ToInt32(Request.UrlReferrer.Query.Split('&')[0].ToString().Split('=')[1].ToString());

                            iframeCallCenter.vPhone = Request.UrlReferrer.Query.Split('&')[2].ToString().Split('=')[1].ToString();
                            iframeCallCenter.vServiceID = Request.UrlReferrer.Query.Split('&')[3].ToString().Split('=')[1].ToString();
                            iframeCallCenter.vAgenteID = Request.UrlReferrer.Query.Split('&')[4].ToString().Split('=')[1].ToString();
                            iframeCallCenter.vNumeroSocio = Convert.ToInt32(Request.UrlReferrer.Query.Split('&')[5].ToString().Split('=')[1].ToString());

                            if (Request.UrlReferrer.Query.Split('&').Length > 5)
                            {
                                iframeCallCenter.vID_Fraudes_CC = Request.UrlReferrer.Query.Split('&')[6].ToString().Split('=')[1].ToString();
                                iframeCallCenter.vID_Fraudes_CC = iframeCallCenter.vID_Fraudes_CC.Replace("%", " ");
                            }
                            datosReporte.moduloAtencion = Enumeraciones.CAT_MODULO_ATENCION.CALL_CENTER;
                            funciones.RegistraLlamadaCallCenter(iframeCallCenter, datosReporte);

                        }
                    }
                    return Json(new { estatus = 200, mensaje = "ejecucion Correcta" }, JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

            
        public ActionResult RegistrarCallCenter(IframeCallCenter iframeCallCenter)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            /*if (cl == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {

                if (iframeCallCenter.vContactID != null || iframeCallCenter.vContactID.HasValue)
                {                    
                    datosReporte.moduloAtencion = Enumeraciones.CAT_MODULO_ATENCION.CALL_CENTER;
                    funciones.RegistraLlamadaCallCenter(iframeCallCenter, datosReporte);
                }
                //return View(iframeCallCenter);
                //return RedirectToAction("LlamadaSalida", "Finalizados", iframeCallCenter);
            }*/
            if (cl != null)
            {
                TempData["UrlNav"] = System.Web.HttpContext.Current.Request.Url.ToString();
            
                List<CAT_UNE_TIPO_REPORTE> listaTipoReporte = null;
                List<CAT_UNE_MEDIO_CONTACTO> listaMedioContacto = null;
                db = new ContextUne
                    (cl.Usuario, cl.Contrasena);
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                if (permisos.PERMISO_REGISTRAR == 1)
                {
                    ViewData["sucursales"] = funcion.ListaSucursales(cl.Usuario, cl.Contrasena);
                    ViewData["listaEntidades"] = funcion.ListaEntidades(cl.Usuario, cl.Contrasena);
					ViewData["listaTipoReportes"] = funcion.obtenerTipoReporte(cl.Usuario, cl.Contrasena);
                    ViewData["listaMediosContacto"] = funcion.obtenerMediosContacto(cl.Usuario, cl.Contrasena);
                    ViewData["listaCanales"] = db.CAT_UNE_CANAL.ToList();
                    ViewData["listaMotivoCancelacion"] = db.CAT_UNE_MOTIVO_CANCELACION.ToList();
                    ViewData["listaProductos"] = db.CAT_UNE_PRODUCTO.ToList();
                    Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.ToString();
                    ViewData["listaMedioMovimiento"] = bancaDB.CAT_CALLCENTER_MEDIO_MOVIMIENTO.ToList();
                    //Se cargan la lista de tipos de reporte y segun el usuario se cargan ciertos elementos
                    listaTipoReporte  = funcion.obtenerTipoReporte(cl.Usuario, cl.Contrasena);
                    //Se cargan la lista de tipos de medios de contacto y segun el usuario se cargan ciertos elementos
                    listaMedioContacto = funcion.obtenerMediosContacto(cl.Usuario, cl.Contrasena);
                    
                    if (permisos.USUARIO_CALL_CENTER == false || permisos.USUARIO_CALL_CENTER == null)
                    {
                        ViewData["tipoCuentas"] = db.CAT_UNE_TIPO_CUENTA.Where(x => x.ID_TIPO_CUENTA <= 3).ToList();
                        ViewData["listaTipoReportes"] = listaTipoReporte.Where(x => x.ID_TIPO_REPORTE <= 3).ToList();
                        ViewData["listaMediosContacto"] = listaMedioContacto.ToList();
                    }
                    else
                    {
                        if (permisos.USUARIO_SUCURSAL == true)
                        {
                            ViewData["tipoCuentas"] = db.CAT_UNE_TIPO_CUENTA.Where(x => x.ID_TIPO_CUENTA == 5 || x.ID_TIPO_CUENTA == 6).ToList();

                            ViewData["listaMediosContacto"] = listaMedioContacto.Where(x => x.ID_MEDIO_CONTACTO == 1).ToList();
                        }

                        else
                        {
                            ViewData["tipoCuentas"] = db.CAT_UNE_TIPO_CUENTA.Where(x => x.ID_TIPO_CUENTA > 3).ToList();
                            ViewData["listaMediosContacto"] = listaMedioContacto.Where(x => x.ID_MEDIO_CONTACTO == 2).ToList();
                        }
                            
                        ViewData["listaTipoReportes"] = listaTipoReporte.Where(x => x.ID_TIPO_REPORTE > 3).ToList();
                        
                    }
                    
                    db.SP_UNE_REESTABLECER_FOLIOS_USUARIO_SESION(cl.Numusuario, Session.SessionID);
                    //Session["iframe"] = iframeCallCenter;
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

        public ActionResult pdf(TBL_UNE_REPORTE reporte)
        {
            TBL_UNE_ARCHIVOS_ADJUNTOS archivo = db.TBL_UNE_ARCHIVOS_ADJUNTOS.Where(x => x.FOLIO == reporte.FOLIO).FirstOrDefault();
            TBL_UNE_REPORTE rep = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporte.FOLIO).FirstOrDefault();
            string ruta = archivo.RUTA_ARCHIVO;
            ViewData["ruta"] = ruta;
            ViewData["rep"] = rep;
            return View();
        }

        [HttpPost]
        public ActionResult RegistrarReporte(TBL_UNE_REPORTE reporte)
        {
            IframeCallCenter iframeCallCenter = new IframeCallCenter();
            
            //Request.UrlReferrer.Segments.Last().ToString()

            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {

                db = new ContextUne();
                bancaDB = new ContextBanca();
                SP_BANCA_OBTIENE_SOCIO_Result DatosSocio = db.SP_BANCA_OBTIENE_SOCIO(reporte.NUMERO, cl.Numusuario, 1).FirstOrDefault();
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;


                if (reporte.ID_SUCURSAL_REGISTRO == null)
                    reporte.ID_SUCURSAL_REGISTRO = 1000;

                if (reporte.ES_SOCIO == 1 && permisos.USUARIO_CALL_CENTER != true)
                {
                    var estatus = db.SP_UNE_ACTUALIZA_TELEFONOS(reporte.NUMERO, reporte.TELEFONO, reporte.TEL_CELULAR).FirstOrDefault();
                }

                DateTime hoy = DateTime.Now;

                
                if (permisos.USUARIO_UNE == 1)//se valida si el usuario que esta registrando el reporte es de call center o de UNE
                {
                    reporte.ID_ESTATUS_REPORTE = 2;
                    reporte.FECHA_Canalizacion = hoy;
                    reporte.DIAS_RESTANTES_GENERAL = 30;
                }
                else if (reporte.ID_MEDIO_CONTACTO == 2 && cl.Id_de_sucursal == 1)
                {
                    reporte.ID_ESTATUS_REPORTE = 2;
                    reporte.FECHA_Canalizacion = hoy;
                }
                if (permisos.USUARIO_CALL_CENTER == true)
                {
                    if (reporte.ID_ESTATUS_REPORTE == 11)
                    {
                        reporte.ID_ESTATUS_REPORTE = 11;
                        reporte.Fecha_cierre = hoy;
                    }
                    else
                    {
                        reporte.ID_ESTATUS_REPORTE = 2;                        
                        if (reporte.ID_CUENTA != 33 && reporte.ID_TIPO_CUENTA != 6)
                        {
                            reporte.NUM_FOLIO = 0;
                            reporte.DIAS_RESTANTES_GENERAL = 0;
                        }
                        else
                            reporte.NUM_FOLIO = reporte.FOLIO;
                    }
                    
                }
                else
                {
                    reporte.ID_ESTATUS_REPORTE = 1;
                    reporte.NUM_FOLIO = reporte.FOLIO;
                }
                    

                reporte.FECHA_ALTA = hoy;
                

                reporte.VoBo = 0;
                reporte.Vencido = 0;
                reporte.ID_PROCEDE_DEBITO = 0;
                reporte.BANDEJA_DEBITO = 0;
                reporte.ID_FINALIZADO_DEBITO = 0;
                if (reporte.ID_TIPO_REPORTE == 3)
                {
                    reporte.ID_RESOLUCION = 503;
                    reporte.ID_CAUSA_RESOLUCION = 654;

                    reporte.ID_CANAL = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).Select(x => x.ID_CANAL).FirstOrDefault();
                    reporte.ID_MOTIVO_CANCELACION = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).Select(x => x.ID_MOTIVO_CANCELACION).FirstOrDefault();
                    reporte.ID_PRODUCTO = db.CAT_UNE_CUENTAS.Where(x => x.ID_CUENTA == reporte.ID_CUENTA).Select(x => x.ID_PRODUCTO).FirstOrDefault();
                }



                int archivo = 0;
                int? estatusSP;

                if (reporte.ID_TIPO_REPORTE == null && permisos.USUARIO_CALL_CENTER == true)
                {
                    reporte.ID_TIPO_REPORTE = 4;
                    if (permisos.USUARIO_SUCURSAL == true)
                    {
                        reporte.ID_MEDIO_CONTACTO = 1;
                    }
                    else
                    {
                        reporte.ID_MEDIO_CONTACTO = 2;
                    }
                    

                }

                if(permisos.USUARIO_CALL_CENTER == true)
                {
                    var diasRequeridos = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).Select(x => x.DIAS_REQUERIDOS).FirstOrDefault();
                    reporte.DIAS_RESTANTES_GENERAL = diasRequeridos;
                }
                

                try
                {
                    estatusSP = db.SP_UNE_REGISTRA_REPORTE(reporte.NUM_FOLIO, reporte.ES_SOCIO, reporte.NUMERO, reporte.NOMBRE_S, reporte.APELLIDO_PATERNO, reporte.APELLIDO_MATERNO, reporte.TELEFONO, reporte.TEL_CELULAR, reporte.USUARIO_REGISTRA, reporte.ID_DE_SUCURSAL, reporte.DESCRIPCION_REPORTE, reporte.ENTIDAD, reporte.IMPORTE_RECLAMACION, reporte.IMPORTE_SOLUCION, reporte.ID_TIPO_REPORTE, reporte.ID_SUPUESTOS_REPORTE, reporte.ID_MEDIO_CONTACTO, reporte.ID_ESTATUS_REPORTE, reporte.ID_TIPO_CUENTA, reporte.ID_CUENTA, reporte.ID_SUCURSAL_REGISTRO, reporte.DIAS_RESTANTES_GENERAL, reporte.Num_Tarjeta, reporte.DOMICILIO).FirstOrDefault();
                    DateTime fechaCompromiso = Convert.ToDateTime( db.SP_CALLCENTER_GENERAR_FECHA_COMPROMISO_REPORTE(Convert.ToInt16(reporte.DIAS_RESTANTES_GENERAL)).FirstOrDefault());
                    if (estatusSP == 1)
                    {
                        //Validamos si es un reporte de BancaCMV, si lo es ejecutamos el sp SP_CALLCENTER_INSERTAR_REGISTRO_REPORTE
                        if (permisos.USUARIO_CALL_CENTER == true)
                        {
                            if (Request.UrlReferrer.Query.Length > 0)
                            {
                                iframeCallCenter.vContactID = Convert.ToDecimal(Request.UrlReferrer.Query.Split('&')[1].ToString().Split('=')[1].ToString());
                            }
                                SP_CALLCENTER_INSERTAR_REGISTRO_REPORTE_Result registroBanca = bancaDB.SP_CALLCENTER_INSERTAR_REGISTRO_REPORTE(reporte.NUMERO, reporte.ID_SUPUESTOS_REPORTE, reporte.DESCRIPCION_REPORTE, cl.Numusuario.ToString(), 63).FirstOrDefault();

                                bancaDB.SaveChanges();

                                reporte.folio_banca = registroBanca.id_reporte;
                                reporte.reporte_banca = true;
                                ///Aqui se actualizara el folio de UNE..cmv_rec_data
                                int registroExitoso;
                                datosReporte.moduloAtencion = Enumeraciones.CAT_MODULO_ATENCION.CALL_CENTER;
                                datosReporte.folio = Convert.ToInt64(registroBanca.id_reporte);
                                datosReporte.folioUNE = reporte.NUM_FOLIO;
                                if (Request.UrlReferrer.Query.Length > 0)
                                {
                                    registroExitoso = funciones.ActualizarRegistroLlamada(iframeCallCenter, datosReporte);
                                }
                                /// 
                                ///*/
                            

                        }
                        else
                            reporte.reporte_banca = false;


                        db.TBL_UNE_REPORTE.Add(reporte);
                        db.SP_UNE_ACTUALIZA_FOLIO_USUARIO_SESION(cl.Numusuario, reporte.NUM_FOLIO, Session.SessionID);
                        db.SaveChanges();
                        if(reporte.ID_ESTATUS_REPORTE != 11 && permisos.USUARIO_CALL_CENTER == true)
                        {
                            if (reporte.ID_CUENTA == 33 || reporte.ID_CUENTA == 38 || reporte.ID_CUENTA == 39)
                            {
                                funcion.ValidaNotificaciones(reporte, 63, Convert.ToInt16(DatosSocio.id_tipo_notificacion), reporte.folio_banca, fechaCompromiso, "");
                            }
                        }
                        
                        /*
                        Iframe iframe = Session["iframe"] as Iframe;
                        if (iframe.vContactID.HasValue)
                        {
                            uneDB = new ContextUneBD(cl.Usuario, cl.Contrasena);
                            CMV_REC_DATA cmv_rec_data = new CMV_REC_DATA();
                            cmv_rec_data.INB_OUT_ID = iframe.vContactID;
                            cmv_rec_data.CALLTYPE = iframe.vCALLTYPE;
                            cmv_rec_data.PHONE = iframe.vPhone;
                            cmv_rec_data.RDATE = DateTime.Now;
                            cmv_rec_data.CRM_FOLIO = reporte.NUM_FOLIO.ToString();
                            uneDB.CMV_REC_DATA.Add(cmv_rec_data);
                            uneDB.SaveChanges();
                        }*/

                        if (reporte.ID_MEDIO_CONTACTO == 1 && permisos.USUARIO_CALL_CENTER != true)
                        {
                            archivo = generaPDF(reporte);
                            reporte.FOLIO = archivo;
                            return RedirectToAction("pdf", "Registrar", reporte);
                        }

                        if (reporte.ID_ESTATUS_REPORTE == 11)
                        {
                            return RedirectToAction("Registros", "Registros");
                        }
                    }
                    else
                    {
                        reporte = db.TBL_UNE_REPORTE.Where(x => x.NUM_FOLIO == reporte.NUM_FOLIO).FirstOrDefault();
                        return RedirectToAction("pdf", "Registrar", reporte);
                    }
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

                return RedirectToAction("Preview", "Preview", reporte);
            }
            else
                return RedirectToAction("Index", "Login");

        }

        [HttpPost]
        public JsonResult BuscaNumSocio(int NUMERO, int tipoPersona)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;

            if (cl == null)
            {
                RedirectToAction("Index", "Login");
                return null;
            }
            else
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                var DatosSocio = db.SP_UNE_BUSCA_NUMERO_SOCIO(NUMERO, tipoPersona).FirstOrDefault();

                TBL_UNE_PERMISOS_ADMIN permisoAcceso = (Session["permiso"] as TBL_UNE_PERMISOS_ADMIN);
                SP_BANCA_OBTIENE_SOCIO_Result DatosSocioBanca = new SP_BANCA_OBTIENE_SOCIO_Result();

                if (permisoAcceso.USUARIO_SUCURSAL == true)
                    DatosSocioBanca = db.SP_BANCA_OBTIENE_SOCIO(DatosSocio.Numero, cl.Numusuario, 1).FirstOrDefault();
                else
                    DatosSocioBanca = db.SP_BANCA_OBTIENE_SOCIO(DatosSocio.Numero, null, 1).FirstOrDefault();


                List<Object> json2 = new List<object>();
                json2.Add(DatosSocio);
                json2.Add(DatosSocioBanca);


                if (DatosSocio.estatus == 1)
                    return Json(json2);
                else
                {
                    DatosSocio.estatus = 0;
                    return Json(json2);
                }
            }

            
        }

        [HttpPost]
        public JsonResult ObtenerProxFolio()
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            var folio = db.SP_UNE_OBTENER_FOLIO(cl.Numusuario, Session.SessionID);
            return Json(folio);
        }


        [HttpPost]
        public JsonResult ActualizaTelefono(int NUMERO, string TELEFONO, string TEL_CELULAR)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            var estatus = db.SP_UNE_ACTUALIZA_TELEFONOS(NUMERO, TELEFONO, TEL_CELULAR).FirstOrDefault();
            return Json(estatus);
        }

        public JsonResult ObtenerSupuestos(int id, int idTipoCuenta, int idCuenta)
        {

            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            var supuestos = db.SP_UNE_CARGAR_SUPUESTOS(id, idTipoCuenta, idCuenta);
            return Json(supuestos);
        }

        public JsonResult obtenerRepresentates(int numero)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            var representantes = db.SP_UNE_REPRESENTANTES_LEGALES(numero);
            return Json(representantes);
        }

        public JsonResult BuscaPM(int id_persona_rel)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            var persona = db.SP_UNE_CARGA_DATOS_REPRESENTANTE_LEGAL(id_persona_rel);
            return Json(persona);
        }

        public int obtenerEntidadFederativa()
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            var entidad = db.SP_UNE_OBTENER_ENTIDAD_FEDERATIVA(cl.Id_de_sucursal).FirstOrDefault();
            return entidad.ID_ENTIDAD_FEDERATIVA;
        }

        public JsonResult obtenerCuentas(int tipoCuenta, int numero)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);

            TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
            var DatosSocioBanca = db.SP_BANCA_OBTIENE_SOCIO(numero, null, 1).FirstOrDefault();

            if (permisos.USUARIO_CALL_CENTER == true )
            {
                TBL_BANCA_SOCIOS bancaSocio = bancaDB.TBL_BANCA_SOCIOS.Where(x => x.numero_socio == numero && x.banca_activa == true).FirstOrDefault();
                if (tipoCuenta == 5)
                {
                    var lista = new Object();

                    if (DatosSocioBanca.id_estatus_banca == 4 && DatosSocioBanca.banca_activa== false)
                    {
                        //var lista = new Object();

                        lista = (from x in db.CAT_UNE_CUENTAS where x.ID_TIPO_CUENTA == tipoCuenta && x.ID_CUENTA == 36 select new { x.DESCRIPCION, x.ID_CUENTA });
                        //return Json(lista);
                    }

                    else if (DatosSocioBanca.id_motivo_bloqueo != 1)
                    {
                                                
                            lista = (from x in db.CAT_UNE_CUENTAS where x.ID_TIPO_CUENTA == tipoCuenta && x.ID_CUENTA == 35 select new { x.DESCRIPCION, x.ID_CUENTA });

                    }
                    
                    else if (DatosSocioBanca.id_estatus_banca==8)
                    {
                        lista = (from x in db.CAT_UNE_CUENTAS where x.ID_TIPO_CUENTA == tipoCuenta && (x.ID_CUENTA != 35 && x.ID_CUENTA != 36) select new { x.DESCRIPCION, x.ID_CUENTA });

                        
                    }
                    else
                    {
                        lista = (from x in db.CAT_UNE_CUENTAS where x.ID_TIPO_CUENTA == tipoCuenta && (x.ID_CUENTA != 35) select new { x.DESCRIPCION, x.ID_CUENTA });
                    }

                    if (permisos.USUARIO_SUCURSAL == true && tipoCuenta == 5)
                    {
                        lista = (from x in db.CAT_UNE_CUENTAS where x.ID_TIPO_CUENTA == tipoCuenta && (x.ID_CUENTA == 33) select new { x.DESCRIPCION, x.ID_CUENTA });
                    }

                    return Json(lista);
                    
                }
                
                else
                {
                    var lista = new Object();
                    
                    //else
                    //{
                        lista = (from x in db.CAT_UNE_CUENTAS where x.ID_TIPO_CUENTA == tipoCuenta select new { x.DESCRIPCION, x.ID_CUENTA });
                    //}
                    
                    return Json(lista);
                }

            }
            else
            {
                var lista = (from x in db.CAT_UNE_CUENTAS where x.ID_TIPO_CUENTA == tipoCuenta select new { x.DESCRIPCION, x.ID_CUENTA });
                return Json(lista);
            }            
        }

        public int generaPDF(TBL_UNE_REPORTE reporte)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;

            CAT_UNE_TIPO_REPORTE cat = db.CAT_UNE_TIPO_REPORTE.Where(x => x.ID_TIPO_REPORTE == reporte.ID_TIPO_REPORTE).FirstOrDefault();
            TBL_UNE_REPORTE rep = db.TBL_UNE_REPORTE.Where(x => x.NUM_FOLIO == reporte.NUM_FOLIO).FirstOrDefault();

            string directorio = System.Web.Hosting.HostingEnvironment.MapPath(@"~/");
            string derecha = "style='text-align:right;'";
            string izq = "style='text-align:left;'";
            string css = "style='font-size:4.5px; color:white; text-align:center; font-weight:bold;'";
            string cabecera = "bgcolor='#6B8E23' style='font-weight:bold; text-align:center; color:white'";
            DateTime fecha = Convert.ToDateTime(reporte.FECHA_ALTA);
            String mes = funcion.ObtenerMes(fecha.Month);
            string ruta = funcion.obtenerRuta(Convert.ToInt32(reporte.NUM_FOLIO));
            string nombre = "CONSTANCIA DE REGISTRO DE REPORTE.pdf";
            string datosGenerales = string.Empty;
            string nombreSocio = rep.NOMBRE_S + " " + rep.APELLIDO_PATERNO + " " + rep.APELLIDO_MATERNO;
            directorio += @"\Estilos\Imagenes";
            if (!System.IO.Directory.Exists(ruta))
            {
                System.IO.Directory.CreateDirectory(ruta);
            }

            try
            {
                datosGenerales = @"<html><head></head><body><img src='" + Path.Combine(directorio, "header.png") + @"' width = '125' height = '75' /> <br/><P ALIGN=right style='padding-right: 30px;'>" + fecha.ToString("dd") + " de " + mes + " del " + fecha.Year + "</p>"
                    + "<br/><br/><br/>"
                    + "<p ALIGN=left style='padding-left: 30px;padding-right: 30px;'>Estimado socio(a) " + nombreSocio + ":<br/><br/>"
                    + "<div style='border: black 2px solid;'><p>Mediante el presente escrito la Unidad Especializada de Caja Morelia Valladolid S.C. de A.P. de R.L. de C.V., le informa qué ha quedado debidamente levantada su <u><strong>"
                    + cat.DESCRIPCION
                    + "</strong></u> asignándosele el número de folio <u><strong>" + reporte.NUM_FOLIO + "</strong></u>,  a la cual se le dará contestación dentro del plazo máximo de 30 treinta días"
                    + " hábiles, contados a partir de la fecha de su recepción, tal y como lo establece el artículo 50 bis de la Ley de Protección y Defensa al Usuario de Servicios Financieros.</p><br/><br/>"
                    + "<p>Cualquier duda al respecto marcar al 01-800-3000-268 o acudir a cualquiera de nuestras sucursales.</p></div>"
                    + "<br/><br/><br/><br/><br/>"
                    + "<p ALIGN=center>Atentamente</p><br/><br/>"
                    + "<p ALIGN=center>_______________________________________</p></p><br/>"
                    //+ "<p ALIGN=center>Caja Morelia Valladolid S.C. de A.P. de R.L. de C.V.</p></p>"
                    + "</body>";
                ruta = ruta + "\\" + nombre;

                TBL_UNE_ARCHIVOS_ADJUNTOS archivo = new TBL_UNE_ARCHIVOS_ADJUNTOS();
                archivo.FOLIO = rep.FOLIO;
                archivo.NOMBRE_ARCHIVO = nombre;
                archivo.RUTA_ARCHIVO = ruta.Replace("\\", "//").Replace("\"", "/");
                archivo.FECHA_ALTA = DateTime.Now;
                archivo.ID_TIPO_ARCHIVO = 1;
                archivo.NUMUSUARIO = cl.Numusuario;
                db.TBL_UNE_ARCHIVOS_ADJUNTOS.Add(archivo);
                db.SaveChanges();

                Document document = new Document(PageSize.LETTER, 5, 30, 0, 0);
                PdfWriter PDFWriter = PdfWriter.GetInstance(document, new FileStream(ruta, FileMode.Create));

                PDF eventos = new PDF();
                PDFWriter.PageEvent = eventos;

                document.Open();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(datosGenerales.ToString()), new StyleSheet()))
                {
                    document.Add(E);
                }
                document.AddAuthor("CMV_CALLCENTER");
                document.AddTitle("Acuse de solicitud");
                document.AddCreator("CMV_CALLCENTER");
                document.AddKeywords("CAJA MORELIA");
                document.AddSubject("Acuse de solicitud");
                document.CloseDocument();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return rep.FOLIO;
        }

        public JsonResult ObtenerSuspuestosValidar()
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            var lista = db.SP_UNE_OBTENER_SUPUESTOS_VALIDA_IMPORTE();
            return Json(lista);
        }

        public JsonResult DiferenciaFechas(String fecha)
        {
            DateTime fechaHoy = DateTime.Now;
            DateTime fechaValida = Convert.ToDateTime(fecha);
            TimeSpan tiempoTranscurrido;
            tiempoTranscurrido = fechaHoy.Subtract(fechaValida);

            return Json(tiempoTranscurrido.Days);
        }

        public ActionResult validaAutenticacionBanca(int idSupuesto)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {
                bool? requiere_autenticacion = false;
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                if (permisos.USUARIO_SUCURSAL == true)
                    requiere_autenticacion = false;
                else
                    requiere_autenticacion = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == idSupuesto).Select(x => x.requiere_autentificacion).FirstOrDefault();

            return Json(requiere_autenticacion);
            }
            else
            {
                return new HttpStatusCodeResult(460, "La sesión se ha vencido.");
            }
        }

        public ActionResult CargarCuestionario(int numero)
        {
            
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            
            if (cl != null)
            {
                List<PreguntaBanca> preguntaBanca = funcion.CargarCuestionario(numero);
                return Json(preguntaBanca);
            }
            else
            {
                return new HttpStatusCodeResult(460, "La sesión se ha vencido.");
            }

        }

        public JsonResult Prueba()
        {
            try
            {
                bool valida;
                Dictionary<string, string> resul = SmsMailUtils.SmsMail.SendSms(new SmsMailUtils.Notificacion { celular = "4432646306", cuerpo = "Prueba SMS" });
                valida = Convert.ToBoolean(resul["result"].ToString());

                //valida = SmsMailUtils.SmsMail.SendSms(new SmsMailUtils.Notificacion { celular = "4432646306", cuerpo = "Prueba SMS" });
                return Json(valida);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public JsonResult ValidaSocioBanca(int numero)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            bool valida = false;
            TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
            Funciones funciones = new Funciones();
            if (permisos.USUARIO_CALL_CENTER == true)
            {
                //SP_BANCA_OBTIENE_SOCIO_Result bancaSocio = db.SP_BANCA_OBTIENE_SOCIO(numero, cl.Numusuario, 1).FirstOrDefault();


                Socio bancaSocio = funciones.ObtenerSocioRegistradoBanca(numero);

                if (bancaSocio != null)
                    valida = true;
                else
                    valida = false;
            }
            else
                valida = true;

            return Json(valida);
        }
        public JsonResult ObtenerIdMovProducto(int id)
        {
            int? prod = 0;

            CAT_UNE_CUENTAS cuentas = db.CAT_UNE_CUENTAS.Where(x => x.ID_CUENTA == id).FirstOrDefault();

            if (cuentas != null)
                prod = cuentas.ID_MOV_HAPE;
            else
                prod = 0;

            return Json(prod);
        }

        public JsonResult ObtenerProductoUNE(int idMov)
        {
            int? prod = 0;

            CAT_UNE_PRODUCTO producto = db.CAT_UNE_PRODUCTO.Where(x => x.ID_MOV_HAPE == idMov).FirstOrDefault();

            if (producto != null)
                prod = producto.ID_PRODUCTO;
            else
                prod = 0;

            return Json(prod);
        }

        public JsonResult ObtenerPlazosFijos(int numero)
        {
            var plazos = db.SP_UNE_OBTENER_PLAZOS_FIJOS_ACTIVOS(numero);

            return Json(plazos);
        }

        public JsonResult ObtenerNumPtmo(int numero, int idMov)
        {
            var numPtmo = db.SP_UNE_OBTENER_NUMPTMO(numero, idMov);

            return Json(numPtmo);
        }

        public JsonResult ObtenerCanal(int idSupuesto)
        {
            int? idCanal = 0;

            CAT_UNE_SUPUESTOS_REPORTE supuesto = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == idSupuesto).FirstOrDefault();

            if (supuesto != null)
                idCanal = supuesto.ID_CANAL;
            return Json(idCanal);
        }

        public JsonResult ObtenerMotivoCancelacion(int idSupuesto)
        {
            int? idMotivo = 0;

            CAT_UNE_SUPUESTOS_REPORTE supuesto = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == idSupuesto).FirstOrDefault();

            if (supuesto != null)
                idMotivo = supuesto.ID_MOTIVO_CANCELACION;
            return Json(idMotivo);
        }

        public JsonResult ObtenerProducto(int id)
        {
            int? idProducto = 0;

            CAT_UNE_CUENTAS cuenta = db.CAT_UNE_CUENTAS.Where(x => x.ID_CUENTA == id).FirstOrDefault();

            if (cuenta != null)
                idProducto = cuenta.ID_PRODUCTO;
            return Json(idProducto);

        }

        public ActionResult PruebaNotificaciones()
        {
            TBL_UNE_REPORTE reporte = new TBL_UNE_REPORTE();
            reporte.NUMERO = 837648;
            int idTipoNotificacion = 2;
            int idTipoBitacora = 44;
            Funciones funciones = new Funciones();
            funcion.ValidaNotificaciones(reporte, idTipoBitacora,idTipoNotificacion,1, null, "");
            return null;
        }





    }
}
