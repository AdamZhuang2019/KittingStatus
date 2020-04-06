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
        public string ToolSide_list = "";
        public string From_Model_list = "";
        public string YorN = "";
        

        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = DAL.DbHelper.ExecuteSql_Table("select distinct Workcell from [T_Bay]");
                if (dt.Rows.Count == 0)
                {
                    return;
                }


                List<DataModel.select2> ToolSideObj = new List<DataModel.select2>();
              
                    DataModel.select2 o_0 = new DataModel.select2();
                    o_0.id = 0;
                    o_0.text = "TOP";
                    ToolSideObj.Add(o_0);
                    DataModel.select2 o_1 = new DataModel.select2();
                    o_1.id = 1;
                    o_1.text = "BOT";
                    ToolSideObj.Add(o_1);
               
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
                ToolSide_list = jsonSerialize.Serialize(ToolSideObj);
                Workcell_list = jsonSerialize.Serialize(WorkcellObj);
                YorN = jsonSerialize.Serialize(ToolYorNObj);
                


                System.Data.DataTable dt_Model = new System.Data.DataTable();
                dt_Model = DAL.DbHelper.ExecuteSql_Table("select * from [T_Bay] where [Workcell]='HP/Printer Motherboard' and  [BayName]='BAY06'");
                if (dt_Model.Rows.Count == 0)
                {
                    return;
                }

                string[] Modelstring = Convert.ToString(dt_Model.Rows[0]["Model"]).Split(';');

                List<DataModel.select2> ModelObj = new List<DataModel.select2>();
                for (int i = 0; i < Modelstring.Length; i++)
                {
                    DataModel.select2 o = new DataModel.select2();
                    o.id = i;
                    o.text = Modelstring[i].Trim();
                    ModelObj.Add(o);
                }

                From_Model_list = jsonSerialize.Serialize(ModelObj);

            }
        }
    }
}