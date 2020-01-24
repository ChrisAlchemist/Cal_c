using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMV_CALLCENTER.Entidad
{
    public class Iframe
    {
        public string vAgentID { get; set; }
        public string vServiceID { get; set; }
        public Decimal? vContactID { get; set; }
        public int vCALLTYPE { get; set; }
        public string vPhone { get; set; }
        public int vQCode { get; set; }
        public String vFolio { get; set; }
    }
}