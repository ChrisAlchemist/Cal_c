$(document).ready(function () {
    ElementosVisiblesCallcenter();
    var montoRecl = $("#inputImporteReclamacion").val().toString().replace(/ |,/g, '');
    montoRecl = montoRecl.toString().replace('$', '');
    $("#inputImporteReclamacionHidden").val(montoRecl);
    $("#inputImporteReclamacion").val(formatCurrency($("#inputImporteReclamacion").val()));

    $('#fechaAbono').datepicker({ autoclose: true, format: 'dd/mm/yyyy' });
    $("#fechaAbono").val(moment().format("DD/MM/YYYY"));

    var montoSol = $("#inputImporteSolucion").val().toString().replace(/ |,/g, '');
    montoSol = montoSol.toString().replace('$', '');
    $("#inputImporteSolucionHidden").val(montoSol);
    $("#inputImporteSolucion").val(formatCurrency($("#inputImporteSolucion").val()));
});


function validacion()
{
    var validaFrm = validaFormulario();

    if (validaFrm === true)
        generaSubmit();

}

function validaFormulario()
{
    var valida = true;

    if ($("#selectCausaResolucion").val() === "0") {
        $("#selectCausaResolucion").addClass("rojoValidacion");
        valida = false;
    }

    if ($("#inputObservaciones").val() === "" || $("#inputObservaciones").val().length >= 1000)
    {
        $("#inputObservaciones").addClass("rojoValidacion");
        valida = false;
    }
    return valida;
}

function generaSubmit()
{
    $("#frmCerrar").submit();
}

function ObservacionesReporte()
{
    $("#inputObservaciones").removeClass("rojoValidacion");
}

function validaInputImporte(value)
{
    var montoRecl = value.toString().replace(/ |,/g, '');
    montoRecl = montoRecl.toString().replace('$', '');
    $("#inputImporteReclamacionHidden").val(montoRecl);
    $("#inputImporteSolucion").removeClass("rojoValidacion");
}

function validaInputImporteSolucion(value) {
    var montoRecl = value.toString().replace(/ |,/g, '');
    montoRecl = montoRecl.toString().replace('$', '');
    $("#inputImporteSolucionHidden").val(montoRecl);
    $("#inputImporteSolucion").removeClass("rojoValidacion");
}

function ElementosVisiblesCallcenter()
{
    if (reporteBanca.folioBanca != null && reporteBanca.folioBanca > 0) {
        $("#div-Tel").hide();
        $("#div-suc-causa").hide();
        $("#inputTelCel").attr("readOnly");
        $("#div-representante-leg").hide();
        $("#div-tarjeta-cred").hide();
        $("#div-noCuentaPrestamo").hide();

        ///datos ocultos por normativa//
        $("#div-celular").hide();
        $("#div-domicilio").hide();
        $("#div-sucursal").hide();
        $("#div-EntFed").hide();
    

        if (reporteBanca.idEstatusReporte == 2) {
            $("#selectMedioMov").removeAttr("disabled");
            $("#inputFolioAutorizacion").removeAttr("disabled");
        }
        if (reporteBanca.idTipoCuenta == 4) {

            $("#selectCausaResolucion").val(654);
            $("#div-datosAdicionalesUNE").hide();
            $("#div-no-referencia").hide();
            $("#div-importeReclamacion").hide();
            $("#div-fech-aclaracion").hide();
            $("#div-fech-transaccion").hide();
            $("#div-select-medDetMov").hide();
            $("#div-folio-autorizacion").hide();
            $("#div-adjunDocumento").hide();
        }
    }
    else {
        $("#div-select-medDetMov").hide();
        $("#div-folio-autorizacion").hide();
    }

}