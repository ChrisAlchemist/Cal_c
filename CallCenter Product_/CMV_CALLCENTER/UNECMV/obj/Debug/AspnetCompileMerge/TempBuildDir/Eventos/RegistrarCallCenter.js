var esSocio = "0", medioDeContacto = "0", diasTransaccion = 0, idSupuesto = 0, tipoProducto = 0;
var supuestosMonto = new Array();
var socioJson;


$(document).ready(function () {
    cargarSuspuestosMonto();
    $('#fechaAclaracion').datepicker({ autoclose: true, format: 'yyyy/mm/dd' });
    $('#fechaTransaccion').datepicker({ autoclose: true, format: 'yyyy/mm/dd' });
    if (usuarioCallCenter == true) {
        $("#idMotivoBloqueo").hide();
        HabilitarCamposUsuarioCallCenter();        
        RegistroLlamada();
        
        //$("#inputSocio").Attr("disabled");
    }
    else {
        $("#divselectMedioMov").hide();
        $("#divFolioAutorizacion").hide();
    }
    $('#fechaAclaracion').datepicker({ autoclose: true, format: 'yyyy/mm/dd' });
    $('#fechaTransaccion').datepicker({ autoclose: true, format: 'yyyy/mm/dd' });

    $("#fechaTransaccion").val(moment().format("YYYY/MM/DD"));

    
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
    $('#rootwizard .finish').click(function () { ValidaRequiereAutenticacion(); });
    window.prettyPrint && prettyPrint();
});


function cargarSuspuestosMonto() {
    $.ajax({
        type: "POST", url: rootUrl("/RegistrarCallCenter/ObtenerSuspuestosValidar"), dataType: "json",
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
    //$("#inputSocio").val("");
    reiniciaDatosSocio();

    $("#inputEsSocio").removeClass("rojoValidacion");

    $("#inputEsSocio_").val(valor);
    if (valor === "1") {
        //$("#inputSocio").removeAttr("disabled");
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
                type: "POST", url: rootUrl("/RegistrarCallCenter/ObtenerProxFolio"), dataType: "json",
                success: function (folio) {
                    $("#inputFolio").val(folio[0].FOLIO);
                    $("#spanFolio").val(folio[0].FOLIO);
                    $("#inputFolio_").val(folio[0].FOLIO);
                }
            });
        }
    }
    else {
        //$("#inputSocio").removeAttr("disabled");
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
        idSupuesto = value;
        for (var i = 0; i < supuestosMonto.length; i++) {
            if (supuestosMonto[i] == value) {
                //Si es un reporte de banca e incluye dinero, permitimos que se registre el monto
                if ($("#selectReporte").val() == "4") {
                    $("#inputImporteReclamacion").removeAttr("disabled");
                    $("#selectMedioMov").removeAttr("disabled");
                }//Si es un reporte de debito mostramos la alerta de instrucciones
                else if ($("#selectReporte").val() == "3") {
                    alertify.alert('<p align=LEFT>PARA AGILIZAR LA RESOLUCIÓN DE ESTA CAUSA ES IMPORTANTE ESPECIFICAR, <strong>EN EL AREA DE DESCRIPCIÓN</strong>, LA SIGUIENTE INFORMACIÓN: <br /> <b /><ul align=LEFT><li>LUGAR DONDE SE LLEVO ACABO LA OPERACIÓN</li> <li>FECHA DE LA OPERACIÓN</li></ul>');
                }
                else {
                    $("#inputImporteReclamacion").attr("disabled", "disabled");
                    $("#inputImporteReclamacion").val("$0.00");
                    $("#selectMedioMov").attr("disabled", "disabled");
                }
                document.getElementById("inputDescripcion").focus();
                $("#inputDescripcion").addClass("rojoValidacion");
                break;
            }
            else {
                $("#inputDescripcion").removeClass("rojoValidacion");
                $("#inputImporteReclamacion").removeClass("rojoValidacion");
            }
        }
        ///Cargamos los valores a seteas para el canal y el motivo de cancelaciÃ³n
        $.ajax({
            type: "POST", url: rootUrl("/RegistrarCallCenter/ObtenerCanal"), data: { idSupuesto: value }, dataType: "json",
            success: function (idCanal) {
                document.getElementById("selectCanal").value = idCanal;
            },
            failure: function (response) { alertify.alert(response.d); }
        });

        $.ajax({
            type: "POST", url: rootUrl("/RegistrarCallCenter/ObtenerMotivoCancelacion"), data: { idSupuesto: value }, dataType: "json",
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

    $("#selectPtmoCapt option[value=" + 6 + "]").show();

    $("#inputTel").removeClass("rojoValidacion");
    $("#inputTelCel").removeClass("rojoValidacion");
    $("#btnPersonaMoral").removeAttr("disabled");
    //$("#selectEntidad option[value=" + 0 + "]").attr("selected", true);
    $("#selectPtmoCapt option[value=" + 0 + "]").attr("selected", true);
    $("#selectTipoCuenta option[value=" + 0 + "]").attr("selected", true);
    $("#selectSupuesto option[value=" + 0 + "]").attr("selected", true);

    $("#inputImporteReclamacion").removeAttr("disabled");

    if (numero !== "") {
        var tipoPersona = 1;

        if ($("#inputEsSocio").val() === "3") {
            tipoPersona = 7;
        }

        if (tipoPersona === 1) {
            $.ajax({
                type: "POST", url: rootUrl("/RegistrarCallCenter/BuscaNumSocio"), data: { NUMERO: numero, tipoPersona: tipoPersona }, dataType: "json",
                success: function (socio) {
                    socioJson = socio[1];
                    console.log(socio);
                    if (socio[0].estatus == 1)
                    {
                        if (usuarioCallCenter === true) {
                            $.ajax({
                                type: "POST", url: rootUrl("/RegistrarCallCenter/ValidaSocioBanca"), data: { numero: numero }, dataType: "json",
                                success: function (valida) {
                                    if (usuarioCallCenter == true)
                                    {
                                        
                                        if (socio[1].Bloqueado_Cobranza == 'A' || socio[1].Bloqueado_Exclusion == 'A') {
                                            alertify.alert("El socio ingresado tiene un bloqueo alto.");
                                            //$("#inputSocio").val('');
                                            $("#selectEntidad option[value=" + 0 + "]").attr("selected", true);
                                            $("#selectPtmoCapt option[value=" + 0 + "]").attr("selected", true);
                                            $("#selectTipoCuenta option[value=" + 0 + "]").attr("selected", true);
                                            $("#selectSupuesto option[value=" + 0 + "]").attr("selected", true);
                                            $("#selectEntidad").attr("disabled", "disabled");
                                            $("#selectPtmoCapt").attr("disabled", "disabled");
                                            $("#selectTipoCuenta").attr("disabled", "disabled");
                                            $("#selectSupuesto").attr("disabled", "disabled");
                                            $("#inputDescripcion").val("")
                                            $("#inputDescripcion").attr("disabled", "disabled");
                                            $(".next").addClass("disabled");

                                        }
                                        else if (valida === true)
                                        {

                                            //selectPtmoCapt
                                            

                                            if (socio[1].banca_activa == false || socio[1].id_estatus_banca == 9) {
                                                
                                                
                                                if (socio[1].id_estatus_banca == 9) {
                                                    if (socioJson.tieneContratoBanca == 0) {
                                                        alertify.alert("El socio ingresado no esta dado de alta en CMV Finanzas.");
                                                    }
                                                    else {
                                                        alertify.alert("El socio ingresado no tiene activo el servicio de CMV Finanzas.");
                                                    }

                                                    $("#selectEntidad").attr("disabled", "disabled");                                                    
                                                    $("#selectPtmoCapt").attr("disabled", "disabled");
                                                    $("#inputDescripcion").attr("disabled", "disabled");
                                                    $("#selectReporte").attr("disabled", "disabled");

                                                    $("#selectTipoCuenta").attr("disabled", "disabled");
                                                    $("#selectSupuesto").attr("disabled", "disabled");
                                                    $(".next").addClass("disabled");
                                                }
                                                else {
                                                    alertify.alert("El socio ingresado no ha realizado el registro en CMV Finanzas.");
                                                    $("#selectEntidad").removeAttr("disabled", "disabled");
                                                    $("#selectPtmoCapt").removeAttr("disabled", "disabled");
                                                    $("#inputDescripcion").removeAttr("disabled", "disabled");
                                                    $("#selectReporte").removeAttr("disabled");
                                                    $(".next").removeClass("disabled");

                                                    $("#selectTipoCuenta").removeAttr("disabled", "disabled");
                                                    $("#selectSupuesto").removeAttr("disabled", "disabled");
                                                }
                                                

                                                $("#div-importe-reclamado").hide();
                                                //Elimina la opcion de incidencias
                                                $("#selectPtmoCapt").val(0);

                                                //$("#selectPtmoCapt option[value=" + 6 + "]").remove();
                                                $("#selectPtmoCapt option[value=" + 6 + "]").hide();

                                                $("#inputSocio_").val(numero);
                                                $("#inputNombre").val(socio[0].Nombre_s);
                                                $("#inputAP").val(socio[0].Apellido_Paterno);
                                                $("#inputAM").val(socio[0].Apellido_Materno);
                                                $("#inputCalle").val(socio[0].CALLE + " #" + socio[0].NUMERO_EXTERIOR + " " + socio[0].NUMERO_INTERIOR + ", " + socio[0].Nombre_Colonia + ", CP. " + socio[0].CP + ", " + socio[0].municipio + ", " + socio[0].estado);
                                                $("#inputTel").val(socio[0].Telefono);
                                                $("#inputTelCel").val(socio[0].Tel_Celular);
                                                $("#inputTel").removeAttr("readonly");

                                                if (usuarioCallCenter != true) {
                                                    $("#inputTelCel").removeAttr("readonly");
                                                }

                                                $("#selectSucursal").val(socio[0].Id_de_Sucursal);
                                                $("#inputNumTarjeta").val(socio[0].Num_Tarjeta);

                                                if ($("#inputFolio").val() === "") {
                                                    $.ajax({
                                                        type: "POST", url: rootUrl("/RegistrarCallCenter/ObtenerProxFolio"), dataType: "json",
                                                        success: function (folio) {
                                                            $("#inputFolio").val(folio[0].FOLIO);
                                                            $("#spanFolio").val(folio[0].FOLIO);
                                                            $("#inputFolio_").val(folio[0].FOLIO);
                                                        }
                                                    });
                                                }
                                            }
                                            if (socio[1].banca_activa == true && socio[1].id_motivo_bloqueo != 10) {
                                                
                                                if (socio[1].id_motivo_bloqueo != 1) {
                                                    $("#idMotivoBloqueo").val(socio[1].id_motivo_bloqueo);
                                                    //alertify.alert("El socio ingresado tiene su cuenta bloqueada en CMV Finanzas.");
                                                    if ($("#idMotivoBloqueo").val() != 13) {
                                                        $("#myModalCuentaBloqueada-h4").text("El socio ingresado tiene su cuenta bloqueada en CMV Finanzas.");
                                                    }
                                                    else {
                                                        $("#myModalCuentaBloqueada-h4").text("El socio ingresado tiene su cuenta bloqueada por motivo de anomalias en sus movimientos.");
                                                    }
                                                    
                                                    $('#myModalCuentaBloqueada').modal({ fadeDuration: 100 });
                                                    $("#selectPtmoCapt").val(0);
                                                    if (socio[1].id_motivo_bloqueo != 14) {
                                                        $("#selectPtmoCapt option[value=" + 6 + "]").hide();
                                                    }
                                                    
                                                }

                                                /*if (socio[1].existeError == true) {
                                                    alertify.alert(socio[1].mensajeError);
                                                    return;
                                                }*/


                                                console.log(socio[1]);                                               
                                                $("#selectEntidad").removeAttr("disabled", "disabled");
                                                $("#selectPtmoCapt").removeAttr("disabled", "disabled");
                                                $("#inputDescripcion").removeAttr("disabled", "disabled");

                                                $(".next").removeClass("disabled");

                                                $("#inputSocio_").val(numero);
                                                $("#inputNombre").val(socio[0].Nombre_s);
                                                $("#inputAP").val(socio[0].Apellido_Paterno);
                                                $("#inputAM").val(socio[0].Apellido_Materno);
                                                $("#inputCalle").val(socio[0].CALLE + " #" + socio[0].NUMERO_EXTERIOR + " " + socio[0].NUMERO_INTERIOR + ", " + socio[0].Nombre_Colonia + ", CP. " + socio[0].CP + ", " + socio[0].municipio + ", " + socio[0].estado);
                                                $("#inputTel").val(socio[0].Telefono);
                                                $("#inputTelCel").val(socio[0].Tel_Celular);
                                                $("#inputTel").removeAttr("readonly");
                                                if (usuarioCallCenter != true) {
                                                    $("#inputTelCel").removeAttr("readonly");
                                                }

                                                $("#selectSucursal").val(socio[0].Id_de_Sucursal);
                                                $("#inputNumTarjeta").val(socio[0].Num_Tarjeta);

                                                if ($("#inputFolio").val() === "") {
                                                    $.ajax({
                                                        type: "POST", url: rootUrl("/RegistrarCallCenter/ObtenerProxFolio"), dataType: "json",
                                                        success: function (folio) {
                                                            $("#inputFolio").val(folio[0].FOLIO);
                                                            $("#spanFolio").val(folio[0].FOLIO);
                                                            $("#inputFolio_").val(folio[0].FOLIO);
                                                        }
                                                    });
                                                }
                                            }
                                        }
                                        else {

                                            //$("#inputSocio").val('');
                                            $("#selectEntidad option[value=" + 0 + "]").attr("selected", true);
                                            $("#selectPtmoCapt").val("0");
                                            $("#selectTipoCuenta option[value=" + 0 + "]").attr("selected", true);
                                            $("#selectSupuesto option[value=" + 0 + "]").attr("selected", true);
                                            if (socioJson.tieneContratoBanca == 0) {
                                                alertify.alert("El socio ingresado no esta dado de alta en CMV Finanzas.");
                                            }
                                            else {
                                                alertify.alert("El socio ingresado no tiene activo el servicio de CMV Finanzas.");
                                            }
                                            
                                            $("#div-importe-reclamado").hide();

                                            $("#selectEntidad").attr("disabled", "disabled");
                                            $("#selectPtmoCapt").attr("disabled", "disabled");
                                            $("#selectTipoCuenta").attr("disabled", "disabled");
                                            $("#selectSupuesto").attr("disabled", "disabled");
                                            $("#inputDescripcion").val(" ")
                                            $("#inputDescripcion").attr("disabled", "disabled");
                                            $(".next").addClass("disabled");
                                        }
                                    }
                                    else {
                                        if (valida === true) {

                                            $("#selectEntidad").removeAttr("disabled", "disabled");
                                            $("#selectPtmoCapt").removeAttr("disabled", "disabled");
                                            $("#inputDescripcion").removeAttr("disabled", "disabled");

                                            $(".next").removeClass("disabled");

                                            $("#inputSocio_").val(numero);
                                            $("#inputNombre").val(socio[0].Nombre_s);
                                            $("#inputAP").val(socio[0].Apellido_Paterno);
                                            $("#inputAM").val(socio[0].Apellido_Materno);
                                            $("#inputCalle").val(socio[0].CALLE + " #" + socio[0].NUMERO_EXTERIOR + " " + socio[0].NUMERO_INTERIOR + ", " + socio[0].Nombre_Colonia + ", CP. " + socio[0].CP + ", " + socio[0].municipio + ", " + socio[0].estado);
                                            $("#inputTel").val("");
                                            $("#inputTelCel").val("");
                                            $("#inputTel").removeAttr("readonly");
                                            $("#inputTelCel").removeAttr("readonly");
                                            $("#selectSucursal").val(socio[0].Id_de_Sucursal);
                                            $("#inputNumTarjeta").val(socio[0].Num_Tarjeta);

                                            if ($("#inputFolio").val() === "") {
                                                $.ajax({
                                                    type: "POST", url: rootUrl("/RegistrarCallCenter/ObtenerProxFolio"), dataType: "json",
                                                    success: function (folio) {
                                                        $("#inputFolio").val(folio[0].FOLIO);
                                                        $("#spanFolio").val(folio[0].FOLIO);
                                                        $("#inputFolio_").val(folio[0].FOLIO);
                                                    }
                                                });
                                            }
                                        }
                                    }
                                }
                            });
                        }
                        else
                        {
                            $("#selectReporte").removeAttr("disabled");
                            $(".next").removeClass("disabled");

                            $("#inputSocio_").val(numero);
                            $("#inputNombre").val(socio[0].Nombre_s);
                            $("#inputAP").val(socio[0].Apellido_Paterno);
                            $("#inputAM").val(socio[0].Apellido_Materno);
                            $("#inputCalle").val(socio[0].CALLE + " #" + socio[0].NUMERO_EXTERIOR + " " + socio[0].NUMERO_INTERIOR + ", " + socio[0].Nombre_Colonia + ", CP. " + socio[0].CP + ", " + socio[0].municipio + ", " + socio[0].estado);
                            $("#inputTel").val(socio[0].Telefono);
                            $("#inputTelCel").val(socio[0].Tel_Celular);
                            $("#inputTel").removeAttr("readonly");

                            if (usuarioCallCenter != true) {
                                $("#inputTelCel").removeAttr("readonly");
                            }

                            $("#selectSucursal").val(socio[0].Id_de_Sucursal);
                            $("#inputNumTarjeta").val(socio[0].Num_Tarjeta);

                            $("#selectEntidad").removeAttr("disabled", "disabled");
                            $("#selectPtmoCapt").removeAttr("disabled", "disabled");
                            $("#inputDescripcion").removeAttr("disabled", "disabled");

                            $(".next").removeClass("disabled");

                            $("#inputSocio_").val(numero);
                            $("#inputNombre").val(socio[0].Nombre_s);
                            $("#inputAP").val(socio[0].Apellido_Paterno);
                            $("#inputAM").val(socio[0].Apellido_Materno);
                            $("#inputCalle").val(socio[0].CALLE + " #" + socio[0].NUMERO_EXTERIOR + " " + socio[0].NUMERO_INTERIOR + ", " + socio[0].Nombre_Colonia + ", CP. " + socio[0].CP + ", " + socio[0].municipio + ", " + socio[0].estado);
                            $("#inputTel").val("");
                            $("#inputTelCel").val("");
                            $("#inputTel").removeAttr("readonly");
                            $("#inputTelCel").removeAttr("readonly");
                            $("#selectSucursal").val(socio[0].Id_de_Sucursal);
                            $("#inputNumTarjeta").val(socio[0].Num_Tarjeta);

                            if ($("#inputFolio").val() === "") {
                                $.ajax({
                                    type: "POST", url: rootUrl("/RegistrarCallCenter/ObtenerProxFolio"), dataType: "json",
                                    success: function (folio) {
                                        $("#inputFolio").val(folio[0].FOLIO);
                                        $("#spanFolio").val(folio[0].FOLIO);
                                        $("#inputFolio_").val(folio[0].FOLIO);
                                    }
                                });
                            }
                        }
                    }
                    else { alertify.alert("El número ingresado no existe"); }
                },
                failure: function (response) { alertify.alert(response.d); }
            });
        }
        else {

            $.ajax({
                type: "POST", url: rootUrl("/RegistrarCallCenter/obtenerRepresentates"), data: { NUMERO: numero }, dataType: "json",
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
            type: "POST", url: rootUrl("/RegistrarCallCenter/BuscaPM"), data: { id_persona_rel: $("#selectPerMoral").val() }, dataType: "json",
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
                        type: "POST", url: rootUrl("/RegistrarCallCenter/ObtenerProxFolio"), dataType: "json",
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
    }
    else {
        $("#inputImporteReclamacion").attr("disabled", "disabled");
        $("#inputImporteReclamacion").val("$0.00");
    }

    if (id === "4") {

        $.ajax({
            type: "POST", url: rootUrl("/RegistrarCallCenter/ValidaSocioBanca"), data: { numero: $("#inputSocio").val() }, dataType: "json",
            success: function (valida) {
                if (valida === true) {
                    $("#inputFolioAutorizacion").removeAttr("disabled");
                }
                else {
                    alertify.alert("El socio ingresado no cuenta con el servicio de CMV Finanzas.");
                    $("#inputFolioAutorizacion").val("");
                    $("#inputFolioAutorizacion").attr("disabled", "disabled");
                    $("#selectReporte").val("0");
                }
            }

        });
    }
    else {
        $("#inputFolioAutorizacion").val("");
        $("#inputFolioAutorizacion").attr("disabled", "disabled");
    }

    //Mostramos o ocultamos los nuevos componentes del formulario para el archivo 2701
    if (id === "3") {
        $("#divNumCuenta").removeClass("hidden");
        $("#div-contrato27").removeClass("hidden");
        $("#divCanalTransaccion").removeClass("hidden");
        $("#divMotivoReclamacion").removeClass("hidden");
        $("#divFormA27").removeClass("hidden");
        $("#fechaTransaccion").removeAttr("disabled");
    }
    else {
        $("#divNumCuenta").addClass("hidden");
        $("#div-contrato27").addClass("hidden");
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

        if ($("#inputNumeroCuentaPtmo").val() === "" && $("#selectReporte").val() === "3") {
            validaForm = false;
            $("#inputNumeroCuentaPtmo").addClass("rojoValidacion");
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

        //if (parseFloat($("#inputImporteReclamacion").val().replace("$", "")) == 0 && $("#selectPtmoCapt").val() == 6) {
        //    $("#inputImporteReclamacion").addClass("rojoValidacion");
        //    validaForm = false;
        //}

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

    if ($("#selectReporte").val() === "3") {
        $("#spanIR").val($("#inputImporteReclamacion").val());
    }
    else
        $("#spanIR").val($("#inputImporteReclamacion").val());

    $("#spanDescr").val($("#inputDescripcion").val());
    $("#spanSucursalRegistro").val($("#selectSucursalRegistro option:selected").html());
    $("#spanEntidadFederativa").val($("#selectEntidad option:selected").html());
    $("#spanTarjetaD").val($("#inputNumTarjeta").val());
    $("#spanNumReferencia").val($("#inputNumReferencia").val());
    $("#spanFechaAcla").val($("#fechaAclaracion").val());
    $("#spanFechaTran").val($("#fechaTransaccion").val());
    $("#spanMedioDeteccion").val($("#selectMedioMov option:selected").html());
    $("#spanFolioBanca").val($("#inputFolioAutorizacion").val());
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

function ValidaRequiereAutenticacion() {
    $.ajax({
        type: "POST", url: rootUrl("/RegistrarCallCenter/validaAutenticacionBanca"), data: { idSupuesto: idSupuesto }, dataType: "json",
        success: function (validaAutenticacionBanca) {
            if (validaAutenticacionBanca) {
                $("#dvLoading").removeAttr("hidden");
                $.ajax({
                    type: "POST", url: rootUrl("/RegistrarCallCenter/CargarCuestionario"), data: { numero: $("#inputSocio").val() }, dataType: "json",
                    success: function (validaAutenticacionBanca) {
                        console.log(validaAutenticacionBanca);
                        var html = '';
                        //Pregunta 1
                        html = '<div class="row"><div class="col-md-12">' + validaAutenticacionBanca[0].Pregunta + '</div></div>';
                        if (validaAutenticacionBanca[0].respuestasAutentificacion.length == 2) {
                            html += '<div class="row">'
                                + '<div class="col-md-12" > <input type="radio" name="pregunta1" value="' + validaAutenticacionBanca[0].respuestasAutentificacion[0].respuestaCorrecta + '">' + validaAutenticacionBanca[0].respuestasAutentificacion[0].respuesta + '</div>'
                                + '</div>';
                            html += '<div class="row">'
                                + '<div class="col-md-12" > <input type="radio" name="pregunta1" value="' + validaAutenticacionBanca[0].respuestasAutentificacion[1].respuestaCorrecta + '">' + validaAutenticacionBanca[0].respuestasAutentificacion[1].respuesta + '</div>'
                                + '</div>';
                        }
                        if (validaAutenticacionBanca[0].respuestasAutentificacion.length == 3) {
                            html += '<div class="row">'
                                + '<div class="col-md-12" > <input type="radio" name="pregunta1" value="' + validaAutenticacionBanca[0].respuestasAutentificacion[0].respuestaCorrecta + '">' + validaAutenticacionBanca[0].respuestasAutentificacion[0].respuesta + '</div>'
                                + '</div>';
                            html += '<div class="row">'
                                + '<div class="col-md-12" > <input type="radio" name="pregunta1" value="' + validaAutenticacionBanca[0].respuestasAutentificacion[1].respuestaCorrecta + '">' + validaAutenticacionBanca[0].respuestasAutentificacion[1].respuesta + '</div>'
                                + '</div>';
                            html += '<div class="row">'
                                + '<div class="col-md-12" > <input type="radio" name="pregunta1" value="' + validaAutenticacionBanca[0].respuestasAutentificacion[2].respuestaCorrecta + '">' + validaAutenticacionBanca[0].respuestasAutentificacion[2].respuesta + '</div>'
                                + '</div>';
                        }

                        //Pregunta 2
                        html += '<br/><div class="row"><div class="col-md-12">' + validaAutenticacionBanca[1].Pregunta + '</div></div>';
                        if (validaAutenticacionBanca[1].respuestasAutentificacion.length == 2) {
                            html += '<div class="row">'
                                + '<div class="col-md-12" > <input type="radio" name="pregunta2" value="' + validaAutenticacionBanca[1].respuestasAutentificacion[0].respuestaCorrecta + '">' + validaAutenticacionBanca[1].respuestasAutentificacion[0].respuesta + '</div>'
                                + '</div>';
                            html += '<div class="row">'
                                + '<div class="col-md-12" > <input type="radio" name="pregunta2" value="' + validaAutenticacionBanca[1].respuestasAutentificacion[1].respuestaCorrecta + '">' + validaAutenticacionBanca[1].respuestasAutentificacion[1].respuesta + '</div>'
                                + '</div>';
                        }
                        if (validaAutenticacionBanca[1].respuestasAutentificacion.length == 3) {
                            html += '<div class="row">'
                                + '<div class="col-md-12" > <input type="radio" name="pregunta2" value="' + validaAutenticacionBanca[1].respuestasAutentificacion[0].respuestaCorrecta + '">' + validaAutenticacionBanca[1].respuestasAutentificacion[0].respuesta + '</div>'
                                + '</div>';
                            html += '<div class="row">'
                                + '<div class="col-md-12" > <input type="radio" name="pregunta2" value="' + validaAutenticacionBanca[1].respuestasAutentificacion[1].respuestaCorrecta + '">' + validaAutenticacionBanca[1].respuestasAutentificacion[1].respuesta + '</div>'
                                + '</div>';
                            html += '<div class="row">'
                                + '<div class="col-md-12" > <input type="radio" name="pregunta2" value="' + validaAutenticacionBanca[1].respuestasAutentificacion[2].respuestaCorrecta + '">' + validaAutenticacionBanca[1].respuestasAutentificacion[2].respuesta + '</div>'
                                + '</div>';
                        }

                        //Pregunta 3
                        html += '<br/><div class="row"><div class="col-md-12">' + validaAutenticacionBanca[2].Pregunta + '</div></div>';
                        if (validaAutenticacionBanca[2].respuestasAutentificacion.length == 2) {
                            html += '<div class="row">'
                                + '<div class="col-md-12" > <input type="radio" name="pregunta3" value="' + validaAutenticacionBanca[2].respuestasAutentificacion[0].respuestaCorrecta + '">' + validaAutenticacionBanca[2].respuestasAutentificacion[0].respuesta + '</div>'
                                + '</div>';
                            html += '<div class="row">'
                                + '<div class="col-md-12" > <input type="radio" name="pregunta3" value="' + validaAutenticacionBanca[2].respuestasAutentificacion[1].respuestaCorrecta + '">' + validaAutenticacionBanca[2].respuestasAutentificacion[1].respuesta + '</div>'
                                + '</div>';
                        }
                        if (validaAutenticacionBanca[2].respuestasAutentificacion.length == 3) {
                            html += '<div class="row">'
                                + '<div class="col-md-12" > <input type="radio" name="pregunta3" value="' + validaAutenticacionBanca[2].respuestasAutentificacion[0].respuestaCorrecta + '">' + validaAutenticacionBanca[2].respuestasAutentificacion[0].respuesta + '</div>'
                                + '</div>';
                            html += '<div class="row">'
                                + '<div class="col-md-12" > <input type="radio" name="pregunta3" value="' + validaAutenticacionBanca[2].respuestasAutentificacion[1].respuestaCorrecta + '">' + validaAutenticacionBanca[2].respuestasAutentificacion[1].respuesta + '</div>'
                                + '</div>';
                            html += '<div class="row">'
                                + '<div class="col-md-12" > <input type="radio" name="pregunta3" value="' + validaAutenticacionBanca[2].respuestasAutentificacion[2].respuestaCorrecta + '">' + validaAutenticacionBanca[2].respuestasAutentificacion[2].respuesta + '</div>'
                                + '</div>';
                        }

                        $("#cuerpoCuestionario").html(html);
                    },
                    failure: function (response) { alertify.alert(response.d); },
                    error: function (xhr, status, error) {
                        if (xhr.status == 460) {
                            window.location = rootUrl("Login/Index");
                        }
                    }
                });

                $("#dvLoading").attr("hidden", "hidden");
                $("#myModalCuestionario").modal({backdrop: 'static', keyboard: false });
            }
            else { generaSubmit(); }
        },
        failure: function (response) { alertify.alert(response.d); },
        error: function (xhr, status, error) {
            if (xhr.status == 460) {
                window.location = rootUrl("Login/Index");
            }
        }
    });
}

function selectCuenta(value) {

    $("#selectTipoCuenta").val(0);
    $("#selectSupuesto").val(0);
    $("#selectSupuesto").attr("disabled", "disabled");

    if ($("#selectPtmoCapt").val() !== "0") {
        $("#selectPtmoCapt").removeClass("rojoValidacion");

        if ($("#selectPtmoCapt").val() === "4" /*|| $("#selectPtmoCapt").val() === "5" || $("#selectPtmoCapt").val() === "6"*/)
        {
            if ($("#selectPtmoCapt").val() === "6") {
                $("#inputNumeroCuentaPtmo").removeAttr("readonly");
                $("#div-importe-reclamado").show();
                $("#inputImporteReclamacion").removeAttr("disabled");
                $("#div-contrato27").show();
                $("#span-div-importe-rec").show();
            }
            else {
                $("#selectProducto").val("1");
                $("#selectCanal").val("1");
                $("#selectMotivoCancelacion").val("1")
                $("#div-importe-reclamado").hide();
                $("#div-contrato27").hide();
                $("#divFormA27").hide();
                $("#span-div-importe-rec").hide();
            }

            $.ajax({
                type: "POST", url: rootUrl("/RegistrarCallCenter/ValidaSocioBanca"), data: { numero: $("#inputSocio").val() }, dataType: "json",
                success: function (valida) {
                    if (valida === true) {

                        $.ajax({
                            type: "POST", url: rootUrl("/RegistrarCallCenter/obtenerCuentas"), data: { tipoCuenta: $("#selectPtmoCapt").val(), numero: $("#inputSocio").val() }, dataType: "json",
                            success: function (cuentas) {
                                $("#selectTipoCuenta").removeAttr("disabled");
                                var html = '';
                                var html = '<option value="0">SELECCIONAR</option>';
                                $.each(cuentas, function (i, cuenta) {
                                    html += '<option value="' + cuenta.ID_CUENTA + '">' + cuenta.DESCRIPCION + '</option>';
                                });
                                $("#selectTipoCuenta").html(html);
                            }
                        });

                    }
                    else {
                        alertify.alert("El socio ingresado no cuenta con el servicio de CMV Finanzas activo.");
                        $("#selectPtmoCapt").val("0");
                    }
                }

            });
        }
        else {
            $.ajax({
                type: "POST", url: rootUrl("/RegistrarCallCenter/obtenerCuentas"), data: { tipoCuenta: $("#selectPtmoCapt").val(), numero: $("#inputSocio").val()  }, dataType: "json",
                success: function (cuentas) {
                    $("#selectTipoCuenta").removeAttr("disabled");
                    var html = '';
                    var html = '<option value="0">SELECCIONAR</option>';
                    $.each(cuentas, function (i, cuenta) {
                        html += '<option value="' + cuenta.ID_CUENTA + '">' + cuenta.DESCRIPCION + '</option>';
                    });
                    $("#selectTipoCuenta").html(html);
                }
            });
        }
    }
    else { $("#selectSupuesto").attr("disabled", "disabled"); }
}

function tipoCuenta(value) {

    //if (socioJson != undefined) {
    //    alert(socioJson.banca_activa);
    //}

    if (value != 0) {
        if ($("#selectPtmoCapt").val() === "6") {

            $("#inputImporteReclamacion").removeAttr("disabled");
        }
        
        $("#selectTipoCuenta").removeClass("rojoValidacion");
        $("#selectSupuesto").removeAttr("disabled");
        $.ajax({
            type: "POST", url: rootUrl("/RegistrarCallCenter/ObtenerSupuestos"), data: { id: $("#selectReporte").val(), idTipoCuenta: $("#selectPtmoCapt").val(), idCuenta: value },
            dataType: "json",
            success: function (supuestos) {
                var html = '<option value="0">SELECCIONAR</option>';
                $.each(supuestos, function (i, supuesto) {
                    html += '<option value="' + supuesto.ID_SUPUESTOS_REPORTE + '">' + supuesto.DESCRIPCION + '</option>';
                });
                $("#selectSupuesto").html(html);

                ///Se omite la opcion de enviar la contraseña por que solamente se debe de mostrar cunado el socio no tiene su cuenta activa
                if ($("#selectTipoCuenta").val() == 36) {
                    if (socioJson.banca_activa == true) {
                        $("#selectSupuesto option[value=2172]").hide();
                    }
                    else {
                        $("#selectSupuesto option[value=2173]").hide();
                    }
                    
                }
            }
        });

        if (value == 5 && ($("#selectReporte").val() == "1" || $("#selectReporte").val() == "3")) {
            $("#fechaAclaracion").removeAttr("disabled");
            $("#fechaAclaracion").val(moment().format("DD/MM/YYYY"));
        }
        else {
            $("#fechaAclaracion").attr("disabled", "disabled");
        }

        if ((($("#selectReporte").val() === "1" && $("#selectTipoCuenta").val() === "5") || ($("#selectReporte").val() === "3")) || $("#selectPtmoCapt").val() == 6) {
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
        else if ($("#selectPtmoCapt").val() == "6") {
            $("#inputNumeroCuentaPtmo").removeAttr("readOnly");
        }

        else {
            $("#labelinputNumeroCuentaPtmo").html("Número de cuenta/préstamo/DPF:");
            $("#inputNumeroCuentaPtmo").attr("readOnly", "readOnly");
        }
        var id_mov;
        //Cargamos los idMovs de lo seleccionado en la aplicaciÃ³n
        if ($("#selectReporte").val() === "3") {
            $.ajax({
                type: "POST", url: rootUrl("/RegistrarCallCenter/ObtenerProducto"), data: { id: $("#selectTipoCuenta").val() },
                dataType: "json",
                success: function (idProd) {
                    $("#selectProducto").val(idProd);
                }
            });

            $.ajax({
                type: "POST", url: rootUrl("/RegistrarCallCenter/ObtenerIdMovProducto"), data: { id: $("#selectTipoCuenta").val() },
                dataType: "json",
                success: function (prod) {
                    tipoProducto = prod;
                    if (tipoProducto > 0) {
                        $.ajax({
                            type: "POST", url: rootUrl("/RegistrarCallCenter/ObtenerProductoUNE"), data: { idMov: tipoProducto },
                            dataType: "json",
                            success: function (idMov) {
                                id_mov = idMov;
                            }
                        });

                        if (prod === 105) {
                            $.ajax({
                                type: "POST", url: rootUrl("/RegistrarCallCenter/ObtenerPlazosFijos"), data: { numero: $("#inputSocio").val() },
                                dataType: "json",
                                success: function (dpfs) {
                                    console.log(dpfs);
                                    if (dpfs.length === 1) {
                                        $("#labelinputNumeroCuentaPtmo").html("NÃºmero de DPF:");
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
                                type: "POST", url: rootUrl("/RegistrarCallCenter/ObtenerNumPtmo"), data: { numero: $("#inputSocio").val(), idMov: prod },
                                dataType: "json",
                                success: function (numPtmo) {
                                    $("#labelinputNumeroCuentaPtmo").html("NÃºmero de prÃ©stamo:");
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

function ValidarCuestionarioBanca() {

    if ($('input[name="pregunta1"]').is(':checked') && $('input[name="pregunta2"]').is(':checked') && $('input[name="pregunta3"]').is(':checked')) {
        if ($('input:radio[name=pregunta1]:checked').val() == "true" && $('input:radio[name=pregunta2]:checked').val() == "true" && $('input:radio[name=pregunta3]:checked').val() == "true") {
            $("#inputIdEstatusReporte_").val(0);
            generaSubmit();
        }
        else {
            $("#inputIdEstatusReporte_").val(11);
            //alertify.alert("La autenticación del socio no fue correcta. El reporte se cerrara en automatico.", function (e) {
            //    if (e) {
            //        generaSubmit();
            //    }
            //});
            alertify.alert("La autenticación del socio no fue correcta. El reporte se cerrara en automatico.", function (e) {
                if (e) {
                    generaSubmit();
                }
            });

        }
        //generaSubmit();
    }
    else {
        alertify.alert("Favor de responder todas las preguntas. <br /> ")
    }
}

function prueba() {
    $.ajax({
        type: "POST", url: rootUrl("/RegistrarCallCenter/Prueba"), data: {}, dataType: "json",
        success: function (diasDif) {

        }
    });

}

function HabilitarCamposUsuarioCallCenter() {

    var numeroSocio = ObtenerParametroURL("vNumeroSocio");
    
    if (numeroSocio == "" || numeroSocio == null) {
        $("#myModalSinLlamada").modal("show");
        //alert("Debes de tener una llamada entrante para poder registrar un reporte.");        
    }
    else {
        
        $('#inputSocio').val(parseInt(numeroSocio));
        hacer_click(numeroSocio);
        
    }
    

    //$("#inputSocio").removeAttr("disabled");
    $("#buscarNumSocio").removeAttr("disabled");
    $("#inputEsSocio option[value=" + 1 + "]").attr("selected", true);
    $("#inputEsSocio").attr("disabled", "disabled");
    esSocio = "1";
    EsSocio(esSocio);

    $("#div-telefono").hide();
    $("#inputTelCel").attr("readonly", "readonly");
    $("#selectSucursalRegistro").attr("disabled", "disabled");
    //if (usuarioSucursal == true) {
        //$("#selectMedioContacto option[value=" + 1 + "]").attr("selected", true);
    //}
    //else {
        //$("#selectMedioContacto option[value=" + 2 + "]").attr("selected", true);
    //}
    
    medioContacto();
    $("#selectMedioContacto").attr("onlyread", "onlyread");
    $("#selectMedioContacto").attr("disabled", "disabled");


    $("#selectReporte option[value=" + 4 + "]").attr("selected", true);

    $("#selectReporte").attr("onlyread", "onlyread");
    $("#selectReporte").attr("disabled", "disabled");
    //cargarSupuestos($("#selectReporte").val());
    $("#div-no-referencia").hide();
    if ($("#selectPtmoCapt").val() === "0") {
        $("#div-importe-reclamado").hide();
        $("#div-contrato27").hide();
        $("#divFormA27").hide();
        $("#span-div-importe-rec").hide();
    }
    if ($("#selectPtmoCapt").val() === "6") {
        $("#div-importe-reclamado").removeAttr("hidden");
        $("#div-contrato27").removeAttr("hidden");
        $("#divFormA27").removeAttr("hidden");

    } else {
        $("#span-div-importe-rec").hide();
    }

    $("#div-contrato27").hide();
    $("#div-importe-reclamado").hide();
    $("#div-no-tarjeta").hide();
    $("#div-representante-leg").hide();
    $("#div-NoFolioUne").hide();
    $("#DatosAdiccionales").hide();

    $("#span-DatosAdiccionales").hide();
    $("#span-div-representante").hide();
    $("#span-div-telefono").hide();
    $("#span-div-importe-rec").hide();
    $("#span-div-no-tarjeta").hide();
    $("#span-div-no-referencia").hide();
    $("#span-div-NoFolioUne").hide();

    //////Datos ocultos por normativa//
    $("#selectEntidad").val(16);

    $("#div-celular").hide();
    $("#div-domicilio").hide();
    $("#div-sucursal").hide();
    $("#div-EntFed").hide();

    $("#div-spanTelCel").hide();
    $("#div-spanDomicilio").hide();
    $("#div-spanSucursal").hide();
    $("#div-spanEntFed").hide();


    cargarSupuestos(4)



    //$("#DatosAdiccionales").hide();

}

function setPermisos(permiso) {
    usuarioCallCenter = permiso;
    if (usuarioCallCenter == true) {
        HabilitarCamposUsuarioCallCenter();
    }
    return permiso;
}

function limpiarInputReporte() {

    var montoReclamacion = parseFloat($("#inputImporteReclamacion").val().replace("$", ""));
    if (montoReclamacion == 0) {
        $("#inputImporteReclamacion").val("");
    } else {
        $("#inputImporteReclamacion").val(montoReclamacion);
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

function isNumber(evt) {
    var iKeyCode = (evt.which) ? evt.which : evt.keyCode
    if (iKeyCode != 46 && iKeyCode > 31 && (iKeyCode < 48 || iKeyCode > 57))
        return false;

    return true;
}

function asignarDPF(dpf) {
    $("#labelinputNumeroCuentaPtmo").html("NÃºmero de DPF:");
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

function cuentaBloqueada() {
    if ($("#idMotivoBloqueo").val() == 13) {
        $("#myModalCuentaBloqueada").modal("hide");
        var vContactID = ObtenerParametroURL("vContactID");
        idSeguimientoAFraudes(vContactID);

        window.location = rootUrl("/Registros/Registros");

    }
    else {
        $("#myModalCuentaBloqueada").modal("hide");
        
    }
}

function RegistroLlamada() {

    var iframeCallCenter = {};

    $.ajax({
        type: "GET",
        url: "http://localhost:6080/GetAgentID",
        contentType: "text/xml; charset=utf-8",
        async: false,
        success: function (response) {
            $(response).find("AgentId").each(function () {
                //$("#agenteId").text($(this).text());
                iframeCallCenter.vAgenteID = $(this).text();
            });
        },
        error: function (response, error, fail) {
            $('#myModalPrecesnseSinSesion').modal({ fadeDuration: 100, backdrop: 'static' });
            //alert("Es posible que la sesión del agente este inactiva");
        }
    });


    $.ajax({
        type: "GET",
        url: "http://localhost:6080/GetAgentName",
        contentType: "text/xml; charset=utf-8",
        async: false,
        success: function (response) {
            $(response).find("AgentName").each(function () {
                //$("#agenteNombre").text($(this).text());
                iframeCallCenter.AgenteAtiende = $(this).text();
            });
        },
        error: function (response, error, fail) {
            console.log('seccion no iniciada')
            //alert("Es posible que la sesión del agente este inactiva");
        }
    });
    
    
    //iFrameCallcenter.vAgenteID = $("#agenteId").text();
    //iFrameCallcenter.AgenteAtiende = $("#agenteNombre").text();
    
    $.ajax({
        method: "POST",
        url: rootUrl("/RegistrarCallCenter/RegistrarLlamada"),
        data: iframeCallCenter,
        dataType: "json",
        success: function (data) {
            if (data.estatus == 200) {
                //alert("Registro Correcto");
                console.log("Peticion de registro de reporte correcta")
            }
        },
        error: function (xhr, status, error) {
            alert(error);
        },
    });
}


function idSeguimientoAFraudes(vContactID) {
    var url = "http://localhost:6080/AddCallData?Key=ID_CC_Fraudes&Value=" + vContactID;

    $.ajax({
        type: "GET",
        url: url,
        contentType: "text/xml; charset=utf-8",
        success: function (response) {
            $(response).find("Result").each(function () {
                //alert("Redireccionamiento exitoso");
                console.log("Redireccionamiento exitoso");
            });
        },
        error: function (response, error, fail) {
            alert(error)
        }
    });

}

function ObtenerParametroURL(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function RegistroSinLlamada() {
    window.location = rootUrl("/Registros/Registros");
}