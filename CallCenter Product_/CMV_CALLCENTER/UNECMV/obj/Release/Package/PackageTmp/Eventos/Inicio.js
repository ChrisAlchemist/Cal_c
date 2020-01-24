function modalValor(folio) {
    $("#divAlertaExito").attr("hidden", "hidden");
    $("#divAlertaFracaso").attr("hidden", "hidden");
    $("#btnCanalizar").removeAttr("disabled");
    document.getElementById("uneFolio").innerHTML = folio;
    $("#inputUNE").val(folio);
}

function canalizar(folio) {
    var idCan = folio + "+Canalizar";
    var idCerrar = folio + "+Cerrar";
    $.ajax({
        type: "POST",
        url: "/Inicio/canalizarReporte",
        data: { folio: folio },
        dataType: "json",
        success: function (estatus) {
            if (estatus === 1)
            {
                $("#divAlertaExito").removeAttr("hidden");
                $("#"+folio).text("Enviado a 01800");
                document.getElementById(idCan).disabled = true;
                document.getElementById(idCerrar).href = "#";
                $("#" + idCerrar).addClass("disabled");
                $("#btnCanalizar").attr("disabled", "disabled");
            }
            else
                $("#divAlertaFracaso").removeAttr("hidden");
        }
    });
}