var supuestosMonto = new Array();
var numFiles = 0;
var urlFormatoAclProcedente;

var tablaReportesBanca;
var reporteBanca;
var cantReportes = 0;
var esProcedente = 1;
var multifoliosPendientes;


$(document).ready(function () {
    urlFormatoAclProcedente = "";
    var montoRecl = $("#inputImporteReclamacion").val().toString().replace(/ |,/g, '');
    montoRecl = montoRecl.toString().replace('$', '');
    $("#inputImporteReclamacionHidden").val(montoRecl);
    $("#inputImporteReclamacion").val(formatCurrency($("#inputImporteReclamacion").val()));
    if ($("#inputConfirmarImporte").val() != null || $("#inputConfirmarImporte").text() != "") {
        $("#inputConfirmarImporte").val(formatCurrency($("#inputConfirmarImporte").val()));

    }
    
    if ($("#inputSolucion").val() !== "" && $("#inputSolucion").val()!== undefined)
    {
        var montoSol = $("#inputSolucion").val().toString().replace(/ |,/g, '');
        montoSol = montoSol.toString().replace('$', '');
        $("#inputSolucionHidden").val(montoSol);
        $("#inputSolucion").val(formatCurrency($("#inputSolucion").val()));
    }
    $("#btnEliminaDoc").click(function (evt) {evt.preventDefault();});
    cargarSuspuestosMonto();
    $('#fechaAclaracion').datepicker({autoclose: true,format: 'dd/mm/yyyy'});
    $('#fechaTransaccion').datepicker({ autoclose: true, format: 'dd/mm/yyyy' });

    //$('#fechaAbono').datepicker({ format: 'dd/mm/yyyy' });
    //$("#div-folioCorrespondienteBanca").attr("hidden", "hidden");
    //$("#div-select-cuentaNoAfectada").attr("hidden", "hidden");
    //$("#div-montoDepositarBanca").attr("hidden", "hidden");
    ElementosVisiblesCallcenter();

    //Reportes banca
    tablaReportesBanca = $("#tbl-reportesBanca").dataTable({ "searching": false, "lengthChange": false, /*"paging": false,*/ "info": false });
    MostrarAllReportesBanca(parseInt($("#kbd-folioUNE").text()));
    
});

function cargarSuspuestosMonto() {
    $.ajax({
        type: "POST", url: rootUrl("/Registrar/ObtenerSuspuestosValidar"), dataType: "json",
        success: function (ValidarS) {
            var i = 0;
            var item;
            for (item in ValidarS) {
                supuestosMonto[i] = ValidarS[item].ID_SUPUESTOS_REPORTE;
                i = i + 1;
            }
        },
        failure: function (response) { alertify.alert(response.d); }
    });
}

function generaSubmit() {
    var validacion = false;
    validacion = valida();
    if (validacion === true)
        document.getElementById("frmModificar").submit();
}

function habilitarModificacion() {
    $("#inputTel").removeAttr("readonly");
    if (usuarioCallCenter != true) {
        $("#inputTelCel").removeAttr("readonly");
    }
    $("#selectReporte").removeAttr("disabled");
    $("#selectSupuesto").removeAttr("disabled");
    $("#inputDescripcion").removeAttr("readOnly");
    $("#selectPtmoCapt").removeAttr("disabled");
    $("#selectTipoCuenta").removeAttr("disabled");
    $("#inputMedioContacto").removeAttr("disabled");
    $("#btnGuardar").removeAttr("disabled");
    $("#btnCancelar").removeAttr("disabled");

    if($("#selectReporte").val()==="3")
    {
        $("#inputImporteReclamacion").removeAttr("readonly");
    }
}

function cargarSupuestos(id) {
    $("#selectReporte").removeClass("rojoValidacion");
    $("#selectPtmoCapt").val(0);
    $("#selectTipoCuenta").val(0);
    $("#selectTipoCuenta").attr("disabled", "disabled");
    $("#selectSupuesto").val(0);
    $("#selectSupuesto").attr("disabled", "disabled");

    if (id != 0) {
        $("#selectPtmoCapt").removeAttr("disabled");
        $("#selectReporte").removeClass("rojoValidacion");

    } else {
        $("#selectPtmoCapt").attr("disabled", "disabled");
    }

    if (id === "3") {
        $("#inputImporteReclamacion").removeAttr("readOnly");
    }
    else {
        $("#inputImporteReclamacion").attr("readOnly", "readOnly");
        $("#inputImporteReclamacion").val("0.0");
    }
}

function valida()
{
    var validacion = true;

    if ($("#inputTel").val() == "" && $("#inputTelCel").val() == "")
    {
        if ($("#inputTel").val() === "") {
            $("#inputTel").addClass("rojoValidacion");
            validacion = false;
        }

        if ($("#inputTelCel").val() === "") {
            $("#inputTelCel").addClass("rojoValidacion");
            validacion = false;
        }
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

    if ($("#selectReporte").val() === "3")
    {
        for (var i = 0; i < supuestosMonto.length; i++) {
            if (supuestosMonto[i] == Number($("#selectSupuesto").val())) {
                var montoRecl = $("#inputImporteReclamacion").val().toString().replace(/ |,/g, '');
                montoRecl = montoRecl.toString().replace('$', '');
                $("#inputImporteReclamacionHidden").val(montoRecl);
                var importe = Number($("#inputImporteReclamacionHidden").val());
                if (importe <= 0) {
                    $("#inputImporteReclamacion").addClass("rojoValidacion");
                    validacion = false;
                }
                break;
            }
        }
    }

    if ($("#selectPtmoCapt").val() === "0") {
        $("#selectPtmoCapt").addClass("rojoValidacion");
        validacion = false;
    }

    if ($("#selectTipoCuenta").val() === "0" || $("#selectTipoCuenta").val() === "") {
        $("#selectTipoCuenta").addClass("rojoValidacion");
        validacion = false;
    }

    if ($("#selectTipoCuenta").val() === "5" && ($("#selectReporte").val() == "1" || $("#selectReporte").val() == "3") && Number($("#numEvidencias").val()) < 2) {
        if (numFiles < 2) {
            validacion = false;
            $("#file").addClass("rojoValidacion");
            alertify.alert("Es necesario adjuntar formato de solicitud y copia de identificación.");
        }
    }

    if ($("#inputNumeroCuentaPtmo").val() === "") {
        validaForm = false;
        $("#inputNumeroCuentaPtmo").addClass("rojoValidacion");
    }

    if ($("#selectCanal").val() === "0") {
        validaForm = false;
        $("#selectCanal").addClass("rojoValidacion");
    }

    if ($("#selectMotivoCancelacion").val() === "0") {
        validaForm = false;
        $("#selectMotivoCancelacion").addClass("rojoValidacion");
    }

    if ($("#selectProducto").val() === "0") {
        validaForm = false;
        $("#selectProducto").addClass("rojoValidacion");
    }

    if (($("#selectReporte").val() === "3" || $("#selectReporte").val() === "1") && $("#selectTipoCuenta").val() === "5") {
        if ($("#inputImporteReclamacion").val() === "$0.00") {
            validaForm = false;
            $("#inputImporteReclamacion").addClass("rojoValidacion");
        }
    }
   
    return validacion;
}

function supuestos(value)
{
    var supuestosDebito = new Array();
    supuestosDebito = [39, 42, 44, 45, 46, 47, 3, 4, 7, 8, 9];
    if (value != 0)
    {
        for (var i = 0; i < supuestosDebito.length; i++) {
            if (supuestosDebito[i] == value) {
                alertify.alert('<p align=LEFT>PARA AGILIZAR LA RESOLUCIÓN DE ESTA CAUSA ES IMPORTANTE ESPECIFICAR, <strong>EN EL AREA DE DESCRIPCIÓN</strong>, LA SIGUIENTE INFORMACIÓN: <br /> <b /><ul align=LEFT><li>LUGAR DONDE SE LLEVO ACABO LA OPERACIÓN</li> <li>FECHA DE LA OPERACIÓN</li></ul>');
                document.getElementById("inputDescripcion").focus();
                $("#inputDescripcion").addClass("rojoValidacion");
                break;
            }
        }
        $("#selectSupuesto").removeClass("rojoValidacion");
        $("#btnGuardar").removeAttr("disabled");

        $.ajax({
            type: "POST", url: rootUrl("/Registrar/ObtenerCanal"), data: { idSupuesto: value }, dataType: "json",
            success: function (idCanal) {
                document.getElementById("selectCanal").value = idCanal;
            },
            failure: function (response) { alertify.alert(response.d); }
        });

        $.ajax({
            type: "POST", url: rootUrl("/Registrar/ObtenerMotivoCancelacion"), data: { idSupuesto: value }, dataType: "json",
            success: function (idMotivo) {
                document.getElementById("selectMotivoCancelacion").value = idMotivo;
            },
            failure: function (response) { alertify.alert(response.d); }
        });
    }
    else {
        document.getElementById("selectCanal").value = 0;
        document.getElementById("selectMotivoCancelacion").value = 0;
    }
}

function descripcionReporte()
{
    $("#inputDescripcion").removeClass("rojoValidacion");
    $("#btnGuardar").removeAttr("disabled");
}

function selectCuenta(value) {

    $("#selectTipoCuenta").val(0);
    $("#selectSupuesto").val(0);
    $("#selectSupuesto").attr("disabled", "disabled");

    if ($("#selectPtmoCapt").val() !== "0")
    {
        $("#selectTipoCuenta").removeAttr("disabled");
        $("#selectPtmoCapt").removeClass("rojoValidacion");
        $.ajax({
            type: "POST", url: rootUrl("/Registrar/obtenerCuentas"), data: { tipoCuenta: $("#selectPtmoCapt").val(), numero: $("#inputNumSocio").val() }, dataType: "json",
            success: function (cuentas) {
                var html = '';
                html += '<option value="0">SELECCIONAR</option>';
                $.each(cuentas, function (i, cuenta) {
                    html += '<option value="' + cuenta.ID_CUENTA + '">' + cuenta.DESCRIPCION + '</option>';
                });

                $("#selectTipoCuenta").html(html);
                $("#btnGuardar").removeAttr("disabled");
            }
        });
    }
    else { $("#selectTipoCuenta").attr("disabled", "disabled"); }
}

function tipoCuenta(value) {
    if (value != 0)
    {
        $("#selectTipoCuenta").removeClass("rojoValidacion");
        $("#selectSupuesto").removeAttr("disabled");

        $.ajax({
            type: "POST",url: rootUrl("/Registrar/ObtenerSupuestos"),
            data: { id: $("#selectReporte").val(), idTipoCuenta: $("#selectPtmoCapt").val(), idCuenta: value },dataType: "json",
            success: function (supuestos) {
                var html = '<option value="0">SELECCIONAR</option>';
                $.each(supuestos, function (i, supuesto) {
                    html += '<option value="' + supuesto.ID_SUPUESTOS_REPORTE + '">' + supuesto.DESCRIPCION + '</option>';
                });

                $("#selectSupuesto").html(html);
            }
        });
        if (value == 5 && ($("#selectReporte").val() == "1" || $("#selectReporte").val() == "3")) {
            $("#fechaAclaracion").removeAttr("disabled");
            $("#fechaTransaccion").removeAttr("disabled");
            $("#fechaAclaracion").val(moment().format("DD/MM/YYYY"));
            $("#fechaTransaccion").val(moment().format("DD/MM/YYYY"));
            $("#labelAdjuntar").text("Adjuntar evidencias de debito(formato de solicitud y copia de identificación):");
            $("#labelNoObli").attr("hidden","hidden");
        }
        else {
            $("#fechaAclaracion").attr("disabled", "disabled");
            $("#fechaTransaccion").attr("disabled", "disabled");
            $("#labelAdjuntar").text("Adjuntar Archivo*:");
            $("#labelNoObli").removeAttr("hidden");
        }

        if (($("#selectReporte").val() === "1" && $("#selectTipoCuenta").val() === "5") || ($("#selectReporte").val() === "3")) {
            $("#inputImporteReclamacion").removeAttr("disabled");
        }
        else {
            $("#inputImporteReclamacion").attr("disabled", "disabled");
            $("#inputImporteReclamacion").val("$0.00");
        }

        $("#inputNumeroCuentaPtmo").val("");

        if ($("#selectPtmoCapt").val() === "2") {
            $("#labelinputNumeroCuentaPtmo").html("Número de préstamo:");
            $("#inputNumeroCuentaPtmo").removeAttr("readOnly");
        }
        else if ($("#selectPtmoCapt").val() === "1") {
            if (value === "4") {
                $("#labelinputNumeroCuentaPtmo").html("Número de DPF:");
                $("#inputNumeroCuentaPtmo").removeAttr("readOnly");
            } else if (value === "5") {
                $("#labelinputNumeroCuentaPtmo").html("Número de tarjeta(Débito):");
                $("#inputNumeroCuentaPtmo").val($("#inputNumTarjeta").val());
                $("#inputNumeroCuentaPtmo").attr("readOnly", "readOnly");
            }
            else {
                $("#labelinputNumeroCuentaPtmo").html("Número de cuenta:");
                $("#inputNumeroCuentaPtmo").val($("#inputNumSocio").val());
                $("#inputNumeroCuentaPtmo").attr("readOnly", "readOnly");
            }
        }
        else if ($("#selectPtmoCapt").val() === "3") {
            $("#labelinputNumeroCuentaPtmo").html("Número de cuenta de corresponsal:");
            $("#inputNumeroCuentaPtmo").removeAttr("readOnly");
        }
        else if ($("#selectPtmoCapt").val() === "4") {
            $("#labelinputNumeroCuentaPtmo").html("Número de cuenta:");
            $("#inputNumeroCuentaPtmo").val($("#inputSocio").val());
            $("#inputNumeroCuentaPtmo").attr("readOnly", "readOnly");
        }
        else {
            $("#labelinputNumeroCuentaPtmo").html("Número de cuenta/préstamo/DPF:");
            $("#inputNumeroCuentaPtmo").attr("readOnly", "readOnly");
        }

        var id_mov;
        //Cargamos los idMovs de lo seleccionado en la aplicación
        if ($("#selectReporte").val() === "3")
        {
            $.ajax({
                type: "POST", url: rootUrl("/Registrar/ObtenerProducto"), data: { id: $("#selectTipoCuenta").val() },
                dataType: "json",
                success: function (idProd) {
                    $("#selectProducto").val(idProd);
                }
            });

            $.ajax({
                type: "POST", url: rootUrl("/Registrar/ObtenerIdMovProducto"), data: { id: $("#selectTipoCuenta").val() },
                dataType: "json",
                success: function (prod) {
                    tipoProducto = prod;
                    if (tipoProducto > 0) {
                        $.ajax({
                            type: "POST", url: rootUrl("/Registrar/ObtenerProductoUNE"), data: { idMov: tipoProducto },
                            dataType: "json",
                            success: function (idMov) {
                                id_mov = idMov;
                            }
                        });

                        if (prod === 105) {
                            $.ajax({
                                type: "POST", url: rootUrl("/Registrar/ObtenerPlazosFijos"), data: { numero: $("#inputSocio").val() },
                                dataType: "json",
                                success: function (dpfs) {
                                    console.log(dpfs);
                                    if (dpfs.length === 1) {
                                        $("#labelinputNumeroCuentaPtmo").html("Número de DPF:");
                                        $("#inputNumeroCuentaPtmo").val(dpfs[0].num_dpf);
                                        $("#inputNumeroCuentaPtmo").attr("readonly", "readonly");
                                    }
                                    else if (dpfs.length > 1) {
                                        var html = '';
                                        $.each(dpfs, function (i, dpf) {
                                            var ts = dpf.fecha_mov.substring(6, 19);
                                            ts = Number(ts);
                                            var date = new Date(ts);

                                            html += '<tr><td><button  type="button" class="btn btn-info" onclick="asignarDPF(' + dpf.num_dpf + ')">Asignar</button></td><td>' + dateToYMD(date) + '</td><td>' + dpf.num_dpf + '</td><td>' + dpf.monto + '</td></tr>';
                                        });
                                        $("#cuerpoDPFs").html(html);
                                        //$("table#tblDPFs").dataTable();
                                        $("#modalDPFs").modal("show");
                                    }
                                }
                            });
                        }
                        else if (prod > 0 && prod < 10) {

                            $.ajax({
                                type: "POST", url: rootUrl("/Registrar/ObtenerNumPtmo"), data: { numero: $("#inputSocio").val(), idMov: prod },
                                dataType: "json",
                                success: function (numPtmo) {
                                    $("#labelinputNumeroCuentaPtmo").html("Número de préstamo:");
                                    $("#inputNumeroCuentaPtmo").val(numPtmo[0].NUMPTMO);
                                    $("#inputNumeroCuentaPtmo").attr("readonly", "readonly");
                                }
                            });
                        }
                    }
                }
            });
        }
        

    }
    else {
        $("#selectSupuesto").val(0);
        $("#selectSupuesto").attr("disabled", "disabled");
    }
    if (value == 0) {
        $("#selectSupuesto").val(0);
        $("#selectSupuesto").attr("disabled", "disabled");
    }
}

function modalValor(folio, numFolio) {
    $("#divAlertaExito").attr("hidden", "hidden");
    $("#divAlertaFracaso").attr("hidden", "hidden");
    $("#btnCanalizar").removeAttr("disabled");
    document.getElementById("uneFolio").innerHTML = numFolio;
    document.getElementById("uneFolio2").innerHTML = numFolio;
    $("#inputUNE").val(folio);
}

function canalizar(folio)
{

    if ($("#selectTipoCuenta").val() === "5" && ($("#selectReporte").val() == "1" || $("#selectReporte").val() == "3") && Number($("#numEvidencias").val()) < 2) {
        alertify.alert("Es necesario adjuntar formato de solicitud y copia de identificación.");
    }
    else
    {
        $("#dvLoading").removeAttr("hidden");
        $("#btnCanalizarModal").attr("disabled", "disabled");
        $("#btnCancelarCanalizar").attr("disabled", "disabled");
        $.ajax({
            type: "POST",url: rootUrl("/Inicio/canalizarReporte"),data: { folio: folio },dataType: "json",
            success: function (estatus) {
                if (estatus === 1) {
                    $("#divAlertaExitoCanalizar").removeAttr("hidden");
                    setTimeout(function () { window.location.href = rootUrl("/Registros/Registros"); }, 1000);
                }
                else {
                    $("#divAlertaFracasoCanalizar").removeAttr("hidden");
                    $("#btnCanalizarModal").removeAttr("disabled");
                    $("#btnCancelarCanalizar").removeAttr("disabled");
                    $("#dvLoading").attr("hidden", "hidden");
                }
            }
        });
    }
}

function confirmar(id, tipoRep) {

    $("#divAlertaExitoNotificar").attr("hidden", "hidden");
    $("#areaComentarios").removeClass("rojoValidacion");
    $("#areaComentarios").val("");
    $("#btnGuardarNotificar").removeAttr("disabled");
    document.getElementById("areaComentarios").textContent = " ";

    $("#inputFolio").val(id);
    $.ajax({
        type: "POST",url: rootUrl("/Notificar/ObtenerUltimaRespuesta"),data: { folio: id },
        success: function (comentario) {
            document.getElementById("areaDescripcion").textContent = comentario;
        }
    });

    if (tipoRep === 3) {
        $.ajax({
            type: "POST",url: rootUrl("/Notificar/obtenerMontos"),data: { folio: id },dataType: "json",
            success: function (montos) {
                $("#inputReclamacion").val(montos[0]);
                $("#inputSolucion").val(montos[1]);
            }
        });
    }
}

function finalizar(id, numFolio) {
    
    if ($("#selectReporte").val() == 4) {
       
        var folioUNE = parseInt($("#kbd-folioUNE").text());

        $.ajax({
            url: rootUrl("/Banca/ObtenerMultifoliosPendientesJson"),
            data: { folioUNE: folioUNE },
            dataType: "json",
            method: "post",
            async: false,
            beforeSend: function () {
                //MostrarLoading();
            },
            success: function (data) {
                multifoliosPendientes = data.reportesPendientes;
            },
            error: function (xhr, status, error) {
                ControlErrores(xhr, status, error);
            }
        });

        if (multifoliosPendientes > 0) {
            modalFoliosPendientes(multifoliosPendientes);
            //alert("El reporte aun tiene " + multifoliosPendientes + " folio(s) pendiente(s) de dictaminar");
        }
        else {
            var folioUNE = parseInt($("#kbd-folioUNE").text());
            var dictamen = 0;
            $.ajax({
                url: rootUrl("/Banca/ValidarReporteProcedenteBancaJson"),
                data: { folioUNE: folioUNE },
                dataType: "json",
                method: "post",
                async: false,
                beforeSend: function () {
                    //MostrarLoading();
                },
                success: function (data) {
                    dictamen = data.estatus;
                },
                error: function (xhr, status, error) {
                    ControlErrores(xhr, status, error);
                }
            });

            $("#selectSatisfaccion").val(dictamen);
            $("#div-selectSatisfaccion").hide();
            $('#myModalNotificados').modal({ fadeDuration: 100, backdrop: 'static' });
            
        }

    }
    else {
        $('#myModalNotificados').modal({ fadeDuration: 100, backdrop: 'static' });
        
    }

    //Se valida que si existe multifolio de banca en la tabla entonces la respuesta para el socio siempre sera satisfactoria
    //if (cantReportes > 0) {
        
    //}
    document.getElementById("spanFolioNotificados").innerHTML = numFolio;
    $("#divAlertaExitoNotificados").attr("hidden", "hidden");
    $("#btnGuardarNotificados").removeAttr("disabled");
    $("#inputFolioNotificados").val(id);
    $("#areaComentariosNotificados").removeClass("rojoValidacion");
    $("#areaComentariosNotificados").val("");
}

function keyAreaComentariosNotificados() {
    $("#areaComentariosNotificados").removeClass("rojoValidacion");
}

//function cambioSatisfaccion() {
    
//    $("#selectFolioSatisfaccion").removeClass("rojoValidacion");        
    
//    if ($("#selectPtmoCapt").val() == 6) {

//        $("#folioCorrespondienteBanca").val($("#inputFolioAutorizacion").val());        
        
//        if ($("#selectFolioSatisfaccion").val() == 1) {
//            $("#div-select-cuentaNoAfectada").removeAttr("hidden", "hidden");
//            if ($("#inputFolioAutorizacion").val() == 0) {
//                $("#div-folioCorrespondienteBanca").removeAttr("hidden", "hidden");  
//                $("#div-montoDepositarBanca").removeAttr("hidden", "hidden");  
//            }            
//        }
//        else {
//            $("#div-montoDepositarBanca").attr("hidden", "hidden");  
//            $("#div-folioCorrespondienteBanca").attr("hidden", "hidden");
//            $("#div-select-cuentaNoAfectada").attr("hidden", "hidden");
//        }
        
//    }
//}

function onFinalizarReporte() {
    if ($("#selectReporte").val() == 4 && $("#selectPtmoCapt").val() == 6)
    {
        
        //var valida = true;
        //valida = validaFinaizaReporteBanca();
        if ($("#selectSatisfaccion").val() == 1 || $("#selectSatisfaccion").val() == 0 ) {
            if ($("#file").val() == "") {
                $("#file").addClass("rojoValidacion");
            }
            else {
                $("#file").removeClass("rojoValidacion");
                generarNotificados();
            }
        }
        

        //mostrarFormato();
    }
    else {
        generarNotificados();
    }
}

function generarNotificados() {
    var res = validaNotificados();
    
    if (res === true) {
        $("#btnGuardarNotificados").attr("disabled", "disabled");
        $("#dvLoading").removeAttr("hidden");
        $.ajax({
            type: "POST",url: rootUrl("/Notificados/finalizar"),
            data: { folio: $("#inputFolioNotificados").val(), comentarios: $("#areaComentariosNotificados").val(), satisfaccion: $("#selectSatisfaccion").val()},
            success: function (estatus) {
                if (estatus == 1) {
                    $("#divAlertaExitoNotificados").removeAttr("hidden");
                    setTimeout(function () { $("#frmAgregarDocFinalizar").submit(); }, 1000);
                }
                else {
                    $("#divAlertaFracasoNotificados").removeAttr("hidden");
                    $("#dvLoading").attr("hidden", "hidden");
                }
            }
        });

        
    }
}

function validaNotificados() {
    var res = true;
    if ($("#areaComentariosNotificados").val() === "") {
        $("#areaComentariosNotificados").addClass("rojoValidacion");
        res = false;
    }

    //if ($("#selectSatisfaccion").val() === "0") {
    //    $("#selectSatisfaccion").addClass("rojoValidacion");
    //    res = false;
    //}
    return res;
}

function descargarSolRec(folio) {
    $.ajax({
        type: "POST",url: rootUrl("/Preview/generaPDF"),data: { folio:folio },
        success: function (numFolio) {
            window.open("../AcusesRecibido/" + numFolio + "_acuseSolicitud.pdf", '_blank');
        }
    });
}

function EliminarArchivo(folio, archivo)
{
    $.ajax({
        type: "POST",
        url: rootUrl("/Preview/EliminaArchivo"),data: { IdArchivo: archivo },success: function (archivosAdjuntos) {
            $("#" + archivo).attr("hidden", "hidden");
        }
    });
    
    numFiles = Number($("#numEvidencias").val()) - 1;
    $("#numEvidencias").val(numFiles);
    if (numFiles < 2) {
        $("#labelAdjuntar").text("Adjuntar evidencias de debito(formato de solicitud y copia de identificación):");
        $("#labelNoObli").attr("hidden", "hidden");
        $("#enlaceCanalizar").removeAttr("href");
        $("#enlaceCanalizar").attr("onclick", "alertaEvidenciasDebito()");
    }
}

function habilitarGuardar() {
    $("#btnGuardar").removeAttr("disabled");
    var fp = $("#file");
    numFiles = fp[0].files.length + Number($("#numEvidencias").val());
    $("#file").removeClass("rojoValidacion");
}

function validaInputImporte(value) {
    var montoRecl = value.toString().replace(/ |,/g, '');
    montoRecl = montoRecl.toString().replace('$', '');
    $("#inputImporteReclamacionHidden").val(montoRecl);
    $("#btnGuardar").removeAttr("disabled");
}

function ValidaArchivoAudio(idLlamada) {
    $("#dvLoading").removeAttr("hidden");
    $("#bodyAudio").html("");

    $.ajax({
        type: "POST",url: rootUrl("/Preview/ValidarArchivoAudio"),data: { idLlamada: idLlamada },dataType: "json",
        success: function (audio) {
            if (audio.estatus == 1) {
                var html = "";
                var html = '<audio controls="controls" id="audio"><source src="' + audio.ruta + '" type="audio/wav" />Tu navegador no soporta el elemento de audio.</audio>';
                $("#bodyAudio").html(html);
                document.getElementById("uneFolioAudio").innerHTML = audio.idLlamada;
                document.getElementById("spanFolioLlamada").innerHTML = audio.folio;
                document.getElementById("spanTelefonoLlamada").innerHTML = audio.telefono;
                if (audio.TipoLlamada == 1)
                    document.getElementById("spanTipoLlamada").innerHTML = "Entrada";
                else
                    document.getElementById("spanTipoLlamada").innerHTML = "Salida";
                $("#myModalAudio").modal();
                $("#dvLoading").attr("hidden", "hidden");
            }
            else { alertify.alert("Ocurrio un error.");}
        }
    });
}

function detenerReproduccion() {
    $("#bodyAudio").html("");
}

function alertaEvidenciasDebito() {
    alertify.alert("Es necesario adjuntar formato de solicitud y copia de identificación.");
}

function CambioSucursalRegistro() {
    if ($("#selectSucursalRegistro").val() == "1000") {
        alertify.alert("Este registro solo puede ser seleccionado si esta seleccionada la opción de corresponsalias");
        $("#selectSucursalRegistro").val("0");
    }

    if ($("#selectSucursalRegistro").val !== "0")
        $("#selectSucursalRegistro").removeClass("rojoValidacion");
}

function cambioEstadoCheckbox() {

    if ($('#checkCorresponsalias:checked').val() == "True") {
        $("#selectSucursalRegistro").val("1000");
        $("#selectSucursalRegistro").attr("readonly", "readonly");
        $("#selectSucursalRegistro").attr("disabled", "disabled");
        $("#inputNumReferencia").removeAttr("readOnly");
    }
    else {
        $("#selectSucursalRegistro").val("0");
        $("#selectSucursalRegistro").removeAttr("readonly");
        $("#selectSucursalRegistro").removeAttr("disabled");
        $("#inputNumReferencia").attr("readOnly", "readOnly");
        $("#inputNumReferencia").val("");
    }
}

function EnviarEnlacePassword(numero, folio) {
    $("#dvLoading").removeAttr("hidden");
    $.ajax({
        type: "POST", url: rootUrl("/Preview/EnviarEnlacePassword"), data: { numero: numero, folio:folio }, dataType: "json",
        success: function (resultado) {
            if (resultado === true) {
                alertify.alert("Acción ejecutada correctamente.");
                $("#dvLoading").attr("hidden", "hidden");
            }
            else {
                alertify.alert("Ocurrio un error, favor de intentar nuevamente.");
                $("#dvLoading").attr("hidden", "hidden");
            }
        }
    });
}

function FinalizarReporteEnlacePassword(numero, folio) {
    if ($("#inputComentariosFinalizar").val() === "") {
        alertify.alert("Favor de llenar el cuadro de comentarios.");
    }
    else {
        $("#dvLoading").removeAttr("hidden");
        $.ajax({
            type: "POST", url: rootUrl("/Preview/FinalizarReporteEnlacePassword"), data: { folio: folio, comentarios: $("#inputComentariosFinalizar").val() }, dataType: "json",
            success: function (resultado) {
                if (resultado === 1) {
                    alertify.alert("Reporte finalizado correctamente.");
                    setTimeout(function () { window.location.href = rootUrl("/Finalizados/Finalizados"); }, 500);
                    $("#dvLoading").attr("hidden", "hidden");
                }
                else {

                    console.log(resultado);
                    if (resultado === 0) {//Error interno de la aplicación
                        alertify.alert("Ocurrio un error al finalizar el reporte");
                        $("#dvLoading").attr("hidden", "hidden");
                    }
                    else if (resultado === 2) {//Sesion Vencida
                        window.location.href = rootUrl("/Login/Index");
                    }
                    
                }
            }
        });
    }
}

function ElementosVisiblesCallcenter() {
    if (reporteBanca.folioBanca != null) {
        $("#div-Tel").hide();
        $("#div-suc-causa").hide();
        $("#inputTelCel").attr("readOnly");
        $("#div-representante-leg").hide();
        $("#div-tarjeta-cred").hide();        
        $("#div-fech-aclaracion").hide();
        if($("#divComentAreaEsp").val() == ""){
            $("#divComentAreaEsp").hide();
        }
        

        $("#div-dictaminar").hide();
        $("#fechaTransaccion").attr("disabled", "disabled");
        $("#div-causaResol").hide();
        
        $("#div-no-referencia").hide();

        ///datos ocultos por normativa//
        $("#div-celular").hide();
        $("#div-domicilio").hide();
        $("#div-sucursal").hide();
        $("#div-EntFed").hide();

        if ($("#inputSolucion").val() == "$0.00") {
            $("#div-montoautorizacion").hide();
        }

        if ($("#ComentarioAprueba").val() == "") {
            $("#div-ComenAprueba").hide();
        }
        if ($("#ComentarioCan").val() == "")
        {
            $("#div-comenCanalizacion").hide();
        }

        if ($("#comentarioRechazo").val() == "")
        {
            $("#div-cometarioRechazo").hide();
        }
        
            $("#div-importeReclamacion").hide();

            $("#div-select-medDetMov").hide();
            $("#div-folio-autorizacion").hide();
            $("#div-dictamen").hide();
            $("#div-causaResol").hide();
            $("#div-fech-abonoCliente").hide();
            $("#div-fech-transaccion").hide();
            
            $("#div-formato27").hide();        
            $("#div-selectTipoCuentaBanca").hide();

            $("#div-montoautorizacion").hide();
        
        if (reporteBanca.idEstatusReporte == 9) {
            $("#div-montoautorizacion").hide();
            $("#div-folio-autorizacion").hide();
            $("#div-fech-transaccion").hide();
        }

        if (reporteBanca.idEstatusReporte == 2) {
            $("#div-select-medDetMov").hide();
            $("#div-fech-transaccion").hide();
            //$("a").attr("href", "#");
            
            //$("#selectMedioMov").removeAttr("disabled");
            $("#inputFolioAutorizacion").removeAttr("disabled");
            $("#div-folio-autorizacion").hide();
            $("#inputEstatus").val("Reporte Informativo");
            if ($("#selectPtmoCapt").val() == 5) {
                $("#div-importeReclamacion").hide();
            }
            $("#div-montoautorizacion").hide();
        }

        else if (reporteBanca.idEstatusReporte == 3 || reporteBanca.idEstatusReporte == 4) {
            $('#selectMedioMov').removeAttr('hidden');
            $('#div-importeReclamacion').removeAttr('hidden');
            //if ($("#selectPtmoCapt").val() == 6) {
            //    $("#inputImporteReclamacion").val(formatCurrency(reporteBanca.importeReclamacion));
            //}
        }

        else {
            $("#div-select-medDetMov").hide();
        }
        if (reporteBanca.idTipoCuenta == 4 || ReporteBanca.idCuenta == 34 || ReporteBanca.idCuenta == 35 || ReporteBanca.idCuenta == 36 || ReporteBanca.idCuenta == 37) {
            $("#div-ComenCanalizacion").hide();
            $("#div-ComenAprueba").hide();
            $("#div-ComenAreaEspe").hide();
            $("#div-ComenNotificacion").hide();
            $("#div-fech-transaccion").hide();
            $("#div-folio-autorizacion").hide();
        }
        if ($("#ComentarioFinalizacion").val() == "") {
            $("#div-ComentarioFinalizacion").hide();
        }
    }
}
    
function Productos(value) {
    if (value != 0) {
        $("#selectProducto").removeClass("rojoValidacion");
        $("#btnGuardar").removeAttr("disabled");
    }
}

function Canales(value) {
    if (value != 0) {
        $("#selectCanal").removeClass("rojoValidacion");
        $("#btnGuardar").removeAttr("disabled");
    }
}

function Motivos(value) {
    if (value != 0) {
        $("#selectMotivoCancelacion").removeClass("rojoValidacion");
        $("#btnGuardar").removeAttr("disabled");
    }
}

function cambioinputNumeroCuentaPtmo() {
    $("#inputNumeroCuentaPtmo").removeClass("rojoValidacion");
}

function cambioFecha(id) {
    if (id === "fechaAclaracion") {
        if ($("#fechaAclaracion").val().length === 10) {
            $("#fechaAclaracion").removeClass("rojoValidacion");
        }
    }

    if (id === "fechaTransaccion") {
        if ($("#fechaTransaccion").val().length === 10) {
            $("#fechaTransaccion").removeClass("rojoValidacion");
        }

        $.ajax({
            type: "POST", url: rootUrl("/Registrar/DiferenciaFechas"), data: { fecha: $("#fechaTransaccion").val() }, dataType: "json",
            success: function (diasDif) {
                diasTransaccion = diasDif;
                if (diasDif > 90) {
                    alertify.alert("La transacción no debe rebasar los 90 días naturales.");
                    $("#fechaTransaccion").addClass("rojoValidacion");
                }
                else {
                    $("#fechaTransaccion").removeClass("rojoValidacion");
                }
            }
        });
    }
}

function validaNumeros(value){
    return value.match(/^\d+\.?\d{0,1}/);
}

function asignarDPF(dpf) {
    $("#labelinputNumeroCuentaPtmo").html("Número de DPF:");
    $("#inputNumeroCuentaPtmo").val(dpf);
    $("#inputNumeroCuentaPtmo").attr("readonly", "readonly");
    $("#modalDPFs").modal("hide");
}

function dateToYMD(date) {
    var d = date.getDate();
    var m = date.getMonth() + 1; //Month from 0 to 11
    var y = date.getFullYear();
    return '' + y + '-' + (m <= 9 ? '0' + m : m) + '-' + (d <= 9 ? '0' + d : d);
}

function mostrarFormato_(url) {

    url = rootUrl(url);
    urlFormatoAclProcedente = url;
    $('#content-formato').html("");
    var iframe = $('<embed src="" width="100%" height="600" alt="pdf" pluginspage="http://www.adobe.com/products/acrobat/readstep2.html">');
    iframe.innerHTML = "";
    iframe.attr('src', url);
    $('#content-formato').append(iframe);
    var content = iframe.innerHTML;
    iframe.innerHTML = content;
    $("#dvLoading").attr("hidden", "hidden");

}

function validaFinaizaReporteBanca(esProcedente) {
    var validacion = true;
    //esProcedente = 2;
    if ($("#selectReporte").val() == 4) {
        if ($("#selectPtmoCapt").val() == 6 && esProcedente == 2) {
            if ($("#folioCorrespondienteBanca").val() != "" && $("#folioCorrespondienteBanca").val() != "0") {
                var numeroSocio = $("#inputNumSocio").val();
                var folioAutorizacion = $("#folioCorrespondienteBanca").val();
                var folioBanca = $("#inputFolioBanca").val();
            
                $.ajax({
                    method: "POST",
                    url: rootUrl("/Banca/ValidarFolioBanca"),
                    data: { numeroSocio, folioAutorizacion, folioBanca },
                    dataType: "json",
                    async: false,
                    success: function (data) {

                        if (!data.estatus) {
                           // $("#inputFolioAutorizacion").addClass("rojoValidacion");
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
            if ($("#selectCuentaNoAfect").val() == 0) {
                $("#selectCuentaNoAfect").addClass("rojoValidacion");
                validacion = false;
            }

            if (parseFloat($("#inputConfirmarImporte").val().replace("$", "")) == 0) {
                $("#inputConfirmarImporte").addClass("rojoValidacion");
                validacion = false;
            }
        
        }

        if (multifoliosPendientes == 0) {
            if ($("#areaComentariosNotificados").val() == "") {
                $("#areaComentariosNotificados").addClass("rojoValidacion");
                validacion = false;
            }
        }
        
    }

    //if ($("#selectFolioSatisfaccion").val() == 0) {
    //    $("#selectFolioSatisfaccion").addClass("rojoValidacion");
    //    validacion = false;
    //}
    //if ($("#areaComentariosNotificados").val() == "") {
    //    $("#areaComentariosNotificados").addClass("rojoValidacion");
    //    validacion = false;
    //}
    if (esProcedente == 2 && ($("#folioCorrespondienteBanca").val() == "" || $("#folioCorrespondienteBanca").val() == 0)) {
        $("#folioCorrespondienteBanca").addClass("rojoValidacion");
        validacion = false;
    }
    

    return validacion;
}

function onChangeSelectCuentaNoAfect() {    
    if ($("#selectCuentaNoAfect").val() !=0) {
        $("#selectCuentaNoAfect").removeClass("rojoValidacion");
    }
}
function keyfolioCorrespondienteBanca() {
    if ($("#folioCorrespondienteBanca").val() != "" || $("#folioCorrespondienteBanca").val() != 0) {
        $("#folioCorrespondienteBanca").removeClass("rojoValidacion");
    }
}

function mostrarFormato() {
    
    var valida = true;
    valida = validaFinaizaReporteBanca();
    
        
        var datosAdicionales = {};
        datosAdicionales.reporteFolioBanca = $("#inputFolioBanca").val();
        datosAdicionales.fechaTransacion = $("#fechaTransaccion").val();
        //datosAdicionales.importeReclamacion = $("#inputImporteReclamacion").val();
        datosAdicionales.medioDeteccionMovimiento = $("#selectMedioMov option:selected").text();
        datosAdicionales.folioAutorizacion = $("#folioCorrespondienteBanca").val();
        datosAdicionales.tipoCuenta = $("#selectTipoCuentaBanca option:selected").text();
        datosAdicionales.comentarioNotificacion = $("#ComentarioNotifica").text();
        datosAdicionales.satisfaccion = $("#selectFolioSatisfaccion").val();
        datosAdicionales.comentariosFinalizacion = $("#areaComentariosNotificados").val();
        datosAdicionales.cuentaNoAfectada = $("#selectCuentaNoAfect option:selected").text();
        datosAdicionales.montoADepositar = $("#inputConfirmarImporte").val().replace("$", "").replace(",", ""); //parseFloat($("#inputConfirmarImporte").val().replace("$", "").replace(",", ""));;
    if (valida === true) {
        
        $("#selectFolioSatisfaccion").removeClass("rojoValidacion");
        $("#areaComentariosNotificados").removeClass("rojoValidacion");
        $("#folioCorrespondienteBanca").removeClass("rojoValidacion");
        $("#selectCuentaNoAfect").removeClass("rojoValidacion");
        //$("#dvLoading").removeAttr("hidden");
        $.ajax({
            method: "POST",
            url: rootUrl("/Banca/GenerarFormatoPDF"),
            data: datosAdicionales,
            dataType: "json",
            success: function (data) {
                if (data.estatus) {
                    $("#dvLoading").attr("hidden", "hidden");
                    mostrarFormato_(data.ruta);
                    $('#mensaje-formato').text("aqui va el PDF");
                    $('#myModalFormatoBanca').modal({ fadeDuration: 100, backdrop: 'static' });

                } else {
                    alert(data.mensaje);
                }

            },
            error: function (xhr, status, error) {
                $("#dvLoading").attr("hidden", "hidden");
                $("#divAlertaFracaso").removeAttr("hidden");
                alert(error);
            },
        });
        
    }

    
}

function confirmacionVerificada() {
    $("#dvLoading").attr("hidden", "hidden");
    $("#myModalFormatoBanca").modal("hide");
    $("#myModalConfirmarImpresion").modal("hide");
    //generarNotificados(urlFormatoAclProcedente);
    
    //btnAceptarModal();
    //$('#myModalCanalizacionCorrecta').modal({ fadeDuration: 100 });

}

function irInicio() {
    window.location = rootUrl("/Registros/Registros");
}

function confirmarImpresion() {
    $("#dvLoading").attr("hidden", "hidden");
    $('#myModalConfirmarImpresion').modal({ fadeDuration: 100, backdrop: 'static' });

    //setTimeout(function () { $("#frmAgregarDocFinalizar").submit(); }, 1000);
}

function validaInputImporteDeposito(value) {
    $("#inputConfirmarImporte").removeClass("rojoValidacion");
}

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
        if (reporteBanca.idEstatusReporte == 5 || reporteBanca.idEstatusReporte == 6) {
            var estatusFolio = "";
            var respuestaFolio = "";
            switch (reporte.idSatisfacion) {
                case 0:
                    estatusFolio = '<span class="badge" style="background-color:#f0ad4e;">Pendiente</span>' 
                    respuestaFolio = '<span data-toggle="tooltip" title="No Procedente"><button type="button" class="btn btn-icon btn-sm waves-effect waves-light btn-danger" data-toggle="modal" data-target="modal"' +
                        'onclick ="MostrarDetalleReporteImprocedente(\'' + reporte.idIncidenciaReporte + '\')"> <span class ="glyphicon glyphicon-remove"/> </button></span>' + ' ' +
                        '<span data-toggle="tooltip" title="Procedente"><button type="button" class="btn btn-icon btn-sm waves-effect waves-light btn-success" data-toggle="modal" ' +
                        'onclick ="MostrarDetalleReporteBanca(\'' + reporte.idIncidenciaReporte + '\')"> <span class ="glyphicon glyphicon-ok"/> </button></span>'
                    break;
                case 1:
                    estatusFolio = '<span class="badge badge-danger" style="background-color:#d9534f;">Improcedente</span>'
                    respuestaFolio = '<span class="badge" style="background-color:#5bc0de;">Atendido</span >'
                    break;
                case 2:
                    estatusFolio = '<span class="badge" style="background-color:#5cb85c;">Procedente</span>'
                    respuestaFolio = '<span class="badge" style="background-color:#5bc0de;">Atendido</span >'
                    break;
            }
            
            tablaReportesBanca.fnAddData([
                reporte.folioAutorizacion,
                formatCurrency(reporte.importeReclamo),
                reporte.medioMovimiento,
                reporte.tipoCuentaBanca,
                ObtenerFechaJson(reporte.fechaTransacion),
                estatusFolio,
                respuestaFolio
            ]);
        }
        else {
            tablaReportesBanca.fnAddData([
                reporte.folioAutorizacion,
                formatCurrency(reporte.importeReclamo),
                reporte.medioMovimiento,
                reporte.tipoCuentaBanca,
                ObtenerFechaJson(reporte.fechaTransacion),
                
            ]);
        }
        
        tablaReportesBanca.fnDraw(false);
    });
    $('[data-toggle="tooltip"]').tooltip();
}


function ObtenerFechaJson(fecha) {
    var fecha_ = new Date(fecha.match(/\d+/)[0] * 1);
    return fecha_.toLocaleDateString('en-GB');
}


function MostrarDetalleReporteBanca(IdIncidenciaReporte) {

    $("#folioCorrespondienteBanca").removeClass("rojoValidacion");
    $("#selectCuentaNoAfect").removeClass("rojoValidacion");
    $("#inputConfirmarImporte").removeClass("rojoValidacion");
    //$('#myModalEditarMiltifolioBanca').modal();
    $('#myModalEditarMiltifolioBanca').modal({ backdrop: 'static' });

    $.ajax({
        url: rootUrl("/Banca/ObtenerDetalleReporteBancaJson"),
        data: { IdIncidenciaReporte: IdIncidenciaReporte },
        dataType: "json",
        method: "post",
        beforeSend: function () {
            //MostrarLoading();
        },
        success: function (data) {
            //OcultarLoading();
            PintarDetallesReporteBanca(data.reportesSocio);
            //cantReportes = data.reportesSocio.length;
        },
        error: function (xhr, status, error) {
            ControlErrores(xhr, status, error);
        }
    });
}

function LLenarListaCuentas(item,index) {
    $('#selectCuentaNoAfect').append(`<option value="${item.ID_CUENTA_NO_AFECTADA}"> 
            ${item.DESCRIPCION} 
        </option>`);
}

function PintarDetallesReporteBanca(reporteSocio) {
    
    $("#selectCuentaNoAfect").html("");
    $('#selectCuentaNoAfect').append(`<option value="${0}"> 
            Seleccionar 
        </option>`);
    //var fruits = ["select", "cuenta origen", "cuenta destino"];
    tipoTransferenciaFolio.forEach(LLenarListaCuentas);
    //fruits.forEach(LLenarListaCuentas);

    //alert(reporteSocio.folioAutorizacion + ""+ reporteSocio.importeReclamo)
    $("#folioCorrespondienteBanca").val(reporteSocio.folioAutorizacion);
    $("#selectCuentaNoAfect").val(reporteSocio.idTipoCuenta);
    $("#inputConfirmarImporte").val("$ " + parseFloat(reporteSocio.importeReclamo));
    

    $("#label-IdIncidenciaReporte").text(reporteSocio.idIncidenciaReporte);
    $("#label-IdIncidenciaReporte").val(reporteSocio.idIncidenciaReporte);
    $("#label-IdIncidenciaReporte").hide();

    if (reporteSocio.folioAutorizacion != 0) {
        if (reporteSocio.tipoTransferenciaFolioBanca == 2) {
            //$("#selectCuentaNoAfect").re

            $("#selectCuentaNoAfect").find("option[value='2']").remove();
        }
    }
    


    $("#label-TipoTransferenciaFolio").text(reporteSocio.TipoTransferenciaFolio);
    $("#label-TipoTransferenciaFolio").val(reporteSocio.TipoTransferenciaFolio);
    //$("#label-TipoTransferenciaFolio").hide();

}

function EditarFolio(reporteEsProcedente) {

    var valida = true;
    valida = validaFinaizaReporteBanca(reporteEsProcedente);

    var datosFolioBanca = {};
    if (reporteEsProcedente == 2) {
        datosFolioBanca.IdIncidenciaReporte = $("#label-IdIncidenciaReporte").val();
    }
    else if (reporteEsProcedente == 1) {    
        datosFolioBanca.IdIncidenciaReporte = $("#label-IdIncidenciaReporteImprocedente").val();
    }
    
    datosFolioBanca.folioAutorizacion = $("#folioCorrespondienteBanca").val();    
    datosFolioBanca.importeReclamo = parseFloat($("#inputConfirmarImporte").val().replace("$", "").replace(",", ""));
    datosFolioBanca.idCuentaNoAfectada = $("#selectCuentaNoAfect option:selected").val();
    datosFolioBanca.idSatisfacion = reporteEsProcedente;

    
    if (valida) {
        //alert("elementos completos");
        $.ajax({
            method: "POST",
            url: rootUrl("/Banca/EditarReportesBancaJson"),
            data: datosFolioBanca,
            dataType: "json",
            success: function (data) {
                //if (data.estatus) {
                    $("#dvLoading").attr("hidden", "hidden");
                    //alert("Se edito correcto")
                    if (reporteEsProcedente == 2) {
                        $('#myModalEditarMiltifolioBanca').modal("hide");
                    }
                    else if (reporteEsProcedente == 1) {
                        $('#myModalConfirmarImprocedente').modal("hide");
                    }
                    
                    MostrarAllReportesBanca(parseInt($("#kbd-folioUNE").text()));

                //} else {
                //    alert(data.mensaje);
                //}

            },
            error: function (xhr, status, error) {
                $("#dvLoading").attr("hidden", "hidden");
                $("#divAlertaFracaso").removeAttr("hidden");
                alert(error);
            },
        });

    }

    
}

function MostrarDetalleReporteImprocedente(idIncidenciaReporte) {
    $('#myModalConfirmarImprocedente').modal({ backdrop: 'static' });
    $("#label-IdIncidenciaReporteImprocedente").text(idIncidenciaReporte);   
    $("#label-IdIncidenciaReporteImprocedente").val(idIncidenciaReporte);
    $("#label-IdIncidenciaReporteImprocedente").hide();
}

function limpiarInputReporte() {

    var montoReclamacion = parseFloat($("#inputConfirmarImporte").val().replace("$", ""));
    if (montoReclamacion == 0) {
        $("#inputConfirmarImporte").val("");
    } else {
        $("#inputConfirmarImporte").val(montoReclamacion);
    }
}

function modalFoliosPendientes(multifoliosPendientes) {
    $("#p-mensajeMultifoliosPendientes").text("El reporte aun tiene " + multifoliosPendientes + " folio(s) pendiente(s) de dictaminar");
    $('#myModalMultifoliosPendientes').modal({ fadeDuration: 100, backdrop: 'static' });
}

