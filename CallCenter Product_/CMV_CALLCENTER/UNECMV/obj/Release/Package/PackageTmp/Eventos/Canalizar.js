$(document).ready(function () {
    $(".js-example-basic-single").select2({
        placeholder: "Seleccionar usuario"
    });
    $(".js-example-basic-multiple").select2({
        placeholder: "Seleccionar usuarios"
    });
});

function inputArea()
{
    $("#comentarios").removeClass("rojoValidacion");
}

function canalizar()
{
    var valida = true;

    valida = validaCanalizacion();

    if (valida === true)
    {
        $("#dvLoading").removeAttr("hidden");
        $("#btnCanalizar").attr("disabled", "disabled");

        var usuarios = new Array()
        usuarios = $("#selectUsuariosCopia").val();
        $.ajax({
            type: "POST",
            url: "/Canalizar/canalizaUsuario",
            data: {
                folio: $("#inputFolio").val(), numusuario: $("#inputNumusuario").val(), responsable: $("#selectUsuario").val(),
                COresponsables: "" + usuarios, comentario: $("#comentarios").val()
            },
            dataType: "json",
            success: function (estatus) {
                $("#dvLoading").attr("hidden","hidden");
                $("#divAlertaExito").removeAttr("hidden");
            },
            error: function (xhr, status) {
                $("#dvLoading").attr("hidden", "hidden");
                $("#divAlertaFracaso").removeAttr("hidden");
            },
        });
    }
    else
    {
        $("#divAlertaInvalido").removeAttr("hidden");
        $("#btnCanalizar").removeAttr("disabled");
    }
}


function validaCanalizacion()
{
    var validacion = true;

    if ($("#selectUsuario").val() === null) {
        $("#selectUsuario").addClass("rojoValidacion");
        validacion = false;
    }

    if ($("#selectUsuariosCopia").val() === null)
    {
        $("#selectUsuariosCopia").addClass("rojoValidacion");
        validacion = false;
    }

    if ($("#comentarios").val() === "")
    {
        $("#comentarios").addClass("rojoValidacion");
        validacion = false;
    }

    return validacion;
}