using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace kittingStatus.jabil.web.WebUI
{
    public partial class WebForm_ShowGrid : System.Web.UI.Page
    {
       
        public string ID = "";
        public string Tpye = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ID = Request.QueryString["ID"];
                Tpye = Request.QueryString["Tpye"];
            }
        }
    }
}