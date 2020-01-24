using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMV_CALLCENTER.Entidad
{
    public class Socio
    {
        public int idSocio { get; set; }
        public int idPersona { get; set; }
        public Int64 numeroSocio { get; set; }
        public DateTime fechaAltaPersona { get; set; }
        public int idEstatusBanca { get; set; }
        public int idPreguntaSecreta { get; set; }
        public string respuesta { get; set; }
        public int idMotivoBloqueo { get; set; }
        public int bancaActiva { get; set; }
        public DateTime fechaMotivoBloqueo { get; set; }
        public int idImagenAntiphishing { get; set; }
        
        //vigencia_contrasena_temporal
        //viene_de_bloqueo
        //id_ultima_sesion
        //fecha_ultima_sesion
        //intentos_sesion
        //intentos_respuesta
        //fecha_alta_solicitud
        //fecha_de_desbloqueo
        //contrasena_estado_cuenta
        //descripcion_bloqueo
        //descripcion_cancelacion
        //fecha_bloqueo_OTP
        //fecha_cancelacion
        //codigo_contrasena
        //fecha_codigo_contrasena
        //recuperar_contrasena
        //fecha_alta_recuperar_contrasena
    }
}