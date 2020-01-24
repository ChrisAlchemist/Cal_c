using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMV_CALLCENTER.Models;
using CMV_CALLCENTER.Entidad;

namespace CMV_CALLCENTER.Controllers
{
    public class ResueltoAreaController : Controller
    {
        private ContextUne db = new ContextUne();

        //
        // GET: /ResueltoArea/

        public ActionResult ResueltoArea()
        {
            CLAVES cl = Session["SesionUsuario"] as CLAVES;
            TBL_UNE_PERMISOS_ADMIN permisos = Session["permiso"] as TBL_UNE_PERMISOS_ADMIN;
            if(cl!=null)
            {
                int? tipoJefe = Session["tipoJefe"] as int?;
                db = new ContextUne(cl.Usuario, cl.Contrasena);
                ViewData["causas"] = db.CAT_UNE_SUPUESTOS_REPORTE.ToList();

                if(permisos.USUARIO_CALL_CENTER == true)
                {
                    ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => x.ID_ESTATUS_REPORTE == 4 && x.reporte_banca == true).ToList();

                    ViewData["listaReponsables"] = (from t in db.TBL_UNE_REPORTE
                                                    join u in db.TBL_UNE_USUARIOS_ASIGNADOS
                                                    on t.FOLIO equals u.folio
                                                    where ((t.ID_ESTATUS_REPORTE == 4 && t.reporte_banca == true) && u.responsable == 1 && t.reporte_banca == true)
                                                    select t).ToList().GroupBy(p => p.FOLIO).Select(g => g.FirstOrDefault()).ToList();

                }
                else if (permisos.USUARIO_UNE == 1)
                {
                    ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => x.ID_ESTATUS_REPORTE == 4 && x.reporte_banca == false).ToList();
                }
                else if (tipoJefe == 4)
                {
                    string regionActual = db.SUCURSALES.Where(x => x.Id_de_Sucursal == cl.Id_de_sucursal).Select(x => x.Region_Actual).FirstOrDefault();

                    ViewData["listaReportes"] = (from t in db.TBL_UNE_REPORTE
                                         join s in db.SUCURSALES
                                         on t.ID_SUCURSAL_REGISTRO equals s.Id_de_Sucursal
                                         where (s.Region_Actual.Contains(regionActual) && t.ID_ESTATUS_REPORTE == 4)
                                         select t).ToList();
                }
                else
                {
                    ViewData["listaReportes"] = db.TBL_UNE_REPORTE.Where(x => x.ID_ESTATUS_REPORTE == 4 && x.USUARIO_REGISTRA == cl.Numusuario).ToList();
                }
                Session["urlAnterior"] = System.Web.HttpContext.Current.Request.Url.AbsolutePath.ToString();
                Session["segmento1"] = System.Web.HttpContext.Current.Request.Url.Segments[2];
                return View();
            }
            else
                return RedirectToAction("Index", "Login");
        }


        public int numeroRespuesta(int folio)
        {
            db = new ContextUne();

            TBL_UNE_USUARIOS_ASIGNADOS can = new TBL_UNE_USUARIOS_ASIGNADOS();
            can = db.TBL_UNE_USUARIOS_ASIGNADOS.Where(x => x.folio == folio).FirstOrDefault();

            int numeroRespuestas = db.TBL_UNE_CANALIZACIONES.Count(x => x.numusuario == can.numusuario && x.FOLIO == can.folio);

            return numeroRespuestas;
        }

        public DateTime obtenerFecha(int folio, String Usuario, String Contrasena)
        {
            db = new ContextUne(Usuario, Contrasena);
            var comentario = db.SP_UNE_CARGA_COMENTARIO(folio, 1).FirstOrDefault();
            return Convert.ToDateTime(comentario.fecha_alta);
        }

    }
}
