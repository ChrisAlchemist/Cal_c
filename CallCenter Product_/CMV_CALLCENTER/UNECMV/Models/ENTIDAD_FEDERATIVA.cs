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
    
    public partial class ENTIDAD_FEDERATIVA
    {
        public short ID_ENTIDAD_FEDERATIVA { get; set; }
        public string DESCRIPCION { get; set; }
        public int USUARIO_ALTA { get; set; }
        public Nullable<int> USUARIO_MODIFICA { get; set; }
        public System.DateTime FECHA_ALTA { get; set; }
        public Nullable<System.DateTime> FECHA_MODIFICA { get; set; }
        public string CLAVE_ESTADO { get; set; }
        public Nullable<int> ID_PROSA_ENTIDAD_F { get; set; }
        public Nullable<int> Id_Pais { get; set; }
        public Nullable<int> NIVEL_RIESGO { get; set; }
    }
}
