using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using CMV_CALLCENTER.Models;
using AccesoDatos;
using System.Data;
using SmsMailUtils;
using CMV_CALLCENTER.conectaServicioSAS;
using System.ServiceModel;
using System.Web.Mvc;
using SmsMailUtils;

namespace CMV_CALLCENTER.Entidad
{
    public class Funciones
    {
        private ContextUne db = new ContextUne();
        private DBManager dbManager = null;

        public String obtenerRuta(int numFolio)
        {
            TBL_UNE_REPORTE reporte = db.TBL_UNE_REPORTE.Where(x => x.NUM_FOLIO == numFolio).FirstOrDefault();

            DateTime fecha = Convert.ToDateTime(reporte.FECHA_ALTA);
            String mes = ObtenerMes(fecha.Month);
            string ruta = ConfigurationSettings.AppSettings["rutaArchivosEvidencias"] + "\\" + reporte.CAT_UNE_TIPO_REPORTE.DESCRIPCION + "\\" + fecha.Year + "\\" + mes + "\\" + reporte.NUM_FOLIO;
            return ruta;
        }


        public String ObtenerMes(int mes)
        {
            String cadena = "Enero";
            switch (mes)
            {
                case 1:
                    cadena = "Enero";
                    break;
                case 2:
                    cadena = "Febrero";
                    break;
                case 3:
                    cadena = "Marzo";
                    break;
                case 4:
                    cadena = "Abril";
                    break;
                case 5:
                    cadena = "Mayo";
                    break;
                case 6:
                    cadena = "Junio";
                    break;
                case 7:
                    cadena = "Julio";
                    break;
                case 8:
                    cadena = "Agosto";
                    break;
                case 9:
                    cadena = "Septiembre";
                    break;
                case 10:
                    cadena = "Octubre";
                    break;
                case 11:
                    cadena = "Noviembre";
                    break;
                case 12:
                    cadena = "Diciembre";
                    break;
            }
            return cadena;
        }


        public List<SUCURSALES> ListaSucursales(string usuario, string contrasena)
        {
            db = new ContextUne(usuario, contrasena);
            var listaSucursales = db.SUCURSALES.OrderBy(x => x.Descripcion).ToList();
            foreach (var item in listaSucursales)
            {
                item.Descripcion = item.Descripcion.ToLower();
                item.Descripcion = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.Descripcion);
            }

            return listaSucursales;
        }

        public List<ENTIDAD_FEDERATIVA> ListaEntidades(string usuario, string contrasena)
        {
            db = new ContextUne(usuario, contrasena);
            var listaEntidades = db.ENTIDAD_FEDERATIVA.Where(x => x.ID_ENTIDAD_FEDERATIVA > 0).ToList();

            foreach (var item in listaEntidades)
            {
                item.DESCRIPCION = item.DESCRIPCION.ToLower();
                item.DESCRIPCION = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.DESCRIPCION);
            }

            return listaEntidades;
        }

        public List<CAT_UNE_TIPO_REPORTE> obtenerTipoReporte(string usuario, string contrasena)
        {
            db = new ContextUne(usuario, contrasena);
            var tipoReporte = db.CAT_UNE_TIPO_REPORTE.ToList();
            return tipoReporte;
        }

        public List<CAT_UNE_MEDIO_CONTACTO> obtenerMediosContacto(string usuario, string contrasena)
        {
            db = new ContextUne(usuario, contrasena);
            var mediosContacto = db.CAT_UNE_MEDIO_CONTACTO.ToList();
            return mediosContacto;
        }

        public List<PreguntaBanca> CargarCuestionario(int numero)
        {
            List<PreguntaBanca> preguntasAutentificacion = new List<PreguntaBanca>();
            List<RespuestaBanca> respuestasAutentificacion = new List<RespuestaBanca>();
            PreguntaBanca preguntaAutentificacion = null;
            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=hape;User Id=" + ConfigurationSettings.AppSettings["usuarioBase"] + ";Password=" + ConfigurationSettings.AppSettings["password"]))
                {
                    dbManager.Open();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "numero_socio", numero);
                    dbManager.ExecuteReader(CommandType.StoredProcedure, "sp_callcenter_obtener_cuestionario");

                    while (dbManager.DataReader.Read())
                    {
                        preguntaAutentificacion = new PreguntaBanca();
                        preguntaAutentificacion.idPregunta = Convert.ToInt32(dbManager.DataReader["id_pregunta"].ToString());
                        preguntaAutentificacion.Pregunta = dbManager.DataReader["descripcion_pregunta"] == DBNull.Value ? null : dbManager.DataReader["descripcion_pregunta"].ToString();
                        preguntasAutentificacion.Add(preguntaAutentificacion);
                    }
                    dbManager.DataReader.NextResult();
                    while (dbManager.DataReader.Read())
                    {
                        RespuestaBanca respuestaAutentificacion = new RespuestaBanca();

                        respuestaAutentificacion.idPregunta = Convert.ToInt32(dbManager.DataReader["id_pregunta"].ToString());
                        respuestaAutentificacion.respuesta = dbManager.DataReader["respuesta"].ToString();
                        respuestaAutentificacion.respuestaCorrecta = Convert.ToBoolean(dbManager.DataReader["correcta"].ToString());

                        respuestasAutentificacion.Add(respuestaAutentificacion);
                    }
                    preguntasAutentificacion.ForEach(x => x.respuestasAutentificacion.AddRange(respuestasAutentificacion.FindAll(y => y.idPregunta == x.idPregunta)));
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return preguntasAutentificacion;
        }

        public List<TipoNotificacion> MostrarTiposNotificacion()
        {
            List<TipoNotificacion> tiposNotificacion = new List<TipoNotificacion>();
            TipoNotificacion tipoNotificacion = null;
            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=banca;User Id=" + ConfigurationSettings.AppSettings["usuarioBase"] + ";Password=" + ConfigurationSettings.AppSettings["password"]))
                {
                    dbManager.Open();

                    dbManager.ExecuteReader(CommandType.StoredProcedure, "SP_BANCA_MOSTRAR_TIPOS_NOTIFICACION").NextResult();

                    while (dbManager.DataReader.Read())
                    {
                        tipoNotificacion = new TipoNotificacion();
                        {
                            tipoNotificacion.idTipoNotificacion = Convert.ToInt32(dbManager.DataReader["id_tipo_notificacion"].ToString());
                            tipoNotificacion.descripcion = dbManager.DataReader["descripcion"] == DBNull.Value ? null : dbManager.DataReader["descripcion"].ToString();
                        };
                        tiposNotificacion.Add(tipoNotificacion);
                    }
                    if (tiposNotificacion != null)
                        tiposNotificacion.Insert(0, new TipoNotificacion() { descripcion = "-- SELECCIONA --", idTipoNotificacion = -1 });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return tiposNotificacion;

        }
        public int ObtenerSiguienteFolioBanca()
        {
            int folio = 0;

            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=banca;User Id=" + ConfigurationSettings.AppSettings["usuarioBase"] + ";Password=" + ConfigurationSettings.AppSettings["password"]))
                {
                    dbManager.Open();
                    dbManager.ExecuteReader(CommandType.StoredProcedure, "SP_CALLCENTER_REPORTES_OBTENER_ULTIMO");
                    while (dbManager.DataReader.Read())
                    {
                        folio = Convert.ToInt32(dbManager.DataReader["siguiente_folio"]);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return folio;
        }

        public List<Notificacion> ObtenerNotificacionSocio(int Numero, int idTipoBitacora, int idTipoNotificacion, int? folioBanca, DateTime? fechaCompromiso, string medioReporte, DateTime? fechaAperturaReporte)
        {
            List<Notificacion> notificaciones = new List<Notificacion>();
            Notificacion notificacion = null;
            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=banca;User Id=" + ConfigurationSettings.AppSettings["usuarioBase"] + ";Password=" + ConfigurationSettings.AppSettings["password"]))
                {
                    dbManager.Open();
                    dbManager.CreateParameters(9);
                    dbManager.AddParameters(0, "@numeroSocio", Numero);
                    dbManager.AddParameters(1, "@id_tipo_bitacora", idTipoBitacora);
                    if (idTipoNotificacion == 0)
                        dbManager.AddParameters(2, "@id_tipo_notificacion", null);
                    else
                        dbManager.AddParameters(2, "@id_tipo_notificacion", idTipoNotificacion);
                    dbManager.AddParameters(3, "@noCel", DBNull.Value);
                    dbManager.AddParameters(4, "@correo", DBNull.Value);
                    dbManager.AddParameters(5, "@ticket_banca", folioBanca);
                    dbManager.AddParameters(6, "@fecha_compromiso", fechaCompromiso);
                    dbManager.AddParameters(7, "@medio_reporte", medioReporte);
                    dbManager.AddParameters(8, "@fecha_apertura_reporte", fechaAperturaReporte);

                    dbManager.ExecuteReader(System.Data.CommandType.StoredProcedure, "SP_BANCA_OBTENER_NOTIFICACION");
                    if (dbManager.DataReader.Read())
                    {
                        if (dbManager.DataReader["estatus"].ToString().Equals("200"))
                        {
                            if (dbManager.DataReader.NextResult())
                            {
                                while (dbManager.DataReader.Read())
                                {
                                    notificacion = new Notificacion();
                                    notificacion.celular = dbManager.DataReader["celular"] == DBNull.Value ? "" : dbManager.DataReader["celular"].ToString();
                                    if (dbManager.DataReader["correo"] != DBNull.Value)
                                    {
                                        notificacion.para.Add(dbManager.DataReader["correo"].ToString());
                                    }
                                    notificacion.idTipoNotificacion = (TIPO_NOTIFICACION)Convert.ToInt16(dbManager.DataReader["idTipoNotificacion"].ToString());
                                    notificacion.cuerpo = dbManager.DataReader["cuerpo"] == DBNull.Value ? null : dbManager.DataReader["cuerpo"].ToString();
                                    notificacion.asunto = dbManager.DataReader["asunto"] == DBNull.Value ? null : dbManager.DataReader["asunto"].ToString();
                                    notificaciones.Add(notificacion);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return notificaciones;
        }

        public static String EliminarToken(string numeroUsusario)
        {
            string result = "Success";
            try
            {
                ConectaServicesImplClient sas = new ConectaServicesImplClient();
                if (string.Equals(ObtieneUsuario(numeroUsusario), "Success", StringComparison.CurrentCultureIgnoreCase))
                {   //result = sas.EliminarUsuario(nombreUsuario);
                    //result = sas.RevocarToken(numeroUsusario);
                    if (string.Compare(result, "Success") == 0)
                        result = sas.EliminarUsuario(numeroUsusario);
                }
            }
            catch (FaultException ex)
            {
                if (String.Equals(ex.Message.Replace(" ", ""), "Error:0x01", StringComparison.CurrentCultureIgnoreCase))
                    return "Success";
                else
                    return ObtenerErrorSAS(ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        public static string ObtieneUsuario(string nombreUsuario)
        {
            User result = null;
            try
            {
                ConectaServicesImplClient sas = new ConectaServicesImplClient();
                result = sas.ObtenerUsuario(nombreUsuario);
                if (result != null)
                    return "Success";
            }
            catch (FaultException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "Success";
        }


        private static string ObtenerErrorSAS(string faultCode)
        {
            switch (faultCode.Replace(" ", "").ToString())
            {
                case "Error:0x01":
                    return faultCode + ": No existe el usuario";
                    break;
                case "Error:0x02":
                    return faultCode + ": No se pudo realizar la adición del usuario.";
                    break;
                case "Error:0x03":
                    return faultCode + ": No ha sido posible el aprovisionamiento del usuario.";
                    break;
                case "Error:0x04":
                    return faultCode + ": Ha ocurrido un error al revocar el token del usuario";
                    break;
                case "Error:0x05":
                    return faultCode + ": No ha sido posible la eliminación del usuario";
                    break;
                default:
                    return "Error desconocido " + faultCode;
                    break;
            }
            return string.Empty;
        }

        public static String AgregarAprovisionarToken(SP_BANCA_OBTIENE_SOCIO_Result socio)
        {
            try
            {
                UserCustom u = new UserCustom();
                u.FirstName = socio.Nombre_s;
                u.Email = socio.Mail;
                u.Lastname = socio.Apellido_Paterno + (string.IsNullOrEmpty(socio.Apellido_Materno) ? "" : " " + socio.Apellido_Materno);
                u.Mobile = socio.Tel_Celular;
                u.UserName = socio.Numero.ToString();

                return new ConectaServicesImplClient().AgregarAprovisionarUsuario(u);
            }
            catch (FaultException ex)
            {
                return ObtenerErrorSAS(ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool ValidaNotificaciones(TBL_UNE_REPORTE socio, int idTipoBitacora, int idTipoNotificacion, int? folioBanca, DateTime? fechaCompromiso, string medioReporte)
        {
            bool resultado = true;
            Funciones funcion = new Funciones();
            List<Notificacion> listaNotificaciones = funcion.ObtenerNotificacionSocio(Convert.ToInt32(socio.NUMERO), idTipoBitacora, idTipoNotificacion, folioBanca, fechaCompromiso, medioReporte, socio.FECHA_ALTA);

            try
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(socio.NUMERO.ToString());
                String cuerpo = "";
                foreach (var item in listaNotificaciones)
                {
                    switch (item.idTipoNotificacion)
                    {
                        case TIPO_NOTIFICACION.SMS:

                            Dictionary<string, string> resul = SmsMailUtils.SmsMail.SendSms(new SmsMailUtils.Notificacion { celular = item.celular, cuerpo = item.cuerpo });
                            resultado = Convert.ToBoolean(resul["result"].ToString() == "1" ? true : false);
                            break;
                        case TIPO_NOTIFICACION.CORREO_ELECTRONICO:
                            cuerpo = item.cuerpo.Replace("@Numero", System.Convert.ToBase64String(plainTextBytes));
                            Dictionary<string, string> resul_ = SmsMailUtils.SmsMail.SendCorreExterno(new SmsMailUtils.Notificacion { cuerpo = cuerpo, para = item.para, asunto = item.asunto });
                            resultado = Convert.ToBoolean(resul_["result"].ToString() == "1" ? true : false);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                resultado = false;
            }
            return resultado;
        }

        public bool generaLayout(GridView gridview, string ruta, List<int> quitarCols, string subReporte)
        {
            try
            {
                string line = string.Empty;
                string separador = ";";
                StreamWriter writer = new StreamWriter(ruta);

                foreach (GridViewRow row in gridview.Rows)
                {
                    line = subReporte;
                    for (int i = 2; i < gridview.HeaderRow.Cells.Count; i++)
                    {
                        IEnumerable<int> res = quitarCols.Where(s => s == i);
                        if (res.Count() == 0)
                            line += separador + row.Cells[i].Text;
                    }
                    writer.WriteLine(line);
                    line = "";
                }
                writer.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        // Funcion que crea la estructura de carpetas para los reportes regulatorios
        // Entrada:  mes y año en formato string
        // Salida: La ruta Generada en formato String
        public string crearCarpetas(string ruta, string mes, string anio)
        {
            //String ruta = "";

            try
            {
                // Creamos la Carpeta Raiz del Proyecto
                if (!Directory.Exists(ruta))
                {
                    DirectoryInfo di = Directory.CreateDirectory(ruta);
                }
                // Creamos la carpeta del Año
                if (!Directory.Exists(ruta + "\\" + anio))
                {
                    DirectoryInfo diAnio = Directory.CreateDirectory(ruta + "\\" + anio);
                }

                // Creamos la Carpeta del Mes
                if (!Directory.Exists(ruta + "\\" + anio + "\\" + mes))
                {
                    DirectoryInfo diAnio = Directory.CreateDirectory(ruta + "\\" + anio + "\\" + mes);
                }

                ruta = ruta + "\\" + anio + "\\" + mes + "\\";

            }
            catch (IOException ioex)
            {
                ruta = "C:\\";
            }
            return ruta;
        }

        public bool RegistrarBitacora(int id_tipo_bitacora, int numero_socio, int tipo_origen, int numero_usuario)
        {
            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=banca;User Id=" + ConfigurationSettings.AppSettings["usuarioBase"] + ";Password=" + ConfigurationSettings.AppSettings["password"]))
                {

                    dbManager.Open();
                    dbManager.CreateParameters(4);
                    dbManager.AddParameters(0, "@id_tipo_bitacora", id_tipo_bitacora);
                    dbManager.AddParameters(1, "@numero_socio", numero_socio);
                    dbManager.AddParameters(2, "@tipo_origen", tipo_origen);
                    dbManager.AddParameters(3, "@numero_usuario", numero_usuario);

                    dbManager.ExecuteReader(CommandType.StoredProcedure, "SP_BANCA_INSERTAR_BITACORA");
                    if (dbManager.DataReader.Read())
                    {
                        if (Convert.ToInt32(dbManager.DataReader["status"].ToString()) == 1)
                            return true;
                        else
                            throw new Exception(dbManager.DataReader["error_message"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public int InsertarComisionBanca(TBL_UNE_REPORTE reporte, SP_BANCA_OBTIENE_SOCIO_Result datosSocio, int idTipoComision, int idOrigenOperacion)
        {
            int idComision = 0;
            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=banca;User Id=" + ConfigurationSettings.AppSettings["usuarioBase"] + ";Password=" + ConfigurationSettings.AppSettings["password"]))
                {

                    dbManager.Open();
                    dbManager.CreateParameters(6);
                    dbManager.AddParameters(0, "@numero", reporte.NUMERO);
                    dbManager.AddParameters(1, "@folio_UNE", reporte.NUM_FOLIO);
                    dbManager.AddParameters(2, "@id_tipo_comision", idTipoComision);
                    dbManager.AddParameters(3, "@id_origen_operacion", idOrigenOperacion);
                    dbManager.AddParameters(4, "@numusuario", reporte.USUARIO_REGISTRA);
                    dbManager.AddParameters(5, "@id_Tipo_Persona", datosSocio.Id_Tipo_Persona);

                    dbManager.ExecuteReader(CommandType.StoredProcedure, "SP_BANCA_INSERTAR_COMISION");
                    if (dbManager.DataReader.Read())
                    {
                        if (Convert.ToInt32(dbManager.DataReader["estatus"].ToString()) == 1)
                            idComision = Convert.ToInt32(dbManager.DataReader["id_comision"].ToString());
                        else
                            throw new Exception(dbManager.DataReader["mensaje"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return idComision;

        }

        public int CobrarComisionBanca(SP_BANCA_OBTIENE_SOCIO_Result datosSocio, int idComision)
        {
            int estatusCobro = 0;
            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=banca;User Id=" + ConfigurationSettings.AppSettings["usuarioBase"] + ";Password=" + ConfigurationSettings.AppSettings["password"]))
                {

                    dbManager.Open();
                    dbManager.CreateParameters(3);
                    dbManager.AddParameters(0, "@numero", datosSocio.Numero);
                    dbManager.AddParameters(1, "@id_comision", idComision);
                    dbManager.AddParameters(2, "@id_tipo_persona", datosSocio.Id_Tipo_Persona);
                    dbManager.ExecuteReader(CommandType.StoredProcedure, "SP_BANCA_REALIZA_COBRO_COMISION");
                    if (dbManager.DataReader.Read())
                    {
                        if (Convert.ToInt32(dbManager.DataReader["estatus"].ToString()) == 1)
                            estatusCobro = Convert.ToInt32(dbManager.DataReader["estatus"].ToString());
                        //else
                        //throw new Exception(dbManager.DataReader["mensaje"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return estatusCobro;

        }

        public bool ValidarFolioBanca(Int64 folio, Int64 numeroSocio)
        {
            bool folioCorrecto = false;
            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=banca;User Id=" + ConfigurationSettings.AppSettings["usuarioBase"] + ";Password=" + ConfigurationSettings.AppSettings["password"]))
                {
                    dbManager.Open();
                    dbManager.CreateParameters(2);
                    dbManager.AddParameters(0, "@id_banca_folio", folio);
                    dbManager.AddParameters(1, "@numero_socio", numeroSocio);
                    dbManager.ExecuteReader(CommandType.StoredProcedure, "SP_BANCA_VALIDA_FOLIO_SOCIO");
                    if (dbManager.DataReader.Read())
                    {
                        if (Convert.ToInt32(dbManager.DataReader["estatus"].ToString()) == 1)
                        {
                            folioCorrecto = true;
                        }
                        else
                        {
                            folioCorrecto = false;
                            //throw new Exception(dbManager.DataReader["error_message"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return folioCorrecto;


        }

        public Boolean ValidaTienePrestamo(Int64 numeroSocio)
        {
            bool tienePrestamo = true;
            int estatus;

            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=hape;User Id=" + ConfigurationSettings.AppSettings["usuarioBase"] + ";Password=" + ConfigurationSettings.AppSettings["password"]))
                {
                    dbManager.Open();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@numero_socio", numeroSocio);
                    dbManager.ExecuteReader(System.Data.CommandType.StoredProcedure, "SP_BANCA_OBTENER_PRESTAMOS_SOCIO");
                    if (dbManager.DataReader.Read())
                    {
                        estatus = Convert.ToInt32(dbManager.DataReader["estatus"].ToString());
                        if (estatus == 1)
                        {
                            tienePrestamo = true;
                        }

                    }
                    else
                    {
                        tienePrestamo = false;
                    }

                    return tienePrestamo;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public long DecifrarNumero(Int64 numeroCifrado)
        {
            try
            {

                using (ContextBanca bancaContext = new ContextBanca())
                {
                    string numeroDescifrado = bancaContext.Database.SqlQuery<string>("select dbo.FN_BANCA_DESCIFRAR(" + numeroCifrado + ")").Single().ToString();
                    //Response.Write(str); //output:'Hey this works'
                    return Convert.ToInt64(numeroDescifrado);
                }


            }
            catch (Exception)
            {

                throw;
            }
        }

        public Socio ObtenerSocioRegistradoBanca(Int64 numeroSocio)
        {
            Socio socio = null;
            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=banca;User Id=" + ConfigurationSettings.AppSettings["usuarioBase"] + ";Password=" + ConfigurationSettings.AppSettings["password"]))
                {
                    dbManager.Open();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@numero_socio", numeroSocio);
                    dbManager.ExecuteReader(CommandType.StoredProcedure, "SP_BANCA_OBTENER_SOCIO_REGISTRADO");
                    if (dbManager.DataReader.Read())
                    {
                        socio = new Socio();
                        socio.idSocio = Convert.ToInt32(dbManager.DataReader["id_socio"]);
                        socio.idPersona = Convert.ToInt32(dbManager.DataReader["id_persona"]);
                        socio.numeroSocio = Convert.ToInt64(dbManager.DataReader["numero_socio"]);
                        socio.fechaAltaPersona = dbManager.DataReader["fecha_alta_persona"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dbManager.DataReader["fecha_alta_persona"].ToString());
                        socio.idEstatusBanca = dbManager.DataReader["id_estatus_banca"] == DBNull.Value ? 0 : Convert.ToInt32(dbManager.DataReader["id_estatus_banca"].ToString());
                        socio.idPreguntaSecreta = dbManager.DataReader["id_pregunta_secreta"] == DBNull.Value ? 0 : Convert.ToInt32(dbManager.DataReader["id_pregunta_secreta"].ToString());
                        socio.respuesta = dbManager.DataReader["respuesta"].ToString();
                        socio.idMotivoBloqueo = dbManager.DataReader["id_motivo_bloqueo"] == DBNull.Value ? 0 : Convert.ToInt32(dbManager.DataReader["id_motivo_bloqueo"].ToString());

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return socio;

        }

        public List<ReporteMultifolio> ObtenerMultifoliosSocio(Int64 folioUNE, string Usuario, string Contrasena)
        {
            List<ReporteMultifolio> foliosSocio = new List<ReporteMultifolio>();
            ReporteMultifolio folioSocio = null;
            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=banca;User Id=" + Usuario/*ConfigurationSettings.AppSettings["usuarioBase"]*/ + ";Password=" + Contrasena/*ConfigurationSettings.AppSettings["password"]*/))
                {
                    dbManager.Open();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@folioUNE", folioUNE);
                    dbManager.ExecuteReader(CommandType.StoredProcedure, "SP_CALLCENTER_OBTENER_FOLIOS_MULTIFOLIO");

                    while (dbManager.DataReader.Read())
                    {
                        int estatus = Convert.ToInt32(dbManager.DataReader["estatus"].ToString());
                        if (estatus == 1)
                        {
                            folioSocio = new ReporteMultifolio();
                            folioSocio.idIncidenciaReporte = Convert.ToInt32(dbManager.DataReader["ID_INCIDENCIA_REPORTE"].ToString());
                            folioSocio.folioUNE = dbManager.DataReader["FOLIO_UNE"] == DBNull.Value ? 0 : Convert.ToInt32(dbManager.DataReader["FOLIO_UNE"].ToString());
                            folioSocio.folioAutorizacion = dbManager.DataReader["FOLIO_AUTORIZACION"] == DBNull.Value ? 0 : Convert.ToInt32(dbManager.DataReader["FOLIO_AUTORIZACION"].ToString());
                            folioSocio.numeroSocio = dbManager.DataReader["NUMERO_SOCIO"] == DBNull.Value ? 0 : Convert.ToInt64(dbManager.DataReader["NUMERO_SOCIO"].ToString());
                            folioSocio.importeReclamo = dbManager.DataReader["IMPORTE_RECLAMADO"] == DBNull.Value ? 0 : Convert.ToDouble(dbManager.DataReader["IMPORTE_RECLAMADO"].ToString());
                            folioSocio.idMedioMovimiento = dbManager.DataReader["ID_MEDIO_MOVIMIENTO"] == DBNull.Value ? 0 : Convert.ToInt32(dbManager.DataReader["ID_MEDIO_MOVIMIENTO"].ToString());
                            folioSocio.medioMovimiento = dbManager.DataReader["MEDIO_MOVIMIENTO"].ToString();
                            folioSocio.idTipoCuentaBanca = dbManager.DataReader["ID_TIPO_CUENTA_BANCA"] == DBNull.Value ? 0 : Convert.ToInt32(dbManager.DataReader["ID_TIPO_CUENTA_BANCA"].ToString());
                            folioSocio.tipoCuentaBanca = dbManager.DataReader["TIPO_CUENTA_BANCA"].ToString();
                            folioSocio.fechaTransacion = dbManager.DataReader["FECHA_TRANSACION"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dbManager.DataReader["FECHA_TRANSACION"].ToString());
                            folioSocio.usuarioRegistra = dbManager.DataReader["USUARIO_REGISTRA"] == DBNull.Value ? 0 : Convert.ToInt32(dbManager.DataReader["USUARIO_REGISTRA"].ToString());
                            folioSocio.idSatisfacion = dbManager.DataReader["ID_SATISFACTORIO"] == DBNull.Value ? 0 : Convert.ToInt32(dbManager.DataReader["ID_SATISFACTORIO"].ToString());
                            folioSocio.idCuentaNoAfectada = dbManager.DataReader["ID_CUENTA_NO_AFECTADA_BANCA"] == DBNull.Value ? 0 : Convert.ToInt32(dbManager.DataReader["ID_CUENTA_NO_AFECTADA_BANCA"].ToString());
                            foliosSocio.Add(folioSocio);
                        }
                        //else
                        //{
                        //    folioSocio.mensaje = dbManager.DataReader["estatus"].ToString();
                        //}

                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return foliosSocio;

        }

        public int InsertarReporteMultifolio(ReporteMultifolio reporteMultifolio, int numUsuario, string Usuario, string Contrasena)
        {
            int estatus = 0;

            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=banca;User Id=" + Usuario + ";Password=" + Contrasena))
                {
                    dbManager.Open();
                    dbManager.CreateParameters(8);
                    dbManager.AddParameters(0, "@folioUNE", reporteMultifolio.folioUNE);
                    dbManager.AddParameters(1, "@folioAutorizacion", reporteMultifolio.folioAutorizacion);
                    dbManager.AddParameters(2, "@numeroSocio", reporteMultifolio.numeroSocio);
                    dbManager.AddParameters(3, "@importeReclamo", reporteMultifolio.importeReclamo);
                    dbManager.AddParameters(4, "@idMedioMov", reporteMultifolio.idMedioMovimiento);
                    dbManager.AddParameters(5, "@idTipoCuentaBanca", reporteMultifolio.idTipoCuentaBanca);
                    dbManager.AddParameters(6, "@fechaTransacion", reporteMultifolio.fechaTransacionEnviada);
                    dbManager.AddParameters(7, "@usuarioRegistra", numUsuario);
                    dbManager.ExecuteReader(System.Data.CommandType.StoredProcedure, "SP_CALLCENTER_INSERTAR_FOLIO_MULTIFOLIO");
                    if (dbManager.DataReader.Read())
                    {
                        estatus = Convert.ToInt32(dbManager.DataReader["estatus"].ToString());
                        //if (estatus == 1)
                        //{
                        //    return estatus;
                    }

                    //}
                    //else
                    //{
                    //    tienePrestamo = false;
                    //}


                }
                return estatus;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int EliminarReporteMultifolio(int idIncidenciaReporte, string Usuario, string Contrasena)
        {
            int estatus = 0;

            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=banca;User Id=" + Usuario + ";Password=" + Contrasena))
                {
                    dbManager.Open();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@idIncidenciaReporte", idIncidenciaReporte);
                    dbManager.ExecuteReader(System.Data.CommandType.StoredProcedure, "SP_CALLCENTER_ELIMINAR_FOLIO_MULTIFOLIO");
                    if (dbManager.DataReader.Read())
                    {
                        estatus = Convert.ToInt32(dbManager.DataReader["estatus"].ToString());
                    }

                }
                return estatus;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public SP_CALLCENTER_INSERTAR_REGISTRO_REPORTE_Result InsertarRegistroReporteBanca(Nullable<long> numero_Socio, Nullable<long> iD_SUPUESTOS_REPORTE, string detalles_llamada, string numero_usuario, Nullable<int> id_tipo_bitacora, string Usuario, string Contrasena)
        {
            SP_CALLCENTER_INSERTAR_REGISTRO_REPORTE_Result registroReporteResult = null;

            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=banca;User Id=" + Usuario + ";Password=" + Contrasena))
                {
                    dbManager.Open();
                    dbManager.CreateParameters(5);
                    dbManager.AddParameters(0, "@numero_Socio", numero_Socio);
                    dbManager.AddParameters(1, "@ID_SUPUESTOS_REPORTE", iD_SUPUESTOS_REPORTE);
                    dbManager.AddParameters(2, "@detalles_llamada", detalles_llamada);
                    dbManager.AddParameters(3, "@numero_usuario", numero_usuario);
                    dbManager.AddParameters(4, "@id_tipo_bitacora", id_tipo_bitacora);
                    dbManager.ExecuteReader(System.Data.CommandType.StoredProcedure, "SP_CALLCENTER_INSERTAR_REGISTRO_REPORTE");
                    if (dbManager.DataReader.Read())
                    {
                        //estatus = Convert.ToInt32(dbManager.DataReader["estatus"].ToString());
                        registroReporteResult.status = Convert.ToInt32(dbManager.DataReader["status"].ToString());
                        registroReporteResult.error_procedure = dbManager.DataReader["error_procedure"].ToString();
                        registroReporteResult.error_line = dbManager.DataReader["error_line"].ToString();
                        registroReporteResult.error_severity = dbManager.DataReader["error_severity"].ToString();
                        registroReporteResult.error_message = dbManager.DataReader["error_message"].ToString();
                        registroReporteResult.id_reporte = Convert.ToInt32(dbManager.DataReader["id_reporte"].ToString());
                    }

                }
                return registroReporteResult;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public ReporteMultifolio ObtenerDetalleFolioBanca(Int64 IdIncidenciaReporte, string Usuario, string Contrasena)
        {
            
            ReporteMultifolio folioSocio = null;
            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=banca;User Id=" + Usuario/*ConfigurationSettings.AppSettings["usuarioBase"]*/ + ";Password=" + Contrasena/*ConfigurationSettings.AppSettings["password"]*/))
                {
                    dbManager.Open();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@IdIncidenciaReporte", IdIncidenciaReporte);
                    dbManager.ExecuteReader(CommandType.StoredProcedure, "SP_CALLCENTER_OBTENER_FOLIO_A_EDITAR");

                    if(dbManager.DataReader.Read())
                    {
                        int estatus = Convert.ToInt32(dbManager.DataReader["estatus"].ToString());
                        if (estatus == 1)
                        {
                            folioSocio = new ReporteMultifolio();
                            folioSocio.idIncidenciaReporte = Convert.ToInt32(dbManager.DataReader["ID_INCIDENCIA_REPORTE"].ToString());
                            folioSocio.folioUNE = dbManager.DataReader["FOLIO_UNE"] == DBNull.Value ? 0 : Convert.ToInt32(dbManager.DataReader["FOLIO_UNE"].ToString());
                            folioSocio.folioAutorizacion = dbManager.DataReader["FOLIO_AUTORIZACION"] == DBNull.Value ? 0 : Convert.ToInt32(dbManager.DataReader["FOLIO_AUTORIZACION"].ToString());
                            folioSocio.numeroSocio = dbManager.DataReader["NUMERO_SOCIO"] == DBNull.Value ? 0 : Convert.ToInt64(dbManager.DataReader["NUMERO_SOCIO"].ToString());
                            folioSocio.importeReclamo = dbManager.DataReader["IMPORTE_RECLAMADO"] == DBNull.Value ? 0 : Convert.ToDouble(dbManager.DataReader["IMPORTE_RECLAMADO"].ToString());
                            folioSocio.idMedioMovimiento = dbManager.DataReader["ID_MEDIO_MOVIMIENTO"] == DBNull.Value ? 0 : Convert.ToInt32(dbManager.DataReader["ID_MEDIO_MOVIMIENTO"].ToString());
                            folioSocio.medioMovimiento = dbManager.DataReader["MEDIO_MOVIMIENTO"].ToString();
                            folioSocio.idTipoCuentaBanca = dbManager.DataReader["ID_TIPO_CUENTA_BANCA"] == DBNull.Value ? 0 : Convert.ToInt32(dbManager.DataReader["ID_TIPO_CUENTA_BANCA"].ToString());
                            folioSocio.tipoCuentaBanca = dbManager.DataReader["TIPO_CUENTA_BANCA"].ToString();
                            folioSocio.fechaTransacion = dbManager.DataReader["FECHA_TRANSACION"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dbManager.DataReader["FECHA_TRANSACION"].ToString());
                            folioSocio.usuarioRegistra = dbManager.DataReader["USUARIO_REGISTRA"] == DBNull.Value ? 0 : Convert.ToInt32(dbManager.DataReader["USUARIO_REGISTRA"].ToString());

                            if(folioSocio.folioAutorizacion != 0)
                            {
                                folioSocio.tipoTransferenciaFolioBanca = dbManager.DataReader["TIPO_TRANSFERENCIA_FOLIO"] == DBNull.Value ? 0 : Convert.ToInt32(dbManager.DataReader["TIPO_TRANSFERENCIA_FOLIO"].ToString());
                            }

                            
                        }
                        

                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return folioSocio;

        }

        public int EditarReporteMultifolio(ReporteMultifolio reporteMultifolio, string Usuario, string Contrasena)
        {
            int estatus = 0;

            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=banca;User Id=" + Usuario + ";Password=" + Contrasena))
                {
                    dbManager.Open();
                    dbManager.CreateParameters(5);
                    dbManager.AddParameters(0, "@idIncidenciaReporte", reporteMultifolio.idIncidenciaReporte);
                    dbManager.AddParameters(1, "@folioAutorizacion", reporteMultifolio.folioAutorizacion);
                    dbManager.AddParameters(2, "@importeReclamo", reporteMultifolio.importeReclamo);
                    dbManager.AddParameters(3, "@idSatisfacion", reporteMultifolio.idSatisfacion);
                    dbManager.AddParameters(4, "@idCuentaNoAfectada", reporteMultifolio.idCuentaNoAfectada);
                    
                    dbManager.ExecuteReader(System.Data.CommandType.StoredProcedure, "SP_CALLCENTER_EDITAR_FOLIO_MULTIFOLIO");
                    if (dbManager.DataReader.Read())
                    {
                        estatus = Convert.ToInt32(dbManager.DataReader["estatus"].ToString());
                        
                    }
                    
                }
                return estatus;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int ObtenerMultifoliosPendientes(Int64 folioUNE, string Usuario, string Contrasena)
        {
            int reportesPendientes = 0;
            try
            {                
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=banca;User Id=" + Usuario/*ConfigurationSettings.AppSettings["usuarioBase"]*/ + ";Password=" + Contrasena/*ConfigurationSettings.AppSettings["password"]*/))
                {
                    dbManager.Open();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@folioUNE", folioUNE);
                    dbManager.ExecuteReader(CommandType.StoredProcedure, "SP_CALLCENTER_OBTENER_MULTIFOLIOS_PENDIENTES");

                    if (dbManager.DataReader.Read())
                    {
                        reportesPendientes = Convert.ToInt32(dbManager.DataReader["reportes_pendientes"].ToString());
                        
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return reportesPendientes;

        }

        public int ValidarReporteProcedenteBanca(int folioUNE)
        {
            int reporteDictamen = 0;
            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=banca;User Id=" + ConfigurationSettings.AppSettings["usuarioBaseUNE"] + ";Password=" + ConfigurationSettings.AppSettings["password"]))
                {
                    dbManager.Open();
                    dbManager.CreateParameters(1);
                    dbManager.AddParameters(0, "@folioUNE", folioUNE);                    
                    dbManager.ExecuteReader(CommandType.StoredProcedure, "SP_CALLCENTER_VALIDA_REPORTE_MULTIFOLIO_PROCEDENTES");
                    if (dbManager.DataReader.Read())
                    {
                        reporteDictamen = Convert.ToInt32(dbManager.DataReader["estatus"].ToString());                        
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return reporteDictamen;
            
        }


        public int RegistraLlamadaCallCenter(IframeCallCenter iframeCallCenter, DatosReporteCallCenter DatosReporte)
        {
            int llamadaRegistrada = 0;
            try
            {
                //using (dbManager = new DBManager("Server=cmv8049;Database=une;User Id=Presence;Password=Abcde1"))
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=une;User Id=" + ConfigurationSettings.AppSettings["usuarioBaseUNE"] + ";Password=" + ConfigurationSettings.AppSettings["password"]))
                {
                    dbManager.Open();
                    dbManager.CreateParameters(9);
                    dbManager.AddParameters(0, "@vContactID", iframeCallCenter.vContactID);
                    dbManager.AddParameters(1, "@vCallType", iframeCallCenter.vCallType);
                    dbManager.AddParameters(2, "@vPhone", iframeCallCenter.vPhone);
                    dbManager.AddParameters(3, "@vServiceID", iframeCallCenter.vServiceID);
                    dbManager.AddParameters(4, "@vAgenteID", iframeCallCenter.vAgenteID);
                    dbManager.AddParameters(5, "@folio", DatosReporte.folio);
                    dbManager.AddParameters(6, "@moduloAtencion", DatosReporte.moduloAtencion);
                    dbManager.AddParameters(7, "@nombreAgente", iframeCallCenter.AgenteAtiende);
                    if (DatosReporte.moduloAtencion == Enumeraciones.CAT_MODULO_ATENCION.FRAUDES)
                    {
                        dbManager.AddParameters(8, "@idSeguimiento", iframeCallCenter.vID_CC_Fraudes);
                    }
                    else if (DatosReporte.moduloAtencion == Enumeraciones.CAT_MODULO_ATENCION.CALL_CENTER)
                    {
                        dbManager.AddParameters(8, "@idSeguimiento", iframeCallCenter.vID_Fraudes_CC);
                    }


                    dbManager.ExecuteReader(CommandType.StoredProcedure, "SP_CALLCENTER_REGISTRAR_LLAMADA");
                    if (dbManager.DataReader.Read())
                    {
                        llamadaRegistrada = Convert.ToInt32(dbManager.DataReader["estatus"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return llamadaRegistrada;

        }

        public int ActualizarRegistroLlamada(IframeCallCenter iframeCallCenter, DatosReporteCallCenter DatosReporte)
        {
            int llamadaRegistrada = 0;
            try
            {
                using (dbManager = new DBManager("Server=" + ConfigurationSettings.AppSettings["servidorBD"] + ";Database=une;User Id=" + ConfigurationSettings.AppSettings["usuarioBaseUNE"] + ";Password=" + ConfigurationSettings.AppSettings["password"]))
                {
                    dbManager.Open();
                    dbManager.CreateParameters(3);
                    dbManager.AddParameters(0, "@contactID", iframeCallCenter.vContactID);
                    dbManager.AddParameters(1, "@crmFolio", DatosReporte.folioUNE);
                    dbManager.AddParameters(2, "@folioAtencion", DatosReporte.folio);

                    dbManager.ExecuteReader(CommandType.StoredProcedure, "SP_CALLCENTER_ACTUALIZAR_GRABACION_DATOS");
                    if (dbManager.DataReader.Read())
                    {
                        llamadaRegistrada = Convert.ToInt32(dbManager.DataReader["estatus"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return llamadaRegistrada;

        }

    }
}
