using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Entidad;
using CMV_CALLCENTER.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.IO;
using System.Globalization;

namespace CMV_CALLCENTER.Controllers
{
    public class NotificarController : Controller
    {
        private ContextUne db = new ContextUne();
        private Funciones funcion = new Funciones();
        private ContextBanca bancaDB = new ContextBanca();
        //
        // GET: /Notificar/

        public ActionResult Notificar()
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if(cl!=null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                ViewData["listaNotificaciones"] = db.TBL_UNE_REPORTE.Where(x => x.ID_ESTATUS_REPORTE == 5 && x.USUARIO_REGISTRA == cl.Numusuario).ToList();
                ViewData["listaMedioMovimiento"] = bancaDB.CAT_CALLCENTER_MEDIO_MOVIMIENTO.ToList();
                ViewData["listaTipoCuentaBanca"] = db.CAT_UNE_TIPO_CUENTA_BANCA.ToList();
                return View();
            }
            else
                return RedirectToAction("Index", "Login");
        }

        public ActionResult pdf(TBL_UNE_REPORTE reporte)
        {
            TBL_UNE_ARCHIVOS_ADJUNTOS archivo = db.TBL_UNE_ARCHIVOS_ADJUNTOS.Where(x => x.FOLIO == reporte.FOLIO && x.ID_TIPO_ARCHIVO == 2).FirstOrDefault();
            TBL_UNE_REPORTE rep = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporte.FOLIO).FirstOrDefault();
            string ruta = archivo.RUTA_ARCHIVO;
            ViewData["ruta"] = ruta;
            ViewData["rep"] = rep;
            return View();
        }

        public ActionResult RegistrarNotificacion(TBL_UNE_REPORTE reporte)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if(cl!=null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_REPORTE rep = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporte.FOLIO).FirstOrDefault();
                if(cl.Numusuario == rep.USUARIO_REGISTRA && rep.ID_ESTATUS_REPORTE == 4 || rep.reporte_banca ==true)
                {
                    ViewData["tipoCuentas"] = db.CAT_UNE_TIPO_CUENTA.ToList();
                    ViewData["cuentas"] = db.CAT_UNE_CUENTAS.Where(x => x.ID_TIPO_CUENTA == reporte.ID_TIPO_CUENTA).ToList();
                    ViewData["sucursales"] = funcion.ListaSucursales(cl.Usuario, cl.Contrasena);
                    ViewData["listaEntidades"] = funcion.ListaEntidades(cl.Usuario, cl.Contrasena);
                    ViewData["listaTipoReportes"] = funcion.obtenerTipoReporte(cl.Usuario, cl.Contrasena);
                    ViewData["listaMediosContacto"] = funcion.obtenerMediosContacto(cl.Usuario, cl.Contrasena);
                    ViewData["listaSupuestos"] = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_TIPO_REPORTE == rep.ID_TIPO_REPORTE).ToList();
                    ViewData["listaMedioMovimiento"] = bancaDB.CAT_CALLCENTER_MEDIO_MOVIMIENTO.ToList();
                    ViewData["listaTipoCuentaBanca"] = db.CAT_UNE_TIPO_CUENTA_BANCA.ToList();
                    ViewData["listaCanales"] = db.CAT_UNE_CANAL.ToList();
                    ViewData["listaMotivoCancelacion"] = db.CAT_UNE_MOTIVO_CANCELACION.ToList();
                    ViewData["listaProductos"] = db.CAT_UNE_PRODUCTO.ToList();
                    ViewData["listaResolucion"] = db.CAT_UNE_RESOLUCION.ToList();
                    ViewData["ListaCausaResolucion"] = db.CAT_UNE_CAUSA_RESOLUCION.ToList();
                    Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.ToString();

                    if (rep.folio_banca == null)
                        rep.folio_banca = 0;

                    return View(rep);
                }
                else
                    return RedirectToAction("Permiso", "Error");
            }
            else
                return RedirectToAction("Index", "Login");
        }

        public String ObtenerUltimaRespuesta(int folio)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            var can = db.SP_UNE_CARGA_ULTIMO_COMENTARIO(folio).FirstOrDefault();
            if (can != null)
                return can.COMENTARIOS;
            else
                return "";
        }

        public JsonResult obtenerMontos(int folio)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == folio).FirstOrDefault();
            decimal[] arr = new decimal[2];
            arr[0] = Convert.ToDecimal(reporte.IMPORTE_RECLAMACION);
            arr[1] = Convert.ToDecimal(reporte.IMPORTE_SOLUCION);
            return Json(arr);
        }

        [HttpPost]
        public ActionResult RegistrarDocumentoNotificar(TBL_UNE_REPORTE reporteInt)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {


                db = new ContextUne(cl.Usuario, cl.Contrasena);
                HttpPostedFileBase file = Request.Files[0];

                TBL_UNE_CANALIZACIONES canalizacion = new TBL_UNE_CANALIZACIONES();

                canalizacion.FOLIO = reporteInt.FOLIO;
                canalizacion.COMENTARIOS = reporteInt.Observaciones_cierre;
                canalizacion.fecha_alta = DateTime.Now;
                canalizacion.numusuario = cl.Numusuario;
                canalizacion.ID_TIPO_COMENTARIO = 6;
                db.TBL_UNE_CANALIZACIONES.Add(canalizacion);

                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporteInt.FOLIO).FirstOrDefault();
                reporte.ID_ESTATUS_REPORTE = 5;
                if (reporte.ID_TIPO_REPORTE == 3)
                    reporte.FECHA_ABONO = reporteInt.FECHA_ABONO;
                else
                    reporte.FECHA_ABONO = DateTime.Now;
                db.SaveChanges();

                List<TBL_UNE_USUARIOS_ASIGNADOS> list = db.TBL_UNE_USUARIOS_ASIGNADOS.Where(x => x.folio == reporte.FOLIO).ToList();
                string correoResponsable;

                var claves = db.CLAVES.Where(x => x.Numusuario == reporte.USUARIO_REGISTRA).Select(x => new { x.Correo }).FirstOrDefault();
                correoResponsable = claves.Correo;


                string[] CC = new string[list.Count];
                int i = 0;
                foreach (var item in list)
                {
                    if (item.numusuario == 0)
                    {
                        String correoMediosPago = ConfigurationSettings.AppSettings["CorreoMediosPago"];
                        CC[i] = correoMediosPago;
                    }
                    else
                    {
                        claves = db.CLAVES.Where(x => x.Numusuario == item.numusuario).Select(x => new { x.Correo }).FirstOrDefault();
                        CC[i] = claves.Correo;
                    }
                    i++;
                }

                int estatus = new Correo().EnviarFinalizar(correoResponsable, CC, Convert.ToInt32(reporte.NUM_FOLIO), reporteInt.Observaciones_cierre, Server.MapPath("~/Estilos/Imagenes/firmaUNE.jpg"), reporte);


                if (file.ContentLength > 0)
                {
                    TBL_UNE_REPORTE rep = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporte.FOLIO).FirstOrDefault();
                    TBL_UNE_ARCHIVOS_ADJUNTOS adjunto = new TBL_UNE_ARCHIVOS_ADJUNTOS();

                    Random rnd = new Random();
                    int rdn = rnd.Next(0, 10000);
                    String nombre = rdn + "_" + Path.GetFileName(file.FileName);
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
                    adjunto.ID_TIPO_ARCHIVO = 8;
                    adjunto.NUMUSUARIO = cl.Numusuario;
                    db.TBL_UNE_ARCHIVOS_ADJUNTOS.Add(adjunto);
                    db.SaveChanges();
                    file.SaveAs(archivo);
                }

                if (reporte.ID_MEDIO_CONTACTO == 1 && permisos.USUARIO_CALL_CENTER != true)
                {
                    int guardoArchivo = generaPDFRespuestaArea(reporte);
                    return RedirectToAction("pdf", "Notificar", reporte);
                }
                else
                    return RedirectToAction("ResueltoArea", "ResueltoArea");
            }
            else
                return RedirectToAction("Index", "Login");
        }

        public int generaPDFRespuestaArea(TBL_UNE_REPORTE reporte)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;

            int estatus = 1;
            TBL_UNE_ARCHIVOS_ADJUNTOS adjunto = new TBL_UNE_ARCHIVOS_ADJUNTOS();
            CAT_UNE_TIPO_REPORTE cat = db.CAT_UNE_TIPO_REPORTE.Where(x => x.ID_TIPO_REPORTE == reporte.ID_TIPO_REPORTE).FirstOrDefault();
            CAT_UNE_PROCEDE_DEBITO debito = db.CAT_UNE_PROCEDE_DEBITO.Where(x => x.ID_PROCEDE_DEBITO == reporte.ID_PROCEDE_DEBITO).FirstOrDefault();

            String dictamen = "";
            if (reporte.ID_CUENTA == 5)
            {
                if (reporte.ID_TIPO_REPORTE == 3 && reporte.IMPORTE_RECLAMACION>0)
                    dictamen = cat.DESCRIPCION + " " + debito.DESCRIPCION;
                else
                    dictamen = new CMV_CALLCENTER.Controllers.PreviewController().obtenerComentarioArea(reporte.FOLIO, cl.Usuario, cl.Contrasena);
            }
            else
                dictamen = new CMV_CALLCENTER.Controllers.PreviewController().obtenerComentarioArea(reporte.FOLIO, cl.Usuario, cl.Contrasena);

            string directorio = System.Environment.CurrentDirectory;
            string derecha = "style='text-align:right;'";
            string izq = "style='text-align:left;'";
            string css = "style='font-size:4.5px; color:white; text-align:center; font-weight:bold;'";
            string cabecera = "bgcolor='#6B8E23' style='font-weight:bold; text-align:center; color:white'";
            string nombre = "constancia de respuesta.pdf";
            string ruta = funcion.obtenerRuta(Convert.ToInt32(reporte.NUM_FOLIO));
            string datosGenerales = string.Empty;
            string rutaSql = "";
            string nombreSocio = reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO;
            directorio += @"\Estilos\Imagenes";

            DateTime fecha = DateTime.Now;
            String mes = funcion.ObtenerMes(fecha.Month);

            adjunto.FOLIO = reporte.FOLIO;
            adjunto.NOMBRE_ARCHIVO = nombre;
            adjunto.FECHA_ALTA = fecha;
            adjunto.ID_TIPO_ARCHIVO = 2;

            if (!System.IO.Directory.Exists(ruta))
            {
                System.IO.Directory.CreateDirectory(ruta);
            }

            try
            {
                datosGenerales = @"<html><head></head><body><img src='" + Path.Combine(directorio, "header.png") + @"' width = '125' height = '75' /> <br/><br/><P ALIGN=right>" + fecha.ToString("dd") + " de " + mes + " del " + fecha.Year + "</p>"
                    + "<br/><br/>"
                    + "<p ALIGN=left style='padding-left: 30px;padding-right: 30px;'>N° DE FOLIO <u><strong>" + reporte.NUM_FOLIO + "</strong></u></p><br /><br />"
                    + "<p ALIGN=left style='padding-left: 30px;padding-right: 30px;'>Estimado socio(a) " + nombreSocio + ":<br/><br/>"
                    + "<div style='border: black 2px solid;'><p>Mediante el presente escrito la Unidad Especializada de Caja Morelia Valladolid S.C. de A.P. de R.L. de C.V., da formal respuesta a su <u>"
                    + cat.DESCRIPCION
                    + "</u> realizada con fecha <u>" + fecha.ToString("dd") + " de " + mes + " del " + fecha.Year + "</u>, la cual es contestada dentro del término establecido por la Ley, aplicando el siguiente dictamen: "
                    + "<br/>"+ dictamen+"<br/>"
                    + "<br/><br/><br/><br/><br/><br/>"
                    + "<p ALIGN=center>Atentamente</p><br/><br/>"
                    + "<p ALIGN=center>_______________________________________</p><br/>"
                    + "<p ALIGN=center>Caja Morelia Valladolid S.C. de A.P. de R.L. de C.V.</p></p>"
                    + "</body>";

                ruta = ruta + "\\" + nombre;
                rutaSql = ruta;
                rutaSql = rutaSql.Replace("\\", "//").Replace("\"", "/");
                adjunto.RUTA_ARCHIVO = rutaSql;
                adjunto.NUMUSUARIO = cl.Numusuario;
                db.TBL_UNE_ARCHIVOS_ADJUNTOS.Add(adjunto);

                Document document = new Document(PageSize.LETTER, 0, 30, 2, 0);
                PdfWriter PDFWriter = PdfWriter.GetInstance(document, new FileStream(ruta, FileMode.Create));

                PDF eventos = new PDF();
                PDFWriter.PageEvent = eventos;

                document.Open();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(datosGenerales.ToString()), new StyleSheet()))
                {
                    document.Add(E);
                }
                document.AddAuthor("CMV_CALLCENTER");
                document.AddTitle("constancia de respuesta");
                document.AddCreator("CMV_CALLCENTER");
                document.AddKeywords("CAJA MORELIA");
                document.AddSubject("constancia de respuesta");
                document.CloseDocument();
                estatus = 1;
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return estatus;
        }

    }
}
