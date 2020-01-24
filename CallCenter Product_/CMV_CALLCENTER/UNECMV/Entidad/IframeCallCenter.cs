using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMV_CALLCENTER.Entidad
{
    public class IframeCallCenter
    {
        public Decimal? vContactID { get; set; }
        public int vCallType { get; set; }
        public string vPhone { get; set; }
        public string vServiceID { get; set; }
        public string vAgenteID { get; set; }
        public int vNumeroSocio { get; set; }
        public string AgenteAtiende { get; set; }
        public string LlamadaURL { get; set; }
        public string vID_CC_Fraudes { get; set; }
        public string vID_Fraudes_CC { get; set; }
    }
}