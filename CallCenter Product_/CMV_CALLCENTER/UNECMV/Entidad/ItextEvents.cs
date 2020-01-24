using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMV_CALLCENTER.Entidad;

namespace CMV.BancaAdmin.UTILS
{
    public class ItextEvents : PdfPageEventHelper
    {
        private string css = "style='font-size:4.5px; color:white; text-align:center; font-weight:bold;'";
        private string negritasYCentradas = "style='text-align:center; font-weight:bold;'";
        private string titulosCabeceras = "style='font-weight:bold;  color:7a7a7a'";
        private string filaTabla = "bgcolor='#bcbcbc'";
        private string centradas = "style='text-align:center;'";
        private string derecha = "style='text-align:right;'";
        private string izq = "style='text-align:left;'";
        private string cabeceraHeader = "bgcolor='#77b221' style='font-weight:bold; text-align:right; color:white'";
        private string cabeceraTablas = "bgcolor='#404040' style='font-weight:bold; text-align:center; color:white'";
        public string pathLogoCajaMorelia { get; set; }
        public string pathMarcaAgua { get { return _pathMarcaAgua; } set { _pathMarcaAgua = value; } }
        private string cssTabla = @"style='text-align:center;font-size:8px;font-family:Arial'";
        private int tamaño = 8;
        string _pathMarcaAgua = "";
        public string TituloCabecera { get; set; }

        // This is the contentbyte object of the writer
        PdfContentByte cb;

        // we will put the final number of pages in a template
        PdfTemplate headerTemplate, template;

        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;

        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;


        #region Fields
        private string _header;
        #endregion

        #region Properties
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }
        #endregion

        public ItextEvents()
        {
           this.pathLogoCajaMorelia =  Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(@"~/"),"Entidad", "Resources", "logo_con_letras.png");
           this.pathMarcaAgua = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(@"~/"), "Entidad", "Resources", "marcaAgua.png");
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {

            }
            catch (System.IO.IOException ioe)
            {

            }
        }

        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            try
            {
                base.OnEndPage(writer, document);
                String text = "Página " + writer.PageNumber + " de ";
                float len = bf.GetWidthPoint(text, tamaño);
                Rectangle pageSize = document.PageSize;
                cb.SetRGBColorFill(100, 100, 100);
                cb.BeginText();
                cb.SetFontAndSize(bf, tamaño);
                cb.SetTextMatrix(pageSize.GetRight(85), pageSize.GetBottom(65));
                cb.ShowText(text);
                cb.EndText();
                cb.AddTemplate(template, pageSize.GetRight(85) + len, pageSize.GetBottom(65));

                cb.BeginText();
                cb.SetFontAndSize(bf, tamaño);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "CAJA MORELIA VALLADOLID, S.C. DE A.P. DE R.L. DE C.V. ACATITA DE BAJAN",
                    pageSize.GetLeft(30),
                    pageSize.GetBottom(65), 0);
                cb.EndText();

                cb.BeginText();
                cb.SetFontAndSize(bf, tamaño);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "NO 222 COL. LOMAS DE HIDALGO C.P. 58240 MORELIA, MICH RFC: CMV980925LQ7",
                    pageSize.GetLeft(30),
                    pageSize.GetBottom(55), 0);
                cb.EndText();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            
            //this.pathLogoCajaMorelia = "logo_con_letras.png";
            string encabezado = @"<br/><html><body>
                <table width='100%'" + cssTabla + @"   CELLPADDING='0'>
                    <tr>
                        <td rowspan ='2'> 
                            <img src='" + pathLogoCajaMorelia /*ConfigurationManager.AppSettings["logoCajaMorelia"] */+ @"' width = '175' height = '45' />
                        </td>
                        <td " + cabeceraHeader + @">"+this.TituloCabecera+@"</td>
                    </tr>
                    <tr>
                        <td  " + derecha + @">Centro de Atención a Socios <b>01 800 3000 268 opción 5</b> www.<b>cajamorelia</b>.com.mx</td>
                    </tr>
                    <tr>
                        <td   width='35%' " + derecha + @"></td>
                        <td   width='60%' " + derecha + @"></td>
                    </tr>
                </table><br/>";

            foreach (IElement E in HTMLWorker.ParseToList(new StringReader(encabezado.ToString()), new StyleSheet()))
            {
                document.Add(E);
            }
            AddWaterMark(document);
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            try
            {
                base.OnCloseDocument(writer, document);
                template.BeginText();
                template.SetFontAndSize(bf, tamaño);
                template.SetTextMatrix(0, 0);
                template.ShowText("" + (writer.PageNumber));
                template.EndText();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public  void AddWaterMark(Document objPdfDocument)
        {
            try
            {
                //if (!File.Exists("marcaAgua.png"))
                //{
                //    RecursoUtils.marcaAgua.Save("marcaAgua.png");
                //}
                //this._pathMarcaAgua = "marcaAgua.png";
                Image objImagePdf;

                //    //// Crea la imagen
                objImagePdf = Image.GetInstance(_pathMarcaAgua);
                //    //// Cambia el tamaño de la imagen
                //objImagePdf.ScalePercent(45, 45);
            
                //    //// Se indica que la imagen debe almacenarse como fondo
                objImagePdf.Alignment = iTextSharp.text.Image.UNDERLYING;
                //    //// Coloca la imagen en una posición absoluta
                objImagePdf.SetAbsolutePosition((objPdfDocument.PageSize.Width / 4), (objPdfDocument.PageSize.Height / 4));
                //    //// Imprime la imagen como fondo de página
                objPdfDocument.Add(objImagePdf);

            }
            catch (Exception ex)
            {
                
                throw ex;
            }


        }

    }
}
