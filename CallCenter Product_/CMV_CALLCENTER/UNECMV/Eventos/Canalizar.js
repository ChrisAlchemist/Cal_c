var numFiles = 0;

var tablaReportesBanca;
var reporteBanca;
var cantReportes = 0;

$(document).ready(function () {
    var montoRecl = $("#inputImporteReclamacionMultifolio").val().toString().replace(/ |,/g, '');
    montoRecl = montoRecl.toString().replace('$', '');
    $("#inputImporteReclamacionMultifolio").val(formatCurrency($("#inputImporteReclamacionMultifolio").val()));
    $(".js-example-basic-single").select2({ placeholder: "Seleccionar usuario" });
    $(".js-example-basic-multiple").select2({ placeholder: "Seleccionar usuarios" });
    $('#fechaTransaccion').datepicker({ autoclose: true, format: "dd/mm/yyyy", endDate: new Date() });

    $('#div-diasReporte').hide();
    $('#file').change(function () {
        var fp = $("#file");
        console.log(fp[0].files.length);
        numFiles = fp[0].files.length;
        $("#file").removeClass("rojoValidacion");
    });

    ElementosVisiblesCallcenter();

    //Reportes banca
    tablaReportesBanca = $("#tbl-reportesBanca").dataTable({ "searching": false, "lengthChange": false, /*"paging": false,*/ "info": false});
    MostrarAllReportesBanca(parseInt($("#kbd-folioUNE").text()));

});


//Reportes banca

function MostrarAllReportesBanca(folioUNE) {
    $.ajax({
        url: rootUrl("/Banca/ObtenerReportesBancaJson"),
        data: { folioUNE: folioUNE },
        dataType: "json",
        method: "post",
        beforeSend: function () {
            //MostrarLoading();
        },
        success: function (data) {
            //OcultarLoading();
            PintarTablaReportesBanca(data.reportesSocio);
            cantReportes = data.reportesSocio.length;
        },
        error: function (xhr, status, error) {
            ControlErrores(xhr, status, error);
        }
    });
}

function PintarTablaReportesBanca(data) {
    tablaReportesBanca.fnClearTable();
    tablaReportesBanca.fnDraw();
    data.forEach(function (reporte, index) {
        tablaReportesBanca.fnAddData([
            reporte.folioAutorizacion,
            formatCurrency(reporte.importeReclamo),
            reporte.medioMovimiento,
            reporte.tipoCuentaBanca,
            ObtenerFechaJson(reporte.fechaTransacion),
            
            //'<span data-toggle="tooltip" title="Editar"><button type="button" class="btn btn-icon btn-sm waves-effect waves-light btn-warning" data-toggle="modal" data-target="#modal-reporteBanca"' +
            //'onclick="OnMostrarEditarReporteBanca(\'' + reporte.idIncidenciaReporte + '\')"> <span class ="glyphicon glyphicon-edit"/> </button></span>' + ' ' +
            '<span data-toggle="tooltip" title="Eliminar"><button type="button" class="btn btn-icon btn-sm waves-effect waves-light btn-danger" data-toggle="modal" data-target="#myModal"' +
            'onclick="OnMostrarEliminarReporteBanca(\'' + reporte.idIncidenciaReporte + '\')"> <span class ="glyphicon glyphicon-trash"/> </button></span>'
        ]);
        tablaReportesBanca.fnDraw(false);
    });
    $('[data-toggle="tooltip"]').tooltip();
}


function ObtenerFechaJson(fecha) {
    var fecha_ = new Date(fecha.match(/\d+/)[0] * 1);
    return fecha_.toLocaleDateString('en-GB');
}

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

        var fechaDeTransaccion = $('#fechaTransaccion').val();
        if ($("#selectReporte").val() == 4) {
            var hoy = new Date();
            
            fechaDeTransaccion = hoy.getDate() + "/" + (hoy.getMonth() + 1) + "/" + hoy.getFullYear();
        }
        else {
            fechaDeTransaccion = fechaDeTransaccion.toString("yyyy/MM/dd");
        }
        

        $.ajax({
            type: "POST", url: rootUrl("/Canalizar/canalizaUsuario"),
            data: {
                folio: $("#inputFolio").val(), numusuario: $("#inputNumusuario").val(), responsable: $("#selectUsuario").val(),
                COresponsables: "" + usuarios, comentario: $("#comentarios").val(), idMedioDeteccion: $("#selectMedioMov").val(), fechTransaccion: fechaDeTransaccion
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
            error: function (xhr, status, error) {
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
        //if ($("#selectMedioMov").val() == "0") {
        //    $("#selectMedioMov").addClass("rojoValidacion");
        //    validacion = false;
        //}

        //if ($("#selectTipoCuentaBanca").val() == "0") {
        //    $("#selectTipoCuentaBanca").addClass("rojoValidacion");
        //    validacion = false;
        //}
        //if ($("#inputImporteReclamacion").val() == "") {
        //    $("#inputImporteReclamacion").addClass("rojoValidacion");
        //    validacion = false;
        //}

        //if ($("#inputFolioAutorizacion").val() == "") {

        //    $("#inputFolioAutorizacion").addClass("rojoValidacion");
        //    validacion = false;
        //}
        //if ($("#fechaTransaccion").val() == "") {
        //    $("#fechaTransaccion").addClass("rojoValidacion");
        //    validacion = false;
        //}

        //if ($("#inputFolioAutorizacion").val() == "") {
        //    $("#inputFolioAutorizacion").addClass("rojoValidacion");
        //    validacion = false;
        //}
        //if ($("#selectPtmoCapt").val() == 6) {
        //    if ($("#inputFolioAutorizacion").val() != "" && $("#inputFolioAutorizacion").val() != "0") {
        //        var numeroSocio = $("#inputNumSocio").val();
        //        var folioAutorizacion = $("#inputFolioAutorizacion").val();

        //        $.ajax({
        //            method: "POST",
        //            url: rootUrl("/Banca/ValidarFolioBanca"),
        //            data: { numeroSocio, folioAutorizacion, folioBanca },
        //            dataType: "json",
        //            async: false,
        //            success: function (data) {

        //                if (!data.estatus) {
        //                    $("#inputFolioAutorizacion").addClass("rojoValidacion");
        //                    alertify.alert(data.mensaje);

        //                    validacion = false;
        //                }

        //            },
        //            error: function (xhr, status) {
        //                $("#dvLoading").attr("hidden", "hidden");
        //                $("#divAlertaFracaso").removeAttr("hidden");
        //            },
        //        });

        //    }
        //}

        if ($("#selectPtmoCapt").val() == 6) {
            if (cantReportes == 0) {
                validacion = false;
                alertify.alert("Se debe de agregar minimo un folio para poder canalizar un reporte.");
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
            $("#inputImporteReclamacionMultifolio").val(formatCurrency(importeReclamacion));
            $("#div-importeReclamacion").hide();

        }
        else {
            $("#inputImporteReclamacion").val("");
            $("#inputImporteReclamacionMultifolio").val("");
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
        //$("#div-importeReclamacion").hide();
        //var fechahoy = new Date();
        $("#div-inputImporteReclamacion").hide();
        //$("#fechaTransaccion").val(fechahoy.getDate() + "/" + (fechahoy.getMonth() + 1) + "/" + fechahoy.getFullYear());
        //$("#divFechaTransaccion").hide();

    }
    else {
        $("#div-medioDeteccionMov").hide();
        $("#div-folioAutorizacion").hide();
        $("#div-tipoCuenta").hide();
    }


}

function RespCorrecta(select) {

    if (select == 'selectMedioMov') {
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


    var valida = true;
    valida = validaCanalizacion();

    //$('#fechaTransaccion').datepicker({ autoclose: true, format: "dd/mm/yyyy" });

    var datosAdicionales = {};
    datosAdicionales.reporteFolioBanca = folioBanca;
    datosAdicionales.fechaTransacion = $("#fechaTransaccion").val();
    datosAdicionales.importeReclamacion = $("#inputImporteReclamacion").val();
    datosAdicionales.medioDeteccionMovimiento = $("#selectMedioMov option:selected").text();
    datosAdicionales.folioAutorizacion = $("#inputFolioAutorizacion").val();
    datosAdicionales.tipoCuenta = $("#selectTipoCuentaBanca option:selected").text();
    if (valida === true) {
        $("#dvLoading").removeAttr("hidden");
        $.ajax({
            method: "POST",
            url: rootUrl("/Banca/GenerarFormatoPDF"),
            data: datosAdicionales,
            dataType: "json",
            success: function (data) {
                if (data.existeSesion) {
                    if (data.estatus) {
                        mostrarFormato_(data.ruta);
                    }
                    else {
                        alert(data.mensaje);
                    }
                }
                else {
                    window.location = rootUrl("/Login/Index");
                }

            },
            error: function (xhr, status, error) {
                if (data.existeSesion) {
                    $("#dvLoading").attr("hidden", "hidden");
                    $("#divAlertaFracaso").removeAttr("hidden");
                    alert(error);
                }
                else {
                    window.location = rootUrl("/Login/Index");
                }
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


function onChangeTipoCuenta(select) {
    if (select == 'selectTipoCuentaBanca') {
        $("#selectTipoCuentaBanca").removeClass("rojoValidacion");
    }

    if ($("#selectTipoCuentaBanca option:selected").val() == 2) {

        $.ajax({
            method: "POST",
            url: rootUrl("/Banca/ValidarTienePrestamoSocio"),
            data: { numeroSocio: $("#inputNumSocio").val() },
            dataType: "json",
            success: function (data) {
                if (!data.socioTienePrestamo) {
                    $("#selectTipoCuentaBanca").val(0);
                    $('#myModalNoTienePrestamo').modal({ fadeDuration: 100, backdrop: 'static' });
                    //alert("No tiene Prestamo");

                }
                //else {
                //    alert("tiene Prestamo");
                //}

            },
            error: function (xhr, status, error) {
                alert(error);
            },
        });


        //$('#mensaje-formato').text("aqui va el PDF");
        //$('#myModalNoTienePrestamo').modal({ fadeDuration: 100, backdrop: 'static' });
    }

}

function validaDatosMultifolio() {
    var validacion = true;
    if (folioBanca != null && folioBanca > 0) {
        if ($("#selectMedioMov").val() == "0") {
            $("#selectMedioMov").addClass("rojoValidacion");
            validacion = false;
        }

        if ($("#selectTipoCuentaBanca").val() == "0") {
            $("#selectTipoCuentaBanca").addClass("rojoValidacion");
            validacion = false;
        }
        if ($("#inputImporteReclamacionMultifolio").val() == "") {
            $("#inputImporteReclamacionMultifolio").addClass("rojoValidacion");
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

        var montoRecl = $("#inputImporteReclamacionMultifolio").val().toString().replace(/ |,/g, '');
        montoRecl = montoRecl.toString().replace('$', '');
        $("#inputImporteReclamacionMultifolioHidden").val(montoRecl);
        var importe = Number($("#inputImporteReclamacionMultifolioHidden").val());
        if (importe <= 0) {
            $("#inputImporteReclamacionMultifolio").addClass("rojoValidacion");
            validacion = false;
        }
    }
    return validacion;
}

function AgregarMultifolio() {
    var validaReporte = true;
    validaReporte = validaDatosMultifolio();

    var datosAdicionales = {};
    
    datosAdicionales.folioUNE = parseInt($("#kbd-folioUNE").text()); // folioBanca;
    datosAdicionales.folioAutorizacion = parseInt($("#inputFolioAutorizacion").val());
    datosAdicionales.numeroSocio = parseInt($("#inputNumSocio").val());
    datosAdicionales.importeReclamo = parseFloat($("#inputImporteReclamacionMultifolio").val().replace("$", "").replace(",",""));
    datosAdicionales.idMedioMovimiento = parseInt($("#selectMedioMov option:selected").val());
    datosAdicionales.idTipoCuentaBanca = parseInt($("#selectTipoCuentaBanca option:selected").val());
    datosAdicionales.fechaTransacionEnviada = $("#fechaTransaccion").val();
    //datosAdicionales.fechaTransacion = $("#fechaTransaccion").val($.format.date(new Date(), 'yyyyMMdd')).val();
    

    
    if (validaReporte === true) {
        $("#dvLoading").removeAttr("hidden");
        $.ajax({
            method: "POST",
            url: rootUrl("/Banca/InsertarReportesBancaJson"),
            data: datosAdicionales,
            dataType: "json",
            success: function (data) {
                if (data.estatus != 1) {
                    $("#dvLoading").attr("hidden", "hidden");
                    alertify.alert("El folio que intentas agregar ya existe para este reporte");
                    
                    MostrarAllReportesBanca(parseInt($("#kbd-folioUNE").text()));
                }
                else {
                    $("#dvLoading").attr("hidden", "hidden");
                    $("#myModalNuevoMiltifolioBanca").modal("hide");
                    alertify.alert("Folio agregado con exito");
                    
                    MostrarAllReportesBanca(parseInt($("#kbd-folioUNE").text()));
                }

            },
            error: function (xhr, status, error) {
                if (data.existeSesion) {
                    $("#dvLoading").attr("hidden", "hidden");
                    $("#divAlertaFracaso").removeAttr("hidden");
                    alert(error);
                }
                else {
                    window.location = rootUrl("/Login/Index");
                }
            },
        });
    }
}

function validaInputImporte(value) {
    $("#inputImporteReclamacionMultifolio").removeClass("rojoValidacion");
}

function limpiarInputReporte() {

    var montoReclamacion = parseFloat($("#inputImporteReclamacionMultifolio").val().replace("$", "").replace(",",""));
    if (montoReclamacion == 0) {
        $("#inputImporteReclamacionMultifolio").val("");
    } else {
        $("#inputImporteReclamacionMultifolio").val(montoReclamacion);
    }
}

function validaNumeros(value) {
    return value.match(/^\d+\.?\d{0,1}/);
}


function OnMostrarEliminarReporteBanca(idIncidenciaReporte) {

    $("#dvLoading").removeAttr("hidden");
    $.ajax({
        method: "POST",
        url: rootUrl("/Banca/EliminarReportesBancaJson"),
        data: { idIncidenciaReporte: idIncidenciaReporte },
        dataType: "json",
        success: function (data) {
            if (data.estatus != 1) {
                $("#dvLoading").attr("hidden", "hidden");
                alertify.alert("El folio que intenta eliminar no existe");                
                MostrarAllReportesBanca(parseInt($("#kbd-folioUNE").text()));
            }
            else {
                $("#dvLoading").attr("hidden", "hidden");
                //$("#myModalNuevoMiltifolioBanca").modal("hide");
                alertify.alert("Folio eliminado exitosamente");                
                MostrarAllReportesBanca(parseInt($("#kbd-folioUNE").text()));
            }
             
        },
        error: function (xhr, status, error) {
            if (data.existeSesion) {
                $("#dvLoading").attr("hidden", "hidden");
                $("#divAlertaFracaso").removeAttr("hidden");
                alert(error);
            }
            else {
                window.location = rootUrl("/Login/Index");
            }
        },
    });
}

function LimpiarDatosFolio() {
    $("#inputFolioAutorizacion").removeClass("rojoValidacion");
    $("#inputImporteReclamacionMultifolio").removeClass("rojoValidacion");
    $("#selectMedioMov").removeClass("rojoValidacion");
    $("#selectTipoCuentaBanca").removeClass("rojoValidacion");
    $("#fechaTransaccion").removeClass("rojoValidacion");

    $("#inputFolioAutorizacion").val("");
    $("#inputImporteReclamacionMultifolio").val("$0.00");    
    $("#selectMedioMov").val(0);
    $("#selectTipoCuentaBanca").val(0);
    $("#fechaTransaccion").val("");
}

