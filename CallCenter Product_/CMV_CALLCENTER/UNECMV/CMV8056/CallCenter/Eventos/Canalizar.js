var numFiles = 0;
$(document).ready(function () {
    var montoRecl = $("#inputImporteReclamacion").val().toString().replace(/ |,/g, '');
    montoRecl = montoRecl.toString().replace('$', '');
    $("#inputImporteReclamacion").val(formatCurrency($("#inputImporteReclamacion").val()));
    $(".js-example-basic-single").select2({ placeholder: "Seleccionar usuario" });
    $(".js-example-basic-multiple").select2({ placeholder: "Seleccionar usuarios" });
    $('#fechaTransaccion').datepicker({ autoclose: true, format: 'yyyy/mm/dd' });
    $('#div-diasReporte').hide();
    $('#file').change(function () {
        var fp = $("#file");
        console.log(fp[0].files.length);
        numFiles = fp[0].files.length;
        $("#file").removeClass("rojoValidacion");
    });

    ElementosVisiblesCallcenter();

});

function inputArea() {
    $("#comentarios").removeClass("rojoValidacion");
}

function btnAceptarModal() {
    setTimeout(function () { document.getElementById("frmEnviarArchivo").submit(); }, 1000);
}

function canalizar() {
    var valida = true;
    valida = validaCanalizacion();
    if (valida === true) {

        console.log("1")
        $("#dvLoading").removeAttr("hidden");
        $("#btnCanalizar").attr("disabled", "disabled");
        var usuarios = new Array()
        usuarios = $("#selectUsuariosCopia").val();
        $.ajax({
            type: "POST", url: rootUrl("/Canalizar/canalizaUsuario"),
            data: {
                folio: $("#inputFolio").val(), numusuario: $("#inputNumusuario").val(), responsable: $("#selectUsuario").val(),
                COresponsables: "" + usuarios, comentario: $("#comentarios").val(), idMedioDeteccion: $("#selectMedioMov").val()
            },
            dataType: "json",
            success: function (estatus) {
                $("#dvLoading").attr("hidden", "hidden");

                ///////////////////////////////////////////////
                $("#dvLoading").attr("hidden", "hidden");
                $("#divAlertaExito").removeAttr("hidden");

                if (folioBanca != null && folioBanca > 0) {
                    //Date.prototype.toString = function () { return this.getDate() + "/" + (this.getMonth() + 1) + "/" + this.getFullYear(); }
                    var miFecha = moment();
                    var dias = $('#reporteDiasRestantes').val(); // Número de días a agregar
                    //miFecha.setDate(miFecha.getDate() + dias);
                    var fechaFin = miFecha.add('days', dias).format("DD/MM/YYYY");

                    //console.log(miFecha);
                    $('#mensaje-canalizacionBanca').text("Se realizó la canalización del reporte de forma exitosa");
                    $('#myModalCanalizacionBanca').modal({ backdrop: 'static', keyboard: false, fadeDuration: 100 });
                } else {
                    //if (numFiles > 0)
                    setTimeout(function () { document.getElementById("frmEnviarArchivo").submit(); }, 1000);
                    //else
                    //    setTimeout(function () { window.location.href = rootUrl("/Inicio/Inicio"); }, 1000);
                }


            },
            error: function (xhr, status) {
                if (folioBanca != null && folioBanca > 0) {
                    $('#mensaje-canalizacionBanca').text("Ha ocurrido un error al canalizar");
                    $('#myModalCanalizacionBanca').modal({ backdrop: 'static', keyboard: false, fadeDuration: 100 });
                }
                ////////////////////////////////////////
                $("#dvLoading").attr("hidden", "hidden");
                $("#divAlertaFracaso").removeAttr("hidden");
            },
        });
    }
    else {
        $("#divAlertaInvalido").removeAttr("hidden");
        $("#btnCanalizar").removeAttr("disabled");
    }
}


function validaCanalizacion() {
    var validacion = true;

    if ($("#selectUsuario").val() === null || $("#selectUsuario").val() === "0") {
        $("#selectUsuario").addClass("rojoValidacion");
        $("#divAlertaUsuario").removeAttr("hidden");
        validacion = false;
    }

    if ($("#comentarios").val() === "") {
        $("#comentarios").addClass("rojoValidacion");
        validacion = false;
    }

    if (folioBanca != null && folioBanca > 0) {
        if ($("#selectMedioMov").val() == "0") {
            $("#selectMedioMov").addClass("rojoValidacion");
            validacion = false;
        }

        if ($("#selectTipoCuentaBanca").val() == "0") {
            $("#selectTipoCuentaBanca").addClass("rojoValidacion");
            validacion = false;
        }
        if ($("#inputImporteReclamacion").val() == "") {
            $("#inputImporteReclamacion").addClass("rojoValidacion");
            validacion = false;
        }

        if ($("#inputFolioAutorizacion").val() == "") {

            $("#inputFolioAutorizacion").addClass("rojoValidacion");
            validacion = false;
        }
        if ($("#fechaTransaccion").val() == "") {
            $("#fechaTransaccion").addClass("rojoValidacion");
            validacion = false;
        }

        if ($("#inputFolioAutorizacion").val() == "") {
            $("#inputFolioAutorizacion").addClass("rojoValidacion");
            validacion = false;
        }
        if ($("#selectPtmoCapt").val() == 6) {
            if ($("#inputFolioAutorizacion").val() != "" && $("#inputFolioAutorizacion").val() != "0") {
                var numeroSocio = $("#inputNumSocio").val();
                var folioAutorizacion = $("#inputFolioAutorizacion").val();

                $.ajax({
                    method: "POST",
                    url: rootUrl("/Banca/ValidarFolioBanca"),
                    data: { numeroSocio, folioAutorizacion, folioBanca },
                    dataType: "json",
                    async: false,
                    success: function (data) {

                        if (!data.estatus) {
                            $("#inputFolioAutorizacion").addClass("rojoValidacion");
                            alertify.alert(data.mensaje);

                            validacion = false;
                        }

                    },
                    error: function (xhr, status) {
                        $("#dvLoading").attr("hidden", "hidden");
                        $("#divAlertaFracaso").removeAttr("hidden");
                    },
                });

            }
        }
        
    }


    if (($("#selectReporte").val() == "1" || $("#selectReporte").val() == "3") && $("#selectTipoCuenta").val() == "5" && $("#numEvidencias").val() == "0") {
        if (numFiles < 2) {
            validacion = false;
            $("#file").addClass("rojoValidacion");
            alertify.alert("Es necesario adjuntar formato de solicitud y copia de identificación.");
        }
    }

    

    return validacion;
}


function ElementosVisiblesCallcenter() {
    console.log(folioBanca);
    if (folioBanca != null && folioBanca > 0) {
        $("#div-Tel").hide();
        $("#div-suc-causa").hide();
        $("#inputTelCel").attr("readOnly");
        $("#div-representante-leg").hide();
        $("#div-tarjeta-cred").hide();

        ///datos ocultos por normativa//
        $("#div-celular").hide();
        $("#div-domicilio").hide();
        $("#div-sucursal").hide();
        $("#div-EntFed").hide();

        if ($("#selectPtmoCapt").val() == 6) {
            $("#inputImporteReclamacion").val(formatCurrency(importeReclamacion));
            $("#div-importeReclamacion").hide();

        }
        else {
            $("#inputImporteReclamacion").val("");
            $("#div-inputImporteReclamacion").hide();
            $("#divFechaTransaccion").hide();
            $("#div-formato27").hide();
            $("#div-medioDeteccionMov").hide();
            $("#div-folioAutorizacion").hide();
            $("#div-tipoCuenta").hide();


            var fechaHoy = new Date();
            $("#fechaTransaccion").val(fechaHoy.toLocaleDateString());
            $("#selectMedioMov").val("1");
            $("#inputFolioAutorizacion").val("000");
            $("#selectTipoCuentaBanca").val("1");
        }

        $("#div-fech-aclaracion").hide();
        $("#div-fech-transaccion").hide();
        $("#div-no-referencia").hide();
        $("#div-select-medDetMov").hide();
        $("#divFechaAclaracion").hide();
        $("#fechaTransaccion").removeAttr("disabled");
        $("#fechaTransaccion").removeAttr("readOnly");


        if (estatusReporte == 2) {
            $("#selectMedioMov").removeAttr("disabled");
            $("#inputFolioAutorizacion").removeAttr("disabled");
            $("#div-folio-autorizacion").hide();
        }
        if ($("#selectPtmoCapt").val() == 5) {
            $("#div-importeReclamacion").hide();
            $("#inputImporteReclamacion").val("1");
        }

    }
    else {
        $("#div-medioDeteccionMov").hide();
        $("#div-folioAutorizacion").hide();
        $("#div-tipoCuenta").hide();
    }

}

function RespCorrecta(select) {
    if (select == 'selectTipoCuentaBanca') {
        $("#selectTipoCuentaBanca").removeClass("rojoValidacion");
    }
    else if (select == 'selectMedioMov') {
        $("#selectMedioMov").removeClass("rojoValidacion");
    }
    else if (select == 'inputImporteReclamacion') {
        $("#inputImporteReclamacion").removeClass("rojoValidacion");
    }
    else if (select == 'inputFolioAutorizacion') {
        $("#inputFolioAutorizacion").removeClass("rojoValidacion");
    }
    else if ($("#fechaTransaccion").val() == "") {
        $("#fechaTransaccion").removeClass("rojoValidacion");
    }
}

function mostrarFormato_(url) {

    url = rootUrl(url);
    $('#content-formato').html("");
    var iframe = $('<embed src="" width="100%" height="600" alt="pdf" pluginspage="http://www.adobe.com/products/acrobat/readstep2.html">');
    iframe.innerHTML = "";
    iframe.attr('src', url);
    $('#content-formato').append(iframe);
    var content = iframe.innerHTML;
    iframe.innerHTML = content;
    $("#dvLoading").attr("hidden", "hidden");
    
}

function mostrarFormato() {
    $("#dvLoading").removeAttr("hidden");
    
    var valida = true;
    valida = validaCanalizacion();

    var datosAdicionales = {};
    datosAdicionales.reporteFolioBanca = folioBanca;
    datosAdicionales.fechaTransacion = $("#fechaTransaccion").val();
    datosAdicionales.importeReclamacion = $("#inputImporteReclamacion").val();
    datosAdicionales.medioDeteccionMovimiento = $("#selectMedioMov option:selected").text();
    datosAdicionales.folioAutorizacion = $("#inputFolioAutorizacion").val();
    datosAdicionales.tipoCuenta = $("#selectTipoCuentaBanca option:selected").text();
    if (valida === true) {
        $.ajax({
            method: "POST",
            url: rootUrl("/Banca/GenearaFormatoPDF"),
            data: datosAdicionales,
            dataType: "json",
            success: function (data) {
                if (data.estatus) {                    
                    mostrarFormato_(data.ruta);
                    
                } else {
                    alert(data.mensaje);
                }

            },
            error: function (xhr, status) {
                $("#dvLoading").attr("hidden", "hidden");
                $("#divAlertaFracaso").removeAttr("hidden");
            },
        });


        $('#mensaje-formato').text("aqui va el PDF");
        $('#myModalFormatoBanca').modal({ fadeDuration: 100, backdrop: 'static' });
    }

}

function confirmacionVerificada() {
    $("#myModalFormatoBanca").modal("hide");
    $("#myModalConfirmarImpresion").modal("hide");
    canalizar();
    //btnAceptarModal();
    //$('#myModalCanalizacionCorrecta').modal({ fadeDuration: 100 });

}
function irInicio() {
    window.location = rootUrl("/Registros/Registros");
}

function confirmarImpresion() {
    $('#myModalConfirmarImpresion').modal({ fadeDuration: 100 });
}


