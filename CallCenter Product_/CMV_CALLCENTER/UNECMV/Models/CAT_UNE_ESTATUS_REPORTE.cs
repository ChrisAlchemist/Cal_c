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
    
    public partial class CAT_UNE_ESTATUS_REPORTE
    {
        public CAT_UNE_ESTATUS_REPORTE()
        {
            this.TBL_UNE_REPORTE = new HashSet<TBL_UNE_REPORTE>();
        }
    
        public int ID_ESTATUS_REPORTE { get; set; }
        public string DESCRIPCION { get; set; }
    
        public virtual ICollection<TBL_UNE_REPORTE> TBL_UNE_REPORTE { get; set; }
    }
}
