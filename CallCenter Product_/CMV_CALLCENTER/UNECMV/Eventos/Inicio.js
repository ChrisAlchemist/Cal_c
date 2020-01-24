$(document).ready(function () {
    $("table#example").dataTable();
});

function modalValor(folio,numFolio) {
    $("#divAlertaExito").attr("hidden", "hidden");
    $("#divAlertaFracaso").attr("hidden", "hidden");
    $("#btnCanalizar").removeAttr("disabled");
    document.getElementById("uneFolio").innerHTML = numFolio;
    document.getElementById("uneFolio2").innerHTML = numFolio;
    $("#inputUNE").val(folio);
}

function canalizar(folio) {
    var idCan = folio + "+Canalizar";
    $("#dvLoading").removeAttr("hidden");
    var idCerrar = folio + "+Cerrar";
    $.ajax({
        type: "POST",url: rootUrl("/Inicio/canalizarReporte"),data: { folio: folio },dataType: "json",
        success: function (estatus) {
            if (estatus === 1)
            {
                $("#divAlertaExito").removeAttr("hidden");
                $("#"+folio).text("Enviado a UNE");
                document.getElementById(idCan).disabled = true;
                document.getElementById(idCerrar).href = "#";
                $("#" + idCerrar).addClass("disabled");
                $("#btnCanalizar").attr("disabled", "disabled");
                setTimeout(function () { window.location.href = rootUrl("/Registros/Registros"); }, 1000);
            }
            else
            {
                $("#divAlertaFracaso").removeAttr("hidden");
                $("#dvLoading").attr("hidden", "hidden");
            }
        }
    });
}