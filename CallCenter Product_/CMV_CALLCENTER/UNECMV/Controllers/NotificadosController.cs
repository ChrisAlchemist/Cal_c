using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Models;
using CMV_CALLCENTER.Entidad;
using System.Configuration;

namespace CMV_CALLCENTER.Controllers
{
    public class NotificadosController : Controller
    {
        private ContextUne db = new ContextUne();
        private Funciones funcion = new Funciones();
        //
        // GET: /Notificados/

        public ActionResult Notificados()
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;

            if (cl != null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);

                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                if (permisos.USUARIO_UNE == 1 || permisos.USUARIO_CALL_CENTER == true)
                {
                    bool? esBanca = permisos.USUARIO_CALL_CENTER;

                    ViewData["listaNotificados"] = db.TBL_UNE_REPORTE.Where(x => x.ID_ESTATUS_REPORTE == 5 && x.reporte_banca == esBanca).ToList();
                    ViewData["causas"] = db.CAT_UNE_SUPUESTOS_REPORTE.ToList();
                    Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.ToString();
                    Session["segmento1"] = System.Web.HttpContext.Current.Request.Url.Segments[2];
                    return View();
                }
                else
                    return RedirectToAction("Permiso", "Error");
            }
            else
                return RedirectToAction("Index", "Login");
        }

        public int finalizar(int folio, string comentarios, int satisfaccion)
        {
            try
            {
                string medioReporte = "";
                CLAVES cl = Session["SesionUsuario"] as CLAVES;
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == folio).FirstOrDefault();
                SP_BANCA_OBTIENE_SOCIO_Result datosSocio = db.SP_BANCA_OBTIENE_SOCIO(reporte.NUMERO, cl.Numusuario, 1).FirstOrDefault();

                reporte.Observaciones_cierre = comentarios;
                reporte.Fecha_cierre = DateTime.Now;
                reporte.ID_SATISFACTORIO = satisfaccion;
             

                if (reporte.Vencido == 0)
                    reporte.ID_ESTATUS_REPORTE = 6;
                else if (reporte.Vencido == 1)
                    reporte.ID_ESTATUS_REPORTE = 7;
                else if (reporte.Vencido == 2)
                    reporte.ID_ESTATUS_REPORTE = 8;

                TBL_UNE_CANALIZACIONES canalizaciones = new TBL_UNE_CANALIZACIONES();
                canalizaciones.FOLIO = folio;
                canalizaciones.COMENTARIOS = comentarios;
                canalizaciones.fecha_alta = DateTime.Now;
                canalizaciones.numusuario = cl.Numusuario;
                canalizaciones.ID_TIPO_COMENTARIO = 7;
                db.TBL_UNE_CANALIZACIONES.Add(canalizaciones);

                List<TBL_UNE_USUARIOS_ASIGNADOS> list = db.TBL_UNE_USUARIOS_ASIGNADOS.Where(x => x.folio == folio).ToList();
                string correoResponsable;

                var claves = db.CLAVES.Where(x => x.Numusuario == reporte.USUARIO_REGISTRA).Select(x=> new { x.Correo }).FirstOrDefault();
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
                db.SaveChanges();
                int idTipoBitacora = 0;
                int idComision = 0;
                int estatusCobroComision = 0;

                if (reporte.ID_MEDIO_CONTACTO == 1)
                {
                    var sucursal = db.SUCURSALES.Where(x => x.Id_de_Sucursal == reporte.ID_DE_SUCURSAL).FirstOrDefault();
                    medioReporte = sucursal.Descripcion;
                }
                else if (reporte.ID_MEDIO_CONTACTO == 2)
                {
                    medioReporte = "Centro de atención telefónica";
                }

                if (reporte.ID_CUENTA == 33)
                {
                    
                    idTipoBitacora = 82;
                    funcion.RegistrarBitacora(idTipoBitacora, Convert.ToInt32(datosSocio.Numero), 4, cl.Numusuario);
                    
                    funcion.ValidaNotificaciones(reporte, idTipoBitacora, Convert.ToInt16(datosSocio.id_tipo_notificacion), reporte.folio_banca, null, medioReporte);
                    int estatus = new Correo().EnviarFinalizar(correoResponsable, CC, Convert.ToInt32(reporte.NUM_FOLIO), reporte.Observaciones_cierre, Server.MapPath("~/Estilos/Imagenes/firmaUNE.jpg"), reporte);
                }
                else if (reporte.ID_TIPO_CUENTA == 6)
                {
                    if (reporte.ID_SATISFACTORIO == 0)
                    {

                        idComision = funcion.InsertarComisionBanca(reporte, datosSocio, 1, 4);
                        idTipoBitacora = 83;
                        funcion.RegistrarBitacora(idTipoBitacora, Convert.ToInt32(datosSocio.Numero), 4, cl.Numusuario);

                        //esto va siempre y cuando el estatus del sp de cobranza sea 1
                        estatusCobroComision = funcion.CobrarComisionBanca(datosSocio, idComision);
                        if (estatusCobroComision == 1)
                        {
                            funcion.ValidaNotificaciones(reporte, idTipoBitacora, Convert.ToInt16(datosSocio.id_tipo_notificacion), reporte.folio_banca, null, medioReporte);
                            int estatus = new Correo().EnviarFinalizar(correoResponsable, CC, Convert.ToInt32(reporte.NUM_FOLIO), reporte.Observaciones_cierre, Server.MapPath("~/Estilos/Imagenes/firmaUNE.jpg"), reporte);
                        }

                    }
                    else if (reporte.ID_SATISFACTORIO == 1)
                    {
                        idTipoBitacora = 84;
                        funcion.RegistrarBitacora(idTipoBitacora, Convert.ToInt32(datosSocio.Numero), 4, cl.Numusuario);
                        //funcion.ValidaNotificaciones(reporte, idTipoBitacora, Convert.ToInt16(datosSocio.id_tipo_notificacion), reporte.folio_banca, null, medioReporte);
                        
                        int estatus = new Correo().EnviarReporteProcedente(correoResponsable, CC, Convert.ToInt32(reporte.NUM_FOLIO), reporte.Observaciones_cierre, Server.MapPath("~/Estilos/Imagenes/firmaUNE.jpg"), reporte, "");
                        reporte.ID_ESTATUS_REPORTE = 12;
                        db.SaveChanges();
                    }
                }
                else
                {
                    int estatus = new Correo().EnviarFinalizar(correoResponsable, CC, Convert.ToInt32(reporte.NUM_FOLIO), reporte.Observaciones_cierre, Server.MapPath("~/Estilos/Imagenes/firmaUNE.jpg"), reporte);
                }

                return 1;
            }
            catch (Exception ex)
            {
                return 0;
                throw ex;
            }

            //int estatus = new Correo().EnviarFinalizar(correoResponsable, CC, Convert.ToInt32(reporte.NUM_FOLIO), comentarios, Server.MapPath("~/Estilos/Imagenes/firmaUNE.jpg"));
        }

    }
}
