$(document).ready(function () {
    $("table#example").dataTable();
});

function finalizar(id,numFolio) {
    document.getElementById("spanFolio").innerHTML = numFolio;
    $("#divAlertaExito").attr("hidden", "hidden");
    $("#btnGuardar").removeAttr("disabled");
    $("#inputFolio").val(id);
    $("#areaComentarios").removeClass("rojoValidacion");
    $("#areaComentarios").val("");
}

function keyAreaComentarios() {
    $("#areaComentarios").removeClass("rojoValidacion");
}

function cambioSatisfaccion() {
    $("#selectSatisfaccion").removeClass("rojoValidacion");
}

function generar() {
    var res = valida();
    if (res === true) {
        $("#btnGuardar").attr("disabled", "disabled");
        $("#dvLoading").removeAttr("hidden");
        $.ajax({
            type: "POST",url: rootUrl("/Notificados/finalizar"),
            data: { folio: $("#inputFolio").val(), comentarios: $("#areaComentarios").val(), satisfaccion: $("#selectSatisfaccion").val() },
            success: function (estatus) {
                if (estatus == 1) {
                    $("#divAlertaExito").removeAttr("hidden");
                    setTimeout(function () { window.location.href = rootUrl("/Notificados/Notificados"); }, 2000);
                }
                else {
                    $("#divAlertaFracaso").removeAttr("hidden");
                    $("#dvLoading").attr("hidden", "hidden");
                }
            }
        });
    }
}

function valida() {
    var res = true;
    if ($("#areaComentarios").val() === "") {
        $("#areaComentarios").addClass("rojoValidacion");
        res = false;
    }

    if ($("#selectSatisfaccion").val() === "0") {
        $("#selectSatisfaccion").addClass("rojoValidacion");
        res = false;
    }
    return res;
}