function validacion()
{
    var validaFrm = validaFormulario();

    if (validaFrm === true)
        generaSubmit();

}


function validaFormulario()
{
    if ($("#inputObservaciones").val() === "") {
        $("#inputObservaciones").addClass("rojoValidacion");
        return false;
    }
    else
        return true;
}

function generaSubmit()
{
    $("#frmCerrar").submit();
}

function ObservacionesReporte()
{
    $("#inputObservaciones").removeClass("rojoValidacion");
}