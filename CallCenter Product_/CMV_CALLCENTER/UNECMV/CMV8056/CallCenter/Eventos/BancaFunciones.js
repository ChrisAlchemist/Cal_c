
$(document).ready(function () {

    ///datos ocultos por normativa//
    $("#div-row-datosPersonales").hide();
    

    $("#inputMontoMax").focusout(function () {
        if ($(this).val().length > 0) {
            $(this).css({ "border-color": "#ccc" })
        }
        else {
            MontoIncorrecto("Es obligatorio un monto máximo");
        }
    });
    $("#cmbBloqueo").hide();
    $("#cmbSoftoken").hide();

    $("#inputMotivoBloqueo").focusout(function () {
        if ($(this).val().length > 0) {
            $(this).css({ "border-color": "#ccc" })
        }
    });

    $("#inputMotivoBloqueo").focusin(function () {
        $(this).css({ "border-color": "#337ab7" });
    });


    $("#btn-modal-noDisponible").click(function () {
        setInterval(function () { window.location = rootUrl("/Registros/Registros"); }, 1000);
    });

    $("#btn-modal-bloqueo").click(function () {
        if (canalizar == 33) {
            var href = $('#redirect-preview').attr('href');
            window.location.href = href;
        } else {
           window.location = rootUrl("/Finalizados/Finalizados");
        }

    });

    $("#btn-modal-monto").click(function () {        
            window.location = rootUrl("/Finalizados/Finalizados");
    });

    $("#btn-modal-reposicionar").click(function () {
        window.location = rootUrl("/Finalizados/Finalizados");
    });

    $("#div-ErrorMontoIncorrecto").hide();
    $("#inputMontoMax").val(formatCurrency($("#inputMontoMax").val())); 

    
    
});

//Validaciones
function MontoIncorrecto(mensaje) {
    $("#inputMontoMax").css({ "border-color": "red" });
    $("#inputMontoMax").val("");
    $("#inputMontoMax").attr({ "placeholder": mensaje });
}

function LlenarModalDisposicion(mensaje){
    $('#mensaje-Disposicion').text("El socio "+$('#inputNomCompleto').val()+ mensaje);
    $('#myModalNoDisponible').modal({fadeDuration:100});    
}

function irInicio() {
    setInterval(function () { window.location = rootUrl("/Registros/Registros"); }, 1000);
}

function cerrarVista() {
    if (canalizar == 33)
    {
        //return RedirectToAction("Preview", "Preview", reporte);
        setInterval(function () {
            //window.location = rootUrl("/Preview/PreviewJson?repFolio=" + JSON.stringify(reporte));
            //$("#redirect-preview").trigger("click");
            var href = $('#redirect-preview').attr('href');
            window.location.href = href; 
           
        }, 1000);
    }
    else
    {
        setInterval(function () { window.location = rootUrl("/Finalizados/Finalizados"); }, 1000);
    }
    
}

function validaToken() {
    $('#mensaje-token').text("¿Esta seguro de realizar el reenvio de la liga de enrolamiento?");
    $('#myModalToken').modal({ fadeDuration: 100 });
}

function validaBloqueo() {
    if ($("#inputDescripcionBloqueo").val() == "") {
        $("#inputDescripcionBloqueo").css({ "border-color": "red" });
    } else {        
        $('#mensaje-bloquear').text("¿Esta seguro de realizar el bloqueo del servicio?");
        $('#myModalBloquear').modal({ fadeDuration: 100 });
    }
}
//Modificar monto elemntos dinamicos
function ocultarAdvertencia() {

    $("#div-ErrorMontoIncorrecto").hide();
    $("#inputMontoMax").val($("#inputMontoMax").val().replace("$", "").replace(",", "").replace(".00", ""));
}



function LlenarArvertencia(mensaje) {
    $("#pMensajeAdvertencia").text(mensaje)
    $("#div-ErrorMontoIncorrecto").show();
}

function validaMonto() {
    

    var montoMaximo = parseInt($("#inputMontoMax").val().replace("$", "").replace(",", "").replace(".00", ""));
    var bancaMontoMax = parseInt($("#inputBancaMontoMax").val());
    var socioMontoMax = parseInt($("#inputSocioMontoMax").val());

    if (montoMaximo == 0) {
        LlenarArvertencia("El monto máximo debe de ser mayor o igual a 1.");
        //MontoIncorrecto("Es obligatorio un monto máximo");
        return false;
    } else if ($("#inputMontoMax").val() == "") {
        LlenarArvertencia("Se debe de ingresar un monto máximo.");
        //MontoIncorrecto("Es obligatorio un monto máximo");
        return false;
    }
    else if (montoMaximo > bancaMontoMax || montoMaximo < 1) {
        //alert("El monto máximo debe de ser menor de 50000");
        LlenarArvertencia("El monto debe ser menor al actual y mayor o igual a 1");
        //MontoIncorrecto("El monto debe ser menor");
        return false;
    }
    ////else if (montoMaximo > socioMontoMax) {
    ////    LlenarArvertencia("El monto debe ser menor.");
        
    ////    //MontoIncorrecto("otro" + socioMontoMax);
    ////    return false;
    ////}
    else if (montoMaximo == socioMontoMax || montoMaximo > socioMontoMax) {
        LlenarArvertencia("El monto debe de ser menor al actual.");
        //MontoIncorrecto("El monto  debe de ser diferente");
        return false;
    }else{
            
        $('#mensaje-actualizaMonto').text("¿Esta seguro de realizar la modicicación del monto máximo?");
        $('#myModalActualizaMonto').modal({ fadeDuration: 100 });
    }
}


//modificar monto
function ActualizarMonto() {
    var montoMaximo = parseInt($("#inputMontoMax").val().replace("$", "").replace(",", "").replace(".00", ""));
    var bancaMontoMax = parseInt($("#inputBancaMontoMax").val());
    var socioMontoMax = parseInt($("#inputSocioMontoMax").val());
    
    if (montoMaximo >= 1) {
        $('#myModalActualizaMonto').modal("hide");
        $("#dvLoading").removeAttr("hidden");

        $.ajax({
            //{parametroControlador:parametroVista}
            data: { montoMaximo: montoMaximo, reporte: JSON.stringify(reporte), idTipoNotificacion: $('#cmbMedioNotificacion').select().val() },
            url: rootUrl("/Banca/ModificarMonto"),
            dataType: 'json',
            method: 'post',

            success: function (data) {
                if (data.estatus == 1) {
                    //alert(data.mensaje);
                    $("#dvLoading").attr("hidden", "hidden");
                    $('#mensaje-monto').text("Se ha actualizado con exito el monto del socio " + $('#inputNomCompleto').val());
                    $('#myModalMonto').modal({ fadeDuration: 100 });
                    //setInterval(function () { window.location = rootUrl("/Registros/Registros"); }, 2000);                     
                } else {
                    $("#dvLoading").attr("hidden", "hidden");
                    alert(data.mensaje);
                }
            },
            error: function () {
                $("#dvLoading").attr("hidden", "hidden");
                MontoIncorrecto("No fue posible cambiar el monto al socio, intenta mas tarde");
            }
        });
    }
    
}


///bloquear servicio
function BloquearServicio() {
    var descripcionBloqueo = $("#inputDescripcionBloqueo").val();
    var numeroSocio = $("#inputNumeroSocio").val();
    $('#myModalBloquear').modal("hide");
    
    $("#dvLoading").removeAttr("hidden");
    
    $("#dvLoading").removeAttr("hidden");
    $.ajax({
        //{parametroControlador:parametroVista}
        data: { numeroSocio: numeroSocio, descripcionBloqueo: escapeHtml(descripcionBloqueo), reporte: JSON.stringify(reporte), idTipoNotificacion: $('#cmbBloqueo').select().val(), idCuenta: canalizar },
        url: rootUrl("/Banca/BloquearBanca"),
        dataType: 'json',
        method: 'post',

        success: function (data) {
            if (data.estatus == 1) {
                //alert(data.mensaje);
                $("#dvLoading").attr("hidden", "hidden");
                $('#mensaje-bloqueo').text("El socio "+$('#inputNomCompleto').val()+" ha sido bloqueado");
                $('#myModalBloqueo').modal({fadeDuration:100});
                //setInterval(function () { window.location = rootUrl("/Registros/Registros"); }, 2000);                    
            } else {
                $("#dvLoading").attr("hidden", "hidden");
                alert(data.mensaje);
            }                
        },
        error: function () {
            $("#dvLoading").attr("hidden", "hidden");
            MontoIncorrecto("No fue posible bloquear al socio, intenta mas tarde");
        }
    });
    
}


//Reposicion de softoken 
function ReposicionarToken() {
    $('#myModalToken').modal("hide");
    var numeroSocio = $("#inputNumeroSocio").val();
    $("#dvLoading").removeAttr("hidden");
    $.ajax({
        //{parametroControlador:parametroVista}
        data: { numeroSocio: numeroSocio, reporte: JSON.stringify(reporte), idTipoNotificacion: $('#cmbSoftoken').select().val() },
        url: rootUrl("/Banca/ReposicionSoftoken"),
        dataType: 'json',
        method: 'post',

        success: function (data) {
            if (data.estatus == 1) {
                //alert(data.mensaje);
                $("#dvLoading").attr("hidden", "hidden");
                $('#mensaje-reposicionar').text(data.mensaje);
                $('#myModalReposicionar').modal({ fadeDuration: 100 });
                //setInterval(function () { window.location = rootUrl("/Registros/Registros"); }, 2000);
            } else {
                $("#dvLoading").attr("hidden", "hidden");
                alert(data.mensaje);
            }
        },
        error: function () {
            $("#dvLoading").attr("hidden", "hidden");
            MontoIncorrecto("No fue posible el reenvio de la liga para el enrolamiento del socio, intenta mas tarde");
        }
    });
}


function escapeHtml(unsafe) {
    return unsafe
         .replace(/&/g, "&amp;")
         .replace(/</g, "&lt;")
         .replace(/>/g, "&gt;")
         .replace(/"/g, "&quot;")
         .replace(/'/g, "&#039;");
}




