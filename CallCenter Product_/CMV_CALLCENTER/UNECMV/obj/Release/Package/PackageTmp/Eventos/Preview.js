function generaSubmit() {
    var validacion = false;
    validacion = valida();
    if(validacion===true)
        document.getElementById("frmModificar").submit();
}

function habilitarModificacion() {
    $("#inputTel").removeAttr("readonly");
    $("#inputTelCel").removeAttr("readonly");
    $("#selectReporte").removeAttr("disabled");
    $("#selectSupuesto").removeAttr("disabled");
    $("#inputDescripcion").removeAttr("readOnly");
    $("#inputMedioContacto").removeAttr("disabled");
    $("#btnGuardar").removeAttr("disabled");
    $("#btnCancelar").removeAttr("disabled");
}

function cargarSupuestos(id) {
    $("#selectReporte").removeClass("rojoValidacion");
    $.ajax({
        type: "POST",
        url: "/Registrar/ObtenerSupuestos",
        data: { id: id },
        dataType: "json",
        success: function (supuestos) {
            var html = '<option value="0">SELECCIONAR</option>';
            $.each(supuestos, function (i, supuesto) {
                html += '<option value="' + supuesto.ID_SUPUESTOS_REPORTE + '">' + supuesto.DESCRIPCION + '</option>';
            });
            $("#selectSupuesto").html(html);
        }
    });
}

function valida()
{
    var validacion = true;

    if ($("#inputTel").val() === "")
    {
        $("#inputTel").addClass("rojoValidacion");
        validacion = false;
    }

    if ($("#inputTelCel").val() === "")
    {
        $("#inputTelCel").addClass("rojoValidacion");
        validacion = false;
    }

    if ($("#selectReporte").val() === "0")
    {
        $("#selectReporte").addClass("rojoValidacion");
        validacion = false;
    }

    if ($("#selectSupuesto").val() === "0")
    {
        $("#selectSupuesto").addClass("rojoValidacion");
        validacion = false;
    }

    if ($("#inputDescripcion").val() === "")
    {
        $("#inputDescripcion").addClass("rojoValidacion");
        validacion = false;
    }

    return validacion;
}

function supuestos()
{
    $("#selectSupuesto").removeClass("rojoValidacion");
}

function descripcionReporte()
{
    $("#inputDescripcion").removeClass("rojoValidacion");
}
