var tablaReportesBanca;
var reporteBanca;
var cantReportes = 0;

$(document).ready(function () {
    $("#inputReclamacion").val(formatCurrency($("#inputReclamacion").val()));
    $("#inputSolucion").val(formatCurrency($("#inputSolucion").val()));
    ElementosVisiblesCallcenter();
    tablaReportesBanca = $("#tbl-reportesBanca").dataTable({ "searching": false, "lengthChange": false, /*"paging": false,*/ "info": false });
    MostrarAllReportesBanca(parseInt($("#kbd-folioUNE").text()));
});

function validaA()
{
    if ($("#areaRespuestaAceptar").val() === "")
        $("#areaRespuestaAceptar").addClass("rojoValidacion");
    else
        registrarAceptar();
}

function validaR() {
    if ($("#areaRespuestaRechaza").val() === "")
        $("#areaRespuestaRechaza").addClass("rojoValidacion");
    else
        registrarRechazar();
}

function validaRechaza()
{+
    $("#areaRespuestaRechaza").removeClass("rojoValidacion");
}

function validaAceptar()
{
    $("#areaRespuestaAceptar").removeClass("rojoValidacion");
}

function registrarAceptar()
{
    $("#dvLoading").removeAttr("hidden");
    $("#btnAceptar").attr("disabled", "disabled");
    $("#btnRechazar").attr("disabled", "disabled");
    $("#btnCancelarModalA").click();
    $.ajax({
        type: "POST",url: rootUrl("/Dictaminar/registrarAceptar"),
        data: { folio: $("#inputFolio_").val(), comentariosFinales: $("#areaRespuestaAceptar").val(), numusuario: $("#inputNumusuario_").val() },dataType: "json",
        success: function (estatus) {
            if (estatus === 1) {
                $("#dvLoading").attr("hidden", "hidden");
                $("#divAlertaExito").removeAttr("hidden");
                setTimeout(function () { document.getElementById("frmEnviaDocumentoDictamenAceptar").submit() }, 1000);
            }
        },
        error: function (estatus) {
            $("#dvLoading").attr("hidden", "hidden");
            $("#divAlertaFracaso").removeAttr("hidden");
            $("#btnAceptar").removeAttr("disabled");
            $("#btnRechazar").removeAttr("disabled");
        }
    });
}

function registrarRechazar()
{
    $("#dvLoading").removeAttr("hidden");
    $("#btnAceptar").attr("disabled", "disabled");
    $("#btnRechazar").attr("disabled", "disabled");
    $("#btnCancelarModalR").click();

    $.ajax({
        type: "POST",url: rootUrl("/Dictaminar/registrarRechazo"),
        data: { folio: $("#inputFolio_").val(), comentarios: $("#areaRespuestaRechaza").val(), numusuario: $("#inputNumusuario_").val() },dataType: "json",
        success: function (estatus) {
            if (estatus === 1) {
                $("#dvLoading").attr("hidden", "hidden");
                $("#divAlertaExito").removeAttr("hidden");
                setTimeout(function () { document.getElementById("frmEnviaDocumentoDictamenRechazar").submit() }, 1000);
            }
        },
        error: function (estatus) {
            $("#dvLoading").attr("hidden", "hidden");
            $("#divAlertaFracaso").removeAttr("hidden");
            $("#btnAceptar").removeAttr("disabled");
            $("#btnRechazar").removeAttr("disabled");
        }
    });
}

function ElementosVisiblesCallcenter() {
    if (folioBanca != null && folioBanca > 0) {
        $("#div-Tel").hide();
        $("#div-suc-causa").hide();
        $("#inputTelCel").attr("readOnly");
        $("#div-representante-leg").hide();
        $("#div-tarjeta-cred").hide();
        
        //$("#inputImporteReclamacion").val("");
        //$("#inputImporteReclamacion").removeAttr("ReadOnly");
        $("#div-fech-aclaracion").hide();
        $("#div-fech-transaccion").hide();
        $("#div-no-referencia").hide();
        $("#div-select-medDetMov").hide();
        $("#div-fechSolicitudAcl").hide();
        $("#div-inputSolucion").hide();

        ///datos ocultos por normativa//
        $("#div-celular").hide();
        $("#div-domicilio").hide();
        $("#div-sucursal").hide();
        $("#div-EntFed").hide();

        if ($("#selectPtmoCapt").val() != 6) {
            $("#div-importeReclamacion").hide();

            $("#div-datosTransacionAdic").hide();
            $("#div-selectTipoCuentaBanca").hide();
            $("#div-importeCorrespondiente").hide();
        }


        if (estatusReporte == 2) {
            $("#selectMedioMov").removeAttr("disabled");
            $("#inputFolioAutorizacion").removeAttr("disabled");
            $("#div-folio-autorizacion").hide();
        }
        $("#div-importeReclamacion").hide();
        $("#div-selectTipoCuentaBanca").hide();
        $("#div-selectMedioMov").hide();
        $("#div-inputFolioAutorizacion").hide();
        $("#div-importeCorrespondiente").hide();
        $("#div-fechtrans").hide();
        //div-fechSolicitudAcl
    }
    else {
        $("#div-selectTipoCuentaBanca").hide();
        $("#div-inputFolioAutorizacion").hide();
        $("#div-selectMedioMov").hide();
        
    }
}

function MostrarAllReportesBanca(folioUNE) {
    $.ajax({
        url: rootUrl("/Banca/ObtenerReportesBancaJson"),
        data: { folioUNE: folioUNE },
        dataType: "json",
        method: "post",
        beforeSend: function () {
            //MostrarLoading();
        },
        success: function (data) {
            //OcultarLoading();
            PintarTablaReportesBanca(data.reportesSocio);
            cantReportes = data.reportesSocio.length;
        },
        error: function (xhr, status, error) {
            ControlErrores(xhr, status, error);
        }
    });
}

function PintarTablaReportesBanca(data) {
    tablaReportesBanca.fnClearTable();
    tablaReportesBanca.fnDraw();
    data.forEach(function (reporte, index) {
        tablaReportesBanca.fnAddData([
            reporte.folioAutorizacion,
            formatCurrency(reporte.importeReclamo),
            reporte.medioMovimiento,
            reporte.tipoCuentaBanca,
            ObtenerFechaJson(reporte.fechaTransacion),

            //'<span data-toggle="tooltip" title="Editar"><button type="button" class="btn btn-icon btn-sm waves-effect waves-light btn-warning" data-toggle="modal" data-target="#modal-reporteBanca"' +
            //'onclick="OnMostrarEditarReporteBanca(\'' + reporte.idIncidenciaReporte + '\')"> <span class ="glyphicon glyphicon-edit"/> </button></span>' + ' ' +
            //'<span data-toggle="tooltip" title="Eliminar"><button type="button" class="btn btn-icon btn-sm waves-effect waves-light btn-danger" data-toggle="modal" data-target="#myModal"' +
            //'onclick="OnMostrarEliminarReporteBanca(\'' + reporte.idIncidenciaReporte + '\')"> <span class ="glyphicon glyphicon-trash"/> </button></span>'
        ]);
        tablaReportesBanca.fnDraw(false);
    });
    $('[data-toggle="tooltip"]').tooltip();
}


function ObtenerFechaJson(fecha) {
    var fecha_ = new Date(fecha.match(/\d+/)[0] * 1);
    return fecha_.toLocaleDateString('en-GB');
}