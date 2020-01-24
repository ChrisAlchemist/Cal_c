var fecha1, fecha2, contador;

$(document).ready(function () {
    $('#fechaInicio').datepicker({autoclose: true,format:'dd/mm/yyyy'});
    $('#fechaFin').datepicker({autoclose: true,format: 'dd/mm/yyyy'});
    $("table#example").dataTable();
    $("#btnGenera").click(function (evt) {evt.preventDefault();});
});

function cambioFecha(id)
{
    if (id === "fechaInicio")
    {
        if ($("#fechaInicio").val().length === 10)
        {
            $("#fechaInicio").removeClass("rojoValidacion");
        }
    }

    if (id === "fechaFin") {
        if ($("#fechaFin").val().length === 10)
        {
            $("#fechaFin").removeClass("rojoValidacion");
        }
    }
}

function validaForm(fechaInicio, fechaFin)
{
    var validacion = valida()
    if(validacion === true)
    {
        fecha1 = fechaInicio;
        fecha2 = fechaFin;
        $("#dvLoading").removeAttr("hidden");
        $.ajax({
            type: "POST",
            data: {
                tipoReporte: $("#selectTipoReporte").val(),
                estatus: $("#selectEstatusGeneral").val(), subEstatus: $("#selectEstatusInterno").val(),
                sucursalRegistro: $("#selectSucursal").val(), sucursalSocio: $("#selectSucursalSocio").val(),
                fechaInicio: fechaInicio, FechaFin: fechaFin
            },
            url: rootUrl("/Condusef/GeneraListado"),dataType: "json",
            success: function (listado) {
                if (listado.length > 0)
                {
                    contador = listado[0].CONTADOR;
                    var html = '<table id="example" class="display nowrap dataTable dtr-inline" cellspacing="0" width="100%"> <thead> <tr>';
                    html += '<th>Folio</th><th>Estatus</th><th>Sub Estatus</th><th>Numero</th><th>Nombre</th><th>Operación</th><th>Servicio</th>';
                    html += '</tr></thead> <tbody id="tableContenido">';
                    $.each(listado, function (i, lis) {
                        html += '<tr>';
                        html += '<td>' + lis.FOLIO + '</td>';
                        html += '<td>' + lis.ESTATUS + '</td>';
                        html += '<td>' + lis.SUBESTATUS + '</td>';
                        html += '<td>' + lis.NUMERO + '</td>';
                        html += '<td>' + lis.NOMBRE + '</td>';
                        html += '<td>' + lis.OPERACION + '</td>';
                        html += '<td>' + lis.SERVICIO + '</td>';
                        html += '</tr>';
                    });
                    html += '</tbody></table>'
                    $("#tabla").html(html);
                    $("table#example").dataTable();
                    $("#btnGeneraExcel").removeAttr("disabled");
                }
                else {
                    var html = '<div class="alert alert-warning alert-dismissible fade in" role="alert"> <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span></button> <strong>Tu consulta no ha generado resultados.</strong></div>';
                }
               
                $("#dvLoading").attr("hidden", "hidden");
                $("#tabla").html(html);
                $("table#example").dataTable();
            },
            error: function (d) {
                console.log(JSON.stringify(d));
                var html = '<div class="alert alert-warning alert-dismissible fade in" role="alert"> <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span></button> <strong>Error en la generación del reporte.</strong></div>';
                $("#tabla").html(html);
                $("#dvLoading").attr("hidden", "hidden");
            }
        });
    }
}

function valida()
{
    var validacion = true;
    if ($("#fechaInicio").val() === "" || $("#fechaInicio").val().length !== 10)
    {
        $("#fechaInicio").addClass("rojoValidacion");
        validacion = false;
    }

    if ($("#fechaFin").val() === "" || $("#fechaFin").val().length !== 10) {
        $("#fechaFin").addClass("rojoValidacion");
        validacion = false;
    }

    return validacion;
}

function generaExcel()
{
    $("#dvLoading").removeAttr("hidden");
    $.ajax({
        type: "POST",url: rootUrl("/Condusef/generaExcel"),data: {fechaInicio: fecha1, fechaFin: fecha2, contador: contador},dataType: "json",
        success: function (datos) {
            if (datos[0] === "1")
            {
                alertify.alert("El archivo se genero correctamente");
                $("#dvLoading").attr("hidden","hidden");
            }
        },
        error:function(d) {
            alertify.alert("Error en la generación del archivo, revisar si el archivo no se encuentra abierto");
            console.log(JSON.stringify(d));
            $("#dvLoading").attr("hidden", "hidden");
        }
    });
}