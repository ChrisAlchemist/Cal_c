var esSocio = "0", medioDeContacto = "0", diasTransaccion = 0, tipoProducto = 0;
var supuestosMonto = new Array();

$(document).ready(function () {
    cargarSuspuestosMonto();
    $('#fechaAclaracion').datepicker({ autoclose: true, format: 'yyyy/mm/dd' });
    $('#fechaTransaccion').datepicker({ autoclose: true, format: 'yyyy/mm/dd' });
    $("#fechaTransaccion").val(moment().format("YYYY/MM/DD"));
    //$("table#tblDPFs").dataTable();
});

$(function () {
    $('#rootwizard').bootstrapWizard({
        onNext: function (tab, navigation, index) {
            var validacion = false;
            if (index === 1) {
                validacion = validaContinuar();
                if (validacion === true)
                    GeneraResumen();
            }
            return validacion;
        },
        onTabShow: function (tab, navigation, index) {
            var $total = navigation.find('li').length;
            var $current = index + 1;
            var $percent = ($current / $total) * 100;
            $('#rootwizard').find('.bar').css({ width: $percent + '%' });
        },
        onTabClick: function (tab, navigation, index) {
            var validacion = false;
            if (index === 0) {
                validacion = validaContinuar();
                if (validacion === true)
                    GeneraResumen();
            }
            return validacion;
        }
    });
    $('#rootwizard .finish').click(function () { generaSubmit(); });
    window.prettyPrint && prettyPrint();
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

function EsSocio(valor) {
    esSocio = valor;
    $("#inputSocio").val("");
    reiniciaDatosSocio();

    $("#inputEsSocio").removeClass("rojoValidacion");

    $("#inputEsSocio_").val(valor);
    if (valor === "1") {
        $("#inputSocio").removeAttr("disabled");
        $("#buscarNumSocio").removeAttr("disabled");
        $("#btnTutor").attr("disabled", "disabled");

        $("#inputNombre").attr("readonly", "readonly");
        $("#inputAP").attr("readonly", "readonly");
        $("#inputAM").attr("readonly", "readonly");

        $("#inputTel").attr("readonly", "readonly");
        $("#inputTelCel").attr("readonly", "readonly");
        $("#inputCalle").attr("readonly", "readonly");

    }
    else if (valor === "2") {
        $("#inputSocio").attr("disabled", "disabled");
        $("#buscarNumSocio").attr("disabled", "disabled");
        $("#btnTutor").attr("disabled", "disabled");

        $("#inputNombre").removeAttr("readonly");
        $("#inputAP").removeAttr("readonly");
        $("#inputAM").removeAttr("readonly");

        $("#inputTel").removeAttr("readonly");
        $("#inputTelCel").removeAttr("readonly");
        $("#inputCalle").removeAttr("readonly");

        if ($("#inputFolio").val() === "") {
            $.ajax({
                type: "POST", url: rootUrl("/Registrar/ObtenerProxFolio"), dataType: "json",
                success: function (folio) {
                    $("#inputFolio").val(folio[0].FOLIO);
                    $("#spanFolio").val(folio[0].FOLIO);
                    $("#inputFolio_").val(folio[0].FOLIO);
                }
            });
        }
    }
    else {
        $("#inputSocio").removeAttr("disabled");
        $("#buscarNumSocio").removeAttr("disabled");

        $("#inputNombre").attr("readonly", "readonly");
        $("#inputAP").attr("readonly", "readonly");
        $("#inputAM").attr("readonly", "readonly");

        $("#inputTel").attr("readonly", "readonly");
        $("#inputTelCel").attr("readonly", "readonly");
        $("#inputCalle").attr("readonly", "readonly");
    }
}

$(document).ready(function () {

    $("#continuar").click(function (evt) {
        evt.preventDefault();
    });
});

function supuestos(value) {
    if (value != 0) {
        for (var i = 0; i < supuestosMonto.length; i++) {
            if (supuestosMonto[i] == value) {
                alertify.alert('<p align=LEFT>PARA AGILIZAR LA RESOLUCIÓN DE ESTA CAUSA ES IMPORTANTE ESPECIFICAR, <strong>EN EL AREA DE DESCRIPCIÓN</strong>, LA SIGUIENTE INFORMACIÓN: <br /> <b /><ul align=LEFT><li>LUGAR DONDE SE LLEVO ACABO LA OPERACIÓN</li> <li>FECHA DE LA OPERACIÓN</li></ul>');
                document.getElementById("inputDescripcion").focus();
                $("#inputDescripcion").addClass("rojoValidacion");
                break;
            }
            else {
                $("#inputDescripcion").removeClass("rojoValidacion");
                $("#inputImporteReclamacion").removeClass("rojoValidacion");
            }
        }
        ///Cargamos los valores a seteas para el canal y el motivo de cancelación
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

        $("#selectSupuesto").removeClass("rojoValidacion");
    }
    else {
        document.getElementById("selectCanal").value = 0;
        document.getElementById("selectMotivoCancelacion").value = 0;
    }
}

function descripcionReporte() {
    if ($("#inputDescripcion").val().length > 0 || $("#inputDescripcion").val().length <= 1000)
        $("#inputDescripcion").removeClass("rojoValidacion");
}

function hacer_click(numero) {
    $("#inputTel").removeClass("rojoValidacion");
    $("#inputTelCel").removeClass("rojoValidacion");
    $("#btnPersonaMoral").removeAttr("disabled");

    if (numero !== "") {
        var tipoPersona = 1;

        if ($("#inputEsSocio").val() === "3") {
            tipoPersona = 7;
        }

        if (tipoPersona === 1) {
            $.ajax({
                type: "POST", url: rootUrl("/Registrar/BuscaNumSocio"), data: { NUMERO: numero, tipoPersona: tipoPersona }, dataType: "json",
                success: function (socio) {
                    if (socio.estatus == 1) {
                        $("#inputSocio_").val(numero);
                        $("#inputNombre").val(socio.Nombre_s);
                        $("#inputAP").val(socio.Apellido_Paterno);
                        $("#inputAM").val(socio.Apellido_Materno);
                        $("#inputCalle").val(socio.CALLE + " #" + socio.NUMERO_EXTERIOR + " " + socio.NUMERO_INTERIOR + ", " + socio.Nombre_Colonia + ", CP. " + socio.CP + ", " + socio.municipio + ", " + socio.estado);
                        $("#inputTel").val(socio.Telefono);
                        $("#inputTelCel").val(socio.Tel_Celular);
                        $("#inputTel").removeAttr("readonly");
                        $("#inputTelCel").removeAttr("readonly");
                        $("#selectSucursal").val(socio.Id_de_Sucursal);
                        $("#inputNumTarjeta").val(socio.Num_Tarjeta);

                        if ($("#inputFolio").val() === "") {
                            $.ajax({
                                type: "POST", url: rootUrl("/Registrar/ObtenerProxFolio"), dataType: "json",
                                success: function (folio) {
                                    $("#inputFolio").val(folio[0].FOLIO);
                                    $("#spanFolio").val(folio[0].FOLIO);
                                    $("#inputFolio_").val(folio[0].FOLIO);
                                }
                            });
                        }


                    }
                    else { alertify.alert("El número ingresado no existe"); }
                },
                failure: function (response) { alertify.alert(response.d); }
            });
        }
        else {

            $.ajax({
                type: "POST", url: rootUrl("/Registrar/obtenerRepresentates"), data: { NUMERO: numero }, dataType: "json",
                success: function (representante) {
                    if (representante[0].ESTATUS === 1) {
                        $("#selectPerMoral").removeAttr("disabled");
                        var html = '<option value="0">REPRESENTANTE LEGAL</option>';
                        $.each(representante, function (i, rep) {
                            html += '<option value="' + rep.id_persona_rel + '">' + rep.NOMBRE + '</option>';
                        });
                        $("#selectPerMoral").html(html);
                    }
                    else { alertify.alert("El número ingresado no existe"); }
                },
                failure: function (response) { alertify.alert(response.d); }
            });
        }
    }
    else { $("#inputSocio").addClass("rojoValidacion"); }
}

function cargaDatosPM() {
    if ($("#selectPerMoral").val() !== "0") {
        $.ajax({
            type: "POST", url: rootUrl("/Registrar/BuscaPM"), data: { id_persona_rel: $("#selectPerMoral").val() }, dataType: "json",
            success: function (socio) {
                if (socio[0].estatus == 1) {
                    $("#inputSocio_").val($("#inputSocio").val());
                    $("#inputNombre").val(socio[0].nombre);
                    $("#inputAP").val(socio[0].apellido_paterno);
                    $("#inputAM").val(socio[0].apellido_materno);
                    $("#inputCalle").val(socio[0].CALLE + " #" + socio[0].num_ext + " " + socio[0].num_int + ", " + socio[0].Nombre_Colonia + ", CP. " + socio[0].Codigo_Postal);
                    $("#inputMunicipio").val(socio[0].CNBV_Municipio + ", " + socio[0].DESCRIPCION);
                    $("#inputTel").val(socio[0].telefono);
                    $("#inputTelCel").val(socio[0].telefono);
                    $("#inputTel").removeAttr("readonly");
                    $("#inputTelCel").removeAttr("readonly");

                    $.ajax({
                        type: "POST", url: rootUrl("/Registrar/ObtenerProxFolio"), dataType: "json",
                        success: function (folio) {
                            $("#inputFolio").val(folio[0].FOLIO);
                            $("#spanFolio").val(folio[0].FOLIO);
                            $("#inputFolio_").val(folio[0].FOLIO);
                        }
                    });
                }
                else { alertify.alert("El número ingresado no existe"); }
            },
            failure: function (response) { alertify.alert(response.d); }
        });
    }
}

function TelefonoCorrecto(valor) {
    $("#divTelCorrecto").attr("hidden", "hidden");
    if (valor === "2") {
        $("#btnGuardarTel").removeAttr("disabled");
        $("#inputTel").removeAttr("readOnly");
        $("#inputTelCel").removeAttr("readOnly");
        $("#inputTelTr").removeAttr("readOnly");
    }
    else {
        $("#btnGuardarTel").attr("disabled", "disabled");
        $("#inputTel").attr("readOnly", "readOnly");
        $("#inputTelCel").attr("readOnly", "readOnly");
        $("#inputTelTr").attr("readOnly", "readOnly");
    }
}

function MedioDeContacto(medioContacto) {
    $("#divMedioContacto").attr("hidden", "hidden");
    $("#radioMedioContacto").val(medioContacto);
    medioDeContacto = medioContacto;

    if (medioDeContacto === "1")
        $("#spanMedio").val("Presencial");
    else
        $("#spanMedio").val("Via Telefonica");
}

function cargarSupuestos(id) {
    $("#spanTP").val($("#selectReporte option:selected").html());

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
        $("#inputImporteReclamacion").removeAttr("disabled");

        $("#selectProducto").removeAttr("disabled");
        $("#selectCanal").removeAttr("disabled");
        $("#selectMotivoCancelacion").removeAttr("disabled");
    }
    else {
        $("#inputImporteReclamacion").attr("disabled", "disabled");
        $("#inputImporteReclamacion").val("$0.00");


        $("#selectProducto").attr("disabled", "disabled");
        $("#selectCanal").attr("disabled", "disabled");
        $("#selectMotivoCancelacion").attr("disabled", "disabled");

        $("#selectProducto").val("0");
        $("#selectCanal").val("0");
        $("#selectMotivoCancelacion").val("0");

    }


    //Mostramos o ocultamos los nuevos componentes del formulario para el archivo 2701
    if (id === "3") {
        $("#divNumCuenta").removeClass("hidden");
        $("#divProducto").removeClass("hidden");
        $("#divCanalTransaccion").removeClass("hidden");
        $("#divMotivoReclamacion").removeClass("hidden");
        $("#divFormA27").removeClass("hidden");
        $("#fechaTransaccion").removeAttr("disabled");
    }
    else {
        $("#divNumCuenta").addClass("hidden");
        $("#divProducto").addClass("hidden");
        $("#divCanalTransaccion").addClass("hidden");
        $("#divMotivoReclamacion").addClass("hidden");
        $("#divFormA27").addClass("hidden");
        $("#fechaTransaccion").attr("disabled");
        $("#fechaTransaccion").val(moment().format("YYYY/MM/DD"));
    }


}

function medioContacto() {
    $("#selectMedioContacto").removeClass("rojoValidacion");
    var idMedio = $("#selectMedioContacto").val();
}

function validaContinuar() {
    var validaForm = true;
    if (esSocio === "0") {
        $("#inputEsSocio").addClass("rojoValidacion");
        validaForm = false;
    }
    else {
        if (esSocio === "1" || esSocio === "3") {
            if ($("#inputSocio").val() === "") {
                $("#inputSocio").addClass("rojoValidacion");
                validaForm = false;
            }

            if ($("#inputTel").val() == "" && $("#inputTelCel").val() == "") {
                if ($("#inputTel").val() === "") {
                    $("#inputTel").addClass("rojoValidacion");
                    validaForm = false;
                }

                if ($("#inputTelCel").val() === "") {
                    $("#inputTelCel").addClass("rojoValidacion");
                    validaForm = false;
                }
            }
        }
        else {
            if ($("#inputNombre").val() === "") {
                $("#inputNombre").addClass("rojoValidacion");
                validaForm = false;
            }

            if ($("#inputAP").val() === "") {
                $("#inputAP").addClass("rojoValidacion");
                validaForm = false;
            }

            if ($("#inputAM").val() === "") {
                $("#inputAM").addClass("rojoValidacion");
                validaForm = false;
            }

            if ($("#inputTel").val() == "" && $("#inputTelCel").val() == "") {
                if ($("#inputTel").val() === "") {
                    $("#inputTel").addClass("rojoValidacion");
                    validaForm = false;
                }

                if ($("#inputTelCel").val() === "") {
                    $("#inputTelCel").addClass("rojoValidacion");
                    validaForm = false;
                }
            }

            if ($("#inputCalle").val() == "") {
                $("#inputCalle").addClass("rojoValidacion");
                validaForm = false;
            }

        }

        if ($("#selectReporte").val() === "0") {
            $("#selectReporte").addClass("rojoValidacion");
            validaForm = false;
        }

        if ($("#selectReporte").val() === "3") {

            for (var i = 0; i < supuestosMonto.length; i++) {
                if (supuestosMonto[i] == Number($("#selectSupuesto").val())) {
                    var montoRecl = $("#inputImporteReclamacion").val().toString().replace(/ |,/g, '');
                    montoRecl = montoRecl.toString().replace('$', '');
                    $("#inputImporteReclamacionHidden").val(montoRecl);
                    var importe = Number($("#inputImporteReclamacionHidden").val());
                    if (importe <= 0) {
                        $("#inputImporteReclamacion").addClass("rojoValidacion");
                        validaForm = false;
                    }
                    break;
                }
            }
        }

        if ($("#selectSupuesto").val() === "0" || $("#selectSupuesto").val() === "") {
            $("#selectSupuesto").addClass("rojoValidacion");
            validaForm = false;
        }

        if ($("#inputDescripcion").val() === "") {
            $("#inputDescripcion").addClass("rojoValidacion");
            validaForm = false;
        }

        if ($("#inputDescripcion").val().length >= 1000) {
            $("#inputDescripcion").addClass("rojoValidacion");
            validaForm = false
        }

        if ($("#selectMedioContacto").val() === "0") {
            $("#selectMedioContacto").addClass("rojoValidacion");
            validaForm = false;
        }

        if ($("#selectPtmoCapt").val() === "0") {
            $("#selectPtmoCapt").addClass("rojoValidacion");
            validaForm = false;
        }

        if ($("#selectTipoCuenta").val() === "0" || $("#selectTipoCuenta").val() === "") {
            $("#selectTipoCuenta").addClass("rojoValidacion");
            validaForm = false;
        }

        if ($("#selectEntidad").val() === "0") {
            $("#selectEntidad").addClass("rojoValidacion");
            validaForm = false;
        }

        if ($("#selectTipoCuenta").val() == 5 && ($("#selectReporte").val() == "1" || $("#selectReporte").val() == "3")) {

            if (diasTransaccion > 90) {
                validaForm = false;
                $("#fechaTransaccion").addClass("rojoValidacion");
                alertify.alert("La transacción no debe rebasar los 90 días naturales.");
            }
        }
        else if ($("#fechaTransaccion").val() === "") {
            validaForm = false;
            $("#fechaTransaccion").addClass("rojoValidacion");
        }

        //Validamos si es necesario validar los nuevos componentes del formulario del archivo 2701
        if ($("#selectReporte").val() == "3") {
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
        }

        if (($("#selectReporte").val() === "3" || $("#selectReporte").val() === "1") && $("#selectTipoCuenta").val() === "5") {
            if ($("#inputImporteReclamacion").val() === "$0.00") {
                validaForm = false;
                $("#inputImporteReclamacion").addClass("rojoValidacion");
            }
        }

    }
    return validaForm;
}

function GeneraResumen() {

    $("#spanTipoPer").val($("#inputEsSocio option:selected").html());

    var montoRecl = $("#inputImporteReclamacion").val().toString().replace(/ |,/g, '');
    montoRecl = montoRecl.toString().replace('$', '');
    $("#inputImporteReclamacionHidden").val(montoRecl);

    if ($("#inputEsSocio").val() == "2")
        $("#spanNumeroSocio").val("N/A");
    else
        $("#spanNumeroSocio").val($("#inputSocio").val());

    var repLegal = $("#selectPerMoral option:selected").html();
    if (repLegal == "Representante Legal")
        repLegal = "N/A";
    $("#spanRL").val(repLegal);

    $("#spanNombre").val($("#inputNombre").val() + " " + $("#inputAP").val() + " " + $("#inputAM").val());
    $("#spanTel").val($("#inputTel").val());
    $("#spanTelCel").val($("#inputTelCel").val());
    $("#spanDomicilio").val($("#inputCalle").val());
    $("#spanSucursalSocio").val($("#selectSucursal option:selected").html());
    $("#spanTS").val($("#selectSupuesto option:selected").html());
    $("#spanMedio").val($("#selectMedioContacto option:selected").html());
    $("#spanTCuenta").val($("#selectPtmoCapt option:selected").html());
    $("#spanCuentas").val($("#selectTipoCuenta option:selected").html());

    $("#spanIR").val($("#inputImporteReclamacion").val());

    $("#spanDescr").val($("#inputDescripcion").val());
    $("#spanSucursalRegistro").val($("#selectSucursalRegistro option:selected").html());
    $("#spanEntidadFederativa").val($("#selectEntidad option:selected").html());
    $("#spanTarjetaD").val($("#inputNumTarjeta").val());
    $("#spanNumReferencia").val($("#inputNumReferencia").val());
    $("#spanFechaAcla").val($("#fechaAclaracion").val());
    $("#spanFechaTran").val($("#fechaTransaccion").val());
    $("#spaninputNumeroCuentaPtmo").val($("#inputNumeroCuentaPtmo").val());
    $("#spanCanal").val($("#selectCanal option:selected").html());
    $("#spanMotivoReclamacion").val($("#selectMotivoCancelacion option:selected").html());
    $("#spanProducto").val($("#selectProducto option:selected").html());
}


function reiniciaDatosSocio() {
    $("#inputSocio").removeClass("rojoValidacion");
    $("#inputNombre").val("");
    $("#inputAP").val("");
    $("#inputAM").val("");
    $("#inputCalle").val("");
    $("#inputMunicipio").val("");
    $("#inputTel").val("");
    $("#inputTelCel").val("");
    $("#selectSucursal").val("0");
    if ($("#inputEsSocio").val() === "3") {
        $("#selectPerMoral").attr("disabled", "disabled");
    }

}

function generaSubmit() {
    //alertify.alert("FEcha transaccion " + $("#fechaTransaccion").val());
    $("#dvLoading").removeAttr("hidden");
    setTimeout(function () { document.getElementById("frmRegistrar").submit(); }, 1000);
}

function selectCuenta(value) {

    $("#selectTipoCuenta").val(0);
    $("#selectSupuesto").val(0);
    $("#selectSupuesto").attr("disabled", "disabled");

    if ($("#selectPtmoCapt").val() !== "0") {
        $("#selectPtmoCapt").removeClass("rojoValidacion");

        $.ajax({
            type: "POST", url: rootUrl("/Registrar/obtenerCuentas"), data: { tipoCuenta: $("#selectPtmoCapt").val() }, dataType: "json",
            success: function (cuentas) {
                $("#selectTipoCuenta").removeAttr("disabled");
                var html = '';
                html = '<option value="0">SELECCIONAR</option>';
                $.each(cuentas, function (i, cuenta) {
                    html += '<option value="' + cuenta.ID_CUENTA + '">' + cuenta.DESCRIPCION + '</option>';
                });
                $("#selectTipoCuenta").html(html);
            }
        });
    } else { $("#selectSupuesto").attr("disabled", "disabled"); }
}

function tipoCuenta(value) {

    if (value != 0) {
        $("#selectTipoCuenta").removeClass("rojoValidacion");
        $("#selectSupuesto").removeAttr("disabled");
        $.ajax({
            type: "POST", url: rootUrl("/Registrar/ObtenerSupuestos"), data: { id: $("#selectReporte").val(), idTipoCuenta: $("#selectPtmoCapt").val(), idCuenta: value },
            dataType: "json",
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
            $("#fechaAclaracion").val(moment().format("DD/MM/YYYY"));
        }
        else {
            $("#fechaAclaracion").attr("disabled", "disabled");
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
                $("#inputNumeroCuentaPtmo").val($("#inputSocio").val());
                $("#inputNumeroCuentaPtmo").attr("readOnly", "readOnly");
            }
        }
        else if ($("#selectPtmoCapt").val() === "3") {
            $("#labelinputNumeroCuentaPtmo").html("Número de cuenta de corresponsal:");
            $("#inputNumeroCuentaPtmo").removeAttr("readOnly");
        }
        else {
            $("#labelinputNumeroCuentaPtmo").html("Número de cuenta/préstamo/DPF:");
            $("#inputNumeroCuentaPtmo").attr("readOnly", "readOnly");
        }
        var id_mov;
        //Cargamos los idMovs de lo seleccionado en la aplicación
        if ($("#selectReporte").val() === "3") {
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
        $("#selectProducto").val(0);
    }
}

function validaInputImporte(value) {
    $("#inputImporteReclamacion").removeClass("rojoValidacion");
}

function selectEntFed() {
    if ($("#selectEntidad").val() !== "0") {
        $("#selectEntidad").removeClass("rojoValidacion");
    }
}

function CambioSucursalRegistro() {
    if ($("#selectSucursalRegistro").val() == "1000") {
        alertify.alert("Este registro solo puede ser seleccionado si esta seleccionada la opción de corresponsalias");
        $("#selectSucursalRegistro").val("0");
    }

    if ($("#selectSucursalRegistro").val !== "0")
        $("#selectSucursalRegistro").removeClass("rojoValidacion");
}

function validaDomicilio() {
    if ($("#inputCalle").val() != "")
        $("#inputCalle").removeClass("rojoValidacion");
}

function validaCelular() {
    if ($("#inputTelCel").val().length > 0) {
        $("#inputTelCel").removeClass("rojoValidacion");
    }
}

function validaTel() {
    if ($("#inputTel").val().length > 0) {
        $("#inputTel").removeClass("rojoValidacion");
    }
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

        //$.ajax({
        //    type: "POST",url: rootUrl("/Registrar/DiferenciaFechas"),data: { fecha: $("#fechaTransaccion").val() },dataType: "json",
        //    success: function (diasDif) {
        //        diasTransaccion = diasDif;
        //        if (diasDif > 90)
        //        {
        //            alertify.alert("La transacción no debe rebasar los 90 días naturales.");
        //            $("#fechaTransaccion").addClass("rojoValidacion");
        //        }
        //        else {
        //            $("#fechaTransaccion").removeClass("rojoValidacion");
        //        }
        //    }
        //});
    }
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

function cambioinputNumeroCuentaPtmo() {
    $("#inputNumeroCuentaPtmo").removeClass("rojoValidacion");
}

function cambioProducto() {
    $("#selectProducto").removeClass("rojoValidacion");
}

function cambioMotivoCancelacion() {
    $("#selectMotivoCancelacion").removeClass("rojoValidacion");
}

function cambioCanal() {
    $("#selectCanal").removeClass("rojoValidacion");
}

function validaNumeros(value) {
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