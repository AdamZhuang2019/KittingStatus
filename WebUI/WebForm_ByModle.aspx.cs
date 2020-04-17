using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

namespace kittingStatus.jabil.web.WebUI
{
    public partial class WebForm_ByModle : System.Web.UI.Page
    {
        public string Model_list = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = new DAL.DbHelper().QueryDataTable("select * from [EKS_T_Bay] where [Workcell]='Solaredge/Solaredge' and  [BayName]='Bay013X'");
                if (dt.Rows.Count == 0)
                {
                    return;
                }

                string[] Modelstring = Convert.ToString(dt.Rows[0]["Model"]).Split(';');

                List<DataModel.select2> ModelObj = new List<DataModel.select2>();
                for (int i = 0; i < Modelstring.Length; i++)
                {
                    DataModel.select2 o = new DataModel.select2();
                    o.id = i;
                    o.text = Modelstring[i].Trim(); 
                    ModelObj.Add(o);
                }
                JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
                Model_list = jsonSerialize.Serialize(ModelObj);
            
                //end
            }
        }
    }
}