using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Configuration;
using CMV_CALLCENTER.Models;
using System.IO;

namespace CMV_CALLCENTER.Entidad
{
    public class Correo
    {
        private MailMessage mail;
        private SmtpClient client;
        private Attachment at;
        private AlternateView htmlView;
        private LinkedResource imagenFondo;
        private LinkedResource imagenFirma;
        private string DireccionCorreo;
        private string Servidor;

        ContextUne db = new ContextUne();

        public Correo()
        {
            mail = new MailMessage();
            client = new SmtpClient();
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
        }

        public int Enviar(TBL_UNE_REPORTE reporte, string sucursal, string correoCC, string url)
        {
            try
            {
                var DatosCorreo = db.SP_CMV_OBTENER_CORREO_SERVIDOR_MODULO("UNE").FirstOrDefault();
                mail.Body += this.Cabecera(reporte.reporte_banca);
                CAT_UNE_SUPUESTOS_REPORTE cat = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).FirstOrDefault();
                mail.Body += this.Cabecera(reporte.reporte_banca);
                mail.Body += "ASESOR ATENCIÓN A SOCIOS 01 800 <br/>"
                    + "Presente.<br/><br/>"
                    + "Por este medio le informamos que tiene una "+reporte.CAT_UNE_TIPO_REPORTE.DESCRIPCION+" "+reporte.CAT_UNE_MEDIO_CONTACTO.DESCRIPCION+ " asignada con las siguientes propiedades:<br/><br/>"
                    + "Sucursal Origen: " + sucursal + "<br/>"
                    + "Número de Socio: " + reporte.NUMERO + "<br/>"
                    + "Nombre: " + reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO + "<br/>"
                    + "Folio: " + reporte.NUM_FOLIO + "<br/>"
                    + "Causas: " + cat.DESCRIPCION + "<br/>";

                if (reporte.ID_TIPO_REPORTE == 3)
                    mail.Body += "Monto:" + Convert.ToDecimal(reporte.IMPORTE_RECLAMACION).ToString("C2") + "<br/>" + "Tiempo de Respuesta: "+ 30 + " días Hábiles";
                else
                    mail.Body += "Tiempo de Respuesta: "+ 30 + " días Hábiles";

                mail.Body += this.PiePagina();
                InsertaImagenes(url);
                AgregarDestinatarios(ConfigurationSettings.AppSettings["correoResponsable"]);
                mail.From = new MailAddress(DatosCorreo.CUENTA_EMISOR);
                AgregarCC(correoCC);
                client.Host = DatosCorreo.SERVIDOR;
                client.Send(mail);
                mail.Attachments.Clear();
                mail.Body = string.Empty;
                mail.To.Clear();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
            
        }

        public int EnviarRespuesta(TBL_UNE_REPORTE reporte, string url)
        {
            try
            {

                var DatosCorreo = db.SP_CMV_OBTENER_CORREO_SERVIDOR_MODULO("UNE").FirstOrDefault();
                CAT_UNE_SUPUESTOS_REPORTE cat = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).FirstOrDefault();

                if (reporte.reporte_banca == true)
                {
                    mail.Body += this.Cabecera(reporte.reporte_banca);
                    mail.Body += "ASESOR ATENCIÓN A SOCIOS 01 800 <br/>"
                        + "Presente.<br/><br/>"
                        + "Por este medio le informamos que tiene una RESPUESTA asignada con las siguientes propiedades:<br/><br/>"
                        + "Número de Socio: " + reporte.NUMERO + "<br/>"
                        + "Nombre: " + reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO + "<br/>"
                        + "Folio CMV Finanzas: " + reporte.folio_banca + "<br/>"
                        + "Causas: " + cat.DESCRIPCION + "<br/>";
                    mail.Body += this.CMVFinanzasPiePagina();
                }
                else
                {
                    mail.Body += this.Cabecera(reporte.reporte_banca);
                    mail.Body += "ASESOR ATENCIÓN A SOCIOS 01 800 <br/>"
                        + "Presente.<br/><br/>"
                        + "Por este medio le informamos que tiene una RESPUESTA asignada con las siguientes propiedades:<br/><br/>"
                        + "Número de Socio: " + reporte.NUMERO + "<br/>"
                        + "Nombre: " + reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO + "<br/>"
                        + "Folio: " + reporte.NUM_FOLIO + "<br/>"
                        + "Causas: " + cat.DESCRIPCION + "<br/>";

                    if (reporte.ID_TIPO_REPORTE == 3)
                        mail.Body += "Monto:" + Convert.ToDecimal(reporte.IMPORTE_RECLAMACION).ToString("C2") + "<br/>";

                    mail.Body += this.PiePagina();
                }

                
                InsertaImagenes(url);
                AgregarDestinatarios(ConfigurationSettings.AppSettings["correoResponsable"]);
                mail.From = new MailAddress(DatosCorreo.CUENTA_EMISOR);
                client.Host = DatosCorreo.SERVIDOR;
                client.Send(mail);
                mail.Attachments.Clear();
                mail.Body = string.Empty;
                mail.To.Clear();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        public int EnviarCanalizacion(string correoResponsable, string[] correosCC, TBL_UNE_REPORTE reporte, string sucursal, string nombreResponsable, string url)
        {
            try
            {
                var DatosCorreo = db.SP_CMV_OBTENER_CORREO_SERVIDOR_MODULO("UNE").FirstOrDefault();
                CAT_UNE_SUPUESTOS_REPORTE cat = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).FirstOrDefault();
                mail.Body += this.Cabecera(reporte.reporte_banca);
                if(reporte.reporte_banca == true)
                {
                    mail.Body += nombreResponsable + "<br/>"
                    + "Presente.<br/><br/>"
                    + "Por este medio le informamos que tiene una notificación de " + reporte.CAT_UNE_TIPO_REPORTE.DESCRIPCION + " " + reporte.CAT_UNE_MEDIO_CONTACTO.DESCRIPCION + " asignada con las siguientes propiedades:<br/><br/>"
                    + "Número de Socio: " + reporte.NUMERO + "<br/>"
                    + "Nombre: " + reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO + "<br/>"
                    + "Folio CMV Finanzas: " + reporte.folio_banca + "<br/>"
                    + "Causas: " + cat.DESCRIPCION + "<br/>";
                    
                    mail.Body += "Tiempo de Respuesta: " + cat.DIAS_REQUERIDOS + " días Hábiles";
                    mail.Body += this.CMVFinanzasPiePagina();
                }
                else
                {
                    mail.Body += nombreResponsable + "<br/>"
                    + "Presente.<br/><br/>"
                    + "Por este medio le informamos que tiene una " + reporte.CAT_UNE_TIPO_REPORTE.DESCRIPCION + " " + reporte.CAT_UNE_MEDIO_CONTACTO.DESCRIPCION + " asignada con las siguientes propiedades:<br/><br/>"
                    + "Número de Socio: " + reporte.NUMERO + "<br/>"
                    + "Nombre: " + reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO + "<br/>"
                    + "Folio: " + reporte.NUM_FOLIO + "<br/>"
                    + "Causas: " + cat.DESCRIPCION + "<br/>";

                    if (reporte.ID_TIPO_REPORTE == 3)
                        mail.Body += "Monto:" + Convert.ToDecimal(reporte.IMPORTE_RECLAMACION).ToString("C2") + "<br/>" + "Tiempo de Respuesta: " + cat.DIAS_REQUERIDOS + " días Hábiles";
                    else
                        mail.Body += "Tiempo de Respuesta: " + cat.DIAS_REQUERIDOS + " días Hábiles";
                    mail.Body += this.PiePagina();
                }
                

                
                InsertaImagenes(url);
                AgregarDestinatarios(correoResponsable);

                if(correosCC[0]!=null)
                    for (int i = 0; i < correosCC.Length; i++)
                        AgregarCC(correosCC[i]);

                mail.From = new MailAddress(DatosCorreo.CUENTA_EMISOR);
                client.Host = DatosCorreo.SERVIDOR;
                client.Send(mail);
                mail.Attachments.Clear();
                mail.Body = string.Empty;
                mail.To.Clear();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int EnviarAceptar(string correoResponsable, string[] correosCC, TBL_UNE_REPORTE reporte, string comentarios, string nombreResponsable, string url)
        {
            try
            {
                var DatosCorreo = db.SP_CMV_OBTENER_CORREO_SERVIDOR_MODULO("UNE").FirstOrDefault();
                CAT_UNE_SUPUESTOS_REPORTE cat = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).FirstOrDefault();

                if (reporte.reporte_banca == true)
                {
                    mail.Body += this.Cabecera(reporte.reporte_banca);
                    mail.Body += nombreResponsable + "<br/>"
                        + "Presente.<br/><br/>"
                        + "Por este medio le informamos que el reporte de " + reporte.CAT_UNE_TIPO_REPORTE.DESCRIPCION + " ha sido REVISADO POR EL RESPONSABLE, con las siguientes propiedades:<br/><br/>"
                        + "Número de Socio: " + reporte.NUMERO + "<br/>"
                        + "Nombre: " + reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO + "<br/>"
                        + "Folio CMV Finanzas: " + reporte.folio_banca + "<br/>";

                mail.Body += this.CMVFinanzasPiePagina();
                }
                else
                {
                    mail.Body += this.Cabecera(reporte.reporte_banca);
                    mail.Body += nombreResponsable + "<br/>"
                        + "Presente.<br/><br/>"
                        + "Por este medio le informamos que tiene una " + reporte.CAT_UNE_TIPO_REPORTE.DESCRIPCION + " con RESPUESTA SATISFACTORIA con las siguientes propiedades:<br/><br/>"
                        + "Número de Socio: " + reporte.NUMERO + "<br/>"
                        + "Nombre: " + reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO + "<br/>"
                        + "Folio: " + reporte.NUM_FOLIO + "<br/>"
                        + "FAVOR DE HACER LLEGAR AL SOCIO/USUARIO LA RESPUESTA";

                    mail.Body += this.PiePagina();
                }

                
                InsertaImagenes(url);
                AgregarDestinatarios(correoResponsable);

                for (int i = 0; i < correosCC.Length; i++)
                    AgregarCC(correosCC[i]);

                mail.From = new MailAddress(DatosCorreo.CUENTA_EMISOR);
                client.Host = DatosCorreo.SERVIDOR;
                client.Send(mail);
                mail.Attachments.Clear();
                mail.Body = string.Empty;
                mail.To.Clear();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int EnviarFinalizar(string correoResponsable, string[] correosCC, int folio, string comentarios, string url, TBL_UNE_REPORTE reporte)
        {
            try
            {
                var DatosCorreo = db.SP_CMV_OBTENER_CORREO_SERVIDOR_MODULO("UNE").FirstOrDefault();
                CAT_UNE_SUPUESTOS_REPORTE cat = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).FirstOrDefault();

                if (reporte.reporte_banca == true && reporte.ID_ESTATUS_REPORTE == 6)
                {
                    mail.Body += this.Cabecera(reporte.reporte_banca);
                    mail.Body += "ASESOR ATENCIÓN A SOCIOS 01 800 3000 268 <br/>"
                        + "Presente.<br/><br/>"
                        + "Por este medio le informamos que se notifico al socio/usuario sobre la resolución del reporte " + reporte.folio_banca + " con las siguientes propiedades:<br/><br/>"
                        + "Número de Socio: " + reporte.NUMERO + "<br/>"
                        + "Nombre: " + reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO + "<br/>"
                        + "Folio CMV Finanzas: " + reporte.folio_banca + "<br/>"
                        + "Causas: " + cat.DESCRIPCION + "<br/>";

                    mail.Body += this.CMVFinanzasPiePagina();
                    InsertaImagenes(url);
                    AgregarDestinatarios(correoResponsable);

                    for (int i = 0; i < correosCC.Length; i++)
                        AgregarCC(correosCC[i]);

                    mail.From = new MailAddress(DatosCorreo.CUENTA_EMISOR);
                    client.Host = DatosCorreo.SERVIDOR;
                    client.Send(mail);
                    mail.Attachments.Clear();
                    mail.Body = string.Empty;
                    mail.To.Clear();
                }
                else if(reporte.reporte_banca !=true)
                {
                    mail.Body += this.Cabecera(reporte.reporte_banca);
                    mail.Body += "ASESOR ATENCIÓN A SOCIOS 01 800 <br/>"
                        + "Presente.<br/><br/>"
                        + "Por este medio le informamos que se notifico al socio/usuario la resolución del reporte " + reporte.NUM_FOLIO + " con las siguientes propiedades:<br/><br/>"
                        + "Número de Socio: " + reporte.NUMERO + "<br/>"
                        + "Nombre: " + reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO + "<br/>"
                        + "Folio: " + reporte.NUM_FOLIO + "<br/>"
                        + "Causas: " + cat.DESCRIPCION + "<br/>";

                    mail.Body += this.PiePagina();
                    InsertaImagenes(url);
                    AgregarDestinatarios(correoResponsable);

                    for (int i = 0; i < correosCC.Length; i++)
                        AgregarCC(correosCC[i]);

                    mail.From = new MailAddress(DatosCorreo.CUENTA_EMISOR);
                    client.Host = DatosCorreo.SERVIDOR;
                    client.Send(mail);
                    mail.Attachments.Clear();
                    mail.Body = string.Empty;
                    mail.To.Clear();
                }

                
                
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int EnviarReporteProcedente(string correoResponsable, string[] correosCC, int folio, string comentarios, string url, TBL_UNE_REPORTE reporte, string urlReporte)
        {
            try
            {
                var DatosCorreo = db.SP_CMV_OBTENER_CORREO_SERVIDOR_MODULO("UNE").FirstOrDefault();
                CAT_UNE_SUPUESTOS_REPORTE cat = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).FirstOrDefault();

                if (reporte.reporte_banca == true && reporte.ID_ESTATUS_REPORTE == 6)
                {
                    mail.Body += this.Cabecera(reporte.reporte_banca);
                    mail.Body += "ASESOR ATENCIÓN A SOCIOS 01 800 3000 268 <br/>"
                        + "Presente.<br/><br/>"
                        + "Por este medio le informamos que se notifico al socio/usuario sobre la resolución del reporte " + reporte.folio_banca + " con las siguientes propiedades:<br/><br/>"
                        + "Número de Socio: " + reporte.NUMERO + "<br/>"
                        + "Nombre: " + reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO + "<br/>"
                        + "Folio CMV Finanzas: " + reporte.folio_banca + "<br/>"
                        + "Causas: " + cat.DESCRIPCION + "<br/>";

                    mail.Body += this.CMVFinanzasPiePagina();
                    InsertaImagenes(url);
                    AgregarDestinatarios(correoResponsable);

                    for (int i = 0; i < correosCC.Length; i++)
                        AgregarCC(correosCC[i]);

                    mail.From = new MailAddress(DatosCorreo.CUENTA_EMISOR);
                    client.Host = DatosCorreo.SERVIDOR;
                    client.Send(mail);
                    mail.Attachments.Clear();
                    mail.Body = string.Empty;
                    mail.To.Clear();
                }
                else if (reporte.reporte_banca != true)
                {
                    mail.Body += this.Cabecera(reporte.reporte_banca);
                    mail.Body += "ASESOR ATENCIÓN A SOCIOS 01 800 <br/>"
                        + "Presente.<br/><br/>"
                        + "Por este medio le informamos que se notifico al socio/usuario la resolución del reporte " + reporte.NUM_FOLIO + " con las siguientes propiedades:<br/><br/>"
                        + "Número de Socio: " + reporte.NUMERO + "<br/>"
                        + "Nombre: " + reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO + "<br/>"
                        + "Folio: " + reporte.NUM_FOLIO + "<br/>"
                        + "Causas: " + cat.DESCRIPCION + "<br/>";

                    mail.Body += this.PiePagina();
                    InsertaImagenes(url);
                    AgregarDestinatarios(correoResponsable);

                    for (int i = 0; i < correosCC.Length; i++)
                        AgregarCC(correosCC[i]);

                    mail.From = new MailAddress(DatosCorreo.CUENTA_EMISOR);
                    client.Host = DatosCorreo.SERVIDOR;
                    client.Send(mail);
                    mail.Attachments.Clear();
                    mail.Body = string.Empty;
                    mail.To.Clear();
                }



                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public int EnviarNotificacion(string correoCC,  int folio, string comentarios, string url, TBL_UNE_REPORTE reporte)
        {
            try
            {
                var DatosCorreo = db.SP_CMV_OBTENER_CORREO_SERVIDOR_MODULO("UNE").FirstOrDefault();
                mail.Body += this.Cabecera(reporte.reporte_banca);
                mail.Body += "LA RESOLUCIÓN DEL REPORTE NÚMERO <strong>" + folio + "</strong>"
                    + "<br/>HA SIDO NOTIFICADA AL SOCIO CORRESPONDIENTE<br/><br/>"
                    + "COMENTARIOS:<br/>"
                    + comentarios + "<br/>";

                mail.Body += this.PiePagina();
                InsertaImagenes(url);
                AgregarDestinatarios(ConfigurationSettings.AppSettings["correoResponsable"]);

                //AgregarCC(correoCC);

                mail.From = new MailAddress(DatosCorreo.CUENTA_EMISOR);
                client.Host = DatosCorreo.SERVIDOR;
                client.Send(mail);
                mail.Attachments.Clear();
                mail.Body = string.Empty;
                mail.To.Clear();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int EnviarRechazar(string correoResponsable, string[] correosCC, TBL_UNE_REPORTE reporte, string comentarios, string nombreResponsable, string url)
        {
            try
            {
                var DatosCorreo = db.SP_CMV_OBTENER_CORREO_SERVIDOR_MODULO("UNE").FirstOrDefault();
                CAT_UNE_SUPUESTOS_REPORTE cat = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).FirstOrDefault();

                if (reporte.reporte_banca == true)
                {
                    mail.Body += this.Cabecera(reporte.reporte_banca);
                    mail.Body += nombreResponsable + "<br/>"
                        + "Presente.<br/><br/>"
                        + "Por este medio le informamos que el reporte de " + reporte.CAT_UNE_TIPO_REPORTE.DESCRIPCION + " con folio " + reporte.folio_banca + " HA SIDO DEVUELTO, favor de revisar en reportes canalizados:<br/><br/>"
                        + "Número de Socio: " + reporte.NUMERO + "<br/>"
                        + "Nombre: " + reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO + "<br/>"
                        + "Folio CMV Finanzas: " + reporte.folio_banca + "<br/>"
                        + "Motivo de Devolución: " + comentarios + "<br/>";
                        
                    mail.Body += this.CMVFinanzasPiePagina();
                    
                }
                else
                {
                    mail.Body += this.Cabecera(reporte.reporte_banca);
                    mail.Body += nombreResponsable + "<br/>"
                        + "Presente.<br/><br/>"
                        + "Por este medio le informamos que tiene una " + reporte.CAT_UNE_TIPO_REPORTE.DESCRIPCION + " con RESPUESTA NO SATISFACTORIA con las siguientes propiedades:<br/><br/>"
                        + "Número de Socio: " + reporte.NUMERO + "<br/>"
                        + "Nombre: " + reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO + "<br/>"
                        + "Folio: " + reporte.NUM_FOLIO + "<br/>"
                        + "Motivo de Devolución: " + comentarios + "<br/>"
                        + "Tiempo de Respuesta: INMEDIATA";

                    mail.Body += this.PiePagina();
                }

                
                InsertaImagenes(url);
                AgregarDestinatarios(correoResponsable);

                for (int i = 0; i < correosCC.Length; i++)
                    AgregarCC(correosCC[i]);

                mail.From = new MailAddress(DatosCorreo.CUENTA_EMISOR);
                client.Host = DatosCorreo.SERVIDOR;
                client.Send(mail);
                mail.Attachments.Clear();
                mail.Body = string.Empty;
                mail.To.Clear();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int descargaArchivo(string correoResponsable, string nombreArchivo, string url)
        {
            try
            {
                var DatosCorreo = db.SP_CMV_OBTENER_CORREO_SERVIDOR_MODULO("UNE").FirstOrDefault();
                mail.Subject = "REPORTE GENERADO CORRECTAMENTE";
                mail.Body += "<html><body background='cid:imagenFondo'>";
                mail.Body += "SE LE INFORMA QUE EL REPORTE SE HA GENERADO CORRECTAMENTE "
                    + "<br/><br/>"
                    + "POR LO QUE PODRA LOCALIZARLO Y DESCARGARLO EN LA SIGUIENTE RUTA:<br/><a href='"+ nombreArchivo +"'>"
                    + nombreArchivo + "</a><br/>";

                mail.Body += this.PiePagina();
                InsertaImagenes(url);
                AgregarDestinatarios(correoResponsable);

                mail.From = new MailAddress(DatosCorreo.CUENTA_EMISOR);
                client.Host = DatosCorreo.SERVIDOR;
                client.Send(mail);
                mail.Attachments.Clear();
                mail.Body = string.Empty;
                mail.To.Clear();
                return 1;
            }
            catch (Exception ex)
            {

                return 0;
            }
        }

        public string Cabecera(bool? reporteBanca)
        {
            if(reporteBanca == true)
                mail.Subject = "CMV Finanzas";
            else
                mail.Subject = "UNE CMV";
            return "<html><body background='cid:imagenFondo'>";
        }

        public void InsertaImagenes(String url)
        {
            try
            {

                string pathAplicacion = url;
                string pathFondoFirma = url;
                string attachmentPath = Path.Combine(pathFondoFirma);  //valoresConfig.AppSettings.Settings["imagenFirma"].Value.ToString();
                string contentID = Path.GetFileName(attachmentPath).Replace(".", "");
                Attachment imagenFirma = new Attachment(attachmentPath);
                imagenFirma.ContentDisposition.Inline = true;
                imagenFirma.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                imagenFirma.ContentId = contentID;
                imagenFirma.ContentType.MediaType = "image/jpg";
                imagenFirma.ContentType.Name = Path.GetFileName(attachmentPath);
                mail.Attachments.Add(imagenFirma);
                mail.Body = mail.Body.Replace("imagenFirma", contentID);
            }
            catch (Exception ex)
            {
                throw new Exception("No se logro añadir las imagenes de fondo en el correo.\nDetalle: " + ex.Message);
            }
        }


        private String PiePagina()
        {
            StringBuilder pie = new StringBuilder();
            pie.Append("<br /><br /><div>");
            pie.Append("<IMG SRC='cid:imagenFirma' ALT='UNE'>");
            pie.Append("<font face='Tahoma' SIZE=2  COLOR=gray><br/>  ACATITA DE BAJAN No. 222, COL. LOMAS DE HIDALGO <br />");
            pie.Append("TEL: (443) 3226-300, EXT. 2408, MORELIA, MICH.<br />");
            pie.Append("LADA SIN COSTO: 01 800 3000 268<br />");
            pie.Append("www.cajamorelia.com.mx</font>");
            pie.Append("</body>");
            pie.Append("</html>");
            return pie.ToString();
        }

        private String CMVFinanzasPiePagina()
        {
            StringBuilder pie = new StringBuilder();
            pie.Append("<br /><br /><div>");
            pie.Append("<IMG SRC='cid:imagenFirma' ALT='UNE'>");
            pie.Append("<font face='Tahoma' SIZE=2  COLOR=gray><br/>  ACATITA DE BAJAN No. 222, COL. LOMAS DE HIDALGO <br />");
            pie.Append("MORELIA, MICH.<br />");
            pie.Append("LADA SIN COSTO: 01 800 3000 268<br />");
            pie.Append("www.cajamorelia.com.mx</font>");
            pie.Append("</body>");
            pie.Append("</html>");
            return pie.ToString();
        }

        private void AgregarDestinatarios(string correosUsuario)
        {
            mail.To.Add(correosUsuario);
        }

        private void AgregarCC(String correo)
        {
            mail.CC.Add(correo);
        }

        public int EnviarCCGerente(TBL_UNE_REPORTE reporte, string url)
        {
            try
            {
                var DatosCorreo = db.SP_CMV_OBTENER_CORREO_SERVIDOR_MODULO("UNE").FirstOrDefault();
                SP_BANCA_OBTIENE_SOCIO_Result socioBanca = db.SP_BANCA_OBTIENE_SOCIO(reporte.NUMERO, 0,1).FirstOrDefault();
                CLAVES gerenteCorreo = db.CLAVES.Where(x => x.Id_de_sucursal == socioBanca.Id_de_Sucursal && x.Usuario.Contains("ENC_")).FirstOrDefault();
                SUCURSALES suc = db.SUCURSALES.Where(x => x.Id_de_Sucursal == socioBanca.Id_de_Sucursal).FirstOrDefault();
                
                CAT_UNE_SUPUESTOS_REPORTE cat = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == reporte.ID_SUPUESTOS_REPORTE).FirstOrDefault();
                mail.Body += this.Cabecera(reporte.reporte_banca);
                if(reporte.reporte_banca == true)
                {
                    mail.Body += "Encargado " + suc.Descripcion + "<br/>"
                    + "Presente.<br/><br/>"
                    + "Por este medio le informamos que el reporte CMV Finanzas con número de folio: " + reporte.folio_banca + " ha sido aprobado para la devolución monetario al socio.<br/> Datos del reporte:<br/><br/>"
                    + "Número de Socio: " + reporte.NUMERO + "<br/>"
                    + "Nombre: " + reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO + "<br/>"
                    + "Folio CMV Finanzas: " + reporte.folio_banca + "<br/>"
                    + "Folio UNE: " + reporte.NUM_FOLIO + "<br/>"
                    + "Causas: " + cat.DESCRIPCION + "<br/>";

                    mail.Body += "Monto:" + Convert.ToDecimal(reporte.IMPORTE_RECLAMACION).ToString("C2") + "<br/>";
                    mail.Body += "Monto Autorizado:" + Convert.ToDecimal(reporte.IMPORTE_SOLUCION).ToString("C2") + "<br/>";

                }
                else
                {
                    mail.Body += "Encargado " + suc.Descripcion + "<br/>"
                    + "Presente.<br/><br/>"
                    + "Por este medio le informamos que el reporte CMV Finanzas con número de folio: " + reporte.NUM_FOLIO + " ha sido aprobado para la devolución monetario al socio.<br/> Datos del reporte:<br/><br/>"
                    + "Número de Socio: " + reporte.NUMERO + "<br/>"
                    + "Nombre: " + reporte.NOMBRE_S + " " + reporte.APELLIDO_PATERNO + " " + reporte.APELLIDO_MATERNO + "<br/>"
                    + "Folio UNE: " + reporte.NUM_FOLIO + "<br/>"
                    + "Causas: " + cat.DESCRIPCION + "<br/>";

                    mail.Body += "Monto:" + Convert.ToDecimal(reporte.IMPORTE_RECLAMACION).ToString("C2") + "<br/>";
                    mail.Body += "Monto Autorizado:" + Convert.ToDecimal(reporte.IMPORTE_SOLUCION).ToString("C2") + "<br/>";

                }
                mail.Body += this.PiePagina();
                InsertaImagenes(url);
                AgregarDestinatarios(gerenteCorreo.Correo);
                mail.From = new MailAddress(DatosCorreo.CUENTA_EMISOR);
                client.Host = DatosCorreo.SERVIDOR;
                client.Send(mail);
                mail.Attachments.Clear();
                mail.Body = string.Empty;
                mail.To.Clear();
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

    }
}