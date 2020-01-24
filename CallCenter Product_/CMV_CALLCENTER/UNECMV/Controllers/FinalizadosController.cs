using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Models;
using CMV_CALLCENTER.Entidad;
using System.IO;
using System.Configuration;

namespace CMV_CALLCENTER.Controllers
{
    public class FinalizadosController : Controller
    {
        private ContextUne db = new ContextUne();
        private Funciones funcion = new Funciones();
        private ContextUneBD uneDB = new ContextUneBD();
        //
        // GET: /Finalizados/

        public ActionResult Finalizados()
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if (cl != null)
            {
                int? tipoJefe = Session["tipoJefe"] as int?;
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                ViewData["causas"] = db.CAT_UNE_SUPUESTOS_REPORTE.ToList();

                if (permisos.USUARIO_SUCURSAL == true)
                {
                    ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => (x.ID_ESTATUS_REPORTE == 6 || x.ID_ESTATUS_REPORTE == 7 || x.ID_ESTATUS_REPORTE == 8) && x.reporte_banca == true && x.ID_DE_SUCURSAL == cl.Id_de_sucursal).ToList();
                }
                else
                {
                    if (permisos.USUARIO_CALL_CENTER == true && permisos.ADMINISTRADOR_CALL_CENTER == true)
                    {
                        ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => (x.ID_ESTATUS_REPORTE == 6 || x.ID_ESTATUS_REPORTE == 7 || x.ID_ESTATUS_REPORTE == 8) && x.reporte_banca == true).ToList();
                    }
                    else if (permisos.USUARIO_CALL_CENTER == true && permisos.ADMINISTRADOR_CALL_CENTER == false)
                    {
                        ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => (x.ID_ESTATUS_REPORTE == 6 || x.ID_ESTATUS_REPORTE == 7 || x.ID_ESTATUS_REPORTE == 8) && x.reporte_banca == true && x.USUARIO_REGISTRA == cl.Numusuario).ToList();
                    }
                    else if (permisos.USUARIO_UNE == 1)
                    {
                        ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => x.ID_ESTATUS_REPORTE == 6 || x.ID_ESTATUS_REPORTE == 7 || x.ID_ESTATUS_REPORTE == 8 && x.reporte_banca == false).ToList();
                    }
                    else if (tipoJefe == 4)
                    {
                        string regionActual = db.SUCURSALES.Where(x => x.Id_de_Sucursal == cl.Id_de_sucursal).Select(x => x.Region_Actual).FirstOrDefault();
                        ViewData["listaReportes"] = (from t in db.TBL_UNE_REPORTE
                                                     join s in db.SUCURSALES
                                                     on t.ID_SUCURSAL_REGISTRO equals s.Id_de_Sucursal
                                                     where (s.Region_Actual.Contains(regionActual) && (t.ID_ESTATUS_REPORTE == 6 || t.ID_ESTATUS_REPORTE == 7 || t.ID_ESTATUS_REPORTE == 8))
                                                     select t).ToList();
                    }
                    else
                    {
                        ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => (x.ID_ESTATUS_REPORTE == 6 || x.ID_ESTATUS_REPORTE == 7 || x.ID_ESTATUS_REPORTE == 8) && x.USUARIO_REGISTRA == cl.Numusuario).ToList();
                    }
                }

                Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.AbsolutePath.ToString();
                Session["segmento1"] = System.Web.HttpContext.Current.Request.Url.Segments[2];
                return View();
            }
            else
                return RedirectToAction("Index", "Login");
        }

        public int validaArchivoExiste(int folio, string tipoLlamada)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            var llamada = db.TBL_PRESENCE_LLAMADAS.Where(x => x.folio_cmv == folio && x.tipo_llamada == tipoLlamada).FirstOrDefault();

            if (llamada != null)
                return 1;
            else
                return 0;
        }

        [HttpPost]
        public JsonResult guardarActualizarArchivo(TBL_PRESENCE_LLAMADAS presence)
        {
            int estatus = 0;
            try
            {
                CLAVES cl = Session["SesionUsuario"] as CLAVES;
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                HttpPostedFileBase file = Request.Files[0];
                TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == presence.folio_cmv).FirstOrDefault();
                String rutaAudio = funcion.obtenerRuta(Convert.ToInt32(reporte.NUM_FOLIO));

                if (!Directory.Exists(rutaAudio))
                    Directory.CreateDirectory(rutaAudio);

                String nombre = presence.folio_cmv.ToString() + "_" + presence.tipo_llamada;
                String extension = ".mp3";
                presence.nombre_audio = nombre + extension;
                rutaAudio = rutaAudio + "\\" + nombre + extension;
                presence.ruta_audio = rutaAudio.Replace("\\", "//").Replace("\"", "/");

                if (validaArchivoExiste(Convert.ToInt32(presence.folio_cmv), presence.tipo_llamada) == 1)
                {
                    TBL_PRESENCE_LLAMADAS llamada = db.TBL_PRESENCE_LLAMADAS.Where(x => x.folio_cmv == presence.folio_cmv && x.tipo_llamada == presence.tipo_llamada).FirstOrDefault();
                    llamada.ruta_audio = presence.ruta_audio;
                    db.SaveChanges();
                    file.SaveAs(presence.ruta_audio);
                    estatus = 1;
                }
                else
                {
                    db.TBL_PRESENCE_LLAMADAS.Add(presence);
                    db.SaveChanges();
                    file.SaveAs(presence.ruta_audio);
                    estatus = 1;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(estatus);
        }

        public JsonResult ObtenerArchivosAudio(int folio)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            var audios = db.TBL_PRESENCE_LLAMADAS.Where(x => x.folio_cmv == folio);
            return Json(audios);
        }

        public DateTime obtenerFecha(int folio, String Usuario, String Contrasena)
        {
            db = new ContextUne(Usuario, Contrasena);
            var comentario = db.SP_UNE_CARGA_COMENTARIO(folio, 2).FirstOrDefault();
            if (comentario != null)
                return Convert.ToDateTime(comentario.fecha_alta);
            else
                return DateTime.Now;
        }

        public ActionResult LlamadaSalida(Iframe iframe)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            TempData["UrlNav"] = System.Web.HttpContext.Current.Request.Url.ToString();
            if (cl != null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                ViewData["listaPosibleFolios"] = db.SP_UNE_OBTENER_FOLIOS_LLAMADA_SALIDA(iframe.vPhone).ToList();
                return View(iframe);
            }
            else
                return RedirectToAction("Index", "Login");

        }

        public JsonResult ValidarFolio(int num_folio)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            int existe = 0;

            existe = db.TBL_UNE_REPORTE.Count(x => x.NUM_FOLIO == num_folio);

            return Json(existe);
        }

        [HttpPost]
        public ActionResult GuardarLlamadaSalida(Iframe iframe)
        {
            String estatus;
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            uneDB = new ContextUneBD(cl.Usuario, cl.Contrasena);
            try
            {
                CMV_REC_DATA cmv_rec_data = new CMV_REC_DATA();
                cmv_rec_data.INB_OUT_ID = iframe.vContactID;
                cmv_rec_data.CALLTYPE = iframe.vCALLTYPE;
                cmv_rec_data.CRM_FOLIO = iframe.vFolio;
                cmv_rec_data.RDATE = DateTime.Now;
                cmv_rec_data.PHONE = iframe.vPhone;

                uneDB.CMV_REC_DATA.Add(cmv_rec_data);

                uneDB.SaveChanges();

                estatus = "1";
                return Json(estatus);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }

        }


    }
}
