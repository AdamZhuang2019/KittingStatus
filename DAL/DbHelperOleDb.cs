using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
namespace kittingStatus.jabil.web.DAL
{
    public class DbHelper : DatabaseHelper
    {
        public DbHelper(string connectString = "DBCenter") : base(connectString, "sqlserver")
        {

        }

        protected override DbCommand NewCommand()
        {
            return new SqlCommand();
        }

        protected override DbConnection NewConnection()
        {
            return new SqlConnection();
        }

        protected override DbDataAdapter NewDataAdapter()
        {
            return new SqlDataAdapter();
        }

        protected override DbParameter NewParameter()
        {
            return new SqlParameter();
        }
    }
}