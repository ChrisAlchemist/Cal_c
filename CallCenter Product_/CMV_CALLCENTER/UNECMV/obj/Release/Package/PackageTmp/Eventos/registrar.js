var esSocio = "0", medioDeContacto = "0";

$(function () {
    $("#wizard").steps({
        headerTag: "h2",
        bodyTag: "section",
        transitionEffect: "slideLeft",
        onStepChanging: function (event, currentIndex, newIndex) {
            var validacion;
            if (currentIndex === 0)
            {
                validacion = validaContinuar();
                if (validacion === true)
                    GeneraResumen();
            }
            if (currentIndex === 1)
            {
                //$("#dvLoading").removeAttr("hidden");
                validacion = true;
            }
            return validacion;
        }
        //stepsOrientation: "vertical"
    });
});

function EsSocio(valor) {
    esSocio = valor;
    $("#inputSocio").val("");
    reiniciaDatosSocio();

    $("#inputEsSocio").removeClass("rojoValidacion");

    $("#inputEsSocio_").val(valor);
    if (valor === "1") {
        $("#inputSocio").removeAttr("disabled");
        $("#buscarNumSocio").removeAttr("disabled");
        $("#btnTutor").attr("disabled","disabled");

        $("#inputNombre").attr("readonly","readonly");
        $("#inputAP").attr("readonly", "readonly");
        $("#inputAM").attr("readonly", "readonly");

        $("#inputTel").attr("readonly", "readonly");
        $("#inputTelCel").attr("readonly", "readonly");
    }
    else if (valor==="2"){
        $("#inputSocio").attr("disabled", "disabled");
        $("#buscarNumSocio").attr("disabled", "disabled");
        $("#btnTutor").attr("disabled", "disabled");

        $("#inputNombre").removeAttr("readonly");
        $("#inputAP").removeAttr("readonly");
        $("#inputAM").removeAttr("readonly");

        $("#inputTel").removeAttr("readonly");
        $("#inputTelCel").removeAttr("readonly");
    }
    else {
        $("#inputSocio").removeAttr("disabled");
        $("#buscarNumSocio").removeAttr("disabled");

        $("#inputNombre").attr("readonly", "readonly");
        $("#inputAP").attr("readonly", "readonly");
        $("#inputAM").attr("readonly", "readonly");

        $("#inputTel").attr("readonly", "readonly");
        $("#inputTelCel").attr("readonly", "readonly");
    }
}

$(document).ready(function () {

    $("#continuar").click(function (evt) {
        evt.preventDefault();
    });
});

function supuestos()
{
    $("#selectSupuesto").removeClass("rojoValidacion");
}

function descripcionReporte()
{
    $("#inputDescripcion").removeClass("rojoValidacion");
}

function hacer_click(numero) {
    $("#inputTel").removeClass("rojoValidacion");
    $("#inputTelCel").removeClass("rojoValidacion");

    $("#btnPersonaMoral").removeAttr("disabled");

    var tipoPersona = 1;

    if ($("#inputEsSocio").val() === "3")
    {
        tipoPersona = 7;
    }

    if (tipoPersona === 1)
    {
        $.ajax({
            type: "POST", url: "/Registrar/BuscaNumSocio",
            data: { NUMERO: numero, tipoPersona: tipoPersona }, dataType: "json",
            success: function (socio) {
                if (socio.estatus == 1) {
                    $("#inputSocio_").val(numero);
                    $("#inputNombre").val(socio.Nombre_s);
                    $("#inputAP").val(socio.Apellido_Paterno);
                    $("#inputAM").val(socio.Apellido_Materno);
                    $("#inputCalle").val(socio.CALLE + " #" + socio.NUMERO_EXTERIOR + " " + socio.NUMERO_INTERIOR + ", " + socio.Nombre_Colonia + ", CP. " + socio.CP);
                    $("#inputMunicipio").val(socio.municipio + ", " + socio.estado);
                    $("#inputTel").val(socio.Telefono);
                    $("#inputTelCel").val(socio.Tel_Celular);
                    $("#inputTel").removeAttr("readonly");
                    $("#inputTelCel").removeAttr("readonly");

                    $.ajax({
                        type: "POST", url: "/Registrar/ObtenerProxFolio", dataType: "json",
                        success: function (folio) {
                            $("#inputFolio").val(folio[0].FOLIO);
                            $("#spanFolio").val(folio[0].FOLIO);
                            $("#inputFolio_").val(folio[0].FOLIO);
                        }
                    });


                }
                else { alert("El número especificado no existe"); }
            },
            failure: function (response) { alert(response.d); }
        });
    }
    else {

        $.ajax({
            type: "POST", url: "/Registrar/obtenerRepresentates",
            data: { NUMERO: numero }, dataType: "json",
            success: function (representante) {
                if(representante[0].ESTATUS===1)
                {
                    $("#selectPerMoral").removeAttr("disabled");
                    var html = '<option value="0">REPRESENTANTE LEGAL</option>';
                    $.each(representante, function (i, rep)
                    {
                        html += '<option value="' + rep.id_persona_rel + '">' + rep.NOMBRE + '</option>';
                     });
                     $("#selectPerMoral").html(html);
                }
                else
                {
                    alert("EL número especificado no existe");
                }
            },
            failure: function (response) { alert(response.d); }
        });
    }

}

function cargaDatosPM()
{
    if ($("#selectPerMoral").val() !== "0")
    {
        $.ajax({
            type: "POST", url: "/Registrar/BuscaPM",
            data: { id_persona_rel: $("#selectPerMoral").val() }, dataType: "json",
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
                        type: "POST", url: "/Registrar/ObtenerProxFolio", dataType: "json",
                        success: function (folio) {
                            $("#inputFolio").val(folio[0].FOLIO);
                            $("#spanFolio").val(folio[0].FOLIO);
                            $("#inputFolio_").val(folio[0].FOLIO);
                        }
                    });


                }
                else { alert("El número especificado no existe"); }
            },
            failure: function (response) { alert(response.d); }
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

function GuardarTel() {
    $.ajax({
        type: "POST",
        url: "/Registrar/ActualizaTelefono",
        data: { NUMERO: $("#inputSocio").val(), TELEFONO: $("#inputTel").val(), TEL_CELULAR: $("#inputTelCel").val() },
        dataType: "json",
        success: function (estatus) {
            if (estatus[0] === 1) {
                alert("Telefonos actualizados correctamente");
            }
            else
                alert("No fue posible actualizar los numeros telefonicos");
        }
    });
}

function MedioDeContacto(medioContacto)
{
    $("#divMedioContacto").attr("hidden","hidden");
    $("#radioMedioContacto").val(medioContacto);
    medioDeContacto = medioContacto;

    if (medioDeContacto === "1")
        $("#spanMedio").val("Presencial");
    else
        $("#spanMedio").val("Via Telefonica");
}

function cargarSupuestos(id)
{
    $("#spanTP").val($("#selectReporte option:selected").html());
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

function medioContacto()
{
    $("#selectMedioContacto").removeClass("rojoValidacion");
}

function validaContinuar() {

    var validaForm = true;

    if (esSocio === "0")
    {
        $("#inputEsSocio").addClass("rojoValidacion");
        validaForm = false;
    }
    else
    {
        if (esSocio === "1" || esSocio === "3")
        {
            if ($("#inputSocio").val() === "") {
                $("#inputSocio").addClass("rojoValidacion");
                validaForm = false;
            }

            if ($("#inputTel").val() === "")
            {
                $("#inputTel").addClass("rojoValidacion");
                validaForm = false;
            }

            if($("#inputTelCel").val() === "")
            {
                $("#inputTelCel").addClass("rojoValidacion");
                validaForm = false;
            }
                
        }
        else
        {
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

            if ($("#inputTel").val() === "") {
                $("#inputTel").addClass("rojoValidacion");
                validaForm = false;
            }

            if ($("#inputTelCel").val() === "") {
                $("#inputTelCel").addClass("rojoValidacion");
                validaForm = false;
            }
        }

        if($("#selectReporte").val()==="0")
        {
            $("#selectReporte").addClass("rojoValidacion");
            validaForm = false;
        }

        if ($("#selectSupuesto").val() === "0" || $("#selectSupuesto").val() === "") {
            $("#selectSupuesto").addClass("rojoValidacion");
            validaForm = false;
        }

        if ($("#inputDescripcion").val() === "") {
            $("#inputDescripcion").addClass("rojoValidacion");
            validaForm = false;
        }

        if ($("#selectMedioContacto").val() === "0") {
            $("#selectMedioContacto").addClass("rojoValidacion");
            validaForm = false;
        }

    }

    return validaForm;

}

function validaContinuarIndice2()
{
    var validaForm = true;

    if (medioDeContacto === "0")
    {
        $("#divMedioContacto").removeAttr("hidden");
        validaForm = false;
    }

    if ($("#selectReporte").val() === "0") {
        $("#selectReporte").addClass("rojoValidacion");
        validaForm = false;
    }

    if ($("#selectSupuesto").val() === "0" || $("#selectSupuesto").val() === "") {
        $("#selectSupuesto").addClass("rojoValidacion");
        validaForm = false;
    }

    if ($("#inputDescripcion").val() === "") {
        $("#inputDescripcion").addClass("rojoValidacion");
        validaForm = false;
    }

    return validaForm;
}

function GeneraResumen()
{
    $("#spanNombre").val($("#inputNombre").val() + " " + $("#inputAP").val() + " " + $("#inputAM").val());
    $("#spanTel").val($("#inputTel").val());
    $("#spanTelCel").val($("#inputTelCel").val());

    $("#spanTS").val($("#selectSupuesto option:selected").html());

    $("#spanMedio").val($("#selectMedioContacto option:selected").html())

    $("#spanDescr").val($("#inputDescripcion").val());
}


function reiniciaDatosSocio()
{
    $("#inputSocio").removeClass("rojoValidacion");
    $("#inputNombre").val("");
    $("#inputAP").val("");
    $("#inputAM").val("");
    $("#inputCalle").val("");
    $("#inputMunicipio").val("");
    $("#inputTel").val("");
    $("#inputTelCel").val("");
    if($("#inputEsSocio").val()==="3")
    {
        $("#selectPerMoral").attr("disabled","disabled");
    }

}

function generaSubmit() {
    document.getElementById("frmRegistrar").submit();
}