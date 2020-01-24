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

namespace CMV_CALLCENTER.Controllers
{
    public class RegistrarController : Controller
    {
        private ContextUne db = new ContextUne();
        private Funciones funcion = new Funciones();
        private ContextUneBD uneDB = new ContextUneBD();
        //
        // GET: /Registrar/

        /// <summary>
        /// Se altera Metodo para recibir datos desde prescence
        /// </summary>
        /// <param name="iframe"></param>
        /// <returns></returns>
        /// 
        public ActionResult Registrar(Iframe iframe)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;

            if (iframe.vContactID.HasValue && iframe.vCALLTYPE == 0)
            {
                return RedirectToAction("LlamadaSalida", "Finalizados", iframe);
            }

            TempData["UrlNav"] = System.Web.HttpContext.Current.Request.Url.ToString();
            if (cl != null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                if (permisos.PERMISO_REGISTRAR == 1)
                {
                    ViewData["tipoCuentas"] = db.CAT_UNE_TIPO_CUENTA.ToList();
                    ViewData["sucursales"] = funcion.ListaSucursales(cl.Usuario, cl.Contrasena);
                    ViewData["listaEntidades"] = funcion.ListaEntidades(cl.Usuario, cl.Contrasena);
                    //listaUsuarios.Where(x => x.Numusuario == 518 || x.Numusuario == 116).ToList();
                    ViewData["listaTipoReportes"] = funcion.obtenerTipoReporte(cl.Usuario, cl.Contrasena).Where(x => x.ID_TIPO_REPORTE !=4).ToList();
                    ViewData["listaMediosContacto"] = funcion.obtenerMediosContacto(cl.Usuario, cl.Contrasena);
                    ViewData["listaCanales"] = db.CAT_UNE_CANAL.ToList();
                    ViewData["listaMotivoCancelacion"] = db.CAT_UNE_MOTIVO_CANCELACION.ToList();
                    ViewData["listaProductos"] = db.CAT_UNE_PRODUCTO.ToList();
                    Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.ToString();
                    db.SP_UNE_REESTABLECER_FOLIOS_USUARIO_SESION(cl.Numusuario, Session.SessionID);
                    Session["iframe"] = iframe;
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
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;

            if (reporte.ID_SUCURSAL_REGISTRO == null)
                reporte.ID_SUCURSAL_REGISTRO = 1000;

            if (reporte.ES_SOCIO == 1)
            {
                var estatus = db.SP_UNE_ACTUALIZA_TELEFONOS(reporte.NUMERO, reporte.TELEFONO, reporte.TEL_CELULAR).FirstOrDefault();
            }

            DateTime hoy = DateTime.Now;

            if (permisos.USUARIO_UNE == 1)
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
            else
                reporte.ID_ESTATUS_REPORTE = 1;

            reporte.FECHA_ALTA = hoy;
            reporte.NUM_FOLIO = reporte.FOLIO;
            reporte.VoBo = 0;
            reporte.Vencido = 0;
            reporte.ID_PROCEDE_DEBITO = 0;
            reporte.BANDEJA_DEBITO = 0;
            reporte.ID_FINALIZADO_DEBITO = 0;
            if (reporte.ID_TIPO_REPORTE == 3)
            {
                reporte.ID_RESOLUCION = 503;
                reporte.ID_CAUSA_RESOLUCION = 654;

                //reporte.ID_CANAL = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).Select(x => x.ID_CANAL).FirstOrDefault();
                //reporte.ID_MOTIVO_CANCELACION = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).Select(x => x.ID_MOTIVO_CANCELACION).FirstOrDefault();
                //reporte.ID_PRODUCTO = db.CAT_UNE_CUENTAS.Where(x => x.ID_CUENTA == reporte.ID_CUENTA).Select(x => x.ID_PRODUCTO).FirstOrDefault();
            }


            //if (reporte.FECHA_SOLICITUD_ACLARACION == null)
            //    reporte.FECHA_SOLICITUD_ACLARACION = DateTime.Now;

            //if (reporte.FECHA_TRANSACCION == null)
            //    reporte.FECHA_TRANSACCION = DateTime.Now;

            int archivo = 0;
            int? estatusSP;
            try
            {
                estatusSP = db.SP_UNE_REGISTRA_REPORTE(reporte.NUM_FOLIO, reporte.ES_SOCIO, reporte.NUMERO, reporte.NOMBRE_S, reporte.APELLIDO_PATERNO, reporte.APELLIDO_MATERNO, reporte.TELEFONO, reporte.TEL_CELULAR, reporte.USUARIO_REGISTRA, reporte.ID_DE_SUCURSAL, reporte.DESCRIPCION_REPORTE, reporte.ENTIDAD, reporte.IMPORTE_RECLAMACION, reporte.IMPORTE_SOLUCION, reporte.ID_TIPO_REPORTE, reporte.ID_SUPUESTOS_REPORTE, reporte.ID_MEDIO_CONTACTO, reporte.ID_ESTATUS_REPORTE, reporte.ID_TIPO_CUENTA, reporte.ID_CUENTA, reporte.ID_SUCURSAL_REGISTRO, reporte.DIAS_RESTANTES_GENERAL, reporte.Num_Tarjeta, reporte.DOMICILIO).FirstOrDefault();
                if (estatusSP == 1)
                {
                    db.TBL_UNE_REPORTE.Add(reporte);
                    db.SP_UNE_ACTUALIZA_FOLIO_USUARIO_SESION(cl.Numusuario, reporte.NUM_FOLIO, Session.SessionID);
                    db.SaveChanges();

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
                    }

                    if (reporte.ID_MEDIO_CONTACTO == 1)
                    {
                        archivo = generaPDF(reporte);
                        reporte.FOLIO = archivo;
                        return RedirectToAction("pdf", "Registrar", reporte);
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

        [HttpPost]
        public JsonResult BuscaNumSocio(int NUMERO, int tipoPersona)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            var DatosSocio = db.SP_UNE_BUSCA_NUMERO_SOCIO(NUMERO, tipoPersona).FirstOrDefault();
            if (DatosSocio.estatus == 1)
                return Json(DatosSocio);
            else
            {
                DatosSocio.estatus = 0;
                return Json(DatosSocio);
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

        public JsonResult obtenerCuentas(int tipoCuenta)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            var lista = (from x in db.CAT_UNE_CUENTAS where x.ID_TIPO_CUENTA == tipoCuenta select new { x.DESCRIPCION, x.ID_CUENTA });
            return Json(lista);
        }

        public int generaPDF(TBL_UNE_REPORTE reporte)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;

            CAT_UNE_TIPO_REPORTE cat = db.CAT_UNE_TIPO_REPORTE.Where(x => x.ID_TIPO_REPORTE == reporte.ID_TIPO_REPORTE).FirstOrDefault();
            TBL_UNE_REPORTE rep = db.TBL_UNE_REPORTE.Where(x => x.NUM_FOLIO == reporte.NUM_FOLIO).FirstOrDefault();

            string directorio = System.Environment.CurrentDirectory;
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

    }
}
