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
    
    public partial class TBL_UNE_ARCHIVOS_ADJUNTOS
    {
        public int ID_ARCHIVO_ADJUNTAR { get; set; }
        public Nullable<int> FOLIO { get; set; }
        public string RUTA_ARCHIVO { get; set; }
        public Nullable<System.DateTime> FECHA_ALTA { get; set; }
        public Nullable<int> ID_TIPO_ARCHIVO { get; set; }
        public string NOMBRE_ARCHIVO { get; set; }
        public Nullable<int> NUMUSUARIO { get; set; }
    
        public virtual TBL_UNE_TIPO_ARCHIVO TBL_UNE_TIPO_ARCHIVO { get; set; }
    }
}
