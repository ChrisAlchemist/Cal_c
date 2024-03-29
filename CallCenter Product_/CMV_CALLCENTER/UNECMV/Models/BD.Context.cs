﻿//------------------------------------------------------------------------------
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
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class ContextUne : DbContext
    {
        public ContextUne()
            : base("name=ContextUne")
        {
        }
        public ContextUne(String usuario, String contrasena) : base("name=ContextUne")
        {
            String conexion = "data source=" + ConfigurationSettings.AppSettings["servidorBD"] + ";initial catalog=HAPE;user id=" + usuario + ";password=" + contrasena + ";MultipleActiveResultSets=True;App=EntityFramework";
            Database.Connection.ConnectionString = conexion;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<CAT_UNE_ESTATUS_REPORTE> CAT_UNE_ESTATUS_REPORTE { get; set; }
        public DbSet<CAT_UNE_MEDIO_CONTACTO> CAT_UNE_MEDIO_CONTACTO { get; set; }
        public DbSet<CAT_UNE_SUPUESTOS_REPORTE> CAT_UNE_SUPUESTOS_REPORTE { get; set; }
        public DbSet<CAT_UNE_TIPO_REPORTE> CAT_UNE_TIPO_REPORTE { get; set; }
        public DbSet<TBL_UNE_CANALIZACIONES> TBL_UNE_CANALIZACIONES { get; set; }
        public DbSet<TBL_UNE_REPORTE> TBL_UNE_REPORTE { get; set; }
        public DbSet<CLAVES> CLAVES { get; set; }
        public DbSet<TBL_PRESENCE_LLAMADAS> TBL_PRESENCE_LLAMADAS { get; set; }
        public DbSet<TBL_UNE_USUARIOS_ASIGNADOS> TBL_UNE_USUARIOS_ASIGNADOS { get; set; }
        public DbSet<ENTIDAD_FEDERATIVA> ENTIDAD_FEDERATIVA { get; set; }
        public DbSet<CAT_UNE_CUENTAS> CAT_UNE_CUENTAS { get; set; }
        public DbSet<CAT_UNE_TIPO_CUENTA> CAT_UNE_TIPO_CUENTA { get; set; }
        public DbSet<TBL_UNE_RESPUESTA_SATISFACTORIA> TBL_UNE_RESPUESTA_SATISFACTORIA { get; set; }
        public DbSet<SUCURSALES> SUCURSALES { get; set; }
        public DbSet<TBL_UNE_PERMISOS_ADMIN> TBL_UNE_PERMISOS_ADMIN { get; set; }
        public DbSet<TBL_UNE_ARCHIVOS_ADJUNTOS> TBL_UNE_ARCHIVOS_ADJUNTOS { get; set; }
        public DbSet<TBL_UNE_TIPO_ARCHIVO> TBL_UNE_TIPO_ARCHIVO { get; set; }
        public DbSet<TBL_UNE_PIVOTE_REPORTE> TBL_UNE_PIVOTE_REPORTE { get; set; }
        public DbSet<CAT_UNE_PROCEDE_DEBITO> CAT_UNE_PROCEDE_DEBITO { get; set; }
        public DbSet<TBL_UNE_COMISIONES> TBL_UNE_COMISIONES { get; set; }
        public DbSet<CAT_UNE_FINALIZADO_DEBITO> CAT_UNE_FINALIZADO_DEBITO { get; set; }
        public DbSet<SICORP_ROLES> SICORP_ROLES { get; set; }
        public DbSet<TBL_UNE_CONVERSION_LLAMADAS> TBL_UNE_CONVERSION_LLAMADAS { get; set; }
        public DbSet<CAT_UNE_PROCEDE_CORRESPONSALIAS> CAT_UNE_PROCEDE_CORRESPONSALIAS { get; set; }
        public DbSet<CAT_UNE_TIPO_CUENTA_BANCA> CAT_UNE_TIPO_CUENTA_BANCA { get; set; }
        public DbSet<CAT_UNE_CANAL> CAT_UNE_CANAL { get; set; }
        public DbSet<CAT_UNE_CAUSA_RESOLUCION> CAT_UNE_CAUSA_RESOLUCION { get; set; }
        public DbSet<CAT_UNE_MOTIVO_CANCELACION> CAT_UNE_MOTIVO_CANCELACION { get; set; }
        public DbSet<CAT_UNE_PRODUCTO> CAT_UNE_PRODUCTO { get; set; }
        public DbSet<CAT_UNE_RESOLUCION> CAT_UNE_RESOLUCION { get; set; }
    
        public virtual ObjectResult<Nullable<int>> SP_UNE_ACTUALIZA_TELEFONOS(Nullable<int> nUMERO, string tELEFONO, string tEL_CELULAR)
        {
            var nUMEROParameter = nUMERO.HasValue ?
                new ObjectParameter("NUMERO", nUMERO) :
                new ObjectParameter("NUMERO", typeof(int));
    
            var tELEFONOParameter = tELEFONO != null ?
                new ObjectParameter("TELEFONO", tELEFONO) :
                new ObjectParameter("TELEFONO", typeof(string));
    
            var tEL_CELULARParameter = tEL_CELULAR != null ?
                new ObjectParameter("TEL_CELULAR", tEL_CELULAR) :
                new ObjectParameter("TEL_CELULAR", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("SP_UNE_ACTUALIZA_TELEFONOS", nUMEROParameter, tELEFONOParameter, tEL_CELULARParameter);
        }
    
        public virtual ObjectResult<SP_UNE_CARGAR_SUPUESTOS_Result> SP_UNE_CARGAR_SUPUESTOS(Nullable<int> iD_TIPO_REPORTE, Nullable<int> iD_TIPO_CUENTA, Nullable<int> iD_CUENTA)
        {
            var iD_TIPO_REPORTEParameter = iD_TIPO_REPORTE.HasValue ?
                new ObjectParameter("ID_TIPO_REPORTE", iD_TIPO_REPORTE) :
                new ObjectParameter("ID_TIPO_REPORTE", typeof(int));
    
            var iD_TIPO_CUENTAParameter = iD_TIPO_CUENTA.HasValue ?
                new ObjectParameter("ID_TIPO_CUENTA", iD_TIPO_CUENTA) :
                new ObjectParameter("ID_TIPO_CUENTA", typeof(int));
    
            var iD_CUENTAParameter = iD_CUENTA.HasValue ?
                new ObjectParameter("ID_CUENTA", iD_CUENTA) :
                new ObjectParameter("ID_CUENTA", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_CARGAR_SUPUESTOS_Result>("SP_UNE_CARGAR_SUPUESTOS", iD_TIPO_REPORTEParameter, iD_TIPO_CUENTAParameter, iD_CUENTAParameter);
        }
    
        public virtual ObjectResult<Nullable<decimal>> SP_UNE_OBTENER_PROXIMO_FOLIO()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("SP_UNE_OBTENER_PROXIMO_FOLIO");
        }
    
        public virtual ObjectResult<proximo_folio_Result> proximo_folio()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<proximo_folio_Result>("proximo_folio");
        }
    
        public virtual ObjectResult<TBL_UNE_REPORTE> prox_folio()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<TBL_UNE_REPORTE>("prox_folio");
        }
    
        public virtual ObjectResult<TBL_UNE_REPORTE> prox_folio(MergeOption mergeOption)
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<TBL_UNE_REPORTE>("prox_folio", mergeOption);
        }
    
        public virtual ObjectResult<SP_UNE_OBTENER_FOLIO_Result> SP_UNE_OBTENER_FOLIO(Nullable<int> nUMUSUARIO, string sESION)
        {
            var nUMUSUARIOParameter = nUMUSUARIO.HasValue ?
                new ObjectParameter("NUMUSUARIO", nUMUSUARIO) :
                new ObjectParameter("NUMUSUARIO", typeof(int));
    
            var sESIONParameter = sESION != null ?
                new ObjectParameter("SESION", sESION) :
                new ObjectParameter("SESION", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_OBTENER_FOLIO_Result>("SP_UNE_OBTENER_FOLIO", nUMUSUARIOParameter, sESIONParameter);
        }
    
        public virtual ObjectResult<SP_UNE_OBTENER_AREA_SUCURSAL_Result> SP_UNE_OBTENER_AREA_SUCURSAL(Nullable<int> nUMUSUARIO, Nullable<int> iDSUCURSAL)
        {
            var nUMUSUARIOParameter = nUMUSUARIO.HasValue ?
                new ObjectParameter("NUMUSUARIO", nUMUSUARIO) :
                new ObjectParameter("NUMUSUARIO", typeof(int));
    
            var iDSUCURSALParameter = iDSUCURSAL.HasValue ?
                new ObjectParameter("IDSUCURSAL", iDSUCURSAL) :
                new ObjectParameter("IDSUCURSAL", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_OBTENER_AREA_SUCURSAL_Result>("SP_UNE_OBTENER_AREA_SUCURSAL", nUMUSUARIOParameter, iDSUCURSALParameter);
        }
    
        public virtual ObjectResult<SP_UNE_CARGA_ULTIMO_COMENTARIO_Result> SP_UNE_CARGA_ULTIMO_COMENTARIO(Nullable<int> fOLIO)
        {
            var fOLIOParameter = fOLIO.HasValue ?
                new ObjectParameter("FOLIO", fOLIO) :
                new ObjectParameter("FOLIO", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_CARGA_ULTIMO_COMENTARIO_Result>("SP_UNE_CARGA_ULTIMO_COMENTARIO", fOLIOParameter);
        }
    
        public virtual ObjectResult<SP_UNE_REPRESENTANTES_LEGALES_Result> SP_UNE_REPRESENTANTES_LEGALES(Nullable<int> nUMERO)
        {
            var nUMEROParameter = nUMERO.HasValue ?
                new ObjectParameter("NUMERO", nUMERO) :
                new ObjectParameter("NUMERO", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_REPRESENTANTES_LEGALES_Result>("SP_UNE_REPRESENTANTES_LEGALES", nUMEROParameter);
        }
    
        public virtual ObjectResult<SP_UNE_CARGA_DATOS_REPRESENTANTE_LEGAL_Result> SP_UNE_CARGA_DATOS_REPRESENTANTE_LEGAL(Nullable<int> iD_PERSONA_REL)
        {
            var iD_PERSONA_RELParameter = iD_PERSONA_REL.HasValue ?
                new ObjectParameter("ID_PERSONA_REL", iD_PERSONA_REL) :
                new ObjectParameter("ID_PERSONA_REL", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_CARGA_DATOS_REPRESENTANTE_LEGAL_Result>("SP_UNE_CARGA_DATOS_REPRESENTANTE_LEGAL", iD_PERSONA_RELParameter);
        }
    
        public virtual ObjectResult<SP_UNE_OBTENER_ENTIDAD_FEDERATIVA_Result> SP_UNE_OBTENER_ENTIDAD_FEDERATIVA(Nullable<int> iD_DE_SUCURSAL)
        {
            var iD_DE_SUCURSALParameter = iD_DE_SUCURSAL.HasValue ?
                new ObjectParameter("ID_DE_SUCURSAL", iD_DE_SUCURSAL) :
                new ObjectParameter("ID_DE_SUCURSAL", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_OBTENER_ENTIDAD_FEDERATIVA_Result>("SP_UNE_OBTENER_ENTIDAD_FEDERATIVA", iD_DE_SUCURSALParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> SP_UNE_PERMISOS_USUARIO(Nullable<int> iD_ROL, Nullable<int> cASO)
        {
            var iD_ROLParameter = iD_ROL.HasValue ?
                new ObjectParameter("ID_ROL", iD_ROL) :
                new ObjectParameter("ID_ROL", typeof(int));
    
            var cASOParameter = cASO.HasValue ?
                new ObjectParameter("CASO", cASO) :
                new ObjectParameter("CASO", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("SP_UNE_PERMISOS_USUARIO", iD_ROLParameter, cASOParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> SP_UNE_OBTENER_ARCHIVOS_ADJUNTOS(Nullable<int> fOLIO, Nullable<int> cASO)
        {
            var fOLIOParameter = fOLIO.HasValue ?
                new ObjectParameter("FOLIO", fOLIO) :
                new ObjectParameter("FOLIO", typeof(int));
    
            var cASOParameter = cASO.HasValue ?
                new ObjectParameter("CASO", cASO) :
                new ObjectParameter("CASO", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("SP_UNE_OBTENER_ARCHIVOS_ADJUNTOS", fOLIOParameter, cASOParameter);
        }
    
        public virtual int SP_UNE_REESTABLECER_FOLIOS_USUARIO(Nullable<int> numusuario)
        {
            var numusuarioParameter = numusuario.HasValue ?
                new ObjectParameter("numusuario", numusuario) :
                new ObjectParameter("numusuario", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_UNE_REESTABLECER_FOLIOS_USUARIO", numusuarioParameter);
        }
    
        public virtual ObjectResult<SP_UNE_CARGA_COMENTARIO_Result> SP_UNE_CARGA_COMENTARIO(Nullable<int> fOLIO, Nullable<int> cASO)
        {
            var fOLIOParameter = fOLIO.HasValue ?
                new ObjectParameter("FOLIO", fOLIO) :
                new ObjectParameter("FOLIO", typeof(int));
    
            var cASOParameter = cASO.HasValue ?
                new ObjectParameter("CASO", cASO) :
                new ObjectParameter("CASO", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_CARGA_COMENTARIO_Result>("SP_UNE_CARGA_COMENTARIO", fOLIOParameter, cASOParameter);
        }
    
        public virtual ObjectResult<SP_UNE_CARGAR_REPORTE_Result1> SP_UNE_CARGAR_REPORTE(string fECHA_INICIAL, string fECHA_FINAL, Nullable<int> tIPO_REPORTE, Nullable<int> eSTATUS_GENERAL, Nullable<int> sUBESTATUS, Nullable<int> eNTIDAD_FEDERATIVA, Nullable<int> sUCURSAL_SOCIO, Nullable<int> nUMUSUARIO)
        {
            var fECHA_INICIALParameter = fECHA_INICIAL != null ?
                new ObjectParameter("FECHA_INICIAL", fECHA_INICIAL) :
                new ObjectParameter("FECHA_INICIAL", typeof(string));
    
            var fECHA_FINALParameter = fECHA_FINAL != null ?
                new ObjectParameter("FECHA_FINAL", fECHA_FINAL) :
                new ObjectParameter("FECHA_FINAL", typeof(string));
    
            var tIPO_REPORTEParameter = tIPO_REPORTE.HasValue ?
                new ObjectParameter("TIPO_REPORTE", tIPO_REPORTE) :
                new ObjectParameter("TIPO_REPORTE", typeof(int));
    
            var eSTATUS_GENERALParameter = eSTATUS_GENERAL.HasValue ?
                new ObjectParameter("ESTATUS_GENERAL", eSTATUS_GENERAL) :
                new ObjectParameter("ESTATUS_GENERAL", typeof(int));
    
            var sUBESTATUSParameter = sUBESTATUS.HasValue ?
                new ObjectParameter("SUBESTATUS", sUBESTATUS) :
                new ObjectParameter("SUBESTATUS", typeof(int));
    
            var eNTIDAD_FEDERATIVAParameter = eNTIDAD_FEDERATIVA.HasValue ?
                new ObjectParameter("ENTIDAD_FEDERATIVA", eNTIDAD_FEDERATIVA) :
                new ObjectParameter("ENTIDAD_FEDERATIVA", typeof(int));
    
            var sUCURSAL_SOCIOParameter = sUCURSAL_SOCIO.HasValue ?
                new ObjectParameter("SUCURSAL_SOCIO", sUCURSAL_SOCIO) :
                new ObjectParameter("SUCURSAL_SOCIO", typeof(int));
    
            var nUMUSUARIOParameter = nUMUSUARIO.HasValue ?
                new ObjectParameter("NUMUSUARIO", nUMUSUARIO) :
                new ObjectParameter("NUMUSUARIO", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_CARGAR_REPORTE_Result1>("SP_UNE_CARGAR_REPORTE", fECHA_INICIALParameter, fECHA_FINALParameter, tIPO_REPORTEParameter, eSTATUS_GENERALParameter, sUBESTATUSParameter, eNTIDAD_FEDERATIVAParameter, sUCURSAL_SOCIOParameter, nUMUSUARIOParameter);
        }
    
        public virtual ObjectResult<SP_UNE_BUSCA_NUMERO_SOCIO_Result1> SP_UNE_BUSCA_NUMERO_SOCIO(Nullable<int> numero, Nullable<int> tipoPersona)
        {
            var numeroParameter = numero.HasValue ?
                new ObjectParameter("numero", numero) :
                new ObjectParameter("numero", typeof(int));
    
            var tipoPersonaParameter = tipoPersona.HasValue ?
                new ObjectParameter("tipoPersona", tipoPersona) :
                new ObjectParameter("tipoPersona", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_BUSCA_NUMERO_SOCIO_Result1>("SP_UNE_BUSCA_NUMERO_SOCIO", numeroParameter, tipoPersonaParameter);
        }
    
        public virtual int SP_ATM_INSERTAR_COMISION(Nullable<int> numero, Nullable<int> id_tipomov, Nullable<decimal> monto, Nullable<System.DateTime> fecha)
        {
            var numeroParameter = numero.HasValue ?
                new ObjectParameter("numero", numero) :
                new ObjectParameter("numero", typeof(int));
    
            var id_tipomovParameter = id_tipomov.HasValue ?
                new ObjectParameter("id_tipomov", id_tipomov) :
                new ObjectParameter("id_tipomov", typeof(int));
    
            var montoParameter = monto.HasValue ?
                new ObjectParameter("monto", monto) :
                new ObjectParameter("monto", typeof(decimal));
    
            var fechaParameter = fecha.HasValue ?
                new ObjectParameter("fecha", fecha) :
                new ObjectParameter("fecha", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_ATM_INSERTAR_COMISION", numeroParameter, id_tipomovParameter, montoParameter, fechaParameter);
        }
    
        public virtual int SP_UNE_REESTABLECER_FOLIOS_USUARIO_SESION(Nullable<int> numusuario, string sESION)
        {
            var numusuarioParameter = numusuario.HasValue ?
                new ObjectParameter("numusuario", numusuario) :
                new ObjectParameter("numusuario", typeof(int));
    
            var sESIONParameter = sESION != null ?
                new ObjectParameter("SESION", sESION) :
                new ObjectParameter("SESION", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_UNE_REESTABLECER_FOLIOS_USUARIO_SESION", numusuarioParameter, sESIONParameter);
        }
    
        public virtual int SP_UNE_ACTUALIZA_FOLIO_USUARIO_SESION(Nullable<int> numusuario, Nullable<int> fOLIO, string sESION)
        {
            var numusuarioParameter = numusuario.HasValue ?
                new ObjectParameter("numusuario", numusuario) :
                new ObjectParameter("numusuario", typeof(int));
    
            var fOLIOParameter = fOLIO.HasValue ?
                new ObjectParameter("FOLIO", fOLIO) :
                new ObjectParameter("FOLIO", typeof(int));
    
            var sESIONParameter = sESION != null ?
                new ObjectParameter("SESION", sESION) :
                new ObjectParameter("SESION", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_UNE_ACTUALIZA_FOLIO_USUARIO_SESION", numusuarioParameter, fOLIOParameter, sESIONParameter);
        }
    
        public virtual int SP_UNE_ENVIAR_CORREOS()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_UNE_ENVIAR_CORREOS");
        }
    
        public virtual ObjectResult<SP_UNE_OBTENER_USUARIOS_Result> SP_UNE_OBTENER_USUARIOS()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_OBTENER_USUARIOS_Result>("SP_UNE_OBTENER_USUARIOS");
        }
    
        public virtual ObjectResult<SP_UNE_OBTENER_SUPUESTOS_VALIDA_IMPORTE_Result> SP_UNE_OBTENER_SUPUESTOS_VALIDA_IMPORTE()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_OBTENER_SUPUESTOS_VALIDA_IMPORTE_Result>("SP_UNE_OBTENER_SUPUESTOS_VALIDA_IMPORTE");
        }
    
        public virtual ObjectResult<string> SP_UNE_OBTENER_NOMBRE_RESPONSABLE(Nullable<int> fOLIO)
        {
            var fOLIOParameter = fOLIO.HasValue ?
                new ObjectParameter("FOLIO", fOLIO) :
                new ObjectParameter("FOLIO", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("SP_UNE_OBTENER_NOMBRE_RESPONSABLE", fOLIOParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> SP_UNE_REGISTRA_REPORTE(Nullable<int> nUM_FOLIO, Nullable<int> eS_SOCIO, Nullable<int> nUMERO, string nOMBRE_S, string aPELLIDO_PATERNO, string aPELLIDO_MATERNO, string tELEFONO, string tEL_CELULAR, Nullable<int> uSUARIO_REGISTRA, Nullable<int> iD_DE_SUCURSAL, string dESCRIPCION_REPORTE, Nullable<int> eNTIDAD, Nullable<decimal> iMPORTE_RECLAMACION, Nullable<decimal> iMPORTE_SOLUCION, Nullable<int> iD_TIPO_REPORTE, Nullable<int> iD_SUPUESTOS_REPORTE, Nullable<int> iD_MEDIO_CONTACTO, Nullable<int> iD_ESTATUS_REPORTE, Nullable<int> iD_TIPO_CUENTA, Nullable<int> iD_CUENTA, Nullable<int> iD_SUCURSAL_REGISTRO, Nullable<int> dIAS_RESTANTES_GENERAL, string num_Tarjeta, string dOMICILIO)
        {
            var nUM_FOLIOParameter = nUM_FOLIO.HasValue ?
                new ObjectParameter("NUM_FOLIO", nUM_FOLIO) :
                new ObjectParameter("NUM_FOLIO", typeof(int));
    
            var eS_SOCIOParameter = eS_SOCIO.HasValue ?
                new ObjectParameter("ES_SOCIO", eS_SOCIO) :
                new ObjectParameter("ES_SOCIO", typeof(int));
    
            var nUMEROParameter = nUMERO.HasValue ?
                new ObjectParameter("NUMERO", nUMERO) :
                new ObjectParameter("NUMERO", typeof(int));
    
            var nOMBRE_SParameter = nOMBRE_S != null ?
                new ObjectParameter("NOMBRE_S", nOMBRE_S) :
                new ObjectParameter("NOMBRE_S", typeof(string));
    
            var aPELLIDO_PATERNOParameter = aPELLIDO_PATERNO != null ?
                new ObjectParameter("APELLIDO_PATERNO", aPELLIDO_PATERNO) :
                new ObjectParameter("APELLIDO_PATERNO", typeof(string));
    
            var aPELLIDO_MATERNOParameter = aPELLIDO_MATERNO != null ?
                new ObjectParameter("APELLIDO_MATERNO", aPELLIDO_MATERNO) :
                new ObjectParameter("APELLIDO_MATERNO", typeof(string));
    
            var tELEFONOParameter = tELEFONO != null ?
                new ObjectParameter("TELEFONO", tELEFONO) :
                new ObjectParameter("TELEFONO", typeof(string));
    
            var tEL_CELULARParameter = tEL_CELULAR != null ?
                new ObjectParameter("TEL_CELULAR", tEL_CELULAR) :
                new ObjectParameter("TEL_CELULAR", typeof(string));
    
            var uSUARIO_REGISTRAParameter = uSUARIO_REGISTRA.HasValue ?
                new ObjectParameter("USUARIO_REGISTRA", uSUARIO_REGISTRA) :
                new ObjectParameter("USUARIO_REGISTRA", typeof(int));
    
            var iD_DE_SUCURSALParameter = iD_DE_SUCURSAL.HasValue ?
                new ObjectParameter("ID_DE_SUCURSAL", iD_DE_SUCURSAL) :
                new ObjectParameter("ID_DE_SUCURSAL", typeof(int));
    
            var dESCRIPCION_REPORTEParameter = dESCRIPCION_REPORTE != null ?
                new ObjectParameter("DESCRIPCION_REPORTE", dESCRIPCION_REPORTE) :
                new ObjectParameter("DESCRIPCION_REPORTE", typeof(string));
    
            var eNTIDADParameter = eNTIDAD.HasValue ?
                new ObjectParameter("ENTIDAD", eNTIDAD) :
                new ObjectParameter("ENTIDAD", typeof(int));
    
            var iMPORTE_RECLAMACIONParameter = iMPORTE_RECLAMACION.HasValue ?
                new ObjectParameter("IMPORTE_RECLAMACION", iMPORTE_RECLAMACION) :
                new ObjectParameter("IMPORTE_RECLAMACION", typeof(decimal));
    
            var iMPORTE_SOLUCIONParameter = iMPORTE_SOLUCION.HasValue ?
                new ObjectParameter("IMPORTE_SOLUCION", iMPORTE_SOLUCION) :
                new ObjectParameter("IMPORTE_SOLUCION", typeof(decimal));
    
            var iD_TIPO_REPORTEParameter = iD_TIPO_REPORTE.HasValue ?
                new ObjectParameter("ID_TIPO_REPORTE", iD_TIPO_REPORTE) :
                new ObjectParameter("ID_TIPO_REPORTE", typeof(int));
    
            var iD_SUPUESTOS_REPORTEParameter = iD_SUPUESTOS_REPORTE.HasValue ?
                new ObjectParameter("ID_SUPUESTOS_REPORTE", iD_SUPUESTOS_REPORTE) :
                new ObjectParameter("ID_SUPUESTOS_REPORTE", typeof(int));
    
            var iD_MEDIO_CONTACTOParameter = iD_MEDIO_CONTACTO.HasValue ?
                new ObjectParameter("ID_MEDIO_CONTACTO", iD_MEDIO_CONTACTO) :
                new ObjectParameter("ID_MEDIO_CONTACTO", typeof(int));
    
            var iD_ESTATUS_REPORTEParameter = iD_ESTATUS_REPORTE.HasValue ?
                new ObjectParameter("ID_ESTATUS_REPORTE", iD_ESTATUS_REPORTE) :
                new ObjectParameter("ID_ESTATUS_REPORTE", typeof(int));
    
            var iD_TIPO_CUENTAParameter = iD_TIPO_CUENTA.HasValue ?
                new ObjectParameter("ID_TIPO_CUENTA", iD_TIPO_CUENTA) :
                new ObjectParameter("ID_TIPO_CUENTA", typeof(int));
    
            var iD_CUENTAParameter = iD_CUENTA.HasValue ?
                new ObjectParameter("ID_CUENTA", iD_CUENTA) :
                new ObjectParameter("ID_CUENTA", typeof(int));
    
            var iD_SUCURSAL_REGISTROParameter = iD_SUCURSAL_REGISTRO.HasValue ?
                new ObjectParameter("ID_SUCURSAL_REGISTRO", iD_SUCURSAL_REGISTRO) :
                new ObjectParameter("ID_SUCURSAL_REGISTRO", typeof(int));
    
            var dIAS_RESTANTES_GENERALParameter = dIAS_RESTANTES_GENERAL.HasValue ?
                new ObjectParameter("DIAS_RESTANTES_GENERAL", dIAS_RESTANTES_GENERAL) :
                new ObjectParameter("DIAS_RESTANTES_GENERAL", typeof(int));
    
            var num_TarjetaParameter = num_Tarjeta != null ?
                new ObjectParameter("Num_Tarjeta", num_Tarjeta) :
                new ObjectParameter("Num_Tarjeta", typeof(string));
    
            var dOMICILIOParameter = dOMICILIO != null ?
                new ObjectParameter("DOMICILIO", dOMICILIO) :
                new ObjectParameter("DOMICILIO", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("SP_UNE_REGISTRA_REPORTE", nUM_FOLIOParameter, eS_SOCIOParameter, nUMEROParameter, nOMBRE_SParameter, aPELLIDO_PATERNOParameter, aPELLIDO_MATERNOParameter, tELEFONOParameter, tEL_CELULARParameter, uSUARIO_REGISTRAParameter, iD_DE_SUCURSALParameter, dESCRIPCION_REPORTEParameter, eNTIDADParameter, iMPORTE_RECLAMACIONParameter, iMPORTE_SOLUCIONParameter, iD_TIPO_REPORTEParameter, iD_SUPUESTOS_REPORTEParameter, iD_MEDIO_CONTACTOParameter, iD_ESTATUS_REPORTEParameter, iD_TIPO_CUENTAParameter, iD_CUENTAParameter, iD_SUCURSAL_REGISTROParameter, dIAS_RESTANTES_GENERALParameter, num_TarjetaParameter, dOMICILIOParameter);
        }
    
        public virtual ObjectResult<SP_CMV_OBTENER_CORREO_SERVIDOR_MODULO_Result> SP_CMV_OBTENER_CORREO_SERVIDOR_MODULO(string nOM_EXE)
        {
            var nOM_EXEParameter = nOM_EXE != null ?
                new ObjectParameter("NOM_EXE", nOM_EXE) :
                new ObjectParameter("NOM_EXE", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_CMV_OBTENER_CORREO_SERVIDOR_MODULO_Result>("SP_CMV_OBTENER_CORREO_SERVIDOR_MODULO", nOM_EXEParameter);
        }
    
        public virtual ObjectResult<SP_UNE_OBTENER_FOLIOS_LLAMADA_SALIDA_Result> SP_UNE_OBTENER_FOLIOS_LLAMADA_SALIDA(string tELEFONO)
        {
            var tELEFONOParameter = tELEFONO != null ?
                new ObjectParameter("TELEFONO", tELEFONO) :
                new ObjectParameter("TELEFONO", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_OBTENER_FOLIOS_LLAMADA_SALIDA_Result>("SP_UNE_OBTENER_FOLIOS_LLAMADA_SALIDA", tELEFONOParameter);
        }
    
        public virtual ObjectResult<SP_BANCA_OBTIENE_SOCIO_Result> SP_BANCA_OBTIENE_SOCIO(Nullable<long> numeroSocio, Nullable<int> noUsuario, Nullable<int> idTIpoPersona)
        {
            var numeroSocioParameter = numeroSocio.HasValue ?
                new ObjectParameter("numeroSocio", numeroSocio) :
                new ObjectParameter("numeroSocio", typeof(long));
    
            var noUsuarioParameter = noUsuario.HasValue ?
                new ObjectParameter("noUsuario", noUsuario) :
                new ObjectParameter("noUsuario", typeof(int));
    
            var idTIpoPersonaParameter = idTIpoPersona.HasValue ?
                new ObjectParameter("idTIpoPersona", idTIpoPersona) :
                new ObjectParameter("idTIpoPersona", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_BANCA_OBTIENE_SOCIO_Result>("SP_BANCA_OBTIENE_SOCIO", numeroSocioParameter, noUsuarioParameter, idTIpoPersonaParameter);
        }
    
        public virtual ObjectResult<SP_UNE_CARGAR_REPORTE_2701_Result> SP_UNE_CARGAR_REPORTE_2701(string fECHA_INICIAL, string fECHA_FINAL, Nullable<int> tIPO_REPORTE, Nullable<int> eSTATUS_GENERAL, Nullable<int> sUBESTATUS, Nullable<int> eNTIDAD_FEDERATIVA, Nullable<int> sUCURSAL_SOCIO, Nullable<int> nUMUSUARIO, Nullable<int> fILTRO_MONTO_RECLAMACION)
        {
            var fECHA_INICIALParameter = fECHA_INICIAL != null ?
                new ObjectParameter("FECHA_INICIAL", fECHA_INICIAL) :
                new ObjectParameter("FECHA_INICIAL", typeof(string));
    
            var fECHA_FINALParameter = fECHA_FINAL != null ?
                new ObjectParameter("FECHA_FINAL", fECHA_FINAL) :
                new ObjectParameter("FECHA_FINAL", typeof(string));
    
            var tIPO_REPORTEParameter = tIPO_REPORTE.HasValue ?
                new ObjectParameter("TIPO_REPORTE", tIPO_REPORTE) :
                new ObjectParameter("TIPO_REPORTE", typeof(int));
    
            var eSTATUS_GENERALParameter = eSTATUS_GENERAL.HasValue ?
                new ObjectParameter("ESTATUS_GENERAL", eSTATUS_GENERAL) :
                new ObjectParameter("ESTATUS_GENERAL", typeof(int));
    
            var sUBESTATUSParameter = sUBESTATUS.HasValue ?
                new ObjectParameter("SUBESTATUS", sUBESTATUS) :
                new ObjectParameter("SUBESTATUS", typeof(int));
    
            var eNTIDAD_FEDERATIVAParameter = eNTIDAD_FEDERATIVA.HasValue ?
                new ObjectParameter("ENTIDAD_FEDERATIVA", eNTIDAD_FEDERATIVA) :
                new ObjectParameter("ENTIDAD_FEDERATIVA", typeof(int));
    
            var sUCURSAL_SOCIOParameter = sUCURSAL_SOCIO.HasValue ?
                new ObjectParameter("SUCURSAL_SOCIO", sUCURSAL_SOCIO) :
                new ObjectParameter("SUCURSAL_SOCIO", typeof(int));
    
            var nUMUSUARIOParameter = nUMUSUARIO.HasValue ?
                new ObjectParameter("NUMUSUARIO", nUMUSUARIO) :
                new ObjectParameter("NUMUSUARIO", typeof(int));
    
            var fILTRO_MONTO_RECLAMACIONParameter = fILTRO_MONTO_RECLAMACION.HasValue ?
                new ObjectParameter("FILTRO_MONTO_RECLAMACION", fILTRO_MONTO_RECLAMACION) :
                new ObjectParameter("FILTRO_MONTO_RECLAMACION", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_CARGAR_REPORTE_2701_Result>("SP_UNE_CARGAR_REPORTE_2701", fECHA_INICIALParameter, fECHA_FINALParameter, tIPO_REPORTEParameter, eSTATUS_GENERALParameter, sUBESTATUSParameter, eNTIDAD_FEDERATIVAParameter, sUCURSAL_SOCIOParameter, nUMUSUARIOParameter, fILTRO_MONTO_RECLAMACIONParameter);
        }
    
        public virtual ObjectResult<SP_UNE_OBTENER_PLAZOS_FIJOS_ACTIVOS_Result> SP_UNE_OBTENER_PLAZOS_FIJOS_ACTIVOS(Nullable<int> nUMERO)
        {
            var nUMEROParameter = nUMERO.HasValue ?
                new ObjectParameter("NUMERO", nUMERO) :
                new ObjectParameter("NUMERO", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_OBTENER_PLAZOS_FIJOS_ACTIVOS_Result>("SP_UNE_OBTENER_PLAZOS_FIJOS_ACTIVOS", nUMEROParameter);
        }
    
        public virtual ObjectResult<SP_UNE_OBTENER_NUM_PTMO_Result> SP_UNE_OBTENER_NUM_PTMO(Nullable<int> nUMERO, Nullable<int> iDMOV)
        {
            var nUMEROParameter = nUMERO.HasValue ?
                new ObjectParameter("NUMERO", nUMERO) :
                new ObjectParameter("NUMERO", typeof(int));
    
            var iDMOVParameter = iDMOV.HasValue ?
                new ObjectParameter("IDMOV", iDMOV) :
                new ObjectParameter("IDMOV", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_OBTENER_NUM_PTMO_Result>("SP_UNE_OBTENER_NUM_PTMO", nUMEROParameter, iDMOVParameter);
        }
    
        public virtual ObjectResult<SP_UNE_OBTENER_NUMPTMO_Result> SP_UNE_OBTENER_NUMPTMO(Nullable<int> nUMERO, Nullable<int> iDMOV)
        {
            var nUMEROParameter = nUMERO.HasValue ?
                new ObjectParameter("NUMERO", nUMERO) :
                new ObjectParameter("NUMERO", typeof(int));
    
            var iDMOVParameter = iDMOV.HasValue ?
                new ObjectParameter("IDMOV", iDMOV) :
                new ObjectParameter("IDMOV", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_UNE_OBTENER_NUMPTMO_Result>("SP_UNE_OBTENER_NUMPTMO", nUMEROParameter, iDMOVParameter);
        }
    
        public virtual ObjectResult<Nullable<System.DateTime>> SP_CALLCENTER_GENERAR_FECHA_COMPROMISO_REPORTE(Nullable<int> dias_resolucion)
        {
            var dias_resolucionParameter = dias_resolucion.HasValue ?
                new ObjectParameter("dias_resolucion", dias_resolucion) :
                new ObjectParameter("dias_resolucion", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<System.DateTime>>("SP_CALLCENTER_GENERAR_FECHA_COMPROMISO_REPORTE", dias_resolucionParameter);
        }
    }
}
