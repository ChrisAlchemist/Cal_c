using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Entidad;
using CMV_CALLCENTER.Models;

namespace CMV_CALLCENTER.Controllers
{
    public class DictaminarController : Controller
    {
        private ContextUne db = new ContextUne();
        private Funciones funcion = new Funciones();
        private ContextBanca bancaDB = new ContextBanca();
        //
        // GET: /Dictaminar/

        public ActionResult Dictaminar(TBL_UNE_REPORTE reporte)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            if(cl!=null)
            {
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
                if(permisos.USUARIO_UNE == 1 || permisos.USUARIO_CALL_CENTER == true)
                {
                    TBL_UNE_REPORTE dict = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporte.FOLIO).FirstOrDefault();
                    ViewData["tipoCuentas"] = db.CAT_UNE_TIPO_CUENTA.ToList();
                    ViewData["cuentas"] = db.CAT_UNE_CUENTAS.Where(x => x.ID_TIPO_CUENTA == reporte.ID_TIPO_CUENTA).ToList();
                    ViewData["sucursales"] = funcion.ListaSucursales(cl.Usuario, cl.Contrasena);
                    ViewData["listaEntidades"] = funcion.ListaEntidades(cl.Usuario, cl.Contrasena);
                    ViewData["listaTipoReportes"] = funcion.obtenerTipoReporte(cl.Usuario, cl.Contrasena);
                    ViewData["listaMediosContacto"] = funcion.obtenerMediosContacto(cl.Usuario, cl.Contrasena);
                    ViewData["listaSupuestos"] = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_TIPO_REPORTE == dict.ID_TIPO_REPORTE).ToList();
                    ViewData["listaMedioMovimiento"] = bancaDB.CAT_CALLCENTER_MEDIO_MOVIMIENTO.ToList();
                    ViewData["listaTipoCuentaBanca"] = db.CAT_UNE_TIPO_CUENTA_BANCA.ToList();
                    ViewData["listaCanales"] = db.CAT_UNE_CANAL.ToList();
                    ViewData["listaMotivoCancelacion"] = db.CAT_UNE_MOTIVO_CANCELACION.ToList();
                    ViewData["listaProductos"] = db.CAT_UNE_PRODUCTO.ToList();
                    ViewData["listaResolucion"] = db.CAT_UNE_RESOLUCION.ToList();
                    ViewData["ListaCausaResolucion"] = db.CAT_UNE_CAUSA_RESOLUCION.ToList();
                    Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.ToString();

                    if (dict.folio_banca == null)
                        dict.folio_banca = 0;

                    return View(dict);
                }
                else
                    return RedirectToAction("Permiso", "Error");
            }
            else
                return RedirectToAction("Index", "Login");
        }


        public String ObtenerUltimaRespuesta(int folio, String usuario, String contrasena)
        {
            db = new ContextUne(usuario, contrasena);
            //var can = db.SP_UNE_CARGA_ULTIMO_COMENTARIO(folio).FirstOrDefault();
            var can = db.SP_UNE_CARGA_COMENTARIO(folio, 1).FirstOrDefault();
            if (can != null)
                return can.COMENTARIOS;
            else
                return "";
        }

        public int registrarAceptar(int folio, String comentariosFinales, int numusuario)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            TBL_UNE_CANALIZACIONES can = new TBL_UNE_CANALIZACIONES();
            TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
            string nombre;
            can.FOLIO = folio;
            can.fecha_alta = DateTime.Now;
            can.COMENTARIOS = comentariosFinales;
            can.numusuario = numusuario;
            can.ID_TIPO_COMENTARIO = 4;
            db.TBL_UNE_CANALIZACIONES.Add(can);

            TBL_UNE_REPORTE rep = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == folio).FirstOrDefault();
            rep.VoBo = 1;

            if(rep.ID_TIPO_REPORTE == 3 && rep.ID_CUENTA == 5 && rep.ID_PROCEDE_DEBITO == 2)
            {
                int estatusSP = db.SP_ATM_INSERTAR_COMISION(rep.NUMERO, 1009, rep.IMPORTE_SOLUCION, null);
            }

            if (rep.reporte_banca == true && rep.NUM_FOLIO != 0)
            {
                int envio = new Correo().EnviarCCGerente(rep, Server.MapPath("~/Estilos/Imagenes/firmaUNE.jpg"));
            }

            List<TBL_UNE_USUARIOS_ASIGNADOS> list = db.TBL_UNE_USUARIOS_ASIGNADOS.Where(x => x.folio == folio).ToList();
            string correoResponsable;

            var claves = db.CLAVES.Where(x => x.Numusuario == rep.USUARIO_REGISTRA).Select(x=> new { x.Correo, x.Nombre_s, x.Apellido_materno, x.Apellido_paterno, x.Correo_externo }).FirstOrDefault();
            correoResponsable = claves.Correo;
            nombre = claves.Nombre_s + " " + claves.Apellido_paterno + " " + claves.Apellido_materno;


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
                    claves = db.CLAVES.Where(x => x.Numusuario == item.numusuario).Select(x => new { x.Correo, x.Nombre_s, x.Apellido_materno, x.Apellido_paterno, x.Correo_externo }).FirstOrDefault();
                    CC[i] = claves.Correo;
                }
                i++;
            }


            int estatus = new Correo().EnviarAceptar(correoResponsable, CC, rep, comentariosFinales, nombre, Server.MapPath("~/Estilos/Imagenes/firmaUNE.jpg"));

            if (estatus == 1)
                db.SaveChanges();

            return estatus;
        }

        public int registrarRechazo(int folio, String comentarios, int numusuario)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            string nombreResponsable;
            TBL_UNE_CANALIZACIONES can = new TBL_UNE_CANALIZACIONES();

            can.FOLIO = folio;
            can.fecha_alta = DateTime.Now;
            can.COMENTARIOS = comentarios;
            can.numusuario = numusuario;
            can.ID_TIPO_COMENTARIO = 5;
            db.TBL_UNE_CANALIZACIONES.Add(can);
            

            TBL_UNE_REPORTE rep = db.TBL_UNE_REPORTE.Where(x => x.FOLIO == folio).FirstOrDefault();
            rep.ID_ESTATUS_REPORTE = 10;
            rep.BANDEJA_DEBITO = 0;
            rep.ID_FINALIZADO_DEBITO = 0;

            //if (permisos.USUARIO_CALL_CENTER == true)
            //{
            //    rep.fecha
            //}

            CAT_UNE_SUPUESTOS_REPORTE sup = new CAT_UNE_SUPUESTOS_REPORTE();

            sup = db.CAT_UNE_SUPUESTOS_REPORTE.Where(x => x.ID_SUPUESTOS_REPORTE == rep.ID_SUPUESTOS_REPORTE).FirstOrDefault();

            if (sup.DIAS_REQUERIDOS <= rep.DIAS_RESTANTES_GENERAL)
                rep.DIAS_RESTANTES_AREA_ESPECIALIZADA = sup.DIAS_REQUERIDOS;
            else
                rep.DIAS_RESTANTES_AREA_ESPECIALIZADA = rep.DIAS_RESTANTES_GENERAL;

            List<TBL_UNE_USUARIOS_ASIGNADOS> list = db.TBL_UNE_USUARIOS_ASIGNADOS.Where(x => x.folio == folio && x.responsable==0).ToList();

            TBL_UNE_USUARIOS_ASIGNADOS reponsableCorreo = db.TBL_UNE_USUARIOS_ASIGNADOS.Where(x => x.folio == folio && x.responsable == 1).FirstOrDefault();
            string correoResponsable;

            var claves = db.CLAVES.Where(x => x.Numusuario == reponsableCorreo.numusuario).Select(x=> new { x.Correo, x.Nombre_s, x.Apellido_materno, x.Apellido_paterno, x.Correo_externo }).FirstOrDefault();
            correoResponsable = claves.Correo;
            nombreResponsable = claves.Nombre_s + " " + claves.Apellido_paterno + " " + claves.Apellido_materno;

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
                    claves = db.CLAVES.Where(x => x.Numusuario == item.numusuario).Select(x => new { x.Correo, x.Nombre_s, x.Apellido_materno, x.Apellido_paterno, x.Correo_externo }).FirstOrDefault();
                    CC[i] = claves.Correo;
                }
                i++;
            }

            int estatus = new Correo().EnviarRechazar(correoResponsable, CC, rep, comentarios,nombreResponsable, Server.MapPath("~/Estilos/Imagenes/firmaUNE.jpg"));

            if (estatus == 1)
                db.SaveChanges();

            return estatus;
        }

        [HttpPost]
        public ActionResult RegistrarDocumentoDictamen(TBL_UNE_REPORTE reporte)
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            db = new ContextUne(cl.Usuario, cl.Contrasena);
            ContextUne db2 = new ContextUne();
            HttpPostedFileBase file = Request.Files[0];

            if (file.ContentLength>0)
            {
                TBL_UNE_REPORTE rep = db2.TBL_UNE_REPORTE.Where(x => x.FOLIO == reporte.FOLIO).FirstOrDefault();
                TBL_UNE_ARCHIVOS_ADJUNTOS adjunto = new TBL_UNE_ARCHIVOS_ADJUNTOS();
                Random rnd = new Random();
                int rdn = rnd.Next(0, 10000);
                String nombre = rdn+"_"+ Path.GetFileName(file.FileName);
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
                adjunto.ID_TIPO_ARCHIVO = 4;
                db.TBL_UNE_ARCHIVOS_ADJUNTOS.Add(adjunto);
                db.SaveChanges();
                file.SaveAs(archivo);
            }

            return RedirectToAction("ResueltoArea", "ResueltoArea");
        }
    }
}
