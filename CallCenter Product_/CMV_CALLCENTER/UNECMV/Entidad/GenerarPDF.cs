using CMV.BancaAdmin.UTILS;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using CMV_CALLCENTER.Models;

namespace CMV_CALLCENTER.Entidad
{
    public class GenerarPDF
    {

        #region propiedades  css


        private static int i = 0;
        private static string css = "style='font-size:10px;font-family:Arial; color:#3E3E3E''";
        private static string negritasYCentradas = "style='text-align:center; font-weight:bold;'";
        private static string titulosCabeceras = "style='font-weight:bold;  color:3b3b3b;text-align:right'";
        private static string filaTabla1 = "bgcolor='3b3b3b' style='color:white;text-align:center; font-size:7px;'";
        private static string filaTabla2 = "style='color:black;text-align:center;font-size:7px;'";
        private static string centradas = "style='text-align:center;font-size:7px;'";
        private static string derecha = "style='text-align:right;'";
        private static string izq = "style='text-align:left;'";
        private static string cabeceraHeader = "bgcolor='#77b221' style='font-weight:bold; text-align:right; color:white'";
        private static string cabeceraTablas = "bgcolor='#404040' style='font-weight:bold; text-align:center; color:white'";
        private static string cssTabla = @"style='text-align:justify;font-size:9.5px;font-family:Arial; color:#3E3E3E'";
        private static string cssLeyenda = "style='color:7b7b7b;text-align:justify;font-size:10px;'";
        private static string letrasPeques = "style='text-align:top;font-size:7.5px;font-family:Arial; color:7b7b7b'";
        private static string fuenteGeneral = "style='font-size:8px; font-family:Century Gothic;'";
        private static string fuenteGeneralCentrada = "style ='text-align:center; font-size:8px; font-family:Century Gothic;'";
        private static string fuenteGeneralDerecha = "style ='text-align:right; font-size:8px; font-family:Century Gothic;'";
        private static string fuenteFoterCancelar = "style='font-size:8px; font-family:Century Gothic; text-align:center;'";


        #endregion

        private ContextUneBD dbHape = new ContextUneBD();
        private ContextUne uneDB = new ContextUne();
        private Funciones funcion = new Funciones();
        private ContextBanca bancaDB = new ContextBanca();



        public string formatoAcionesNoReconocidas(TBL_UNE_REPORTE reporte, CLAVES cl,string pathFormatosBanca)
        {
            if (!Directory.Exists(pathFormatosBanca))
            {
                Directory.CreateDirectory(pathFormatosBanca);
            }

            SP_BANCA_OBTIENE_SOCIO_Result DatosSocio = uneDB.SP_BANCA_OBTIENE_SOCIO(reporte.NUMERO, cl.Numusuario, 1).FirstOrDefault();

            CAT_UNE_MEDIO_CONTACTO medioContacto = uneDB.CAT_UNE_MEDIO_CONTACTO.Where(x => x.ID_MEDIO_CONTACTO == reporte.ID_MEDIO_CONTACTO).First();
            CAT_UNE_TIPO_REPORTE tipoReporte = uneDB.CAT_UNE_TIPO_REPORTE.Where(x => x.ID_TIPO_REPORTE == reporte.ID_TIPO_REPORTE).First();
            CAT_UNE_TIPO_CUENTA tipoOperacion = uneDB.CAT_UNE_TIPO_CUENTA.Where(x => x.ID_TIPO_CUENTA == reporte.ID_TIPO_CUENTA).First();
            CAT_UNE_CUENTAS operacion = uneDB.CAT_UNE_CUENTAS.Where(x => x.ID_CUENTA == reporte.ID_CUENTA).First();
            CAT_UNE_SUPUESTOS_REPORTE causaReporte = uneDB.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).First();

            var sucursal = uneDB.SUCURSALES.Where(x => x.Id_de_Sucursal == reporte.ID_DE_SUCURSAL).FirstOrDefault();
            string nombreFile = "FormatoAccionNoReconocida" + reporte.NUMERO + ".pdf";
            string pathWebFile = WebConfigurationManager.AppSettings["pathFormatosBanca"].ToString() + nombreFile;
            string file = Path.Combine(pathFormatosBanca, nombreFile);

            

            Document document = new Document(PageSize.A4, 30, 30, 30, 110);
            //Document document = new Document(PageSize.A4, 30, 30, 30, 110);
            PdfWriter PDFWriter = PdfWriter.GetInstance(document, new FileStream(file, FileMode.Create));
            ItextEvents eventos = new ItextEvents();
            eventos.TituloCabecera = "ACLARACION DE ACCIONES NO RECONOCIDAS DE CMV FINANZAS";
            PDFWriter.PageEvent = eventos;
            try
            {
                DateTime fechaActual = System.DateTime.Now;
                DateTimeFormatInfo formatoFecha = new CultureInfo("es-ES", false).DateTimeFormat;
                string nombreMes = formatoFecha.GetMonthName(fechaActual.Month).ToUpper();
                //string html = "<p align='center' " + css.Replace("10", "12") + "><b>" + eventos.TituloCabecera + @"</b></p><br/><br/><br/><br/>";
                string html = "";
                string tipoBloqueo = "";



                html += @"
                    <table>
                        <tr>
                            <td " + fuenteGeneral + " >" + sucursal.Descripcion + @"</td>
                            <td></td>
                            <td " + fuenteGeneral + ">" + fechaActual.Day.ToString() + " de " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nombreMes.ToLower()) + " del " + fechaActual.Year.ToString() + " " + fechaActual.ToShortTimeString() + @"</td>
                        </tr>
                    </table>
                    <br>";

                string html1 = @"
                    
                            <p " + fuenteGeneral + " >Número de socio: <i><u>" + reporte.NUMERO + @"</u></i></p>
                            <p " + fuenteGeneral + "  aling = 'right'>Nombre del socio: <i><u>" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(reporte.NOMBRE_S + " "+ reporte.APELLIDO_PATERNO +" "+ reporte.APELLIDO_MATERNO) + "</u></i></p>" + @"
                            <br><br>";
                html1 += @"

                            <table>
                                <tr>
                                    <td " + cabeceraTablas + @">
                                        <p " + fuenteGeneralCentrada + @">DATOS DEL REPORTE</p>
                                    </td>
                                </tr>
                            </table>
                            <br>
                            <br>
                            <table>                                
                                <tr>
                                    <td " + fuenteGeneral + " >" + 
                                        @"<p " + fuenteGeneral + " >Folio CMV Finanzas: <i><u>" + reporte.folio_banca + @"</u></i></p> <br>" + @" </td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                
                                <tr>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Medio de contacto: <i><u>" + medioContacto.DESCRIPCION + @"</u></i></p>" + @" 
                                    </td>
                                    <td></td>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + "  aling = 'right'>Tipo de reporte: <i><u>" + tipoReporte.DESCRIPCION + "</u></i></p><br>" + @" 
                                    </td>
                                    
                                </tr>
                                
                                <tr>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Tipo de operación: <i><u>" + tipoOperacion.DESCRIPCION + @"</u></i></p>" + @" 
                                    </td>
                                    <td></td>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + "  aling = 'right'>Operación: <i><u>" + operacion.DESCRIPCION + "</u></i></p><br>" + @" 
                                    </td>
                                </tr>
                                
                                <tr>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Causa del reporte: <i><u>" + causaReporte.DESCRIPCION + @"</u></i></p> <br>" + @"
                                    <td></td>
                                    <td></td>
                                </tr>

                            </table>
                            <p " + fuenteGeneral + " >Descripción del reporte: <i><u>" + reporte.DESCRIPCION_REPORTE + @"</u></i></p>" + @" </td>
                            <br><br>";

                string html2 = @"
                        <table>
                            
                            <tr>
                                <td>
                                    " + tipoBloqueo + @"
                                </td>
                            </tr>
                        </table>
                        <br><br><br><br><br>
                        <table>
                            <tr>
                                <td " + fuenteGeneralCentrada + @">AUTORIZACIÓN DEL SOCIO</td> 
                                <td></td>
                                <td " + fuenteGeneralCentrada + @">EJECUTIVO/GERENTE QUE TRAMITÓ EL REPORTE</td>
                            </tr>
                            <tr>
                                <td>
                                    <br><br>
                                    <p>__________________________</p>
                                    <p " + fuenteGeneralCentrada + @">Nombre y firma del socio</p>
                                </td>
                                <td></td>
                                <td>
                                    <br><br>
                                    <p>__________________________</p>
                                    <p " + fuenteGeneralCentrada + @" >Nombre y firma del Ejecutivo/Gerente</p>
                                </td>
                            </tr>
                        </table>
                        <br><br>
                        ";

                html += html1 + html2;

                //html = "<h1>Formato para solicitar la Domiciliación</h1>";


                document.Open();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(html.ToString()), new StyleSheet()))
                {
                    document.Add(E);
                }


                document.AddAuthor("Caja Morelia Valladolid");
                document.AddTitle("Formato de acciones no reconocidas de “CMV Finanzas”");
                document.AddCreator("Cristian Perez Garcia");
                document.AddKeywords("Gererencia de TI");
                document.AddSubject("Formato de acciones no reconocidas en “CMV Finanzas”");
                document.CloseDocument();
                document.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return pathWebFile;
        }

        public string formatoOperacionesNoAplicadas(TBL_UNE_REPORTE reporte, CLAVES cl, string pathFormatosBanca, RequestForOpeNoAplicadas requestForOpeNoAplicadas_)
        {
            if (!Directory.Exists(pathFormatosBanca))
            {
                Directory.CreateDirectory(pathFormatosBanca);
            }

            SP_BANCA_OBTIENE_SOCIO_Result DatosSocio = uneDB.SP_BANCA_OBTIENE_SOCIO(reporte.NUMERO, cl.Numusuario, 1).FirstOrDefault();

            CAT_UNE_MEDIO_CONTACTO medioContacto = uneDB.CAT_UNE_MEDIO_CONTACTO.Where(x => x.ID_MEDIO_CONTACTO == reporte.ID_MEDIO_CONTACTO).First();
            CAT_UNE_TIPO_REPORTE tipoReporte = uneDB.CAT_UNE_TIPO_REPORTE.Where(x => x.ID_TIPO_REPORTE == reporte.ID_TIPO_REPORTE).First();
            CAT_UNE_TIPO_CUENTA tipoOperacion = uneDB.CAT_UNE_TIPO_CUENTA.Where(x => x.ID_TIPO_CUENTA == reporte.ID_TIPO_CUENTA).First();
            CAT_UNE_CUENTAS operacion = uneDB.CAT_UNE_CUENTAS.Where(x => x.ID_CUENTA == reporte.ID_CUENTA).First();
            CAT_UNE_SUPUESTOS_REPORTE causaReporte = uneDB.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).First();

            var sucursal = uneDB.SUCURSALES.Where(x => x.Id_de_Sucursal == reporte.ID_DE_SUCURSAL).FirstOrDefault();
            string nombreFile = "FormatoOperacionNoAplicada" + reporte.NUMERO + ".pdf";
            string pathWebFile = WebConfigurationManager.AppSettings["pathFormatosBanca"].ToString() + nombreFile;
            string file = Path.Combine(pathFormatosBanca, nombreFile);

            

            Document document = new Document(PageSize.A4, 30, 30, 30, 110);
            //Document document = new Document(PageSize.A4, 30, 30, 30, 110);
            PdfWriter PDFWriter = PdfWriter.GetInstance(document, new FileStream(file, FileMode.Create));
            ItextEvents eventos = new ItextEvents();
            eventos.TituloCabecera = "OPERACIONES NO RECONOCIDAS/NO APLICADAS DE CMV FINANZAS";
            PDFWriter.PageEvent = eventos;
            try
            {
                DateTime fechaActual = System.DateTime.Now;
                DateTimeFormatInfo formatoFecha = new CultureInfo("es-ES", false).DateTimeFormat;
                string nombreMes = formatoFecha.GetMonthName(fechaActual.Month).ToUpper();
                //string html = "<p align='center' " + css.Replace("10", "12") + "><b>" + eventos.TituloCabecera + @"</b></p><br/><br/><br/><br/>";
                string html = "";
                string tipoBloqueo = "";



                html += @"
                    <table>
                        <tr>
                            <td " + fuenteGeneral + " >" + sucursal.Descripcion + @"</td>
                            <td></td>
                            <td " + fuenteGeneral + ">" + fechaActual.Day.ToString() + " de " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nombreMes.ToLower()) + " del " + fechaActual.Year.ToString() + " " + fechaActual.ToShortTimeString() + @"</td>
                        </tr>
                    </table>
                    <br>";

                string html1 = @"
                    
                            <p " + fuenteGeneral + " >Número de socio: <i><u>" + reporte.NUMERO + @"</u></i></p>
                            <p " + fuenteGeneral + "  aling = 'right'>Nombre del socio: <i><u>" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO) + "</u></i></p>" + @"
                            <br>";
                html1 += @"

                            <table>
                                <tr>
                                    <td " + cabeceraTablas + @">
                                        <p " + fuenteGeneralCentrada + @">DATOS DEL REPORTE</p>
                                    </td>
                                </tr>
                            </table>
                            <br>
                            
                            <table>                                
                                <tr>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Folio CMV Finanzas: <i><u>" + reporte.folio_banca + @"</u></i></p>" + @" </td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                
                                <tr>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Medio de contacto: <i><u>" + medioContacto.DESCRIPCION + @"</u></i></p>" + @" 
                                    </td>
                                    <td></td>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + "  aling = 'right'>Tipo de reporte: <i><u>" + tipoReporte.DESCRIPCION + "</u></i></p>" + @" 
                                    </td>
                                    
                                </tr>
                                
                                <tr>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Tipo de operación: <i><u>" + tipoOperacion.DESCRIPCION + @"</u></i></p>" + @" 
                                    </td>
                                    <td></td>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + "  aling = 'right'>Operación: <i><u>" + operacion.DESCRIPCION + "</u></i></p>" + @" 
                                    </td>
                                </tr>
                                
                                <tr>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Causa del reporte: <i><u>" + causaReporte.DESCRIPCION + @"</u></i></p>" + @" </td>
                                    <td></td>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Fecha de la transacción: <i><u>" + requestForOpeNoAplicadas_.fechaTransacion + @"</u></i></p>" + @" </td>
                                </tr>

                                <tr>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Importe de reclamo: <i><u>" + requestForOpeNoAplicadas_.importeReclamacion + @"</u></i></p>" + @" </td>
                                    <td></td>
                                    <td></td>
                                </tr>

                                <tr>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Medio de detección de movimiento: <i><u>" + requestForOpeNoAplicadas_.medioDeteccionMovimiento + @"</u></i></p>" + @" </td>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Folio de autorización: <i><u>" + requestForOpeNoAplicadas_.folioAutorizacion+ @"</u></i></p>" + @" </td>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Tipo de cuenta(CMV Finanzas): <i><u>" + requestForOpeNoAplicadas_.tipoCuenta + @"</u></i></p>" + @" </td>
                                </tr>

                            </table>
                            <p " + fuenteGeneral + " >Descripción del reporte: <i><u>" + reporte.DESCRIPCION_REPORTE + @"</u></i></p>" + @"
                            <br>";

                string html2 = "<p " + fuenteGeneral + @" >El socio acepta que ha sido informado tanto del proceso de solicitud de aclaración, así como del costo por aclaración en caso de requerirse petición de documentación ($250.00 +IVA por movimiento solicitado). Es importante mencionar que cada movimiento que se registre será una aclaración independiente.</p>
                        
                        <p " + fuenteGeneral + @" >De ser una aclaración de cargo duplicado, múltiple o devolución favor de anexar una copia del voucher.
Anexar detalle de movimientos donde se muestren montos para aclaración.
</p>
                        <br><br>
                        <table>
                            <tr>
                                <td " + fuenteGeneralCentrada + @">AUTORIZACIÓN DEL SOCIO</td> 
                                <td></td>
                                <td " + fuenteGeneralCentrada + @">EJECUTIVO/GERENTE QUE TRAMITÓ EL REPORTE</td>
                            </tr>
                            <tr>
                                <td>
                                    <br><br>
                                    <p>__________________________</p>
                                    <p " + fuenteGeneralCentrada + @">Nombre y firma del socio</p>
                                </td>
                                <td></td>
                                <td>
                                    <br><br>
                                    <p>__________________________</p>
                                    <p " + fuenteGeneralCentrada + @" >Nombre y firma del Ejecutivo/Gerente</p>
                                </td>
                            </tr>
                        </table>
                        
                        ";

                html += html1 + html2;

                //html = "<h1>Formato para solicitar la Domiciliación</h1>";


                document.Open();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(html.ToString()), new StyleSheet()))
                {
                    document.Add(E);

                }


                document.AddAuthor("Caja Morelia Valladolid");
                document.AddTitle("Formato de operaciones no reconocidas/no aplicadas de “CMV Finanzas”");
                document.AddCreator("Cristian Perez Garcia");
                document.AddKeywords("Gererencia de TI");
                document.AddSubject("Formato de operaciones no reconocidas/no aplicadas de “CMV Finanzas”");
                document.CloseDocument();
                document.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return pathWebFile;
        }

        public string formatoDictamen(List<ReporteMultifolio> reportesSocio, TBL_UNE_REPORTE reporte, CLAVES cl, string pathFormatosBanca, RequestForOpeNoAplicadas requestForAclProcedente_)
        {
            if (!Directory.Exists(pathFormatosBanca))
            {
                Directory.CreateDirectory(pathFormatosBanca);
            }

            SP_BANCA_OBTIENE_SOCIO_Result DatosSocio = uneDB.SP_BANCA_OBTIENE_SOCIO(reporte.NUMERO, cl.Numusuario, 1).FirstOrDefault();

            CAT_UNE_MEDIO_CONTACTO medioContacto = uneDB.CAT_UNE_MEDIO_CONTACTO.Where(x => x.ID_MEDIO_CONTACTO == reporte.ID_MEDIO_CONTACTO).First();
            CAT_UNE_TIPO_REPORTE tipoReporte = uneDB.CAT_UNE_TIPO_REPORTE.Where(x => x.ID_TIPO_REPORTE == reporte.ID_TIPO_REPORTE).First();
            CAT_UNE_TIPO_CUENTA tipoOperacion = uneDB.CAT_UNE_TIPO_CUENTA.Where(x => x.ID_TIPO_CUENTA == reporte.ID_TIPO_CUENTA).First();
            CAT_UNE_CUENTAS operacion = uneDB.CAT_UNE_CUENTAS.Where(x => x.ID_CUENTA == reporte.ID_CUENTA).First();
            CAT_UNE_SUPUESTOS_REPORTE causaReporte = uneDB.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).First();

            var sucursal = uneDB.SUCURSALES.Where(x => x.Id_de_Sucursal == reporte.ID_DE_SUCURSAL).FirstOrDefault();
            string nombreFile = "FormatoDictamen_" + reporte.folio_banca + "_" + reporte.NUMERO + ".pdf";
            string pathWebFile = WebConfigurationManager.AppSettings["pathFormatosBanca"].ToString() + nombreFile;
            string file = Path.Combine(pathFormatosBanca, nombreFile);

            

            Document document = new Document(PageSize.A4, 30, 30, 30, 110);
            //Document document = new Document(PageSize.A4, 30, 30, 30, 110);
            PdfWriter PDFWriter = PdfWriter.GetInstance(document, new FileStream(file, FileMode.Create));
            ItextEvents eventos = new ItextEvents();
            eventos.TituloCabecera = "DICTAMEN DE REPORTE DE CMV FINANZAS";
            PDFWriter.PageEvent = eventos;
            try
            {
                DateTime fechaActual = System.DateTime.Now;
                DateTimeFormatInfo formatoFecha = new CultureInfo("es-ES", false).DateTimeFormat;
                string nombreMes = formatoFecha.GetMonthName(fechaActual.Month).ToUpper();
                //string html = "<p align='center' " + css.Replace("10", "12") + "><b>" + eventos.TituloCabecera + @"</b></p><br/><br/><br/><br/>";
                string html = "";
                string tipoBloqueo = "";
                string Multifolio = "";
                decimal montoAut =(decimal) reporte.IMPORTE_SOLUCION;
                string repsuestaSatisfaccion = "";

                foreach (ReporteMultifolio incidenciaReporte in reportesSocio)
                {
                    //.ToString("C")
                    string dictamen = "";
                    if(incidenciaReporte.idSatisfacion == 1)
                    {
                        dictamen = "Improcedente";
                    }
                    else if (incidenciaReporte.idSatisfacion == 2)
                    {
                        dictamen = "Procedente";
                    }
                    Multifolio += @"<tr>
                            <td></td>

                            <td " + filaTabla2  + "> <p "+ centradas +"> " + incidenciaReporte.folioAutorizacion + @" </p></td>
 
                            <td " + filaTabla2 +  "> <p " + centradas + "> " + incidenciaReporte.importeReclamo.ToString("C") + @" </p></td>
 
                            <td " + filaTabla2 + "> <p " + centradas + "> " + incidenciaReporte.medioMovimiento + @" </p></td>
                            
                            <td " + filaTabla2 + "> <p " + centradas + "> " + incidenciaReporte.tipoCuentaBanca + @" </p></td>

                            <td " + filaTabla2 + "> <p " + centradas + "> " + incidenciaReporte.fechaTransacion.ToString("dd/MM/yyyy") + @" </p></td>
                            
                            <td " + filaTabla2 + "> <p " + centradas + "> " + dictamen + @" </p></td>
                            
                            <td></td>
    
                        </tr>";
                }

                html += @"
                    <table>
                        <tr>
                            <td " + fuenteGeneral + " >" + sucursal.Descripcion + @"</td>
                            <td></td>
                            <td " + fuenteGeneral + ">" + fechaActual.Day.ToString() + " de " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nombreMes.ToLower()) + " del " + fechaActual.Year.ToString() + " " + fechaActual.ToShortTimeString() + @"</td>
                        </tr>
                    </table>
                    <br>";

                string html1 = @"
                    
                            <p " + fuenteGeneral + " >Número de socio: <i><u>" + reporte.NUMERO + @"</u></i></p>
                            <p " + fuenteGeneral + "  aling = 'right'>Nombre del socio: <i><u>" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO) + "</u></i></p>" + @"
                            <br><br>";
                html1 += @"

                            <table>
                                <tr>
                                    <td " + cabeceraTablas + @">
                                        <p " + fuenteGeneralCentrada + @">DATOS DEL REPORTE</p>
                                    </td>
                                </tr>
                            </table>
                            <br>
                            
                            <table>                                
                                <tr>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Folio CMV Finanzas: <i><u>" + reporte.folio_banca + @"</u></i></p>" + @" </td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                
                                <tr>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Medio de contacto: <i><u>" + medioContacto.DESCRIPCION + @"</u></i></p>" + @" 
                                    </td>
                                    <td></td>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + "  aling = 'right'>Tipo de reporte: <i><u>" + tipoReporte.DESCRIPCION + "</u></i></p>" + @" 
                                    </td>
                                    
                                </tr>
                                
                                <tr>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Tipo de operación: <i><u>" + tipoOperacion.DESCRIPCION + @"</u></i></p>" + @" 
                                    </td>
                                    <td></td>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + "  aling = 'right'>Operación: <i><u>" + operacion.DESCRIPCION + "</u></i></p>" + @" 
                                    </td>
                                </tr>
                                
                                <tr>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Causa del reporte: <i><u>" + causaReporte.DESCRIPCION + @"</u></i></p>" + @" </td>
                                    <td></td>
                                    
                                </tr>

                            </table>
                            <br/>
                            <br/>
                            <table>
                                <tr>
                                    <td></td>
                                    
                                    <td " + filaTabla1 + "><p " + centradas + @">  Folio de Autorización</p></td>
 
                                    <td " + filaTabla1 + "><p " + centradas + @">Importe Reclamado</p></td>
 
                                    <td " + filaTabla1 + "><p " + centradas + @">Medio de detección de movimiento</p></td>
                            
                                    <td " + filaTabla1 + "><p " + centradas + @">Tipo de cuenta (CMV Finanzas)</p></td>

                                    <td " + filaTabla1 + "><p " + centradas + @">Fecha de transación</p></td>

                                    <td " + filaTabla1 + "><p " + centradas + @">Estatus del folio</p></td>
                                    
                                    <td></td>
    
                                </tr>
                                " + Multifolio + @"
                            </table>
                            <br/>
                            <p " + fuenteGeneral + " >Descripción del reporte: <i><u>" + reporte.DESCRIPCION_REPORTE + @"</u></i></p>" + @"
                            <br>
                                
                            <table>
                                <tr>
                                    <td " + cabeceraTablas + @">
                                        <p " + fuenteGeneralCentrada + @">DATOS DEL DICTAMEN</p>
                                    </td>
                                </tr>
                            </table>
                            <br>

                            <table>                                
                                <tr>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + " >Fecha de Solución del reporte: <i><u>" + reporte.FECHA_ABONO + @"</u></i></p>" + @" </td>
                                    <td></td>
                                    
                                    
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    
                                    <td></td>
                                    <td " + fuenteGeneral + " >" +
                                        @"<p " + fuenteGeneral + "  aling = 'right'>Monto Autorizado: <i><u>" + montoAut.ToString("C") + "</u></i></p>" + @" 
                                    </td>
                                    
                                </tr>
                            </table>                            
                            <p " + fuenteGeneral + " >Descripción de la finalización del reporte: <i><u>" + requestForAclProcedente_.comentariosFinalizacion + @"</u></i></p>" + @"
                            ";

                string html2 = @" 
    
                        <br><br>
                        <table>
                            
                            <tr>
                                <td>
                                    <br><br>
                                    <p>__________________________</p>
                                    <p " + fuenteGeneralCentrada + @">Dictamina</p>
                                </td>
                                <td></td>
                                <td>
                                    <br><br>
                                    <p>__________________________</p>
                                    <p " + fuenteGeneralCentrada + @" >Autoriza</p>
                                </td>
                            </tr>
                        </table>
                        <br><br>
                        ";

                html += html1 + html2;

                //html = "<h1>Formato para solicitar la Domiciliación</h1>";


                document.Open();
                foreach (IElement E in HTMLWorker.ParseToList(new StringReader(html.ToString()), new StyleSheet()))
                {
                    document.Add(E);

                }


                document.AddAuthor("Caja Morelia Valladolid");
                document.AddTitle("Formato de dictamen de un reporte de “CMV Finanzas”");
                document.AddCreator("Cristian Perez Garcia");
                document.AddKeywords("Gererencia de TI");
                document.AddSubject("Formato de dictamen de un reporte de “CMV Finanzas”");
                document.CloseDocument();
                document.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return pathWebFile;
        }
    }
}