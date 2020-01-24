using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Entidad;
using CMV_CALLCENTER.Models;
using System.Web.UI.WebControls;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Web.UI;
using ClosedXML;
using ClosedXML.Excel;
using System.Data;
using System.Runtime.InteropServices;
using System.Configuration;
using SpreadsheetLight;

namespace CMV_CALLCENTER.Controllers
{
    public class CondusefController : Controller
    {
        private ContextUne db = new ContextUne();
        private Funciones funcion = new Funciones();
        //
        // GET: /Condusef/

        public ActionResult Condusef()
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if(cl!=null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                ViewData["tipoReporte"] = db.CAT_UNE_TIPO_REPORTE.ToList();
                ViewData["sucursales"] = funcion.ListaSucursales(cl.Usuario, cl.Contrasena);
                ViewData["listaTipoReportes"] = funcion.obtenerTipoReporte(cl.Usuario, cl.Contrasena);
                ViewData["estatusReporte"] = db.CAT_UNE_ESTATUS_REPORTE.ToList();
                Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.ToString();
                return View();
            }
            else
                return RedirectToAction("Index", "Login");
        }

        public ActionResult GeneraReporte()
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                if(permisos.PERMISO_CONDUSEF == 1)
                {
                    ViewData["tipoReporte"] = db.CAT_UNE_TIPO_REPORTE.ToList();
                    ViewData["sucursales"] = funcion.ListaSucursales(cl.Usuario, cl.Contrasena);
                    ViewData["estatusReporte"] = db.CAT_UNE_ESTATUS_REPORTE.ToList();
                    ViewData["listaTipoReportes"] = funcion.obtenerTipoReporte(cl.Usuario, cl.Contrasena);
                    ViewData["entidades"] = db.ENTIDAD_FEDERATIVA.Where(x=>x.ID_ENTIDAD_FEDERATIVA>0).ToList();
                    Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.ToString();
                    return View();
                }
                else
                    return RedirectToAction("Permiso", "Error");

            }
            else
                return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public JsonResult GeneraListado(int tipoReporte, int estatus, int subEstatus, int sucursalRegistro, int sucursalSocio, DateTime fechaInicio, DateTime FechaFin)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne();
            try
            {
                String fecha1 = fechaInicio.ToString("yyyyMMdd");
                String fecha2 = FechaFin.ToString("yyyyMMdd");
                var listado = db.SP_UNE_CARGAR_REPORTE(fecha1, fecha2, tipoReporte, estatus, subEstatus, sucursalRegistro, sucursalSocio, cl.Numusuario).ToList();

                return Json( listado );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult generaExcel(DateTime fechaInicio, DateTime fechaFin, int contador)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne();
            String archivo = ConfigurationSettings.AppSettings["rutaArchivosEvidencias"];
            archivo = archivo+"\\" + "reporte_" + cl.Numusuario + ".xlsx";
            Microsoft.Office.Interop.Excel.Application aplicacion;
            Workbook libros_trabajo;
            Worksheet hoja_trabajo;
            aplicacion = new Microsoft.Office.Interop.Excel.Application();
            libros_trabajo = aplicacion.Workbooks.Add();
            hoja_trabajo = (Worksheet)libros_trabajo.Worksheets.get_Item(1);

            String fecha1 = fechaInicio.ToString("yyyyMMdd");
            String fecha2 = fechaFin.ToString("yyyyMMdd");
            int renglon = 3, contConsultas = 0, contReclamaciones = 0, contAclaraciones = 0;

            GridView gridview = new GridView();
            gridview.DataSource = db.TBL_UNE_PIVOTE_REPORTE.Where(x => x.CONTADOR == contador && x.NUMUSUARIO_REPORTE == cl.Numusuario).ToList();
            gridview.DataBind();

            hoja_trabajo.Cells[1, 1] = "SOCOP ";
            hoja_trabajo.Cells[2, 1] = "PERIODO SELECCIONADO: ";
            hoja_trabajo.Cells[2, 2] = fecha1.Substring(6, 2) + "/" + fecha1.Substring(4, 2) + "/" + fecha1.Substring(0, 4) + " al " + fecha2.Substring(6, 2) + "/" + fecha2.Substring(4, 2) + "/" + fecha2.Substring(0, 4);

            foreach (GridViewRow row in gridview.Rows)
            {
                switch(row.Cells[22].Text)
                {
                    case "ACLARACION":
                        contAclaraciones++;
                        break;
                    case "CONSULTA":
                        contConsultas++;
                        break;
                    case "RECLAMACION":
                        contReclamaciones++;
                        break;
                }
            }

            if(contConsultas>0)
            {
                hoja_trabajo.Cells[renglon, 1] = "Número de consultas: ";
                hoja_trabajo.Cells[renglon, 2] = contConsultas;
                renglon++;
            }

            if (contReclamaciones > 0)
            {
                hoja_trabajo.Cells[renglon, 1] = "Número de Reclamaciones: ";
                hoja_trabajo.Cells[renglon, 2] = contReclamaciones;
                renglon++;
            }

            if (contAclaraciones > 0)
            {
                hoja_trabajo.Cells[renglon, 1] = "Número de Aclaraciones: ";
                hoja_trabajo.Cells[renglon, 2] = contAclaraciones;
                renglon++;
            }

            renglon++;

            for (int i = 4; i <= gridview.HeaderRow.Cells.Count; i++)
            {
                hoja_trabajo.Cells[renglon, i-3] = gridview.HeaderRow.Cells[i - 1].Text;
            }
            renglon++;

            foreach (GridViewRow row in gridview.Rows)
            {
                for (int i = 4; i <= gridview.HeaderRow.Cells.Count; i++)
                {
                    if (i == 5)
                    {
                        if (row.Cells[i - 1].Text == "1")
                        {
                            hoja_trabajo.Cells[renglon, i-3] = "Socio";
                        }
                        else if (row.Cells[i - 1].Text == "2")
                        {
                            hoja_trabajo.Cells[renglon, i - 3] = "No Socio";
                        }
                        else if (row.Cells[i - 1].Text == "3")
                        {
                            hoja_trabajo.Cells[renglon, i - 3] = "Persona Moral";
                        }
                    }
                    else if (i == 18)
                    {
                        if (row.Cells[i - 1].Text == "0.0000")
                            hoja_trabajo.Cells[renglon, i - 3] = "N/A";
                        else
                            hoja_trabajo.Cells[renglon, i - 3] = row.Cells[i - 1].Text.Replace("&nbsp;", "");
                    }
                    else if (i == 19)
                    {
                        if (row.Cells[i - 1].Text == "0.0000")
                            hoja_trabajo.Cells[renglon, i - 3] = "N/A";
                        else
                            hoja_trabajo.Cells[renglon, i - 3] = row.Cells[i - 1].Text.Replace("&nbsp;", "");
                    }
                    else
                        hoja_trabajo.Cells[renglon, i - 3] = row.Cells[i - 1].Text.Replace("&nbsp;", "");
                }
                renglon = renglon + 1;

            }

            Range finalCell = hoja_trabajo.Cells[1, hoja_trabajo.UsedRange.Columns.Count];
            Range rng = hoja_trabajo.get_Range("A2", finalCell);
            //rng.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.ForestGreen);
            hoja_trabajo.UsedRange.Columns.AutoFit();

            if (System.IO.File.Exists(archivo))
            {
                System.IO.File.Delete(archivo);
            }

            libros_trabajo.SaveAs(archivo, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);
            libros_trabajo.Close(true);

            int estatus = new Correo().descargaArchivo(cl.Correo, archivo,"");

            string[] Datos = new string[2];

            Datos[0] = estatus.ToString();
            Datos[1] = archivo;

            //using (XLWorkbook wb = new XLWorkbook())
            //{
            //    wb.Worksheets.Add(dt);
            //    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //    wb.Style.Font.Bold = true;

            //    Response.Clear();
            //    Response.Buffer = true;
            //    Response.Charset = "";
            //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //    Response.AddHeader("content-disposition", "attachment;filename= reporte"+cl.Numusuario+".xlsx");

            //    using (MemoryStream MyMemoryStream = new MemoryStream())
            //    {
            //        wb.SaveAs(MyMemoryStream);
            //        MyMemoryStream.WriteTo(Response.OutputStream);
            //        Response.Flush();
            //        Response.End();
            //    }
            //}

            return Json(Datos);
        }

        [HttpPost]
        public ActionResult GenerarReporteListado(TBL_UNE_REPORTE reporte)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
            db = new ContextUne(cl.Usuario,cl.Contrasena);
            try
            {
                //Se utilizo una variable String, pues generaba errores la conversion de fechas con dias mayores a 12
                String fecha1 = reporte.NOMBRE_S.Substring(6,4)+ reporte.NOMBRE_S.Substring(3, 2)+ reporte.NOMBRE_S.Substring(0, 2);
                String fecha2 = reporte.APELLIDO_PATERNO.Substring(6, 4) + reporte.APELLIDO_PATERNO.Substring(3, 2) + reporte.APELLIDO_PATERNO.Substring(0, 2);

                //var listado = db.SP_UNE_CARGAR_REPORTE(fecha1, fecha2, reporte.ID_TIPO_REPORTE, reporte.ID_CUENTA, reporte.ID_ESTATUS_REPORTE, reporte.ID_SUCURSAL_REGISTRO, reporte.ENTIDAD, cl.Numusuario).ToList();
                var listado = db.SP_UNE_CARGAR_REPORTE_2701(fecha1, fecha2, reporte.ID_TIPO_REPORTE, reporte.ID_CUENTA, reporte.ID_ESTATUS_REPORTE, reporte.ID_SUCURSAL_REGISTRO, reporte.ENTIDAD, cl.Numusuario,reporte.VoBo).ToList();
                String rutaArchivoCompleta = AppDomain.CurrentDomain.BaseDirectory + "prueba.xlsx";

                if (listado.Count > 0)
                {
                    String archivo = ConfigurationSettings.AppSettings["rutaArchivosEvidencias"];
                    //DateTime dtFecha2 = Convert.ToDateTime(reporte.APELLIDO_PATERNO);
                    //String rutaLayout27 =   System.Configuration.ConfigurationManager.AppSettings["rutaArchivosSARR"].ToString() + @"\" +
                    //                        dtFecha2.ToString("yyyy") + @"\" +
                    //                        dtFecha2.ToString("MMMM") + @"\" +
                    //                        dtFecha2.ToString("yyyy") + "_" + dtFecha2.ToString("MM") + "_" + "R27_A_2701.txt";

                    String rutaLayout27 = System.Configuration.ConfigurationManager.AppSettings["rutaArchivosSARR"].ToString() + @"\" +
                                            reporte.APELLIDO_PATERNO.Substring(6, 4) + @"\" +
                                            funcion.ObtenerMes(Convert.ToInt32(reporte.APELLIDO_PATERNO.Substring(3, 2))) + @"\" +
                                            reporte.APELLIDO_PATERNO.Substring(6, 4) + "_" + reporte.APELLIDO_PATERNO.Substring(3, 2) + "_" + "R27_A_2701.txt";

                    new Funciones().crearCarpetas(  System.Configuration.ConfigurationManager.AppSettings["rutaArchivosSARR"].ToString(),
                                                    funcion.ObtenerMes(Convert.ToInt32(reporte.APELLIDO_PATERNO.Substring(3, 2))),
                                                    reporte.APELLIDO_PATERNO.Substring(6, 4));

                    archivo = archivo + "\\" + "reporte_" + cl.Numusuario + ".xlsx";
                    SLDocument sl = new SLDocument();

                    int renglon = 1, contConsultas = 0, contReclamaciones = 0, contAclaraciones = 0;
                    int contador = Convert.ToInt32(listado[0].CONTADOR);

                    GridView gridview = new GridView();
                    gridview.DataSource = listado;//db.TBL_UNE_PIVOTE_REPORTE.Where(x => x.CONTADOR == contador && x.NUMUSUARIO_REPORTE == cl.Numusuario).ToList();
                    gridview.DataBind();

                    //sl.SetCellValue(1, 1, "SOCOP");
                    //sl.SetCellValue(2, 1, "Periodo seleccionado: ");
                    //sl.SetCellValue(2, 2, fecha1.Substring(0, 4)+ "/" + fecha1.Substring(4, 2) + "/" + fecha1.Substring(6, 2) + " al " + fecha2.Substring(0, 4) + "/" + fecha2.Substring(4, 2) + "/" + fecha2.Substring(6, 2));

                    //foreach (GridViewRow row in gridview.Rows)
                    //{
                    //    switch (HttpUtility.HtmlDecode(row.Cells[22].Text))
                    //    {
                    //        case "Aclaración":
                    //            contAclaraciones++;
                    //            break;
                    //        case "Consulta":
                    //            contConsultas++;
                    //            break;
                    //        case "Reclamación":
                    //            contReclamaciones++;
                    //            break;
                    //    }
                    //}

                    //if (contConsultas > 0)
                    //{
                    //    sl.SetCellValue(renglon, 1, "Número de consultas: ");
                    //    sl.SetCellValue(renglon, 2, contConsultas);
                    //    renglon++;
                    //}

                    //if (contReclamaciones > 0)
                    //{
                    //    sl.SetCellValue(renglon, 1, "Número de Reclamaciones: ");
                    //    sl.SetCellValue(renglon, 2, contReclamaciones);
                    //    renglon++;
                    //}

                    //if (contAclaraciones > 0)
                    //{
                    //    sl.SetCellValue(renglon, 1, "Número de Aclaraciones: ");
                    //    sl.SetCellValue(renglon, 2, contAclaraciones);
                    //    renglon++;
                    //}

                    //renglon++;
                    String encabezado="";
                    for (int i = 2; i < gridview.HeaderRow.Cells.Count; i++)
                    {
                        if( i ==2)
                        {
                            encabezado = "PERIODO QUE SE REPORTA";
                        }
                        else if (i == 6)
                            encabezado = "FOLIO DE RECLAMACION";
                        else if (i == 9)
                            encabezado = "NUMERO DE CUENTA";
                        else
                        {
                            encabezado = gridview.HeaderRow.Cells[i].Text;
                            encabezado = encabezado.Replace('_', ' ');
                        }
                        sl.SetCellValue(renglon, i-1, encabezado);
                    }

                    renglon++;
                    int pos = 0;
                    foreach (GridViewRow row in gridview.Rows)
                    {
                        for (int i = 2; i < gridview.HeaderRow.Cells.Count; i++)
                        {
                            //if(i == 5)
                            //{
                            //    DateTime dt = Convert.ToDateTime(listado[pos].FECHA_ALTA);
                            //    String formato = String.Format("{0:yyyy/MM/dd}", dt);
                            //    sl.SetCellValue(renglon, i - 3, formato);
                            //}
                            //else if (i == 6)
                            //{
                            //    if (row.Cells[i - 1].Text == "1")
                            //    {
                            //        sl.SetCellValue(renglon, i - 3, "Socio");
                            //    }
                            //    else if (row.Cells[i - 1].Text == "2")
                            //    {
                            //        sl.SetCellValue(renglon, i - 3, "Usuario");
                            //    }
                            //    else if (row.Cells[i - 1].Text == "3")
                            //    {
                            //        sl.SetCellValue(renglon, i - 3, "Persona Moral");
                            //    }
                            //}
                            //else if (i == 13)
                            //{
                            //    if (row.Cells[i - 1].Text == "01/01/1900 12:00:00 a.m." || row.Cells[i - 1].Text ==  "1/1/1900 12:00:00 AM" || row.Cells[i - 1].Text == "01/01/1900  12:00:00 a.m.")
                            //        sl.SetCellValue(renglon, i - 3, "");
                            //    else
                            //    {
                            //        DateTime dt = Convert.ToDateTime(listado[pos].FECHA_RESOLUCION);
                            //        String formato = String.Format("{0:yyyy/MM/dd}", dt);
                            //        sl.SetCellValue(renglon, i - 3, formato);
                            //    }
                            //}
                            //else if (i == 14)
                            //{
                            //    if (row.Cells[i - 1].Text == "01/01/1900 12:00:00 a.m." || row.Cells[i - 1].Text == "1/1/1900 12:00:00 AM" || row.Cells[i - 1].Text == "01/01/1900  12:00:00 a.m.")
                            //        sl.SetCellValue(renglon, i - 3, "");
                            //    else
                            //    {
                            //        DateTime dt = Convert.ToDateTime(listado[pos].FECHA_NOTIFICACION);
                            //        String formato = String.Format("{0:yyyy/MM/dd}", dt);
                            //        sl.SetCellValue(renglon, i - 3, formato);
                            //    }
                            //}
                            //else if (i == 18)
                            //{
                            //    if (row.Cells[i - 1].Text == "0.0000")
                            //        sl.SetCellValue(renglon, i - 3, "N/A");
                            //    else
                            //        sl.SetCellValue(renglon, i - 3, HttpUtility.HtmlDecode(row.Cells[i - 1].Text.Replace("&nbsp;", "")));
                            //}
                            //else if (i == 19)
                            //{
                            //    if (row.Cells[i - 1].Text == "0.0000")
                            //        sl.SetCellValue(renglon, i - 3, "N/A");
                            //    else
                            //        sl.SetCellValue(renglon, i - 3, HttpUtility.HtmlDecode(row.Cells[i - 1].Text.Replace("&nbsp;", "")));
                            //}
                            //else
                            if (i == 3)
                            {
                                String clvBanco = "'0"+row.Cells[i].Text;
                                sl.SetCellValue(renglon, i - 1, clvBanco);

                            }
                            else if(i==13 || i==18 || i == 20 || i == 21)
                            {
                                String saldo = row.Cells[i].Text;
                                saldo = saldo.Substring(0, saldo.Length - 2);
                                sl.SetCellValue(renglon, i-1, HttpUtility.HtmlDecode(saldo.ToString()));
                            }
                            else
                                sl.SetCellValue(renglon, i-1, HttpUtility.HtmlDecode(row.Cells[i].Text.Replace("&nbsp;", "")));
                        }
                        renglon = renglon + 1;
                        pos = pos +1;
                    }

                    if (System.IO.File.Exists(archivo))
                    {
                        System.IO.File.Delete(archivo);
                    }

                    sl.SaveAs(archivo);

                    ////////////  Modificaciones Reporte27 (layout)  ////////////////////////
                    List<int> quitarCols = new List<int>();
                    quitarCols.Add(0); // contador
                    quitarCols.Add(1); // numusuario_reporte
                    quitarCols.Add(2); // periodo_reporte
                    quitarCols.Add(3); // clave_institucion
                    quitarCols.Add(4); // reporte
                    new Funciones().generaLayout(gridview, rutaLayout27, quitarCols, "2701");
                    /////////////////////////////////////////////////////////////////////////

                    int estatus = new Correo().descargaArchivo(cl.Correo, archivo,Server.MapPath("~/Estilos/Imagenes/firmaUNE.jpg"));

                    if(permisos.ID_ROL == 330)
                    {
                        int estatusLayout = new Correo().descargaArchivo(cl.Correo, rutaLayout27, Server.MapPath("~/Estilos/Imagenes/firmaUNE.jpg"));
                    }

                }

                return Json(listado.Count);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
