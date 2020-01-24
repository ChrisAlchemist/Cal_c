$(document).ready(function () {

    $("#btnGuardarLlamada").click(function (evt) {
        var validacion = valida();
        if(validacion)
        {
            $.ajax({
                type: "POST",data: {num_folio: $("#vFolio").val()},url: rootUrl("/Finalizados/ValidarFolio"),dataType: "json",
                success: function (estatus) {
                    if (estatus > 0) {
                        $("#formGuardarLlamada").submit();
                    }
                    else {
                        alertify.alert("El folio especificado no existe");
                    }
                },
                error: function (d) {
                    console.log(JSON.stringify(d));
                    alertify.alert("Se genero el siguiente error: " + JSON.stringify(d));
                }
            });
        }
    });

    $("#formGuardarLlamada").submit(function (evt) {
        evt.preventDefault();

        $.ajax({
            url: rootUrl("/Finalizados/GuardarLlamadaSalida"),
            type: "post",data: new FormData(this),cache: false,contentType: false,processData: false,
            success: function (estatus) {
                if(estatus == "1")
                {
                    alertify.alert("Se registro correctamente la llamada.");
                    setTimeout(function () { window.location.href = rootUrl("/Finalizados/Finalizados"); }, 2000);
                }
                else {
                    alertify.alert("Se genero el siguiente error: " + status);
                }
            },
            error: function (xhr, error, status) {
                console.log(status);
                console.log(error);
                console.log(xhr);
                alertify.alert("Se genero el siguiente error: " + error+" "+xhr);
            }
        });
    });
});

function asginarInput(num_folio) {
    $("#vFolio").val(num_folio);
}

function valida() {
    var validacion = true;
    if ($("#vFolio").val() == "") {
        $("#vFolio").addClass("rojoValidacion");
        alertify.alert("Favor de teclear el Folio o asignar alguno de los sugeridos.");
        validacion = false;
    }
    return validacion;
}