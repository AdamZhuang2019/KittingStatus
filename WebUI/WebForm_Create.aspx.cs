using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

namespace kittingStatus.jabil.web.WebUI
{
    public partial class WebForm_Create : System.Web.UI.Page
    {
        public string Workcell_list = "";
      
        public string YorN = "";
        

        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = new DAL.DbHelper().QueryDataTable("select distinct Workcell from [EKS_T_Bay]");
                if (dt.Rows.Count == 0)
                {
                    return;
                }

                List<DataModel.select2> ToolYorNObj = new List<DataModel.select2>();
              
                    DataModel.select2 o_a1 = new DataModel.select2();
                    o_a1.id = 0;
                    o_a1.text = "Y";
                    ToolYorNObj.Add(o_a1);
                    DataModel.select2 o_a2= new DataModel.select2();
                    o_a2.id = 1;
                    o_a2.text = "N";
                    ToolYorNObj.Add(o_a2);

               

                List<DataModel.select2> WorkcellObj = new List<DataModel.select2>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataModel.select2 o = new DataModel.select2();
                    o.id = i;
                    o.text = Convert.ToString(dt.Rows[i]["Workcell"]);
                    WorkcellObj.Add(o);
                }                             
                JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
             
                Workcell_list = jsonSerialize.Serialize(WorkcellObj);
                YorN = jsonSerialize.Serialize(ToolYorNObj);
                
                

            }
        }
    }
}