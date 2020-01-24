using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMV_CALLCENTER.Entidad
{
    public class PreguntaBanca
    {
        public int idPregunta { get; set; }
        public String Pregunta { get; set; }

        public List<RespuestaBanca> respuestasAutentificacion { get; set; }

        public PreguntaBanca()
        {
            respuestasAutentificacion = new List<RespuestaBanca>();
        }
    }
}