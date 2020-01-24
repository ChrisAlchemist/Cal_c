$(document).ready(function () {
    $('#fechaInicio').datepicker({autoclose: true,format: 'dd/mm/yyyy'});
    $('#fechaCierre').datepicker({autoclose: true,format: 'dd/mm/yyyy'});
    $("#btnGenera").click(function (evt) {evt.preventDefault();});
});

function validaGeneraReporte() {
    var validacion = valida();
    if (validacion === true) {
        $("#btnGenera").attr("disabled","disabled");
        $("#dvLoading").removeAttr("hidden");
        $("#frmGenerar").submit();
    }
}

function valida() {
    var validacion = true;
    if ($("#fechaInicio").val() === "" || $("#fechaInicio").val().length !== 10) {
        $("#fechaInicio").addClass("rojoValidacion");
        validacion = false;
    }

    if ($("#fechaCierre").val() === "" || $("#fechaCierre").val().length !== 10) {
        $("#fechaCierre").addClass("rojoValidacion");
        validacion = false;
    }

    return validacion;
}

function OnSuccess(listado) {
    $("#dvLoading").attr("hidden", "hidden");
    $("#btnGenera").removeAttr("disabled");
    if (listado>0) {
        alertify.alert("El archivo se género correctamente. En unos momentos recibirá un correo con la ubicación del archivo. ");
    }
    else {
        alertify.alert("Los datos ingresados no generaron resultados");
    }
}

function OnFailure(error) {
    $("#dvLoading").attr("hidden", "hidden");
    $("#btnGenera").removeAttr("disabled");
    alertify.alert("Lo sentimos ocurrio un error al generar su reporte <br /> ")
    console.log(JSON.stringify(error));
}

function cambioFecha(id) {
    if (id === "fechaInicio") {
        if ($("#fechaInicio").val().length === 10) {
            $("#fechaInicio").removeClass("rojoValidacion");
        }
    }

    if (id === "fechaFin") {
        if ($("#fechaFin").val().length === 10) {
            $("#fechaFin").removeClass("rojoValidacion");
        }
    }
}