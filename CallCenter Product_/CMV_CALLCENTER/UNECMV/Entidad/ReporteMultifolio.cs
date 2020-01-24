using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMV_CALLCENTER.Entidad
{
    public class ReporteMultifolio
    {
        public int estatus { get; set; }
        public string mensaje { get; set; }
        public int idIncidenciaReporte { get; set; }
        public int folioUNE { get; set; }
        public int folioAutorizacion { get; set; }
        public Int64 numeroSocio { get; set; }
        public double importeReclamo { get; set; }
        public int idMedioMovimiento { get; set; }
        public string medioMovimiento { get; set; }
        public int idTipoCuentaBanca { get; set; }
        public string tipoCuentaBanca { get; set; }
        public DateTime fechaTransacion { get; set; }
        public string fechaTransacionEnviada { get; set; }
        public int usuarioRegistra { get; set; }
        public int idSatisfacion { get; set; }
        public int idCuentaNoAfectada { get; set; }
        public int tipoTransferenciaFolioBanca { get; set; }
    }
}