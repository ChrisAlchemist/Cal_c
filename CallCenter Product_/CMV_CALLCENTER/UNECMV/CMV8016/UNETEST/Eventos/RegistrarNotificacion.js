$(document).ready(function () {
    $("#inputReclamacion").val(formatCurrency($("#inputReclamacion").val()));
    $("#inputSolucion").val(formatCurrency($("#inputSolucion").val()));
    ElementosVisiblesCallcenter();
    $('#fechaAbono').datepicker({ autoclose: true, format: 'dd/mm/yyyy' });
    $("#fechaAbono").val(moment().format("DD/MM/YYYY"));

});

function confirmar(id, tipoRep) {

    $("#divAlertaExito").attr("hidden", "hidden");
    $("#areaComentarios").removeClass("rojoValidacion");
    $("#areaComentarios").val("");
    $("#btnGuardar").removeAttr("disabled");
    document.getElementById("areaComentarios").textContent = " ";


    $("#inputFolio").val(id);
    $.ajax({
        type: "POST",url: rootUrl("/Notificar/ObtenerUltimaRespuesta"),data: { folio: id },
        success: function (comentario) {
            document.getElementById("areaDescripcion").textContent = comentario;
        }
    });

    if (tipoRep === 3) {
        $.ajax({
            type: "POST",url: rootUrl("/Notificar/obtenerMontos"),data: { folio: id },dataType: "json",
            success: function (montos) {
                $("#inputReclamacion").val(montos[0]);
                $("#inputSolucion").val(montos[1]);
            }
        });
    }
}

function keyAreaComentarios() {
    $("#areaComentarios").removeClass("rojoValidacion");
}

function generar() {
    var res = valida();
    if (res === true) {
        document.getElementById("frmEnviar").submit();
        $("#btnGuardar").attr("disabled", "disabled");
        $("#dvLoading").removeAttr("hidden");
    }

}

function valida() {
    var res = true;
    if ($("#areaComentarios").val() === "") {
        $("#areaComentarios").addClass("rojoValidacion");
        res = false;
    }
    return res;
}

function ElementosVisiblesCallcenter() {
    if (folioBanca != null && folioBanca > 0) {
        $("#div-Tel").hide();
        $("#div-suc-causa").hide();
        $("#inputTelCel").attr("readOnly");
        $("#div-representante-leg").hide();
        $("#div-tarjeta-cred").hide();
        $("#div-fech-aclaracion").hide();
        $("#div-montoautorizadon").hide();
        $("#div-importeCorrespondiente").hide();

        //////////formato 27////////////
        $("#div-dictamen").hide();
        $("#div-causaResol").hide();
        $("#div-fechAbonoCliente").hide();
        ////////////////////////////////
        $("#div-no-referencia").hide();
        $("#div-selectTipoCuentaBanca").hide();

        ///datos ocultos por normativa//
        $("#div-celular").hide();
        $("#div-domicilio").hide();
        $("#div-sucursal").hide();
        $("#div-EntFed").hide();

        if ($("#ComentarioAprueba").val() == "") {
            $("#div-ComenAprueba").hide();
        }

        if ($("#comentarioRechazo").val() == "") {
            $("#div-cometarioRechazo").hide();
        }
        if ($("#ultimoComentario").val() == "") {
            $("#div-respuestaReporte").hide();
        }



        if ($("#selectPtmoCapt").val() != 6) {
            $("#div-importeReclamacion").hide();

            $("#div-importeCorrespondiente").hide();
            $("#div-selectTipoCuentaBanca").hide();
            $("#div-fechAbonoCliente").hide();
            $("#div-folioUne").hide();
            $("#div-select-medDetMov").hide();
            $("#div-folioAutorizacion").hide();
            $("#div-dictamen").hide();
            $("#div-causaResol").hide();
            $("#div-fech-transaccion").hide();
            $("#div-formato27").hide();
        }




        if ($("#div-ComentarioFinalizacion").val() == "") {
            $("#div-ComentarioFinalizacion").hide();
        }
    }
    else {
        $("#div-folioAutorizacion").hide();
        $("#div-select-medDetMov").hide();
        $("#div-selectTipoCuentaBanca").hide();
    }
}