$(document).ready(function () {
    $("table#example").dataTable();
    ElementosVisiblesCallcenter();
});

function ElementosVisiblesCallcenter() {
    if (reporteBanca.folioBanca != null) {
        $("#div-Tel").hide();
        $("#div-suc-causa").hide();
        $("#inputTelCel").attr("readOnly");
        $("#div-representante-leg").hide();
        $("#div-tarjeta-cred").hide();
        $("#div-fech-aclaracion").hide();
        $("#div-montoautorizadon").hide();

        ///datos ocultos por normativa//
        $("#div-celular").hide();
        $("#div-domicilio").hide();
        $("#div-sucursal").hide();
        $("#div-EntFed").hide();


        //$("#div-fech-transaccion").hide();
        $("#div-no-referencia").hide();
        $("#div-selectTipoCuentaBanca").hide();
        if ($("#comentarioRechazo").val() == "") {
            $("#div-cometarioRechazo").hide();
        }
        if ($("#selectPtmoCapt").val() != 6) {
            $("#div-importeReclamacion").hide();
            $("#div-fech-transaccion").hide();
        }

        if (reporteBanca.idEstatusReporte == 2) {
            $("#div-select-medDetMov").hide();
            
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
            $("#div-fech-transaccion").hide();
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