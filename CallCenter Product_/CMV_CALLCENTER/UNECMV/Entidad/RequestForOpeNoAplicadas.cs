using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMV_CALLCENTER.Entidad
{
    public class RequestForOpeNoAplicadas
    {
        public int reporteFolioBanca { get; set; }
        public string fechaTransacion { get; set; }
        public string importeReclamacion { get; set; }
        public string medioDeteccionMovimiento { get; set; }
        public int folioAutorizacion { get; set; }
        public string tipoCuenta { get; set; }

        public string comentarioNotificacion { get; set; }
        public int satisfaccion { get; set; }
        public string comentariosFinalizacion { get; set; }
        public string cuentaNoAfectada { get; set; }
        public decimal montoADepositar { get; set; }
    }
}