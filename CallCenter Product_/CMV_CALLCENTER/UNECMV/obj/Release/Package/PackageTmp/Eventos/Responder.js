function valida()
{
    if ($("#inputRespuesta").val() === "")
        $("#inputRespuesta").addClass("rojoValidacion");
    else
        generaSubmit();
}

function respuesta()
{
    $("#inputRespuesta").removeClass("rojoValidacion");
}

function generaSubmit()
{
    $("#dvLoading").removeAttr("hidden");
    $.ajax({
        type: "POST",
        url: "/Responder/RegistrarRespuesta",
        data: { folio: $("#inputFolio_").val(), numusuario: $("#inputNumusuario_").val(), reclamacion: $("#inputRespuesta").val() },
        dataType: "json",
        success: function (estatus) {
            if (estatus === 1)
            {
                $("#divAlertaExito").removeAttr("hidden");
                $("#btnResponder").attr("disabled", "disabled");

                window.location.href =window.location.href = "/Inicio/Inicio";
            }
        },
        error: function (estatus) {
            $("#dvLoading").attr("hidden", "hidden");
            $("#divAlertaFracaso").removeAttr("hidden");
        }
    });
    //$("#frmResponder").submit();
}