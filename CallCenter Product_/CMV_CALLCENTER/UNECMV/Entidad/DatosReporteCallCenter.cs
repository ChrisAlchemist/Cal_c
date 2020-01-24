using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMV_CALLCENTER.Entidad
{
    public class DatosReporteCallCenter
    {
        public Int64 folio { get; set; }
        public Enumeraciones.CAT_MODULO_ATENCION moduloAtencion { get; set; }
        public int? folioUNE { get; set; }
    }
}