$(document).ready(function () {
    var montoRecl = $("#inputImporteReclamacion").val().toString().replace(/ |,/g, '');
    montoRecl = montoRecl.toString().replace('$', '');
    $("#inputImporteReclamacionHidden").val(montoRecl);
    $("#inputImporteReclamacion").val(formatCurrency($("#inputImporteReclamacion").val()));

    ElementosVisiblesCallcenter();
});

function validacion() {
    var validaFrm = validaFormulario();

    if (validaFrm === true)
        generaSubmit();

}

function validaFormulario() {
    if ($("#inputCancelar").val() === "") {
        $("#inputCancelar").addClass("rojoValidacion");
        return false;
    }
    else
        return true;
}

function generaSubmit() {
    $("#frmCancelar").submit();
}

function ComentariosCancelacion() {
    $("#inputCancelar").removeClass("rojoValidacion");
}


function ElementosVisiblesCallcenter() {
    if (reporteBanca.folioBanca != null) {
        $("#div-Tel").hide();
        $("#div-suc-causa").hide();
        //$("#inputTelCel").attr("readOnly");
        $("#div-representante-leg").hide();
        $("#div-tarjeta-cred").hide();
        $("#div-fech-aclaracion").hide();

        $("#div-dictaminar").hide();
        $("#fechaTransaccion").attr("disabled", "disabled");
        $("#div-causaResol").hide();

        $("#div-no-referencia").hide();

        ///datos ocultos por normativa//
        $("#div-celular").hide();
        $("#div-domicilio").hide();
        $("#div-sucursal").hide();
        $("#div-EntFed").hide();
        $("#selectResolucion").hide();
        $("#lbDictamen").hide();

        if ($("#ComentarioAprueba").val() == "") {
            $("#div-ComenAprueba").hide();
        }
        if ($("#ComentarioCan").val() == "") {
            $("#div-comenCanalizacion").hide();
        }

        if ($("#comentarioRechazo").val() == "") {
            $("#div-cometarioRechazo").hide();
        }
        if ($("#selectPtmoCapt").val() != 6) {
            $("#div-importeReclamacion").hide();

            $("#div-select-medDetMov").hide();
            $("#div-folio-autorizacion").hide();
            $("#div-dictamen").hide();
            $("#div-causaResol").hide();
            $("#div-fech-abonoCliente").hide();
            $("#div-fech-transaccion").hide();

            $("#div-formato27").hide();
            $("#div-selectTipoCuentaBanca").hide();
            $("#div-montoautorizacion").hide();
        }
       

        if (reporteBanca.idEstatusReporte == 2) {
            $("#div-select-medDetMov").hide();
            $("#div-fech-transaccion").hide();
            //$("#selectMedioMov").removeAttr("disabled");
            $("#inputFolioAutorizacion").removeAttr("disabled");
            $("#div-folio-autorizacion").hide();
            $("#inputEstatus").val("Canalizado");
            if ($("#selectPtmoCapt").val() == 5) {
                $("#div-importeReclamacion").hide();
            }
        }

        else if (reporteBanca.idEstatusReporte == 3 || reporteBanca.idEstatusReporte == 4) {
            $('#selectMedioMov').removeAttr('hidden');
            $('#div-importeReclamacion').removeAttr('hidden');
            if ($("#selectPtmoCapt").val() == 6) {
                $("#inputImporteReclamacion").val(formatCurrency(reporteBanca.importeReclamacion));
            }
        }

        else {
            $("#div-select-medDetMov").hide();
        }
        if (reporteBanca.idTipoCuenta == 4 || ReporteBanca.idCuenta == 34 || ReporteBanca.idCuenta == 35 || ReporteBanca.idCuenta == 36 || ReporteBanca.idCuenta == 37) {
            $("#div-ComenCanalizacion").hide();
            $("#div-ComenAprueba").hide();
            $("#div-ComenAreaEspe").hide();
            $("#div-ComenNotificacion").hide();
            $("#div-fech-transaccion").hide();
            $("#div-folio-autorizacion").hide();
        }
        if ($("#div-ComentarioFinalizacion").val() == "") {
            $("#div-ComentarioFinalizacion").hide();
        }
    }
}