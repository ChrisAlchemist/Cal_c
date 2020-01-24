using AccesoDatos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Models;

namespace CMV_CALLCENTER.Controllers
{
    public class LoginController : Controller
    {
        private ContextUne db = new ContextUne();

        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ValidaLogin(CLAVES clave)
        {
            if (ModelState.IsValid)
            {
                String ContrasenaEnc = EncrypSHA1.EnciptaSHA1(clave.Numusuario.ToString(), clave.Contrasena).ToUpper().Substring(0, 30);
                try
                {
                    CLAVES buscaClave = db.CLAVES.FirstOrDefault(f => f.Contrasena == ContrasenaEnc && f.Numusuario == clave.Numusuario && f.Usuario == clave.Usuario);
                    if (buscaClave != null)
                    {

                        TBL_UNE_PERMISOS_ADMIN permisoAcceso = db.TBL_UNE_PERMISOS_ADMIN.Where(x => x.ID_ROL == buscaClave.Id_Rol && x.ACTIVO == 1).FirstOrDefault();
                        if (permisoAcceso != null)
                        {
                            //var a=(from p in db.CLAVES where p.Contrasena==ContrasenaEnc && p.Numusuario==clave.Numusuario && p.Usuario==clave.Usuario);;//.FirstOrDefault
                            //String conexion = "data source=cmv8008\\proyecto1;initial catalog=HAPE;user id="+buscaClave.Numusuario+";password="+buscaClave.Contrasena+";MultipleActiveResultSets=True;App=EntityFramework";
                            var tipoJefe = db.SICORP_ROLES.Where(x => x.Id_Rol == permisoAcceso.ID_ROL).FirstOrDefault();
                            Random rnd = new Random();
                            int sesion = rnd.Next(1, 100000);
                            Session["permiso"] = permisoAcceso;
                            Session["SesionUsuario"] = buscaClave;
                            Session["IdSesion"] = Session.SessionID;
                            Session["tipoJefe"] = tipoJefe.Id_tipo_jefe;
                            Session["urlAnterior"] = "";
                            String url = "";
                            if (TempData["UrlNav"] != null)
                                url = TempData["UrlNav"].ToString();

                            //var proc = Process.Start(Server.MapPath("~/" + ConfigurationManager.AppSettings["Banca"]));
                            //proc.WaitForExit();

                            if (url != "")
                                return Redirect(url);
                            else if (permisoAcceso.PERMISO_REGISTRAR == 1 || permisoAcceso.USUARIO_UNE == 1)
                                return RedirectToAction("Registros", "Registros");
                            else
                                return RedirectToAction("Inicio", "Inicio");
                        }
                        else
                        {
                            ModelState.AddModelError("Contrasena", "Permiso denegado para acceder a la aplicación");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Contrasena", "Usuario/Contraseña incorrecta, revise sus claves por favor.");
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
            return View("index");
        }

        public ActionResult LoginPorURL(int NumUsuario,string Usuario,string Contrasena, int RegistraCallCenter)
        {
            try
            {
                CLAVES buscaClave = db.CLAVES.FirstOrDefault(f => f.Contrasena == Contrasena && f.Numusuario == NumUsuario && f.Usuario == Usuario);
                if (buscaClave != null)
                {

                    TBL_UNE_PERMISOS_ADMIN permisoAcceso = db.TBL_UNE_PERMISOS_ADMIN.Where(x => x.ID_ROL == buscaClave.Id_Rol && x.ACTIVO == 1).FirstOrDefault();
                    if (permisoAcceso != null)
                    {
                        var tipoJefe = db.SICORP_ROLES.Where(x => x.Id_Rol == permisoAcceso.ID_ROL).FirstOrDefault();
                        Random rnd = new Random();
                        int sesion = rnd.Next(1, 100000);
                        Session["permiso"] = permisoAcceso;
                        Session["SesionUsuario"] = buscaClave;
                        Session["IdSesion"] = Session.SessionID;
                        Session["tipoJefe"] = tipoJefe.Id_tipo_jefe;
                        Session["urlAnterior"] = "";
                        String url = "";
                        TempData["SesionUsuario"] = buscaClave;
                        if(RegistraCallCenter == 1)
                        {
                            //permisoAcceso.USUARIO_CALL_CENTER = true;
                            permisoAcceso.USUARIO_SUCURSAL = true;
                            permisoAcceso.USUARIO_CALL_CENTER = true;
                        }
                        if (TempData["UrlNav"] != null)
                            url = TempData["UrlNav"].ToString();

                        if (url != "")
                            return Redirect(url);
                        else if (permisoAcceso.PERMISO_REGISTRAR == 1 || permisoAcceso.USUARIO_UNE == 1)
                            return Redirect("~/Registros/Registros");
                        else
                            return Redirect("~/Inicio/Inicio");
                    }
                    else
                    {
                        return RedirectToAction("Inicio", "Inicio");
                    }
                }
                else
                {
                    return RedirectToAction("Inicio", "Inicio");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //
        // GET: /Login/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Login/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CLAVES claves)
        {
            if (ModelState.IsValid)
            {
                db.CLAVES.Add(claves);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(claves);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}