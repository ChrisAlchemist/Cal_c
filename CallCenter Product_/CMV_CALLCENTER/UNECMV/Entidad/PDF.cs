using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.IO;

namespace CMV_CALLCENTER.Entidad
{
    public class PDF : PdfPageEventHelper
    {
        public object RecursoUtils { get; private set; }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            try
            {
                // AddWaterMark(document);
                AddFooter(document);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void AddFooter(Document objPdfDocument)
        {
            try
            {
                Image objImagePdf;
                string directorio = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Estilos/Imagenes/");
                //System.Environment.CurrentDirectory;
               // directorio += @"\Estilos\Imagenes";
                string imagen = Path.Combine(directorio, "footer.png");
                //string rutaOficiosPreview = System.Web.Hosting.HostingEnvironment.MapPath(@"~\OficiosInternos\");

                //objImagePdf = Image.GetInstance(rutaOficiosPreview + "/footer_oficio_2.png");
                objImagePdf = Image.GetInstance(imagen);
                objImagePdf.ScaleAbsolute(625, 100);
                objImagePdf.SetAbsolutePosition(-5, -2);
                objPdfDocument.Add(objImagePdf);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}