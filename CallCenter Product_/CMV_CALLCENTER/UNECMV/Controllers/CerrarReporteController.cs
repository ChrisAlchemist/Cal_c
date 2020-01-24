using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Models;
using CMV_CALLCENTER.Entidad;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.IO;
using System.Globalization;

namespace CMV_CALLCENTER.Controllers
{
    public class CerrarReporteController : Controller
    {
        private ContextUne db = new ContextUne();
        private Funciones funcion = new Funciones();
        private ContextBanca bancaDB = new ContextBanca();
        //
        // GET: /CerrarReporte/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CerrarReporte(TBL_UNE_REPORTE reporte)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                TBL_UNE_REPORTE reporteCerrar = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporte.FOLIO).FirstOrDefault();
                if((reporteCerrar.ID_ESTATUS_REPORTE == 1 && reporteCerrar.USUARIO_REGISTRA==cl.Numusuario) || (reporteCerrar.ID_ESTATUS_REPORTE==2 && (permisos.USUARIO_UNE==1 || permisos.USUARIO_CALL_CENTER==true)))
                {
                    ViewData["sucursales"] = funcion.ListaSucursales(cl.Usuario, cl.Contrasena);
                    ViewData["tipoCuentas"] = db.CAT_UNE_TIPO_CUENTA.ToList();
                    ViewData["cuentas"] = db.CAT_UNE_CUENTAS.Where(x => x.ID_TIPO_CUENTA == reporte.ID_TIPO_CUENTA).ToList();
                    ViewData["listaEntidades"] = funcion.ListaEntidades(cl.Usuario, cl.Contrasena);
                    ViewData["listaTipoReportes"] = funcion.obtenerTipoReporte(cl.Usuario, cl.Contrasena);
                    ViewData["listaMediosContacto"] = funcion.obtenerMediosContacto(cl.Usuario, cl.Contrasena);
                    ViewData["listaSupuestos"] = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_TIPO_REPORTE == reporteCerrar.ID_TIPO_REPORTE).ToList();
                    ViewData["listaCanales"] = db.CAT_UNE_CANAL.ToList();
                    ViewData["listaMotivoCancelacion"] = db.CAT_UNE_MOTIVO_CANCELACION.ToList();
                    ViewData["listaProductos"] = db.CAT_UNE_PRODUCTO.ToList();
                    ViewData["listaResolucion"] = db.CAT_UNE_RESOLUCION.ToList();
                    ViewData["ListaCausaResolucion"] = db.CAT_UNE_CAUSA_RESOLUCION.ToList();
                    Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.ToString();
                    ViewData["listaMedioMovimiento"] = bancaDB.CAT_CALLCENTER_MEDIO_MOVIMIENTO.ToList();
                    if (reporteCerrar.reporte_banca == false)
                        reporteCerrar.folio_banca = 0;
                    return View(reporteCerrar);
                }
                else
                    return RedirectToAction("Permiso", "Error");

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

        [HttpPost]
        public ActionResult Cerrar(TBL_UNE_REPORTE reporteCerrar)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {


                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporteCerrar.FOLIO).FirstOrDefault();

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
                    adjunto.ID_TIPO_ARCHIVO = 5;

                    adjunto.NUMUSUARIO = cl.Numusuario;
                    db.TBL_UNE_ARCHIVOS_ADJUNTOS.Add(adjunto);
                    db.SaveChanges();
                    file.SaveAs(archivo);
                }

                DateTime ahora = DateTime.Now;
                reporte.Observaciones_cierre = reporteCerrar.Observaciones_cierre;
                reporte.Fecha_cierre = ahora;

                if (reporte.IMPORTE_RECLAMACION != null)
                {
                    reporte.IMPORTE_SOLUCION = reporteCerrar.IMPORTE_SOLUCION;
                }


                if (reporte.reporte_banca == false)
                    reporte.ID_ESTATUS_REPORTE = 5;
                else
                {
                    reporte.ID_ESTATUS_REPORTE = 6;
                    reporte.ID_SATISFACTORIO = 1;
                }



                reporte.ID_RESOLUCION = reporteCerrar.ID_RESOLUCION;
                reporte.ID_CAUSA_RESOLUCION = reporteCerrar.ID_CAUSA_RESOLUCION;

                if (reporte.ID_TIPO_REPORTE == 3)
                    reporte.FECHA_ABONO = reporteCerrar.FECHA_ABONO;
                else
                    reporte.FECHA_ABONO = DateTime.Now;

                TBL_UNE_CANALIZACIONES canalizacion = new TBL_UNE_CANALIZACIONES();
                canalizacion.FOLIO = reporte.FOLIO;
                canalizacion.fecha_alta = DateTime.Now;
                canalizacion.COMENTARIOS = reporteCerrar.Observaciones_cierre;
                canalizacion.numusuario = reporte.USUARIO_REGISTRA;
                canalizacion.ID_TIPO_COMENTARIO = 9;
                db.TBL_UNE_CANALIZACIONES.Add(canalizacion);
                db.SaveChanges();
                int res = new Correo().EnviarNotificacion("", Convert.ToInt32(reporte.NUM_FOLIO), reporteCerrar.Observaciones_cierre, Server.MapPath("~/Estilos/Imagenes/firmaUNE.jpg"), reporte);
                if (reporte.reporte_banca == false)
                {
                    String rutaArchivo = generaPDFRespuestaArea(reporte, reporte.Observaciones_cierre);
                }

                if (reporte.reporte_banca == true)
                {
                    return RedirectToAction("Finalizados", "Finalizados");
                }
                else
                    return RedirectToAction("pdf", "CerrarReporte", reporte);

            }
            else
                return RedirectToAction("Index", "Login");
        }


        public String generaPDFRespuestaArea(TBL_UNE_REPORTE reporte, string comentariosCierre)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;

            int estatus = 1;
            TBL_UNE_ARCHIVOS_ADJUNTOS adjunto = new TBL_UNE_ARCHIVOS_ADJUNTOS();
            CAT_UNE_TIPO_REPORTE cat = db.CAT_UNE_TIPO_REPORTE.Where(x => x.ID_TIPO_REPORTE == reporte.ID_TIPO_REPORTE).FirstOrDefault();

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
                    + "<br/>" + comentariosCierre + "<br/><br/><br/>"
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
                document.Close();
                estatus = 1;
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return rutaSql;
        }
    }
}
