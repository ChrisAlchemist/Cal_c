//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CMV_CALLCENTER.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TBL_PRESENCE_LLAMADAS
    {
        public int contador { get; set; }
        public Nullable<int> folio_cmv { get; set; }
        public string folio_prensence { get; set; }
        public Nullable<int> call_id_presence { get; set; }
        public string ruta_audio { get; set; }
        public string tipo_llamada { get; set; }
        public string nombre_audio { get; set; }
    }
}
