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
    
    public partial class SUCURSALES
    {
        public int Id_de_Sucursal { get; set; }
        public string Descripcion { get; set; }
        public string Calle { get; set; }
        public string Numero_exterior { get; set; }
        public string Numero_interior { get; set; }
        public string Telefono { get; set; }
        public int Zona { get; set; }
        public string Ccostos { get; set; }
        public short Num_Sucursal { get; set; }
        public string Cuentabancos { get; set; }
        public int PolizaIni { get; set; }
        public int PolizaFin { get; set; }
        public string id_colonia { get; set; }
        public string correo { get; set; }
        public string cuentadisponible { get; set; }
        public int socioutilitario { get; set; }
        public int Id_Poblacion { get; set; }
        public string Ciudad { get; set; }
        public int Jef_Sucursal { get; set; }
        public int Depende_de { get; set; }
        public double Prov_Suc { get; set; }
        public int Region { get; set; }
        public int Usuario_Min { get; set; }
        public int Usuario_Max { get; set; }
        public int Monto_Max_Caja { get; set; }
        public short Grupo { get; set; }
        public int Sucursal_Bansefi { get; set; }
        public string Cuenta_Oport { get; set; }
        public string Region_Actual { get; set; }
        public Nullable<short> PLD_Tipo_Riesgo { get; set; }
        public Nullable<int> Id_Colonia_CNBV { get; set; }
        public string Clave_CNBV { get; set; }
        public string Id_Sucursal_Woccu { get; set; }
        public string CveIF_Woccu { get; set; }
        public Nullable<int> Id_Estado_Woccu { get; set; }
        public Nullable<int> Id_Mpio_Woccu { get; set; }
        public Nullable<int> Id_localid_Woccu { get; set; }
        public short Id_Horario { get; set; }
        public Nullable<int> Id_prosa { get; set; }
        public Nullable<bool> OPERA_DOMINGO { get; set; }
        public Nullable<int> baja_logica { get; set; }
        public string LATITUD { get; set; }
        public string LONGITUD { get; set; }
        public Nullable<decimal> maximo_disponible { get; set; }
        public string clave_sucursal_pld { get; set; }
        public Nullable<System.DateTime> fecha_apertura { get; set; }
    }
}
