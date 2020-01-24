$(document).ready(function () {
    $("table#example").dataTable();
});

function crearFolio(folio, numFolio) {
    document.getElementById("spanModalEncabezado").innerHTML = numFolio;
    $("#inputFolio").val(folio);
}

function finalizarReporte() {

    var validacion = valida();
    if (validacion == true) {
        $("#btnCerrar").attr("disabled", "disabled");
        $("#btnFinalizar").attr("disabled", "disabled");

        $.ajax({
            type: "POST",url: rootUrl("/Debito/FinalizaReporte"),data: { folio: $("#inputFolio").val(), comentarios: $("#txtAreaDebito").val() },dataType: "json",
            success: function (estatus) {
                if (estatus === 1) {
                    $("#divAlertaExito").removeAttr("hidden");
                    setTimeout(function () { window.location.href = rootUrl("/Debito/Debito"); }, 2000);
                }
            },
            error: function (err) {
                alertify.alert("Ocurrio un error al finalizar el reporte");
            }
        });
    }
}

function valida() {
    var validacion = true;
    if ($("#txtAreaDebito").val() == "") {
        $("#txtAreaDebito").addClass("rojoValidacion");
        validacion = false;
    }
    return validacion;
}

function AreaDebito() {
    if ($("#txtAreaDebito").val().length <= 1000 && $("#txtAreaDebito").val().length > 0) {
        $("#txtAreaDebito").removeClass("rojoValidacion");
    }
}