using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMV_CALLCENTER.Entidad
{
    public class RespuestaBanca
    {
        public int idRespuestas { get; set; }
        public String respuesta { get; set; }
        public Boolean respuestaCorrecta { get; set; }
        public int idPregunta { get; set; }
    }
}