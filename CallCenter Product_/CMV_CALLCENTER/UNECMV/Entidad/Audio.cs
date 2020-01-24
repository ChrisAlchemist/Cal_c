using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMV_CALLCENTER.Entidad
{
    public class Audio
    {
        public int idLlamada { get; set; }
        public String ruta { get; set; }
        public int estatus { get; set; }
        public String rutaOriginal { get; set; }
        public String fechaLlamada { get; set; }
        public String telefono { get; set; }
        public int TipoLlamada { get; set; }
        public int folio { get; set; }
    }
}
