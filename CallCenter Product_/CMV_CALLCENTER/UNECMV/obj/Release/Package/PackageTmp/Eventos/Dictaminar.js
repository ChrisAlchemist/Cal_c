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
{
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
        type: "POST",
        url: "/Dictaminar/registrarAceptar",
        data: { folio: $("#inputFolio_").val(), comentariosFinales: $("#areaRespuestaAceptar").val(), numusuario: $("#inputNumusuario_").val() },
        dataType: "json",
        success: function (estatus) {
            if (estatus === 1) {
                $("#dvLoading").attr("hidden", "hidden");
                $("#divAlertaExito").removeAttr("hidden");
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
        type: "POST",
        url: "/Dictaminar/registrarRechazo",
        data: { folio: $("#inputFolio_").val(), comentarios: $("#areaRespuestaRechaza").val(), numusuario: $("#inputNumusuario_").val() },
        dataType: "json",
        success: function (estatus) {
            if (estatus === 1) {
                $("#dvLoading").attr("hidden", "hidden");
                $("#divAlertaExito").removeAttr("hidden");
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