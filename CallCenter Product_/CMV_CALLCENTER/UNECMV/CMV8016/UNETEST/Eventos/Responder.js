$(document).ready(function () {
    $("#inputReclamacion").val(formatCurrency($("#inputReclamacion").val()));
    $("#inputSolucion").val(formatCurrency($("#inputSolucion").val()));
    ElementosVisiblesCallcenter();
});

function valida(tipo, requiereMonto) {
    var val = 1;
    if ($("#inputRespuesta").val() === "") {
        $("#inputRespuesta").addClass("rojoValidacion");
        val = 0;
    }

    
    if (tipo === 3)
    {
        if ($("#selectTipoCuenta").val() === "27") {
            if ($("#selectProcedeCorresponsalias").val() === "0") {
                $("#selectProcedeCorresponsalias").addClass("rojoValidacion");
                val = 0;
            }
        }
        else if ($("#selectTipoCuenta").val() === "5"){
            if ($("#selectProcedeDebito").val() === "0" && $("#selectTipoCuenta").val() !== "27") {
                $("#selectProcedeDebito").addClass("rojoValidacion");
                val = 0;
            }
        }
        

        if ($("#selectCausaResolucion").val() === "0") {
            $("#selectCausaResolucion").addClass("rojoValidacion");
            val = 0;
        }

        var montoRecl = $("#inputSolucion").val().toString().replace(/ |,/g, '');
        montoRecl = montoRecl.toString().replace('$', '');
        var monto = $("#inputReclamacion").val().toString().replace(/ |,/g, '');
        monto = monto.toString().replace('$', '');
        console.log("val 4 " + val);
        if (montoRecl <= 0 && monto > 0) {
            if (requiereMonto == 1) {
                $("#inputSolucion").addClass("rojoValidacion");
                val = 0;
            }
        }
    }

    if (val === 1)
        generaSubmit();
}

function respuesta() {
    $("#inputRespuesta").removeClass("rojoValidacion");
}

function generaSubmit() {
    console.log($("#selectCausaResolucion").val())
    var montoRecl = $("#inputSolucion").val().toString().replace(/ |,/g, '');
    montoRecl = montoRecl.toString().replace('$', '');
    var valor = montoRecl;
    $("#dvLoading").removeAttr("hidden");
    $.ajax({
        type: "POST", url: rootUrl("/Responder/RegistrarRespuesta"),
        data: {
            folio: $("#inputFolio_").val(), numusuario: $("#inputNumusuario_").val(), reclamacion: $("#inputRespuesta").val(), monto: valor, procedeDebito: $("#selectProcedeDebito").val(), procedeCorresponsalias: $("#selectProcedeCorresponsalias").val(), idCausaResolucion: $("#selectCausaResolucion").val()
        },
        dataType: "json",
        success: function (estatus) {
            if (estatus === 1) {
                $("#divAlertaExito").removeAttr("hidden");
                $("#btnResponder").attr("disabled", "disabled");
                setTimeout(function () { document.getElementById("frmEnviaDocumento").submit(); }, 1000);
            }
        },
        error: function (estatus) {
            $("#dvLoading").attr("hidden", "hidden");
            $("#divAlertaFracaso").removeAttr("hidden");
        }
    });
}

function inputSolucion() {
    var valor = Number($("#inputSolucion").val());
    if (valor > 0) {
        $("#inputSolucion").removeClass("rojoValidacion");
    }
}

function procedeDebito(value) {
    if (value != 0) {

        $("#selectProcedeDebito").removeClass("rojoValidacion");

        if (value == 1) {
            $("#inputSolucion").removeAttr("readonly");
            $("#divAlertaCargoDebito").attr("hidden", "hidden");
        }
        else {
            $("#inputSolucion").attr("readonly", "readonly");
            $("#inputSolucion").val(formatCurrency($("#inputComision").val()));
            $("#divAlertaCargoDebito").removeAttr("hidden");
        }
    }
    else {
        $("#inputSolucion").attr("readonly", "readonly");
        $("#inputSolucion").val("$0.00");
        $("#divAlertaCargoDebito").attr("hidden", "hidden");
    }
}

function inputCambioSolucion() {
    $("#inputSolucion").removeClass("rojoValidacion");
}

function procedeCorresponsalias(value) {
    if (value != 0) {

        $("#selectProcedeCorresponsalias").removeClass("rojoValidacion");

        if (value == 1) {
            $("#inputSolucion").removeAttr("readonly");
        }
        else {
            $("#inputSolucion").attr("readonly", "readonly");
            $("#inputSolucion").val(formatCurrency($("#inputComision").val()));
        }
    }
    else {
        $("#inputSolucion").attr("readonly", "readonly");
        $("#inputSolucion").val("$0.00");
        $("#divAlertaCargoDebito").attr("hidden", "hidden");
    }
}

function ElementosVisiblesCallcenter() {
    if (folioBanca != null && folioBanca > 0) {
        $("#div-Tel").hide();
        $("#div-suc-causa").hide();
        $("#inputTelCel").attr("readOnly");
        $("#div-representante-leg").hide();
        $("#div-tarjeta-cred").hide();
        $("#div-fech-solicitudAcl").hide();
        //$("#inputImporteReclamacion").val("");
        //$("#inputImporteReclamacion").removeAttr("ReadOnly");
        $("#div-fech-aclaracion").hide();
        $("#div-fech-transaccion").hide();
        $("#div-no-referencia").hide();

        if($("#divComentAreaEsp").val() == ""){
            $("#divComentAreaEsp").hide();
        }

        ///datos ocultos por normativa//
        $("#div-celular").hide();
        $("#div-domicilio").hide();
        $("#div-sucursal").hide();
        $("#div-EntFed").hide();

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

            $("#div-montoAutorizacion").hide();
            $("#div-idTipoCuneta").hide();
        }

        if (estatusReporte == 2) {
            $("#selectMedioMov").removeAttr("disabled");
            $("#inputFolioAutorizacion").removeAttr("disabled");
            $("#div-folio-autorizacion").hide();
        }

    }
    else {
        $("#div-select-medDetMov").hide();
        $("#div-folio-autorizacion").hide();
        $("#div-idTipoCuneta").hide();
    }
}