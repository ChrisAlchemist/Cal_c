$(document).ready(function () {

    $("table#example").dataTable();

    $("#frmAdjuntar").submit(function (evt) {
        evt.preventDefault();
        $("#dvLoading").removeAttr("hidden");
        $.ajax({
            url: rootUrl("/Finalizados/guardarActualizarArchivo"),type: "post",data: new FormData(this),cache: false,contentType: false,processData: false,
            success: function (data) {
                if (data === 1) {
                    $("#divAlertaExito").removeAttr("hidden");
                    $("#btnAdjuntar").attr("disabled", "disabled");
                    setTimeout(function () { window.location.href = rootUrl("/Finalizados/Finalizados"); }, 1000);
                }
                else {
                    $("#divAlertaFracaso").removeAttr("hidden");
                    $("#dvLoading").attr("hidden","hidden");
                }
            },
            error: function (xhr, error, status) {
                console.log(status);
                console.log(error);
                $("#divAlertaFracaso").removeAttr("hidden");
                $("#dvLoading").attr("hidden", "hidden");
            }
        });
    });
});


function crearFolio(folio)
{
    $("#numFolio").val(folio);
    $("#btnAdjuntar").removeAttr("disabled");
    $("#divAlertaExito").attr("hidden", "hidden");
    $("#divAlertaFracaso").attr("hidden", "hidden");
    $("#selectTipoLlamada").val("N");
    $("#divAlertaWarning").attr("hidden", "hidden");
    $("#inputAudio").val("");
}

function validaExisteArchivo() {

    $.ajax({
        type: "POST",url: rootUrl("/Finalizados/validaArchivoExiste"),data: { folio: $("#numFolio").val(), tipoLlamada: $("#selectTipoLlamada").val() },dataType: "json",
        success: function (estatus) {
            if(estatus === 1)
            {
                $("#divAlertaWarning").removeAttr("hidden");
            }
        }
    });
}

function reproducir(id, folio)
{
    document.getElementById("spanModalEncabezadoAudio").innerHTML = folio;
    $.ajax({
        type: "POST",url: rootUrl("/Finalizados/ObtenerArchivosAudio"),data: { folio: id },dataType: "json",
        success: function (audios) {
            if(audios[0]!= null)
            {
                if (audios[0].tipo_llamada === "E")
                {
                    var html = '<audio controls="controls" id="audioE"><source src="' + audios[0].ruta_audio + '" type="audio/mpeg" />Tu navegador no soporta el elemento de audio.</audio>';
                    $("#divAudioE").html(html);
                }
                else {
                    var html = '<audio controls="controls" id="audioS"><source src="' + audios[0].ruta_audio + '" type="audio/mpeg" />Tu navegador no soporta el elemento de audio.</audio>';
                    $("#divAudioS").html(html);
                }
            }

            if (audios[1] != null) {
                if (audios[1].tipo_llamada === "E") {
                    var html = '<audio controls="controls" id="audioE"><source src="' + audios[1].ruta_audio + '" type="audio/mpeg" />Tu navegador no soporta el elemento de audio.</audio>';
                    $("#divAudioE").html(html);
                }
                else {
                    var html = '<audio controls="controls" id="audioS"><source src="' + audios[1].ruta_audio + '" type="audio/mpeg" />Tu navegador no soporta el elemento de audio.</audio>';
                    $("#divAudioS").html(html);
                }
            }
        }
    });
}

function cerrar()
{
    if (document.getElementById("audioE"))
    {
        var vid = document.getElementById("audioE");
        vid.pause();
    }

    if (document.getElementById("audioS")) {
        var vid = document.getElementById("audioS");
        vid.pause();
    }

    var html = '<span>No hay archivo de audio</span>';
    $("#divAudioE").html(html);
    $("#divAudioS").html(html);
}